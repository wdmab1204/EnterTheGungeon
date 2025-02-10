namespace GameEngine.DataSequence.Graph
{
    public interface INode
    {
        int ID { get; set; }
    }

    public interface IGeomertyNode  : INode
    {
        float X { get; set; }
        float Y { get; set; }
    }

    public class GeomertyNode : IGeomertyNode
    {
        public float X { get; set; }

        public float Y { get; set; }

        public int ID { get; set; }
    }
}