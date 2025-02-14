using System.Collections;
using System.Linq;
using UnityEngine;

namespace GameEngine.Pipeline
{
    public class GraphRenderingTask : IPipelineTask<DungeonGeneratorPayLoad>
    {
        public DungeonGeneratorPayLoad PayLoad { get; set; }

        public IEnumerator Process()
        {
            var graph = PayLoad.DungeonGraph;

            foreach (var triangle in graph.GetTriangles())
            {
                Debug.Log($"{triangle.a},{triangle.b},{triangle.c}");
                var circleRenderer = new GameObject("Circle Renderer").AddComponent<LineRenderer>();
                const int count = 50;
                circleRenderer.positionCount = count + 2;
                var points = triangle.GetCircumCircle().GetPoints(count);
                for (int i = 0; i < points.Count; i += 2)
                {
                    var p1 = points[i];
                    var p2 = points[i + 1];
                    circleRenderer.SetPosition(i, p1);
                    circleRenderer.SetPosition(i + 1, p2);
                }

                circleRenderer.SetPosition(count, points.Last());
                circleRenderer.SetPosition(count + 1, points[0]);

                var triangleRenderer = new GameObject("Triangle Renderer").AddComponent<LineRenderer>();
                triangleRenderer.positionCount = 3;
                triangleRenderer.SetPosition(0, triangle.a);
                triangleRenderer.SetPosition(1, triangle.b);
                triangleRenderer.SetPosition(2, triangle.c);

                yield return null;
            }
        }
    }
}
