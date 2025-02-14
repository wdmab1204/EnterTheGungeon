using Unity.VisualScripting;
using UnityEngine;

namespace GameEngine.DataSequence.Shape
{
    public struct Rectangle
    {
        /// a--------d
        /// |        |
        /// |        |
        /// b--------c
        private Vector2 a, b, c, d;

        public Vector2 Min => b;
        public Vector2 Max => d;
        public Vector2 Center => (a + c) / 2;
        public float Width => d.x - a.x;
        public float Height => d.y - c.y;

        public Rectangle(Vector2 center, float width, float height)
        {
            a = new(center.x - width / 2f, center.y + height / 2f);
            b = new(center.x - width / 2f, center.y - height / 2f);
            c = new(center.x + width / 2f, center.y - height / 2f);
            d = new(center.x + width / 2f, center.y + height / 2f);
        }

        public bool IsColliding(Rectangle other)
        {
            return Min.x <= other.Max.x && Max.x >= other.Min.x && Min.y <= other.Max.y && Max.y >= other.Min.y;
        }
    }
}