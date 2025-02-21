using UnityEngine;

namespace GameEngine.DataSequence.Geometry
{
    public class Edge
    {
        public Vertex v1, v2;

        public Edge(Vertex v1, Vertex v2)
        {
            this.v1 = v1;
            this.v2 = v2;
        }

        public Edge(Vector3 v1, Vector3 v2)
        {
            this.v1 = new Vertex(v1);
            this.v2 = new Vertex(v2);
        }
    }
}
