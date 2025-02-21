namespace GameEngine.DataSequence.Graph
{
    public class GeomertyGraph<TNode, TEdge> : Graph<TNode, TEdge>
        where TNode : IGeomertyNode, new()
        where TEdge : IEdge<TNode>, new()
    {
        public TNode AddNode(float x, float y)
        {
            TNode node = new();
            node.X = x;
            node.Y = y;

            return AddNode(node);
        }

        public override TNode AddNode(TNode node)
        {
            return base.AddNode(node);
        }
    }
}