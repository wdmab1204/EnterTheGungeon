using System;
using TMPro;
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
}
