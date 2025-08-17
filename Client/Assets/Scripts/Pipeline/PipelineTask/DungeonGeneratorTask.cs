using GameEngine.DataSequence.Geometry;
using GameEngine.DataSequence.Graph;
using GameEngine.DataSequence.PathFinding;
using GameEngine.DataSequence.Random;
using GameEngine.DataSequence.Tree;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameEngine.Pipeline
{
    public class RoomInstance
    {
        public List<GameObject> Doors { get; private set; } = new();
        public List<GameObject> Mobs { get; private set; } = new();
        public Tilemap[] Tilemaps { get; private set; }

        public void AddDoor(GameObject door)
        {
            door.SetActive(false);
            Doors.Add(door);
        }

        public void AddMob(GameObject mob)
        {
            mob.SetActive(false);
            Mobs.Add(mob);
        }

        public void AddTilemaps(Tilemap[] tilemaps)
        {
            this.Tilemaps = tilemaps;
        }
    }

    public class DungeonGeneratorTask : IPipelineTask<DungeonGeneratorPayLoad>
    {
        public DungeonGeneratorPayLoad PayLoad { get; set; }
        private int roomCount;
        private const int roomPadding = 1;
        private GameObject roomInstanceRootObject;

        private Queue<GameObject> roomPrefabQueue = new();
        private Dictionary<RoomNode, RoomInstance> layoutData = new();

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

            PayLoad.LayoutData = layoutData;
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
                Vector3Int size = GameUtility.GetBoundsIntFromTilemaps(tilemaps).size;

                Vector3Int roomWorldPositionContainPadding = new((int)(roomWorldPosition.x - roomPadding), (int)(roomWorldPosition.y - roomPadding));
                Vector3Int roomTopRight = new((int)(roomWorldPosition.x + size.x + roomPadding), (int)(roomWorldPosition.y + size.y + roomPadding));

                if (isBuilt = CanBuild(dungeonGraph.Vertices, roomWorldPositionContainPadding, roomTopRight))
                {
                    RoomInstance roomInstance = new RoomInstance();
                    RoomNode roomNode = new(roomWorldPosition, size.x, size.y, roomInstance);
                    roomNode.ID = roomPrefab.name == "Start Room" ? 1000 : id;
                    id++;

                    roomInstance.AddTilemaps(tilemaps);

                    var mobs = CreateInstance(roomPrefab, roomWorldPosition, roomInstanceRootObject.transform).
                        GetComponentsInChildren<Monster>();
                    foreach (var mob in mobs) roomInstance.AddMob(mob.gameObject);

                    layoutData.Add(roomNode, roomInstance);
                    dungeonGraph.AddNode(roomNode);
                    tilemapRenderTasks.Add((tilemaps, roomWorldPosition));
                    sample--;
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
            //TODO : 3개 이상의 좌표가 일직선 위에 있지 않도록 배치
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
            gameGrid.IsGizmos = PayLoad.ShowGizmos;

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
                GridCell srcCell = gameGrid.GetCellFromWorldPosition(node1.GetCenter());
                GridCell dstCell = gameGrid.GetCellFromWorldPosition(node2.GetCenter());

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
                        prevCell = gameGrid.GetCellFromWorldPosition(nearestCellWorldPosition);
                    }
                    else
                    {
                        prevCell = pathResultArray[i - 1];
                    }

                    GridCell nextCell;
                    if (i + 1 >= pathResultArray.Length)
                    {
                        Vector3 nearestCellWorldPosition = GetNearestCellFromRoom(curCell.ToVector3(), node2.ToVector3(), node2.GetSize());
                        nextCell = gameGrid.GetCellFromWorldPosition(nearestCellWorldPosition);
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

                var pathResultList = pathResult.ToList();
                GridCell firstPathCell = pathResultList.First();
                GridCell lastPathCell = pathResultList.Last();

                Vector3 firstPathCellPosition = firstPathCell.ToVector3();
                Vector3 lastPathCellPosition = lastPathCell.ToVector3();

                Vector3 startCellWorldPosition = GetNearestCellFromRoom(firstPathCellPosition, node1.ToVector3(), node1.GetSize());
                Vector3 endCellWorldPosition = GetNearestCellFromRoom(lastPathCellPosition, node2.ToVector3(), node2.GetSize());

                GridCell startCell = gameGrid.GetCellFromWorldPosition(startCellWorldPosition);
                GridCell endCell = gameGrid.GetCellFromWorldPosition(endCellWorldPosition);

                pathResultList.Insert(0, startCell);
                pathResultList.Add(endCell);

                edge.PathResult = pathResultList;

                (GameObject sourceDoorPrefab, Tilemap[] sourceDoorTilemaps, Vector3 sourceDoorPosition) = GetDoor(startCellWorldPosition, firstPathCellPosition);
                pathwayList.Add((sourceDoorTilemaps, sourceDoorPosition));
                (GameObject destinationDoorPrefab, Tilemap[] destinationDoorTilemaps, Vector3 destinationDoorPosition) = GetDoor(endCellWorldPosition, lastPathCellPosition);
                pathwayList.Add((destinationDoorTilemaps, destinationDoorPosition));

                var sourceDoorInstance = CreateInstance(sourceDoorPrefab, sourceDoorPosition, roomInstanceRootObject.transform);
                var destinationDoorInstance = CreateInstance(destinationDoorPrefab, destinationDoorPosition, roomInstanceRootObject.transform);

                layoutData[node1].AddDoor(sourceDoorInstance);
                layoutData[node2].AddDoor(destinationDoorInstance);
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
                return PayLoad.VerticalRoad; // 수직
            }
            else if (p1.y == p2.y && p2.y == p3.y)
            {
                return PayLoad.HorizonRoad; // 수평
            }

            else if (p1.y == p2.y && p2.x == p3.x && p3.y > p2.y)
            {
                return PayLoad.RightTopRoad; // ㄴ자 모양 (오른쪽 위)
            }

            else if (p1.y == p2.y && p2.x == p3.x && p2.y > p3.y)
            {
                return PayLoad.LeftBottomRoad; // ㄱ자 모양 (왼쪽 아래)
            }

            else if (p1.x == p2.x && p2.y == p3.y && p3.x > p2.x)
            {
                return PayLoad.RightBottomRoad; // ┌자 모양 (오른쪽 아래)
            }

            else if (p1.x == p2.x && p2.y == p3.y && p2.x > p3.x)
            {
                return PayLoad.LeftTopRoad; // ┘자 모양 (왼쪽 위)
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

        private (GameObject prefab, Tilemap[] doorTilemaps, Vector3 doorPosition) GetDoor(Vector3 firstCellPosition, Vector3 secondCellPosition)
        {
            GameObject prefab;
            Vector3 doorPosition;
            Tilemap[] doorTilemaps;
            int size;
            Vector3 direction = secondCellPosition - firstCellPosition;
            if (direction.y == 0)
            {
                prefab = PayLoad.HorizonDoor;
                doorTilemaps = GetTilemaps(prefab);
                size = GameUtility.GetBoundsIntFromTilemaps(doorTilemaps).size.x;
                if (direction.x > 0)
                    doorPosition = new(secondCellPosition.x - size, secondCellPosition.y);
                else
                    doorPosition = firstCellPosition;
            }
            else if (direction.x == 0)
            {
                prefab = PayLoad.VerticalDoor;
                doorTilemaps = GetTilemaps(prefab);
                size = GameUtility.GetBoundsIntFromTilemaps(doorTilemaps).size.y;
                if (direction.y > 0)
                    doorPosition = new(secondCellPosition.x, secondCellPosition.y - size);
                else
                    doorPosition = firstCellPosition;
            }
            else
                throw new System.InvalidOperationException("Direction is not horizontal or vertical");

            return (prefab, doorTilemaps, doorPosition);
        }

        private Tilemap[] GetTilemaps(GameObject prefab)
        {
            var result = prefab.GetComponentsInChildren<Tilemap>();
            foreach (var tilemap in result)
                tilemap.CompressBounds();
            return result;
        }

        private GameObject CreateInstance(GameObject prefab, Vector3 worldPosition, Transform rootObject)
        {
            var gameObjectInstance = UnityEngine.Object.Instantiate(prefab, rootObject);
            foreach (var tilemap in GetTilemaps(gameObjectInstance))
                GameUtility.Destroy(tilemap.gameObject);
            gameObjectInstance.transform.position = worldPosition;

            return gameObjectInstance;
        }
    }
}