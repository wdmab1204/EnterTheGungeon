

using UnityEngine;

namespace GameEngine.DataSequence.Shape
{
    public struct Circle
    {
        public Vector3 center;
        public float radius;

        public bool Contains(Vector3 point)
        {
            float distance = Vector3.Distance(center, point);
            return distance < radius;
        }
    }
}