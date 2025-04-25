namespace GameEngine.DataSequence.Graph
{
    public class GeomertyGraph<TNode, TEdge> : Graph<TNode, TEdge>
        where TNode : IGeomertyNode
        where TEdge : IEdge<TNode>, new()
    {
        public override TNode AddNode(TNode node)
        {
            return base.AddNode(node);
        }
    }
}