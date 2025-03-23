using GameEngine.DataSequence.Graph;
using GameEngine.DataSequence.PathFinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameEngine.Pipeline
{

    public class TilemapRenderingTask : IPipelineTask<DungeonGeneratorPayLoad>
    {
        public DungeonGeneratorPayLoad PayLoad { get; set; }
        private Grid unityGrid;
        private GameGrid gameGrid;
        private bool showGizmos;

        public TilemapRenderingTask(bool showGizmos)
        {
            this.showGizmos = showGizmos;
        }

        public IEnumerator Process()
        {
            var tilemapRoot = new GameObject("Tilemap Root").transform;
            tilemapRoot.parent = PayLoad.RootGameObject.transform;
            InitializeTilemap(tilemapRoot.gameObject);

            yield return null;

            var destinationTilemaps = tilemapRoot.GetComponentsInChildren<Tilemap>();
            var graph = PayLoad.DungeonGraph;
            var gridCellSize = PayLoad.GridCellSize;
            var gridBoundsInt = graph.GetBoundsInt(gridCellSize);

            Vector3Int gridWorldPosition = graph.GetBottomLeftPos();

            gameGrid.transform.position = gridWorldPosition;

            foreach (var vertex in graph.Vertices)
            {
                var sourceTilemaps = vertex.Prefab.GetComponentsInChildren<Tilemap>();
                CopyTiles(sourceTilemaps, destinationTilemaps, vertex.ToVector3Int(), unityGrid.WorldToCell);

                yield return null;
            }


            gameGrid.CreateGrid(graph.Vertices, gridCellSize, gridBoundsInt);
            gameGrid.isGizmos = showGizmos;

            yield return null;

            BuildRoad(graph.Edges, destinationTilemaps);

            yield return null;
        }

        private void InitializeTilemap(GameObject tilemapRoot)
        {
            unityGrid = tilemapRoot.AddComponent<Grid>();
            gameGrid = tilemapRoot.AddComponent<GameGrid>();
            var rootTransform = tilemapRoot.transform;

            CreateTilemap("Floor", rootTransform, 0);
            var collideableTilemap = CreateTilemap("Collideable", rootTransform, 1);
            AddCollider(collideableTilemap);
        }

        private GameObject CreateTilemap(string name, Transform parent, int sortingOrder)
        {
            GameObject obj = new GameObject(name);
            obj.transform.parent = parent;
            obj.AddComponent<Tilemap>();
            obj.AddComponent<TilemapRenderer>().sortingOrder = sortingOrder;
            return obj;
        }

        private void AddCollider(GameObject obj)
        {
            var collider = obj.AddComponent<TilemapCollider2D>();
            collider.usedByComposite = true;

            obj.AddComponent<CompositeCollider2D>();
            obj.GetOrAddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        }

        private void CopyTiles(IEnumerable<Tilemap> sourceTilemaps, IEnumerable<Tilemap> destinationTilemaps, Vector3Int bottomLeft, Func<Vector3, Vector3Int> getCellPosition)
        {
            Vector3 tilemapCenter = GameUtil.GetBoundsIntFromTilemaps(sourceTilemaps).center;

            foreach (var sourceTilemap in sourceTilemaps)
            {
                var destinationTilemap = destinationTilemaps.FirstOrDefault(dest => dest.name == sourceTilemap.name);
                if (destinationTilemap == null)
                    continue;

                var sourceTilemapCellBounds = sourceTilemap.cellBounds;

                foreach (var tilePosition in sourceTilemapCellBounds.allPositionsWithin)
                {
                    var tile = sourceTilemap.GetTile(tilePosition);
                    if (tile == null)
                        continue;

                    var cellPosition = getCellPosition(tilePosition + bottomLeft);
                    destinationTilemap.SetTile(cellPosition, tile);
                }
            }
        }

        private void BuildRoad(IEnumerable<RoomEdge> edges, Tilemap[] gridTilemaps)
        {
            DungeonRoadBuilder roadBuilder = new(Comparer<GridCell>.Default, gameGrid);

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
                    ;
                    GameObject roadPrefab = GetRoadPrefab(prevPos, curPos, nextPos);
                    if (roadPrefab == null)
                    {
                        Debug.LogError($"Src : {node1.ToVector3()}, Dst : {node2.ToVector3()}");
                        continue;
                    }
                    Tilemap[] sourceTilemap = roadPrefab.GetComponentsInChildren<Tilemap>();
                    CopyTiles(sourceTilemap, gridTilemaps, curCell.ToVector3Int(), unityGrid.WorldToCell);
                }

                var list = pathResult.ToList();
                list.Insert(0, gameGrid.GetCell(GetNearestCellFromRoom(list.First().ToVector3(), node1.ToVector3(), node1.GetSize())));
                list.Add(gameGrid.GetCell(GetNearestCellFromRoom(list.Last().ToVector3(), node2.ToVector3(), node2.GetSize())));
                pathResult = list;

                Color randomColor = new Color(
                    UnityEngine.Random.Range(0f, 1f),
                    UnityEngine.Random.Range(0f, 1f),
                    UnityEngine.Random.Range(0f, 1f),
                    1f
                );

                GameUtil.CreateLineRenderer(randomColor, 0.2f, pathResult.Select(path =>
                {
                    var v = gameGrid.GetCellCenter(path.ToVector3Int());
                    v.z = -1;
                    return v;
                }).ToArray()).transform.parent = PayLoad.RootGameObject.transform;
            }
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
    }
}