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
            DungeonRoadBuilder roadBuilder = new(Comparer<GridCell>.Default, gameGrid);

            foreach (var edge in graph.Edges)
            {
                var node1 = edge.From;
                var node2 = edge.To;
                var pathResult = roadBuilder.GetMinPath(node1.GetCenter(), node2.GetCenter());
                if (pathResult == null)
                    continue;

                GridCell srcCell = gameGrid.GetCell(node1.GetCenter());
                GridCell dstCell = gameGrid.GetCell(node2.GetCenter());

                var pathResultArray = pathResult.ToArray();
                for (int i = 0; i < pathResultArray.Length; i++)
                {
                    GridCell prevCell = i - 1 < 0 ? srcCell : pathResultArray[i - 1];
                    GridCell curCell = pathResultArray[i];
                    GridCell nextCell = i + 1 >= pathResultArray.Length ? dstCell : pathResultArray[i + 1];

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
                    CopyTiles(sourceTilemap, destinationTilemaps, curCell.ToVector3Int(), unityGrid.WorldToCell);
                }
            }

            
        }

        public GameObject GetRoadPrefab(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            if(MathUtility.IsTriangleOrientedClockwise(p1, p2, p3) == false)
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

        public void CopyTiles(IEnumerable<Tilemap> sourceTilemaps, IEnumerable<Tilemap> destinationTilemaps, Vector3Int bottomLeft, Func<Vector3, Vector3Int> getCellPosition)
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

        private void RemoveTiles(IEnumerable<Tilemap> worldTilemaps, IEnumerable<Vector3Int> cellTilePositions)
        {
            foreach(var tilemap in worldTilemaps)
            {
                foreach(var cellTilePosition in cellTilePositions)
                {
                    tilemap.SetTile(cellTilePosition, null);
                }
            }
        }
    }
}