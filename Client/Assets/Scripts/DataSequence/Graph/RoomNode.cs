using System;
using TMPro;
using UnityEngine;

namespace GameEngine.DataSequence.Graph
{
    public class RoomNode : GeomertyNode, IEquatable<RoomNode>
    {
        public RoomNode(Vector3 v) : base(v)
        {
        }

        public RoomNode() : base(Vector3.zero)
        {

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
