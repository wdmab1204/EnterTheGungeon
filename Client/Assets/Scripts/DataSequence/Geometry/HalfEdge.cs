namespace GameEngine.DataSequence.Geometry
{
    public class HalfEdge
    {
        public Vertex v;
        public Triangle t;
        public HalfEdge nextEdge, prevEdge, oppositeEdge;

        public HalfEdge(Vertex v)
        {
            this.v = v;
        }
    }
}
