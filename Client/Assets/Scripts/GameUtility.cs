using GameEngine.DataSequence.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameEngine
{
    public static class GameUtility
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

        public static UnityEngine.Vector2Int ToVector2Int(this IGeomertyNode node)
            => new((int)node.X, (int)node.Y);

        public static void Destroy(UnityEngine.Object obj)
        {
            if (obj == null || obj.IsDestroyed())
                return;

            if(Application.isPlaying)
                GameObject.Destroy(obj);
            else
                GameObject.DestroyImmediate(obj);
        }

        public static LineRenderer CreateLineRenderer(Color color, float width)
        {
            LineRenderer lineRenderer = new GameObject("Line Renderer").AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
            lineRenderer.sharedMaterial.color = color;
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;

            return lineRenderer;
        }

        public static LineRenderer CreateLineRenderer(Color color, float width, params Vector3[] positions)
        {
            LineRenderer lineRenderer = CreateLineRenderer(color, width);
            lineRenderer.positionCount = positions.Length;
            for(int i=0; i<positions.Length - 1; i++)
            {
                lineRenderer.SetPosition(i, positions[i]);
                lineRenderer.SetPosition(i + 1, positions[i + 1]);
            }

            return lineRenderer;
        }

        public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default)
        {
            if (dictionary.TryGetValue(key, out TValue value))
                return value;
            dictionary.Add(key, defaultValue);
            return defaultValue;
        }

        public static BoundsInt GetBoundsIntFromTilemaps(IEnumerable<Tilemap> tilemaps)
        {
            var enumerator = tilemaps.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                throw new ArgumentException("Tilemaps collection is empty");
            }

            BoundsInt totalBounds = enumerator.Current.cellBounds;
            while (enumerator.MoveNext())
            {
                var tilemap = enumerator.Current;
                totalBounds.xMin = Mathf.Min(totalBounds.xMin, tilemap.cellBounds.xMin);
                totalBounds.yMin = Mathf.Min(totalBounds.yMin, tilemap.cellBounds.yMin);
                totalBounds.xMax = Mathf.Max(totalBounds.xMax, tilemap.cellBounds.xMax);
                totalBounds.yMax = Mathf.Max(totalBounds.yMax, tilemap.cellBounds.yMax);
                totalBounds.zMin = Mathf.Max(totalBounds.zMin, tilemap.cellBounds.zMin);
                totalBounds.zMax = Mathf.Max(totalBounds.zMax, tilemap.cellBounds.zMax);
            }

            return totalBounds;
        }

        public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
        {
            Transform transform = FindChild<Transform>(go, name, recursive);
            if (transform == null)
                return null;

            return transform.gameObject;
        }

        public static T FindChild<T>(GameObject go, string name = null, bool recursive = false)
            where T : UnityEngine.Object
        {
            if (go == null)
                return null;

            if (recursive == false)
            {
                for (int i = 0; i < go.transform.childCount; i++)
                {
                    Transform transform = go.transform.GetChild(i);
                    if (string.IsNullOrEmpty(name) || transform.name == name)
                    {
                        T component = transform.GetComponent<T>();
                        if (component != null)
                            return component;
                    }
                }
            }
            else
            {
                foreach (T component in go.GetComponentsInChildren<T>())
                {
                    if (string.IsNullOrEmpty(name) || component.name == name)
                        return component;
                }
            }

            return null;
        }

        public static Vector3? GetMouseWoirldPosition(Camera camera, Vector3 mousePosition)
        {
#if UNITY_EDITOR
            if (float.IsNaN(mousePosition.x)        ||
                float.IsNaN(mousePosition.y)        ||
                float.IsInfinity(mousePosition.x)   ||
                float.IsInfinity(mousePosition.y))
                return null;
#endif
            return camera.ScreenToWorldPoint(mousePosition);
        }

        public static readonly Vector3Int[] clockWiseDirections = new Vector3Int[]
        {
            Vector3Int.up,
            Vector3Int.right,
            Vector3Int.down,
            Vector3Int.left
        };

        public static List<List<Vector3Int>> CellsToOutline(HashSet<Vector3Int> pointSet)
        {
            HashSet<(Vector3Int, Vector3Int)> lineSet = new();
            Dictionary<Vector3Int, Vector3Int> lineConnectMap = new();

            foreach (var cellPosition in pointSet)
            {
                Vector3Int to = cellPosition;
                for (int i = 0; i < 4; i++)
                {
                    Vector3Int from = to;
                    to = from + clockWiseDirections[i];

                    if (lineSet.Remove((from, to)) || lineSet.Remove((to, from)))
                        continue;

                    lineSet.Add((from, to));
                }
            }

            foreach (var line in lineSet)
            {
                lineConnectMap[line.Item1] = line.Item2;
            }

            List<List<Vector3Int>> outlines = new();
            HashSet<Vector3Int> visited = new();

            foreach (var start in lineConnectMap.Keys)
            {
                if (visited.Contains(start))
                    continue;

                List<Vector3Int> outline = new();
                Vector3Int current = start;
                Vector3Int direction = Vector3Int.zero;

                do
                {
                    visited.Add(current);

                    if (!lineConnectMap.TryGetValue(current, out var next))
                        break;

                    var newDirection = next - current;

                    if (direction == Vector3Int.zero)
                    {
                        direction = newDirection;
                        outline.Add(current);  
                    }
                    else if (newDirection != direction)
                    {
                        outline.Add(current);
                        direction = newDirection;
                    }

                    current = next;
                }
                while (!visited.Contains(current));

                outline.Add(current); // 마지막 포인트 추가

                if (outline.Count > 1)
                    outlines.Add(outline);
            }

            return outlines;
        }

        public static HashSet<Vector3Int> AllGetTilePosition(Tilemap tilemap)
        {
            HashSet<Vector3Int> allPoints = new();

            foreach (var p in tilemap.cellBounds.allPositionsWithin)
            {
                if (tilemap.HasTile(p))
                    allPoints.Add(p);
            }

            return allPoints;
        }

        public static int GetRandomSeed() => UnityEngine.Random.Range(int.MinValue, int.MaxValue);
    }
}