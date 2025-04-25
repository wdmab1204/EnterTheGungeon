using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameEngine.DataSequence.Geometry
{
    internal class Polygon
    {
        public static List<Vertex> GetConvexHull(List<Vertex> vertices)
        {
            var minIndex = vertices.Select((v, i) => new { v, i })
            .OrderBy(tuple => tuple.v.position.y).
            ThenBy(tuple => tuple.v.position.x).
            First().i;

            var tmp = vertices[0];
            vertices[0] = vertices[minIndex];
            vertices[minIndex] = tmp;

            List<Vertex> sortedList = new();
            sortedList.Capacity = vertices.Count - 1;
            for (int i = 1; i < vertices.Count; i++)
                sortedList.Add(vertices[i]);

            var minVertex = vertices[0];
            sortedList.Sort((v1, v2) =>
            {
                var ccw = MathUtility.IsAPointLeftOfVectorOrOnTheLine(v1.GetPos2D_XY(), v2.GetPos2D_XY(), minVertex.GetPos2D_XY());
                if (Mathf.Abs(ccw) < Mathf.Epsilon)
                    return Vector3.Distance(v1.position, v2.position) < 0 ? 1 : -1;
                else
                    return ccw < 0f ? 1 : -1;
            });

            sortedList.Insert(0, minVertex);

            //store indices
            Stack<int> stack = new();
            stack.Push(0);
            stack.Push(1);
            int next = 2;

            while (next < vertices.Count)
            {
                while (stack.Count >= 2)
                {
                    int second = stack.Pop();
                    int first = stack.Peek();

                    var firstPos = sortedList[first].GetPos2D_XY();
                    var secondPos = sortedList[second].GetPos2D_XY();
                    var nextPos = sortedList[next].GetPos2D_XY();

                    if (MathUtility.IsTriangleOrientedClockwise(firstPos, secondPos, nextPos) == false)
                    {
                        stack.Push(second);
                        break;
                    }
                }

                stack.Push(next++);
            }

            return stack.Select(i => sortedList[i]).ToList();
        }
    }
}
