using GameEngine.DataSequence.Graph;
using GameEngine.Pipeline;
using System.Collections.Generic;

namespace GameEngine
{
    public class DungeonGeneratorLevel
    {
        public IEnumerable<RoomNode> Rooms { get; set; }
        public IEnumerable<RoomEdge> RoadEdges { get; set; }
        public Dictionary<RoomNode, RoomInstance> LayoutData { get; set; }
        public int GridCellSize { get; set; }
    }
}