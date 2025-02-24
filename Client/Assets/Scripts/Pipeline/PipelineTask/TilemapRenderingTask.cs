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
        private Grid grid;

        public IEnumerator Process()
        {
            var tilemapRoot = new GameObject("Tilemap Root").transform;
            tilemapRoot.parent = PayLoad.RootGameObject.transform;
            InitializeTilemap(tilemapRoot.gameObject);

            yield return null;

            var destinationTilemaps = tilemapRoot.GetComponentsInChildren<Tilemap>();
            var graph = PayLoad.DungeonGraph;

            foreach (var vertex in graph.Vertices)
            {
                var sourceTilemaps = vertex.Prefab.GetComponentsInChildren<Tilemap>();
                CopyTiles(sourceTilemaps, destinationTilemaps, vertex.ToVector3(), grid.WorldToCell);

                yield return null;
            }
        }

        private void InitializeTilemap(GameObject tilemapRoot)
        {
            grid = tilemapRoot.AddComponent<Grid>();
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

        private void CopyTiles(IEnumerable<Tilemap> sourceTilemaps, IEnumerable<Tilemap> destinationTilemaps, Vector3 roomPosition, Func<Vector3, Vector3Int> getCellPosition)
        {
            Vector3 tilemapCenter = GetCenterFromTilemaps(sourceTilemaps);

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

                    var cellPosition = getCellPosition(tilePosition + roomPosition - tilemapCenter);
                    destinationTilemap.SetTile(cellPosition, tile);
                }
            }
        }

        private Vector3 GetCenterFromTilemaps(IEnumerable<Tilemap> tilemaps)
        {
            var minX = int.MaxValue;
            var minY = int.MaxValue;
            var maxX = int.MinValue;
            var maxY = int.MinValue;

            foreach (var tilemap in tilemaps)
            {
                var cellbounds = tilemap.cellBounds;

                if (minX > cellbounds.xMin) minX = cellbounds.xMin;
                if (minY > cellbounds.yMin) minY = cellbounds.yMin;
                if (maxX < cellbounds.xMax) maxX = cellbounds.xMax;
                if (maxY < cellbounds.yMax) maxY = cellbounds.yMax;
            }

            Vector3 size = new((maxX + minX) / 2f - 0.5f, (maxY + minY) / 2f - 0.5f);
            return size;
        }
    }
}