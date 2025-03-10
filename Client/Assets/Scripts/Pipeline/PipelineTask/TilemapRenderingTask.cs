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

                foreach (GridCell cell in pathResult)
                    cell.IsWalkable = false;
                //GameUtil.CreateLineRenderer(Color.red, .2f, pathResult.Select(cell => cell.ToVector3()).ToArray()).transform.parent = PayLoad.RootGameObject.transform;
            }
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

        public static void CopyTiles(IEnumerable<Tilemap> sourceTilemaps, IEnumerable<Tilemap> destinationTilemaps, Vector3Int bottomLeft, Func<Vector3, Vector3Int> getCellPosition)
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
    }
}