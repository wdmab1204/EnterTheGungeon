using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEngine.DataSequence.Graph
{
    public class RoomNode : GeomertyNode, IEquatable<RoomNode>
    {
        public float Width { get; set; }
        public float Height { get; set; }
        public GameObject Prefab { get; set; }

        public RoomNode(Vector3 position, float width, float height, GameObject prefab) : base(position)
        {
            Width = width;
            Height = height;
            Prefab = prefab;
        }

        public bool Equals(RoomNode other)
        {
            Vector3 myVec = this.ToVector3();
            Vector3 otherVec = other.ToVector3();
            return myVec == otherVec;
        }

        public override bool Equals(object obj)
        {
            if(obj is RoomNode node)
                return Equals(node);
            else
                return false;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + X.GetHashCode();
            hash = hash * 23 + Y.GetHashCode();
            return hash;
        }
    }

    public class RoadTileNode : IGeomertyNode, IPathNode, IEquatable<RoadTileNode>, IComparable<RoadTileNode>
    {
        public float gCost { get; set; } = float.MaxValue;
        public float hCost { get; set; }
        public int ID { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public bool IsWalkable { get; set; }
        public Vector3Int CellPosition { get; set; }

        public RoadTileNode(Vector2 pos, Vector3Int cellPosition)
        {
            X = pos.x;
            Y = pos.y;
            CellPosition = cellPosition;
        }

        public RoadTileNode()
        {
            X = 0f;
            Y = 0f;
            CellPosition = default;
        }

        public bool Equals(RoadTileNode other)
        {
            return this.ToVector3() == other.ToVector3();
        }

        public int CompareTo(RoadTileNode other)
        {
            int fCostComp = gCost.CompareTo(other.gCost);
            if (fCostComp == 0)
            {
                int xComp = X.CompareTo(other.X);
                if(xComp == 0)
                    return Y.CompareTo(other.Y);
                else
                    return xComp;
            }
            else
                return fCostComp;
        }
    }
}
