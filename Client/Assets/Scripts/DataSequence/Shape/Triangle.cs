using System.Collections.Generic;
using UnityEngine;

namespace GameEngine.DataSequence.Shape
{
    public struct Triangle
    {
        //    c
        //   /  \
        //  /    \
        // /      \
        //a- - - - b
        public Vector3 a, b, c;

        public Triangle(Vector3 a, Vector3 b, Vector3 c)
        {
            this.a = a; this.b = b; this.c = c;
        }

        public Circle GetCircumCircle()
        {
            if (a == b || b == c || c == a)
                throw new System.InvalidOperationException();

            float mab = (b.x - a.x) / (b.y - a.y) * -1f;
            Vector2 midAB = new(b.x + a.x / 2f, b.y + a.y / 2f);

            float mbc = (b.x - c.x) / (b.y - c.y) * -1f;
            Vector2 midBC = new(b.x + c.x / 2f, b.y + c.y / 2f);

            if(mab == mbc)
                throw new System.InvalidOperationException();

            float x = (mab * midAB.x - mbc * midAB.y + midBC.y - midAB.y) / (mab - mbc);
            float y = mab * (x - midAB.x) + midAB.y;

            Vector3 center = new Vector3(x, y, 0.0f);
            float radius = Vector3.Distance(center, a);

            return new Circle() { center = center, radius = radius };
        }
    }
}