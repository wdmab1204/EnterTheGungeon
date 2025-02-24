using GameEngine.DataSequence.Graph;
using System;
using System.Linq;
using UnityEngine;

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

        public static LineRenderer CreateLineRenderer(Vector3 start, Vector3 end, Color color, float width)
        {
            LineRenderer lineRenderer = new GameObject("Line Renderer").AddComponent<LineRenderer>();
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);
            lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
            lineRenderer.sharedMaterial.color = color;
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;

            return lineRenderer;
        }
    }
}