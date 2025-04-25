using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameEngine.DataSequence.Graph
{
    public class DungeonGraph : GeomertyGraph<RoomNode, RoomEdge>
    {
        private Dictionary<Vector3, RoomNode> posToNodeMap = new();
        private float maxRoomPosX = 0, maxRoomPosY = 0, minRoomPosX = float.MaxValue, minRoomPosY = float.MaxValue;

        public override RoomNode AddNode(RoomNode node)
        {
            posToNodeMap.Add(node.ToVector3(), node);

            if (maxRoomPosX < node.X + node.Width)  maxRoomPosX = node.X + node.Width;
            if (maxRoomPosY < node.Y + node.Height) maxRoomPosY = node.Y + node.Height;
            if (minRoomPosX > node.X)               minRoomPosX = node.X;
            if (minRoomPosY > node.Y)               minRoomPosY = node.Y;
            
            return base.AddNode(node);
        }

        public BoundsInt GetBoundsInt(int gridCellSize)
        {
            BoundsInt boundsInt = new BoundsInt();
            boundsInt.size = 
                new Vector3Int(
                    Mathf.RoundToInt((maxRoomPosX - minRoomPosX) / gridCellSize),
                    Mathf.RoundToInt((maxRoomPosY - minRoomPosY) / gridCellSize),
                    1);
            boundsInt.min = Vector3Int.zero;
            return boundsInt;
        }

        public Vector3Int GetGridWorldPosition() => new((int)minRoomPosX, (int)minRoomPosY);

        public RoomNode GetNodeAtPos(Vector3 pos)
        {
            return posToNodeMap[pos];
        }

        //public RoomNode GetNodeFromPos(Vector3 pos)
        //{
            
        //}
    }
}
