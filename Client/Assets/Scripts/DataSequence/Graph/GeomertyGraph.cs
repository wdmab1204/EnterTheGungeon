using GameEngine.DataSequence.Shape;
using System.Collections.Generic;
using UnityEngine;

namespace GameEngine.DataSequence.Graph
{
    public class GeomertyGraph<TNode, TEdge> : Graph<TNode, TEdge>
        where TNode : IGeomertyNode, new()
        where TEdge : IGeomeryEdge<TNode>, new()
    {
        private List<Triangle> triangles = new();

        public void AutoCreateEdges()
        {
            DelaunayTriangulation();
        }

        private void DelaunayTriangulation()
        {
            Triangle superTriangle = GetSuperTriangle();
            triangles.Add(superTriangle);
            AddNodeFromTriangle(superTriangle);

            Triangle GetSuperTriangle()
            {
                float minX = float.MaxValue;
                float maxX = float.MinValue;
                float minY = float.MaxValue;
                float maxY = float.MinValue;

                foreach(var node in nodeSet)
                {
                    minX = Mathf.Min(minX, node.X);
                    maxX = Mathf.Max(maxX, node.X);
                    minY = Mathf.Min(minY, node.Y);
                    maxY = Mathf.Max(maxY, node.Y);
                }

                float dx = maxX - minX;
                float dy = maxY - minY;

                Vector3 dot1 = new Vector3(minX - dx, minY - dy);
                Vector3 dot2 = new Vector3(minX - dx, maxY + dy * 3);
                Vector3 dot3 = new Vector3(maxX + dx * 3, minY - dy);

                if (dot1 == dot2 || dot2 == dot3 || dot3 == dot1)
                    throw new System.InvalidOperationException();

                return new Triangle(dot1, dot2, dot3);
            }
        }

        private void AddNodeFromTriangle(Triangle triangle)
        {
            var node1 = AddNode(triangle.a.x, triangle.a.y);
            var node2 = AddNode(triangle.b.x, triangle.b.y);
            var node3 = AddNode(triangle.c.x, triangle.c.y);

            AddEdge(node1, node2);
            AddEdge(node2, node3);
            AddEdge(node3, node1);
        }

        public TNode AddNode(float x, float y)
        {
            TNode node = new();
            node.X = x;
            node.Y = y;

            return AddNode(node);
        }
    }
}