using GameEngine.Pipeline;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameEngine.DataSequence.Graph
{
    public class RoomNode : GeomertyNode, IEquatable<RoomNode>
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public bool HasMob => roomInstance.Mobs.Count > 0;

        private RoomInstance roomInstance;

        public RoomNode(Vector3 position, int width, int height, RoomInstance roomInstance) : base(position)
        {
            Width = width;
            Height = height;
            this.roomInstance = roomInstance;
        }

        public Vector3 GetCenter()
        {
            return new Vector3(X + Width / 2f, Y + Height / 2f);
        }

        public Vector3 GetSize()
        {
            return new Vector3(Width, Height);
        }

        public IEnumerable<Tilemap> GetTilemaps() => roomInstance.Tilemaps.Values;
        public Tilemap GetTilemap(string key) => roomInstance.Tilemaps[key];

        public bool Equals(RoomNode other)
        {
            bool compare = this.ID == other.ID;
            if(compare == false)
                compare = this.ToVector3() == other.ToVector3();

            return compare;
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

    public enum CellType
    {
        None,
        Room,
        Road
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
        public CellType CellType { get; set; }

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
            bool compare = this.ID == other.ID;
            if(compare == false)
                compare = this.ToVector3() == other.ToVector3();

            return compare;
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
