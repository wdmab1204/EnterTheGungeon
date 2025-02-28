using GameEngine.DataSequence.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

namespace GameEngine
{
    public static class GameUtil
    {
        public static Vector2Int GetCenterInt(this RectInt rect)
        {
            return new Vector2Int(rect.x + rect.width / 2, rect.y + rect.height / 2);
        }

        public static Vector2Int GetSize(this RectInt rect)
        {
            return new Vector2Int(rect.width, rect.height);
        }

        public static string GetString(this byte[] bytes)
        {
            var bytesInStr = String.Join("", bytes.Select(b => String.Format("{0:X02}", b)));
            var bytesInStrHex = $"0x{bytesInStr}";
            return bytesInStrHex;
        }

        public static UnityEngine.Vector3 ToVector3(this IGeomertyNode node)
            => new(node.X, node.Y);

        public static UnityEngine.Vector3Int ToVector3Int(this IGeomertyNode node)
            => new((int)node.X, (int)node.Y);

        public static void Destroy(UnityEngine.Object obj)
        {
            if(Application.isPlaying)
                GameObject.Destroy(obj);
            else
                GameObject.DestroyImmediate(obj);
        }

        public static LineRenderer CreateLineRenderer(Color color, float width, params Vector3[] positions)
        {
            LineRenderer lineRenderer = new GameObject("Line Renderer").AddComponent<LineRenderer>();
            lineRenderer.positionCount = positions.Length;
            for(int i=0; i<positions.Length - 1; i++)
            {
                lineRenderer.SetPosition(i, positions[i]);
                lineRenderer.SetPosition(i + 1, positions[i + 1]);
            }
            
            lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
            lineRenderer.sharedMaterial.color = color;
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;

            return lineRenderer;
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default)
        {
            if (dictionary.TryGetValue(key, out TValue value))
                return value;
            dictionary.Add(key, defaultValue);
            return defaultValue;
        }

        public static Vector2Int GetSizeFromTilemaps(IEnumerable<Tilemap> tilemaps)
        {
            var minX = int.MaxValue;
            var minY = int.MaxValue;
            var maxX = int.MinValue;
            var maxY = int.MinValue;

            foreach (var tilemap in tilemaps)
            {
                tilemap.CompressBounds();
                var cellbounds = tilemap.cellBounds;

                if (minX > cellbounds.xMin) minX = cellbounds.xMin;
                if (minY > cellbounds.yMin) minY = cellbounds.yMin;
                if (maxX < cellbounds.xMax) maxX = cellbounds.xMax;
                if (maxY < cellbounds.yMax) maxY = cellbounds.yMax;
            }

            Vector2Int size = new(maxX - minX, maxY - minY);
            return size;
        }

        public static Vector3 GetCenterFromTilemaps(IEnumerable<Tilemap> tilemaps)
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

            Vector3 size = new((maxX + minX) / 2f, (maxY + minY) / 2f);
            return size;
        }
    }
}