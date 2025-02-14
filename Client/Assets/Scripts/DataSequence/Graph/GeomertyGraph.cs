using GameEngine.DataSequence.Shape;
using System.Collections.Generic;

namespace GameEngine.DataSequence.Graph
{
    public class GeomertyGraph<TNode, TEdge> : Graph<TNode, TEdge>
        where TNode : IGeomertyNode, new()
        where TEdge : IGeomeryEdge<TNode>, new()
    {
        protected DelaunayTriangulation triangulator = new();

        public List<Triangle> GetTriangles() => triangulator.Triangles;

        public void AutoCreateEdges()
        {
            triangulator.Process();
        }

        public TNode AddNode(float x, float y)
        {
            TNode node = new();
            node.X = x;
            node.Y = y;

            return AddNode(node);
        }

        public override TNode AddNode(TNode node)
        {
            triangulator.AddVertex(node.ToVector3());
            return base.AddNode(node);
        }

        public void Clear()
        {
            triangulator.Clear();
        }
    }
}