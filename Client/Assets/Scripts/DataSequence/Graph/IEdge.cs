using System;

namespace GameEngine.DataSequence.Graph
{
    public interface IEdge<TNode> where TNode : INode
    {
        TNode From { get;  set; }
        TNode To { get;  set; } 
        float Weight { get; set; }
    }

    public class RoomEdge : IEdge<RoomNode>, IComparable<RoomEdge>
    {
        public RoomNode From { get; set; }
        public RoomNode To { get; set; }
        public float Weight { get; set; }

        public RoomEdge(RoomNode from, RoomNode to, float weight)
        {
            From = from;
            To = to;
            Weight = weight;
        }

        public RoomEdge()
        {
            
        }

        public int CompareTo(RoomEdge other)
        {
            return Weight.CompareTo(other.Weight);
        }
    }

    public class RoadEdge : IEdge<RoadTileNode>
    {
        public RoadTileNode From { get; set; }
        public RoadTileNode To { get; set; }
        public float Weight { get; set; }
    }
}