using System.Collections.Generic;
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

        public static bool IsIntersectLinePolygon(Vector2 line_from, Vector2 line_to, List<Vector2> polygon)
        {
            int count = polygon.Count;
            for (int i = 0; i < count; i++)
            {
                Vector2 start = polygon[i];
                Vector2 end = polygon[(i + 1) % count];
                if (IsIntersectTwoLine(line_from, line_to, start, end))
                    return true;
            }

            return false;
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

        public static float GetRadianFrom3Points(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            float x1 = v1.x;
            float x2 = v2.x;
            float x3 = v3.x;

            float y1 = v1.y;
            float y2 = v2.y;
            float y3 = v3.y;

            Vector2 AB = new Vector2(x2 - x1, y2 - y1);

            Vector2 BC = new Vector2(x3 - x2, y3 - y2);

            float dot = Vector2.Dot(AB, BC);
            float det = AB.x * BC.y - AB.y * BC.x;
            float rad = Mathf.Abs(Mathf.Atan2(det, dot));

            return rad;
        }

        public static bool IsPointInRectangle(Vector3 point, Vector3 rectPos, Vector3 rectSize)
        {
            bool isContains = true;
            if(rectPos.x > point.x)
                isContains = false;
            if(rectPos.y > point.y)
                isContains = false;
            if(rectPos.x + rectSize.x <= point.x)
                isContains = false;
            if(rectPos.y + rectSize.y <= point.y)
                isContains = false;

            return isContains;
        }

        public static int ClampListIndex(int index, int listSize)
        {
            index = ((index % listSize) + listSize) % listSize;

            return index;
        }
    }
}
