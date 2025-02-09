namespace GameEngine.DataSequence.Graph
{
    public interface IEdge
    {
        INode To { get; set; } 
    }

    public class WeightEdge : IEdge
    {
        public INode To { get; set; }
        public int weight;
    }
}