namespace GameEngine.DataSequence.Graph
{
    public interface INode
    {
        int ID { get; set; }
    }

    public interface IGeomertyNode  : INode
    {
        float X { get; }
        float Y { get; }


    }
}