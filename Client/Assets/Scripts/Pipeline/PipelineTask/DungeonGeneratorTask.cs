using GameEngine.DataSequence.Geometry;
using GameEngine.DataSequence.Graph;
using GameEngine.DataSequence.PathFinding;
using GameEngine.DataSequence.Queue;
using GameEngine.DataSequence.Random;
using GameEngine.DataSequence.Tree;
using GameEngine.MapGenerator.Room;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameEngine.Pipeline
{
    public class DungeonGeneratorTask : IPipelineTask<DungeonGeneratorPayLoad>
    {
        public DungeonGeneratorPayLoad PayLoad { get; set; }
        private int roomCount;
        private const int roomPadding = 1;
        private GameObject roomInstanceRootObject;

        private Queue<GameObject> roomPrefabQueue = new();

        public DungeonGeneratorTask(int roomCount)
        {
            this.roomCount = roomCount;
        }

        public IEnumerator Process()
        {
            DungeonGraph dungeonGraph = new();
            roomInstanceRootObject = new GameObject("Room Instance Root Object");
            roomInstanceRootObject.transform.parent = PayLoad.RootGameObject.transform;
            CreateRandomRoom(dungeonGraph);
            yield return null;

            BuildEdges(dungeonGraph);
            yield return null;

            PayLoad.DungeonGraph = dungeonGraph;

            int gridCellSize = PayLoad.GridCellSize;
            var boundsInt = dungeonGraph.GetBoundsInt(gridCellSize);

            GameGrid gameGrid = InitializeGameGrid(dungeonGraph.GetGridWorldPosition(), dungeonGraph.Vertices, gridCellSize, boundsInt);
            PayLoad.GameGrid = gameGrid;
            yield return null;

            BuildRoad(dungeonGraph.Edges, gameGrid);
            yield return null;
        }

        private void CreateRandomRoom(DungeonGraph dungeonGraph)
        {
            var rand = PayLoad.Random;
            int sample = roomCount;
            int gridCellSize = PayLoad.GridCellSize;

            roomPrefabQueue = InitializeRoomPrefabQueue(PayLoad.RoomTemplates, roomCount);

            List<(Tilemap[] tilemaps, Vector3 worldPosition)> tilemapRenderTasks = new();

            int id = 1;
            bool isBuilt = true;
            GameObject roomPrefab = null;

            while (sample > 0)
            {
                var roomWorldPosition = GetRoomWorldPosition(rand, gridCellSize);
                if (isBuilt)
                    roomPrefab = roomPrefabQueue.Dequeue();
                var tilemaps = GetTilemaps(roomPrefab);
                Vector3Int size = GameUtil.GetBoundsIntFromTilemaps(tilemaps).size;

                Vector3Int roomWorldPositionContainPadding = new((int)(roomWorldPosition.x - roomPadding), (int)(roomWorldPosition.y - roomPadding));
                Vector3Int roomTopRight = new((int)(roomWorldPosition.x + size.x + roomPadding), (int)(roomWorldPosition.y + size.y + roomPadding));

                if (isBuilt = CanBuild(dungeonGraph.Vertices, roomWorldPositionContainPadding, roomTopRight))
                {
                    RoomNode roomNode = new(roomWorldPosition, size.x, size.y);
                    roomNode.ID = id;
                    id++;

                    dungeonGraph.AddNode(roomNode);
                    tilemapRenderTasks.Add((tilemaps, roomWorldPosition));
                    sample--;

                    CreateRoomInstance(roomPrefab, roomWorldPosition, roomInstanceRootObject.transform);
                }
                else
                {
                    rand.stdev += .1f;
                }
            }

            PayLoad.TilemapRenderTaskList.AddRange(tilemapRenderTasks);
        }

        private Queue<GameObject> InitializeRoomPrefabQueue(IEnumerable<RoomData> roomDatas, int roomCount)
        {
            int totalGuaranteedCount = roomDatas.Sum(x => x.guaranteedCount);
            System.Random random = new System.Random();

            if (totalGuaranteedCount > roomCount)
                throw new System.InvalidOperationException("The minimum number of rooms is greater than the maximum number of rooms");

            var positives = roomDatas.Where(n => n.guaranteedCount >= 1);
            var zeros = roomDatas.Where(n => n.guaranteedCount <= 0).ToList();

            positives = positives.SelectMany(roomData => Enumerable.Repeat(roomData, roomData.guaranteedCount)).ToList();
            int additionalCount = roomCount - totalGuaranteedCount - zeros.Count;
            while(additionalCount-- > 0)
            {
                zeros.Add(zeros[random.Next(zeros.Count)]);
            }

           
            positives = positives.OrderBy(n => random.Next()).ToList();
            zeros = zeros.OrderBy(n => random.Next()).ToList();

            Queue<GameObject> queue = new(positives.Concat(zeros).Select(x => x.prefab));
            return queue;
        }

        private Vector2 GetRoomWorldPosition(NormalDistribution rand, int gridCellSize)
        {
            //TODO : 3�� �̻��� ��ǥ�� ������ ���� ���� �ʵ��� ��ġ
            var randPosX = rand.NextDouble();
            var randPosY = rand.NextDouble();

            int randomRoomPositionX = (int)(Math.Round(randPosX / gridCellSize) * gridCellSize);
            int randomRoomPositionY = (int)(Math.Round(randPosY / gridCellSize) * gridCellSize);

            Vector2 roomWorldPosition = new(randomRoomPositionX, randomRoomPositionY);

            return roomWorldPosition;
        }

        private void BuildEdges(DungeonGraph dungeonGraph)
        {
            var vertices = dungeonGraph.Vertices;

            if (vertices.Count == 2)
            {
                var firstNode = vertices.First();
                var lastNode = vertices.Last();
                RoomEdge edge = new(firstNode, lastNode, Vector3.Distance(firstNode.ToVector3(), lastNode.ToVector3()));
                dungeonGraph.AddEdge(firstNode, edge);
                return;
            }

            var triangles = Triangulation.TriangulateByFlippingEdges(dungeonGraph.Vertices.Select(v => v.ToVector3()).ToList());
            HashSet<RoomEdge> edges = new();

            foreach (var tri in triangles)
            {
                var v1Pos = tri.v1.position;
                var v2Pos = tri.v2.position;
                var v3Pos = tri.v3.position;

                var node1 = dungeonGraph.GetNodeAtPos(v1Pos);
                var node2 = dungeonGraph.GetNodeAtPos(v2Pos);
                var node3 = dungeonGraph.GetNodeAtPos(v3Pos);

                edges.Add(new RoomEdge(node1, node2, Vector3.Distance(v1Pos, v2Pos)));
                edges.Add(new RoomEdge(node2, node1, Vector3.Distance(v1Pos, v2Pos)));
                edges.Add(new RoomEdge(node2, node3, Vector3.Distance(v2Pos, v3Pos)));
                edges.Add(new RoomEdge(node3, node2, Vector3.Distance(v2Pos, v3Pos)));
                edges.Add(new RoomEdge(node3, node1, Vector3.Distance(v1Pos, v3Pos)));
                edges.Add(new RoomEdge(node1, node3, Vector3.Distance(v1Pos, v3Pos)));
            }

            var treeEdges = SpanningTree.TransformMininum(dungeonGraph.Vertices, edges);
            foreach (var edge in treeEdges)
            {
                dungeonGraph.AddEdge(edge.From, edge);
            }
        }

        private bool CanBuild(IEnumerable<RoomNode> vertices, Vector3Int roomWorldPosition, Vector3Int roomTopRight)
        {
            bool canCollide = false;

            Vector3Int min = roomWorldPosition;
            Vector3Int max = roomTopRight;
            foreach (var otherNode in vertices)
            {
                Vector3Int otherMin = otherNode.ToVector3Int();
                otherMin.x -= roomPadding;
                otherMin.y -= roomPadding;
                Vector3Int otherMax = new Vector3Int(otherMin.x + otherNode.Width + roomPadding, otherMin.y + otherNode.Height + roomPadding);
                if (MathUtility.AABB(min, max, otherMin, otherMax))
                    canCollide = true;
            }

            return canCollide == false;
        }

        private GameGrid InitializeGameGrid(Vector3 gridWorldPosition, IEnumerable<RoomNode> roomEnumerable, int gridCellSize, BoundsInt boundsInt)
        {
            GameObject gridObject = new GameObject(nameof(GameGrid));
            GameGrid gameGrid = gridObject.AddComponent<GameGrid>();
            gameGrid.transform.position = gridWorldPosition;
            gameGrid.CreateGrid(roomEnumerable, gridCellSize, boundsInt);
            gameGrid.isGizmos = PayLoad.ShowGizmos;

            gridObject.transform.parent = PayLoad.RootGameObject.transform;

            return gameGrid;
        }

        private void BuildRoad(IEnumerable<RoomEdge> edges, GameGrid gameGrid)
        {
            DungeonRoadBuilder roadBuilder = new(Comparer<GridCell>.Default, gameGrid);
            List<(Tilemap[] tilemaps, Vector3 position)> pathwayList = new();
            foreach (var edge in edges)
            {
                var node1 = edge.From;
                var node2 = edge.To;

                gameGrid.Clear();
                GridCell srcCell = gameGrid.GetCell(node1.GetCenter());
                GridCell dstCell = gameGrid.GetCell(node2.GetCenter());

                var pathResult = roadBuilder.GetMinPath(srcCell, dstCell);
                if (pathResult == null)
                    continue;

                var pathResultArray = pathResult.ToArray();
                for (int i = 0; i < pathResultArray.Length; i++)
                {
                    GridCell curCell = pathResultArray[i];

                    GridCell prevCell;
                    if (i - 1 < 0)
                    {
                        Vector3 nearestCellWorldPosition = GetNearestCellFromRoom(curCell.ToVector3(), node1.ToVector3(), node1.GetSize());
                        prevCell = gameGrid.GetCell(nearestCellWorldPosition);
                    }
                    else
                    {
                        prevCell = pathResultArray[i - 1];
                    }

                    GridCell nextCell;
                    if (i + 1 >= pathResultArray.Length)
                    {
                        Vector3 nearestCellWorldPosition = GetNearestCellFromRoom(curCell.ToVector3(), node2.ToVector3(), node2.GetSize());
                        nextCell = gameGrid.GetCell(nearestCellWorldPosition);
                    }
                    else
                    {
                        nextCell = pathResultArray[i + 1];
                    }

                    Vector3 prevPos = prevCell.ToVector3();
                    Vector3 curPos = curCell.ToVector3();
                    Vector3 nextPos = nextCell.ToVector3();

                    GameObject roadPrefab = GetRoadPrefab(prevPos, curPos, nextPos);
                    if (roadPrefab == null)
                    {
                        Debug.LogError($"Src : {node1.ToVector3()}, Dst : {node2.ToVector3()}");
                        continue;
                    }

                    pathwayList.Add((GetTilemaps(roadPrefab), curCell.ToVector3Int()));
                }

                var list = pathResult.ToList();
                GridCell firstPathCell = list.First();
                GridCell lastPathCell = list.Last();

                Vector3 firstPathCellPosition = firstPathCell.ToVector3();
                Vector3 lastPathCellPosition = lastPathCell.ToVector3();

                Vector3 startCellWorldPosition = GetNearestCellFromRoom(firstPathCellPosition, node1.ToVector3(), node1.GetSize());
                Vector3 endCellWorldPosition = GetNearestCellFromRoom(lastPathCellPosition, node2.ToVector3(), node2.GetSize());

                (Tilemap[] sourceDoorTilemaps, Vector3 sourceDoorPosition) = GetDoor(startCellWorldPosition, firstPathCellPosition);
                pathwayList.Add((sourceDoorTilemaps, sourceDoorPosition));
                (Tilemap[] destinationDoorTilemaps, Vector3 destinationDoorPosition) = GetDoor(endCellWorldPosition, lastPathCellPosition);
                pathwayList.Add((destinationDoorTilemaps, destinationDoorPosition));
            }

            PayLoad.TilemapRenderTaskList.AddRange(pathwayList);
        }

        private GameObject GetRoadPrefab(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            if (MathUtility.IsTriangleOrientedClockwise(p1, p2, p3) == false)
            {
                Vector2 t = p1;
                p1 = p3;
                p3 = t;
            }

            if (p1.x == p2.x && p2.x == p3.x)
            {
                return PayLoad.VerticalRoad; // ����
            }
            else if (p1.y == p2.y && p2.y == p3.y)
            {
                return PayLoad.HorizonRoad; // ����
            }

            else if (p1.y == p2.y && p2.x == p3.x && p3.y > p2.y)
            {
                return PayLoad.RightTopRoad; // ���� ��� (������ ��)
            }

            else if (p1.y == p2.y && p2.x == p3.x && p2.y > p3.y)
            {
                return PayLoad.LeftBottomRoad; // ���� ��� (���� �Ʒ�)
            }

            else if (p1.x == p2.x && p2.y == p3.y && p3.x > p2.x)
            {
                return PayLoad.RightBottomRoad; // ���� ��� (������ �Ʒ�)
            }

            else if (p1.x == p2.x && p2.y == p3.y && p2.x > p3.x)
            {
                return PayLoad.LeftTopRoad; // ���� ��� (���� ��)
            }
            Debug.LogError(p1 + ", " + p2 + ", " + p3);
            return null;
        }

        private Vector3 GetNearestCellFromRoom(Vector3 point, Vector3 roomPos, Vector3 roomSize)
        {
            int cellSize = PayLoad.GridCellSize;
            Vector3 leftRectPos = new(roomPos.x - roomSize.x, roomPos.y);
            bool left = MathUtility.IsPointInRectangle(point, leftRectPos, roomSize);
            if (left)
                return new(roomPos.x, point.y);

            Vector3 upRectPos = new(roomPos.x, roomPos.y + roomSize.y);
            bool up = MathUtility.IsPointInRectangle(point, upRectPos, roomSize);
            if (up)
                return new(point.x, roomPos.y + roomSize.y - cellSize);

            Vector3 rightRectPos = new(roomPos.x + roomSize.x, roomPos.y);
            bool right = MathUtility.IsPointInRectangle(point, rightRectPos, roomSize);
            if (right)
                return new(roomPos.x + roomSize.x - cellSize, point.y);

            Vector3 downRectPos = new(roomPos.x, roomPos.y - roomSize.y);
            bool down = MathUtility.IsPointInRectangle(point, downRectPos, roomSize);
            if (down)
                return new(point.x, roomPos.y);

            throw new System.ArgumentException("Rectangle Contains Point");
        }

        private (Tilemap[] doorTilemaps, Vector3 doorPosition) GetDoor(Vector3 firstCellPosition, Vector3 secondCellPosition)
        {
            Vector3 doorPosition;
            Tilemap[] doorTilemaps;
            int size;
            Vector3 direction = secondCellPosition - firstCellPosition;
            if (direction.y == 0)
            {
                doorTilemaps = GetTilemaps(PayLoad.HorizonDoor);
                size = GameUtil.GetBoundsIntFromTilemaps(doorTilemaps).size.x;
                if (direction.x > 0)
                    doorPosition = new(secondCellPosition.x - size, secondCellPosition.y);
                else
                    doorPosition = firstCellPosition;
            }
            else if (direction.x == 0)
            {
                doorTilemaps = GetTilemaps(PayLoad.VerticalDoor);
                size = GameUtil.GetBoundsIntFromTilemaps(doorTilemaps).size.y;
                if (direction.y > 0)
                    doorPosition = new(secondCellPosition.x, secondCellPosition.y - size);
                else
                    doorPosition = firstCellPosition;
            }
            else
                throw new System.InvalidOperationException("Direction is not horizontal or vertical");

            return (doorTilemaps, doorPosition);
        }

        private Tilemap[] GetTilemaps(GameObject prefab)
        {
            var result = prefab.GetComponentsInChildren<Tilemap>();
            foreach (var tilemap in result)
                tilemap.CompressBounds();
            return result;
        }

        private void CreateRoomInstance(GameObject roomPrefab, Vector3 roomWorldPosition, Transform rootObject)
        {
            var roomInstance = UnityEngine.Object.Instantiate(roomPrefab, rootObject);
            foreach (var tilemap in GetTilemaps(roomInstance))
                GameUtil.Destroy(tilemap.gameObject);
            roomInstance.transform.position = roomWorldPosition;
        }
    }
}