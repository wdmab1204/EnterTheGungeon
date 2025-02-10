namespace GameEngine.DataSequence.Graph
{
    public interface IEdge<in TNode> where TNode : INode
    {
        TNode From { set; }
        TNode To { set; } 
    }

    public interface IGeomeryEdge<in TNode> : IEdge<TNode> where TNode : IGeomertyNode
    {
        int Weight { get; set; }
    }

    public class WeightEdge : IGeomeryEdge<GeomertyNode>
    {
        public GeomertyNode From { get; set; }
        public GeomertyNode To { get; set; }
        public int Weight { get; set; }
    }
}