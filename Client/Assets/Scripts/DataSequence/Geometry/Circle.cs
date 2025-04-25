using System.Collections.Generic;
using UnityEngine;

namespace GameEngine.DataSequence.Geometry
{
    public struct Circle
    {
        public Vector3 center;
        public float radius;

        public bool Contains(Vector3 point)
        {
            float distance = Vector3.Distance(center, point);
            return distance < radius;
        }

        public List<Vector3> GetPoints(int count)
        {
            float unitRad = Mathf.PI * 2f / count;
            float startRad = 0f;

            List<Vector3> points = new();

            for (int i = 0; i < count; i++)
            {
                Vector3 point = new(center.x + Mathf.Cos(startRad) * radius, center.y + Mathf.Sin(startRad) * radius);
                points.Add(point);
                startRad += unitRad;
            }

            return points;
        }
    }
}