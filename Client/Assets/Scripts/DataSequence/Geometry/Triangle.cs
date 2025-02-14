using GameEngine.DataSequence.Geometry;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameEngine.DataSequence.Shape
{
    public struct Triangle : IEquatable<Triangle>
    {
        static int id = 1;

        //    c
        //   /  \
        //  /    \
        // /      \
        //a- - - - b
        public Vector3 a, b, c;
        public int ID;

        public Triangle(Vector3 a, Vector3 b, Vector3 c)
        {
            this.a = a; this.b = b; this.c = c;
            this.ID = id++;

            float denominator = 2 * (this.a.x * (b.y - c.y) + b.x * (c.y - a.y) + c.x * (a.y - b.y));

            if (Math.Abs(denominator) < 1e-6)
            {
                throw new ArgumentException($"세 점이 일직선 상에 있어 외접원을 구할 수 없습니다. {a}, {b}, {c}");
            }
        }

        public List<Edge> GetEdges() =>
            new() { new() { from = a, to = b }, new() { from = b, to = c }, new() { from = c, to = a } };

        public Circle GetCircumCircle()
        {
            float denominator = 2 * (this.a.x * (b.y - c.y) + b.x * (c.y - a.y) + c.x * (a.y - b.y));

            if (Math.Abs(denominator) < 1e-6)
            {
                throw new ArgumentException($"세 점이 일직선 상에 있어 외접원을 구할 수 없습니다. {a}, {b}, {c}");
            }

            float centerX = ((this.a.sqrMagnitude * (b.y - c.y)) + (b.sqrMagnitude * (c.y - a.y)) + (c.sqrMagnitude * (a.y - b.y))) / denominator;
            float centerY = ((this.a.sqrMagnitude * (c.x - b.x)) + (b.sqrMagnitude * (a.x - c.x)) + (c.sqrMagnitude * (b.x - a.x))) / denominator;

            Vector2 center = new Vector2(centerX, centerY);
            float radius = Vector2.Distance(center, a);

            return new() { center = center, radius = radius };
        }

        public Vector3 GetCenter() => (a + b + c) / 3f;

        public bool HasPoint(Vector3 point) => a == point || b == point || c == point;

        public static bool operator ==(Triangle t1, Triangle t2)
        {
            return t1.a == t2.a && t1.b == t2.b && t1.c == t2.c;
        }

        public static bool operator !=(Triangle t1, Triangle t2)
        {
            return !(t1 == t2);
        }

        public override bool Equals(object obj)
        {
            if (obj is Triangle other)
                return Equals(other);
            else
                return false;
        }

        public bool Equals(Triangle other) => this == other;

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + a.GetHashCode();
                hash = hash * 23 + b.GetHashCode();
                hash = hash * 23 + c.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return $"{a}, {b}, {c}";
        }
    }
}