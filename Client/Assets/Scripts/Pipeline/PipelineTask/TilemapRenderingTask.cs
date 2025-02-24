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

        public IEnumerator Process()
        {
            var tilemapRoot = new GameObject("Tilemap Root").transform;
            tilemapRoot.parent = PayLoad.RootGameObject.transform;
            InitializeTilemap(tilemapRoot.gameObject);

            yield return null;

            var destinationTilemaps = tilemapRoot.GetComponentsInChildren<Tilemap>();

            foreach (var vertex in PayLoad.DungeonGraph.Vertices)
            {
                var sourceTilemaps = vertex.Prefab.GetComponentsInChildren<Tilemap>();
                CopyTiles(sourceTilemaps, destinationTilemaps, vertex.ToVector3Int());

                //var clone = Object.Instantiate(vertex.Prefab);
                //clone.transform.parent = PayLoad.RootGameObject.transform;
                //clone.SetActive(true);
                //clone.transform.position = vertex.ToVector3();

                yield return null;
            }
        }

        private void InitializeTilemap(GameObject tilemapRoot)
        {
            tilemapRoot.AddComponent<Grid>();
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

        private void CopyTiles(IEnumerable<Tilemap> sourceTilemaps, IEnumerable<Tilemap> destinationTilemaps, Vector3Int roomPosition)
        {
            Vector3Int tilemapCenter = GetCenterFromTilemaps(sourceTilemaps);

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

                    destinationTilemap.SetTile(tilePosition + roomPosition - tilemapCenter, tile);
                }
            }
        }

        private Vector3Int GetCenterFromTilemaps(IEnumerable<Tilemap> tilemaps)
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

            Vector3Int size = new((maxX + minX) / 2, (maxY + minY) / 2);
            return size;
        }
    }
}