using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEngine.DataSequence.Graph
{
    public class RoomNode : GeomertyNode, IEquatable<RoomNode>
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public GameObject Prefab { get; set; }

        public RoomNode(Vector3 position, int width, int height, GameObject prefab) : base(position)
        {
            Width = width;
            Height = height;
            Prefab = prefab;
        }

        public Vector3 GetCenter()
        {
            return new Vector3(X + Width / 2f, Y + Height / 2f);
        }

        public Vector3 GetCenterInt()
        {
            var center = GetCenter();
            center.x = (int)center.x;
            center.y = (int)center.y;
            return center;
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

    public class GridCell : IGeomertyNode, IPathNode, IEquatable<GridCell>, IComparable<GridCell>
    {
        public float gCost { get; set; } = float.MaxValue;
        public float hCost { get; set; }
        public float fCost => gCost + hCost;
        public int ID { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public bool IsWalkable { get; set; }
        public Vector3Int CellPosition { get; set; }
        public float Weight { get; set; }

        public GridCell(Vector2 pos, Vector3Int cellPosition)
        {
            X = pos.x;
            Y = pos.y;
            CellPosition = cellPosition;
        }

        public GridCell()
        {
            X = 0f;
            Y = 0f;
            CellPosition = default;
        }

        public bool Equals(GridCell other)
        {
            return this.ToVector3() == other.ToVector3();
        }

        public int CompareTo(GridCell other)
        {
            int compare = fCost.CompareTo(other.fCost);
            if (compare == 0)
                compare = hCost.CompareTo(other.hCost);

            return compare;
        }
    }
}
