using UnityEngine;

namespace GameEngine.DataSequence.Geometry
{
    public class Vertex
    {
        public Vector3 position;
        public HalfEdge halfEdge;
        public Triangle triangle;

        public Vector2 GetPos2D_XY() => new Vector2(position.x, position.y);

        public Vertex(Vector3 position)
        {
            this.position = position;
        }
    }
}
