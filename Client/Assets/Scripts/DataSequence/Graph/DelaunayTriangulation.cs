using GameEngine.DataSequence.Shape;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameEngine.DataSequence.Graph
{
    public class DelaunayTriangulation
    {
        private HashSet<Vector3> pointSet = new();
        private Triangle superTriangle;
        public List<Triangle> Triangles { get; private set; } = new();

        public event Action<Triangle> onCreated, onDestroyed;

        public void AddVertex(Vector3 point)
        {
            pointSet.Add(point);
        }

        public void Process()
        {
            superTriangle = GetSuperTriangle();
            var pointList = pointSet.ToList();
            //pointList.Sort((v1, v2) => v1.x.CompareTo(v2.x));
            
            foreach(var vertex in pointList)
            {
                Debug.Log("Process : " + vertex);
                ReCreateTriangle(vertex);
            }
        }

        private Triangle GetSuperTriangle()
        {
            float minX = float.MaxValue;
            float maxX = float.MinValue;
            float minY = float.MaxValue;
            float maxY = float.MinValue;

            foreach (var node in pointSet)
            {
                minX = Mathf.Min(minX, node.x);
                maxX = Mathf.Max(maxX, node.x);
                minY = Mathf.Min(minY, node.y);
                maxY = Mathf.Max(maxY, node.y);
            }

            float dx = maxX - minX;
            float dy = maxY - minY;

            Vector3 dot1 = new Vector3(minX - dx, minY - dy);
            Vector3 dot2 = new Vector3(minX - dx, maxY + dy * 3);
            Vector3 dot3 = new Vector3(maxX + dx * 3, minY - dy);

            if (dot1 == dot2 || dot2 == dot3 || dot3 == dot1)
            {
                Vector3 a = new Vector3(dot1.x, dot1.y + 1);
                Vector3 b = new Vector3(dot1.x - Mathf.Sqrt(3) / 2, dot1.y - 0.5f);
                Vector3 c = new Vector3(dot1.x + Mathf.Sqrt(3) / 2, dot1.y - 0.5f);
                return new Triangle(a, b, c);
            }

            return new Triangle(dot1, dot2, dot3);
        }

        private void ReCreateTriangle(Vector3 point)
        {
            Triangle nearestTri = Triangles.
                OrderBy(tri => Vector3.Distance(tri.GetCenter(), point)).
                FirstOrDefault();

            if (nearestTri == default)
                nearestTri = superTriangle;
             
            List<Triangle> newTris = new();
            newTris.Add(new Triangle(nearestTri.a, nearestTri.b, point));
            newTris.Add(new Triangle(nearestTri.b, nearestTri.c, point));
            newTris.Add(new Triangle(nearestTri.c, nearestTri.a, point));

            RemoveTriangle(nearestTri);

            foreach(var tri in newTris)
            {
                AddTriangle(tri);
                //await Task.Delay(1000);
            }

            foreach (var tri in newTris)
                TryFlip(tri);

            void TryFlip(Triangle tri)
            {
                foreach (var containedPoint in pointSet.Where(p => tri.GetCircumCircle().Contains(p)))
                {
                    if (containedPoint == tri.a || containedPoint == tri.b || containedPoint == tri.c)
                        continue;

                    if (Triangles.Any(tri => tri.HasPoint(containedPoint)) == false)
                        continue;

                    var tri2 = new Triangle(tri.b, tri.a, containedPoint);

                    if(tri.GetCircumCircle().Contains(containedPoint) || tri2.GetCircumCircle().Contains(tri.c))
                    {
                        Debug.Log($"Flipping : {tri}, {tri2}");

                        RemoveTriangle(tri);
                        RemoveTriangle(tri2);

                        var tri3 = new Triangle(containedPoint, tri.c, tri.a);
                        var tri4 = new Triangle(tri.c, containedPoint, tri.b);

                        AddTriangle(tri3);
                        AddTriangle(tri4);

                    }

                    //       tri.c               tri.c     
                    //       /   \               / | \     
                    //      /     \             /  |  \    
                    //  tri.a-----tri.b  => tri.a  |  tri.b
                    //      \     /             \  |  /    
                    //       \   /               \ | /     
                    //         p                   p       
                }
            }
        }

        private void AddTriangle(Triangle tri)
        {
            Triangles.Add(tri);
            onCreated(tri);
        }

        private void RemoveTriangle(Triangle tri)
        {
            Triangles.Add(tri);
            onDestroyed?.Invoke(tri);
        }

        public void Clear()
        {
            pointSet.Clear();
            Triangles.ForEach(tri=>onDestroyed?.Invoke(tri));
            Triangles.Clear();
        }
    }
}
