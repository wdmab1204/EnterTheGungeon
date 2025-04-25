using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameEngine.DataSequence.Geometry
{
    public static class Triangulation
    {
        public static List<Triangle> TriangulateByFlippingEdges(List<Vector3> sites)
        {
            List<Triangle> triangles = TriangulatePoints(sites.Select(p => new Vertex(p)).ToList());

            List<HalfEdge> halfEdges = TransformFromTriangleToHalfEdge(triangles);

            while (true)
            {
                bool hasFlippedEdge = false;

                for (int i = 0; i < halfEdges.Count; i++)
                {
                    HalfEdge thisEdge = halfEdges[i];

                    if (thisEdge.oppositeEdge == null)
                        continue;

                    Vertex a = thisEdge.v;
                    Vertex b = thisEdge.nextEdge.v;
                    Vertex c = thisEdge.prevEdge.v;
                    Vertex d = thisEdge.oppositeEdge.nextEdge.v;

                    Vector2 aPos = a.GetPos2D_XY();
                    Vector2 bPos = b.GetPos2D_XY();
                    Vector2 cPos = c.GetPos2D_XY();
                    Vector2 dPos = d.GetPos2D_XY();

                    if (GetDeterminantForCircumCircleInPoint(aPos, bPos, cPos, dPos) < 0f)
                    {
                        if (IsQuadrilateralConvex(aPos, bPos, cPos, dPos))
                        {
                            if (GetDeterminantForCircumCircleInPoint(bPos, cPos, dPos, aPos) < 0f)
                                continue;

                            hasFlippedEdge = true;
                            FlipEdge(thisEdge);
                        }
                    }
                }

                if (!hasFlippedEdge)
                    break;
            }

            return triangles;
        }

        private static void FlipEdge(HalfEdge one)
        {
            HalfEdge two = one.nextEdge;
            HalfEdge three = one.prevEdge;
            HalfEdge four = one.oppositeEdge;
            HalfEdge five = one.oppositeEdge.nextEdge;
            HalfEdge six = one.oppositeEdge.prevEdge;

            Vertex a = one.v;
            Vertex b = one.nextEdge.v;
            Vertex c = one.prevEdge.v;
            Vertex d = one.oppositeEdge.nextEdge.v;



            a.halfEdge = one.nextEdge;
            c.halfEdge = one.oppositeEdge.nextEdge;

            one.nextEdge = three;
            one.prevEdge = five;

            two.nextEdge = four;
            two.prevEdge = six;

            three.nextEdge = five;
            three.prevEdge = one;

            four.nextEdge = six;
            four.prevEdge = two;

            five.nextEdge = one;
            five.prevEdge = three;

            six.nextEdge = two;
            six.prevEdge = four;

            one.v = b;
            two.v = b;
            three.v = c;
            four.v = d;
            five.v = d;
            six.v = a;

            Triangle t1 = one.t;
            Triangle t2 = four.t;

            one.t = t1;
            three.t = t1;
            five.t = t1;

            two.t = t2;
            four.t = t2;
            six.t = t2;

            t1.v1 = b;
            t1.v2 = c;
            t1.v3 = d;

            t2.v1 = b;
            t2.v2 = d;
            t2.v3 = a;

            t1.halfEdge = three;
            t2.halfEdge = four;
        }

        private static float GetDeterminantForCircumCircleInPoint(Vector2 aVec, Vector2 bVec, Vector2 cVec, Vector2 dVec)
        {
            float a = aVec.x - dVec.x;
            float d = bVec.x - dVec.x;
            float g = cVec.x - dVec.x;

            float b = aVec.y - dVec.y;
            float e = bVec.y - dVec.y;
            float h = cVec.y - dVec.y;

            float c = a * a + b * b;
            float f = d * d + e * e;
            float i = g * g + h * h;

            float determinant = (a * e * i) + (b * f * g) + (c * d * h) - (g * e * c) - (h * f * a) - (i * d * b);

            return determinant;
        }

        private static bool IsQuadrilateralConvex(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            bool isConvex = false;

            bool abc = MathUtility.IsTriangleOrientedClockwise(a, b, c);
            bool abd = MathUtility.IsTriangleOrientedClockwise(a, b, d);
            bool bcd = MathUtility.IsTriangleOrientedClockwise(b, c, d);
            bool cad = MathUtility.IsTriangleOrientedClockwise(c, a, d);

            if (abc && abd && bcd & !cad)
                isConvex = true;

            else if (abc && abd && !bcd & cad)
                isConvex = true;

            else if (abc && !abd && bcd & cad)
                isConvex = true;

            else if (!abc && !abd && !bcd & cad)
                isConvex = true;

            else if (!abc && !abd && bcd & !cad)
                isConvex = true;

            else if (!abc && abd && !bcd & !cad)
                isConvex = true;

            return isConvex;
        }

        private static List<HalfEdge> TransformFromTriangleToHalfEdge(List<Triangle> triangles)
        {
            OrientTrianglesColckWise(triangles);

            List<HalfEdge> halfEdges = new List<HalfEdge>(triangles.Count * 3);

            for (int i = 0; i < triangles.Count; i++)
            {
                Triangle t = triangles[i];

                HalfEdge he1 = new HalfEdge(t.v1);
                HalfEdge he2 = new HalfEdge(t.v2);
                HalfEdge he3 = new HalfEdge(t.v3);

                he1.nextEdge = he2;
                he2.nextEdge = he3;
                he3.nextEdge = he1;

                he1.prevEdge = he3;
                he2.prevEdge = he1;
                he3.prevEdge = he2;

                he1.v.halfEdge = he2;
                he2.v.halfEdge = he3;
                he3.v.halfEdge = he1;

                t.halfEdge = he1;

                he1.t = t;
                he2.t = t;
                he3.t = t;

                halfEdges.Add(he1);
                halfEdges.Add(he2);
                halfEdges.Add(he3);
            }

            for (int i = 0; i < halfEdges.Count; i++)
            {
                HalfEdge he = halfEdges[i];

                Vertex goingToVertex = he.v;
                Vertex goingFromVertex = he.prevEdge.v;

                for (int j = 0; j < halfEdges.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    HalfEdge heOpposite = halfEdges[j];

                    if (goingFromVertex.position == heOpposite.v.position && goingToVertex.position == heOpposite.prevEdge.v.position)
                    {
                        he.oppositeEdge = heOpposite;

                        break;
                    }
                }
            }


            return halfEdges;
        }

        private static void OrientTrianglesColckWise(List<Triangle> triangles)
        {
            for (int i = 0; i < triangles.Count; i++)
            {
                Triangle tri = triangles[i];

                Vector2 v1 = new Vector2(tri.v1.position.x, tri.v1.position.y);
                Vector2 v2 = new Vector2(tri.v2.position.x, tri.v2.position.y);
                Vector2 v3 = new Vector2(tri.v3.position.x, tri.v3.position.y);

                if (!MathUtility.IsTriangleOrientedClockwise(v1, v2, v3))
                    tri.ChangeOrientation();
            }
        }

        public static List<Triangle> TriangulatePoints(List<Vertex> points)
        {
            var convexHullpoints = Polygon.GetConvexHull(points);
            var triangles = TriangulateConvexPolygon(convexHullpoints);

            foreach (var v in points)
            {
                var currentP = v;
                for (int i = 0; i < triangles.Count; i++)
                {
                    var tri = triangles[i];
                    var p1 = tri.v1.GetPos2D_XY();
                    var p2 = tri.v2.GetPos2D_XY();
                    var p3 = tri.v3.GetPos2D_XY();
                    var p = currentP.GetPos2D_XY();

                    if (MathUtility.IsPointInTriangle(p1, p2, p3, p))
                    {
                        var newTri1 = new Triangle(tri.v1, tri.v2, currentP);
                        var newTri2 = new Triangle(tri.v2, tri.v3, currentP);
                        var newTri3 = new Triangle(tri.v3, tri.v1, currentP);

                        triangles.Remove(tri);
                        triangles.Add(newTri1);
                        triangles.Add(newTri2);
                        triangles.Add(newTri3);
                        break;
                    }
                }
            }

            return triangles;
        }

        public static List<Triangle> TriangulateConvexPolygon(List<Vertex> convexHullpoints)
        {
            List<Triangle> triangles = new List<Triangle>();

            for (int i = 2; i < convexHullpoints.Count; i++)
            {
                Vertex a = convexHullpoints[0];
                Vertex b = convexHullpoints[i - 1];
                Vertex c = convexHullpoints[i];

                triangles.Add(new Triangle(a, b, c));
            }

            return triangles;
        }
    }
}
