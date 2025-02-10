using System.Collections;
using UnityEngine;

namespace GameEngine.Pipeline
{
    public class GraphRenderingTask : IPipelineTask<DungeonGeneratorPayLoad>
    {
        public DungeonGeneratorPayLoad PayLoad { get; set; }

        public IEnumerator Process()
        {
            var graph = PayLoad.DungeonGraph;
            foreach (var edge in graph.AllGetEdges())
            {
                var lineRenderer = new GameObject("Edge Renderer").AddComponent<LineRenderer>();
                lineRenderer.SetPosition(0, edge.From.ToVector3());
                lineRenderer.SetPosition(1, edge.To.ToVector3());
                yield return null;
            }
        }
    }
}
