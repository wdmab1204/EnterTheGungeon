using UnityEngine;

namespace GameEngine.DataSequence.Geometry
{
    public class Edge<T>
    {
        public T v1, v2;

        public Edge(T v1, T v2)
        {
            this.v1 = v1;
            this.v2 = v2;
        }
    }

    public class Edge : Edge<Vertex>
    {
        public Edge(Vector3 v1, Vector3 v2) : base(new Vertex(v1), new Vertex(v2))
        {
        }
    }
}
