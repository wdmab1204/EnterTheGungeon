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

        public IEnumerator Process()
        {
            GameObject tilemapRoot = PayLoad.GameGrid.gameObject;
            InitializeTilemap(tilemapRoot);

            yield return null;

            var destinationTilemaps = tilemapRoot.GetComponentsInChildren<Tilemap>();
            var graph = PayLoad.DungeonGraph;
            var gridCellSize = PayLoad.GridCellSize;
            var gridBoundsInt = graph.GetBoundsInt(gridCellSize);

            foreach(var path in PayLoad.TilemapRenderTaskList)
            {
                var sourceTilemaps = path.tilemaps;
                CopyTiles(sourceTilemaps, destinationTilemaps, path.worldPosition, unityGrid.WorldToCell);

                yield return null;
            }
        }

        private void InitializeTilemap(GameObject tilemapRoot)
        {
            unityGrid = tilemapRoot.AddComponent<Grid>();
            var rootTransform = tilemapRoot.transform;

            CreateTilemap("Floor", rootTransform, 0);
            var collideableTilemap = CreateTilemap("Collideable", rootTransform, 1);
            collideableTilemap.tag = "Collideable";
            AddCollider(collideableTilemap);
        }

        private GameObject CreateTilemap(string name, Transform parent, int sortingOrder)
        {
            GameObject obj = new GameObject(name);
            obj.transform.parent = parent;
            obj.transform.localPosition = Vector3.zero;
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

        private void CopyTiles(IEnumerable<Tilemap> sourceTilemaps, IEnumerable<Tilemap> destinationTilemaps, Vector3 bottomLeft, Func<Vector3, Vector3Int> getCellPosition)
        {
            HashSet<Vector3Int> removeTiles = new();
            foreach(Tilemap tilemap in sourceTilemaps)
            {
                foreach(var tilePos in tilemap.cellBounds.allPositionsWithin)
                {
                    var tile = tilemap.GetTile(tilePos);
                    if (tile != null)
                        removeTiles.Add(tilePos);
                }
            }

            foreach(var tilePos in removeTiles)
            {
                foreach(var tilemap in destinationTilemaps)
                {
                    var cellPosition = getCellPosition(tilePos + bottomLeft);
                    tilemap.SetTile(cellPosition, null);
                }
            }

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