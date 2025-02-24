using System.Collections.Generic;
using UnityEngine;

namespace GameEngine.DataSequence.Graph
{
    public class DungeonGraph : GeomertyGraph<RoomNode, RoomEdge>
    {
        private Dictionary<Vector3, RoomNode> posToNodeMap = new();

        public override RoomNode AddNode(RoomNode node)
        {
            posToNodeMap.Add(node.ToVector3(), node);
            return base.AddNode(node);
        }

        public RoomNode GetNodeFromPos(Vector3 pos)
        {
            return posToNodeMap[pos];
        }
    }
}
