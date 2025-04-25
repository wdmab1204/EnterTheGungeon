using GameEngine.DataSequence.Geometry;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameEngine.DataSequence.Geometry
{
    public interface ITriangle<T>
    {
        T v1 { get; set; }
        T v2 { get; set; }
        T v3 { get; set; }
    }

    public class Triangle : ITriangle<Vertex>
    {
        //    v3
        //   /  \
        //  /    \
        // /      \
        //v1- - - - v2
        public Vertex v1 { get; set; }
        public Vertex v2 { get; set; }
        public Vertex v3 { get; set; }

        public HalfEdge halfEdge { get; set; }

        public Triangle(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            this.v1 = new Vertex(v1);
            this.v2 = new Vertex(v2);
            this.v3 = new Vertex(v3);
        }

        public Triangle(Vertex v1, Vertex v2, Vertex v3)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
        }

        public bool Contain(Vertex v) => MathUtility.IsPointInTriangle(v1.position, v2.position, v3.position, v.position);

        public void ChangeOrientation()
        {
            Vertex t = v1;
            v1 = v2;
            v2 = t;
        }
    }
}