using GameEngine.DataSequence.Graph;
using System.Collections;
using UnityEngine;

namespace GameEngine.Pipeline
{
    class VertexRenderer : MonoBehaviour
    {
        private RoomNode node;

        public void Render(RoomNode node)
        {
            this.node = node;
        }

        private void OnDrawGizmos()
        {
            if (node == null)
                return;

            Gizmos.color = Color.gray;

            Gizmos.DrawWireCube(node.GetCenter(), new Vector3(node.Width, node.Height, 0));
        }
    }

    public class GraphRenderingTask : IPipelineTask<DungeonGeneratorPayLoad>
    {
        public DungeonGeneratorPayLoad PayLoad { get; set; }

        public IEnumerator Process()
        {
            var graph = PayLoad.DungeonGraph;
            foreach(var vertex in graph.Vertices)
            {
                VertexRenderer renderer = new GameObject("Vertex Renderer").AddComponent<VertexRenderer>();
                renderer.transform.parent = PayLoad.RootGameObject.transform;
                renderer.Render(vertex);
            }
            

            foreach (var edge in graph.Edges)
            {
                var line = GameUtil.CreateLineRenderer(Color.white, .2f, edge.From.ToVector3(), edge.To.ToVector3()).transform;
                line.parent = PayLoad.RootGameObject.transform;
                yield return null;
            }
        }
    }
}
