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

        public static List<Vertex> GetConvexHullBruteForce(List<Vertex> vertices)
        {
            // 입력 리스트에 3개 미만의 점이 있다면, 그대로 반환합니다. 
            // 볼록 껍질을 형성할 수 없습니다.
            if (vertices.Count < 3)
            {
                return new List<Vertex>(vertices);
            }

            // 1. 볼록 껍질의 시작점, 즉 가장 왼쪽 아래 점을 찾습니다.
            // 이는 Convex Hull에 반드시 포함됩니다.
            Vertex startPoint = vertices
                .OrderBy(v => v.position.x)
                .ThenBy(v => v.position.y)
                .First();

            List<Vertex> hull = new List<Vertex>();
            Vertex currentPoint = startPoint;
            Vertex nextPoint;

            // Convex Hull을 구성하는 점들을 찾습니다.
            do
            {
                hull.Add(currentPoint);

                nextPoint = (currentPoint != vertices[0]) ? vertices[0] : vertices[1];

                foreach (Vertex candidate in vertices)
                {
                    if (candidate == currentPoint)
                    {
                        continue; // 현재 점은 건너뜁니다.
                    }

                    float ccw = MathUtility.IsAPointLeftOfVectorOrOnTheLine(
                        currentPoint.GetPos2D_XY(),
                        candidate.GetPos2D_XY(),
                        nextPoint.GetPos2D_XY());

                    if (ccw > Mathf.Epsilon)
                    {
                        // candidate가 현재 nextPoint보다 더 "왼쪽" (CCW 방향)에 있습니다.
                        // candidate를 새로운 nextPoint로 설정합니다.
                        nextPoint = candidate;
                    }
                    else if (Mathf.Abs(ccw) < Mathf.Epsilon)
                    {
                        // 세 점이 일직선 상에 있습니다.
                        // 더 먼 점을 선택하여 껍질에 있는 불필요한 중간 점을 제거합니다.
                        // Brute Force 특성상 두 점이 같은 각도를 가지는 경우, 
                        // 볼록 껍질의 바깥쪽 끝에 있는 점을 선택합니다.

                        float distSq_candidate = Vector3.SqrMagnitude(candidate.position - currentPoint.position);
                        float distSq_next = Vector3.SqrMagnitude(nextPoint.position - currentPoint.position);

                        if (distSq_candidate > distSq_next)
                        {
                            nextPoint = candidate;
                        }
                    }
                }

                // 3. 다음 점을 현재 점으로 업데이트합니다.
                currentPoint = nextPoint;

            } while (currentPoint != startPoint); // 시작점으로 돌아올 때까지 반복합니다.

            return hull;
        }
    }
}
