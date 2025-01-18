using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace GameEngine.MapGenerator
{
    public class MapRenderer : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private Tilemap tilemap;
        [FormerlySerializedAs("collderMap")] [SerializeField] private Tilemap colliderMap;
        [SerializeField] private TileBase tile;
        private Vector2Int mapSize;
        private List<Rect> list = new();
        
        public void SetMapSize(Vector2Int mapSize)
        {
            this.mapSize = mapSize;
        }

        public void DrawLine(Vector2 from, Vector2 to)
        {
            LineRenderer line = Instantiate<LineRenderer>(this.lineRenderer, this.transform);
            line.SetPosition(0, from);
            line.SetPosition(1, to);
        }

        public void DrawDungeon(int x, int y, int width, int height)
        {
            for(int i=x; i<x+width; i++)
            for (int j = y; j < y + height; j++)
                SetTile(new Vector3Int(i, j, 0), tile);
        }

        public void DrawRoad(Vector2Int node1, Vector2Int node2)
        {
            for (int x = Mathf.Min(node1.x, node2.x); x <= Mathf.Max(node1.x, node2.x); x++)
                SetTile(new Vector3Int(x, node1.y , 0), tile); 
            for (int y = Mathf.Min(node1.y, node2.y); y <= Mathf.Max(node1.y, node2.y); y++)
                SetTile(new Vector3Int(node2.x, y, 0), tile);
        }

        private void SetTile(Vector3Int tilePos, TileBase tile)
        {
            tilemap.SetTile(tilePos, tile);
            colliderMap.SetTile(tilePos, null);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < list.Count; i++)
            {
                var rect = list[i];
                Gizmos.DrawWireCube(new Vector3(rect.x + rect.width / 2, rect.y + rect.height / 2, 0), new Vector3(rect.width, rect.height, 0.1f));
            }
        }
    }
}