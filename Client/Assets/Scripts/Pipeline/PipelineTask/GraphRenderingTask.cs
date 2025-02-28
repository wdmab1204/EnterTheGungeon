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

            foreach (var edge in graph.Edges)
            {
                var line = GameUtil.CreateLineRenderer(Color.white, .2f, edge.From.ToVector3(), edge.To.ToVector3()).transform;
                line.parent = PayLoad.RootGameObject.transform;
                yield return null;
            }
        }
    }
}
