using UnityEngine;

namespace GameEngine
{
    public class MathUtility
    {
        public static bool IsIntersectTwoLine(Vector2 line1_From, Vector2 line1_To, Vector2 line2_From, Vector2 line2_To)
        {
            //Pa = P1 + Ua(P2 - P1);
            float denominator = (line2_To.y - line2_From.y) * (line1_To.x - line1_From.x) - (line2_To.x - line2_From.x) * (line1_To.y - line1_From.y);

            if (Mathf.Abs(denominator) < Mathf.Epsilon)
                return false;

            float u_a = ((line2_To.x - line2_From.x) * (line1_From.y - line2_From.y) - (line2_To.y - line2_From.y) * (line1_From.x - line2_From.x)) / denominator;
            float u_b = ((line1_To.x - line1_From.x) * (line1_From.y - line2_From.y) - (line1_To.y - line1_From.y) * (line1_From.x - line2_From.x)) / denominator;

            return u_a >= 0f && u_a <= 1f && u_b >= 0f && u_b <= 1f;
        }

        public static bool IsTriangleOrientedClockwise(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            bool isClockWise = true;

            float determinant = p1.x * p2.y + p3.x * p1.y + p2.x * p3.y - p1.x * p3.y - p3.x * p2.y - p2.x * p1.y;

            if (determinant > 0f)
            {
                isClockWise = false;
            }

            return isClockWise;
        }

        public static float IsAPointLeftOfVectorOrOnTheLine(Vector2 a, Vector2 b, Vector2 p)
        {
            float determinant = (a.x - p.x) * (b.y - p.y) - (a.y - p.y) * (b.x - p.x);

            return determinant;
        }

        public static bool IsPointInTriangle(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p)
        {
            bool isWithinTriangle = false;

            float denominator = ((p2.y - p3.y) * (p1.x - p3.x) + (p3.x - p2.x) * (p1.y - p3.y));

            float a = ((p2.y - p3.y) * (p.x - p3.x) + (p3.x - p2.x) * (p.y - p3.y)) / denominator;
            float b = ((p3.y - p1.y) * (p.x - p3.x) + (p1.x - p3.x) * (p.y - p3.y)) / denominator;
            float c = 1 - a - b;

            if (a > 0f && a < 1f && b > 0f && b < 1f && c > 0f && c < 1f)
            {
                isWithinTriangle = true;
            }

            return isWithinTriangle;
        }

        public static bool AABB(Vector3 r1Min, Vector3 r1Max, Vector3 r2Min, Vector3 r2Max)
        {
            return r1Max.x > r2Min.x && r1Min.x < r2Max.x && r1Max.y > r2Min.y && r1Min.y < r2Max.y;
        }
    }
}
