using System.Collections.Generic;

namespace GameEngine.DataSequence.Graph
{
    public class Graph<TNode, TEdge>
        where TNode : INode
        where TEdge : IEdge, new()
    {
        protected HashSet<TNode> nodeSet = new();
        protected Dictionary<TNode, List<TEdge>> adjacencyMap = new();

        public virtual void AddNode(TNode node)
        {
            nodeSet.Add(node);
        }

        public virtual void AddEdge(TNode from, TNode to)
        {
            TEdge edge = new()
            {
                To = to,
            };

            AddEdge(from, edge);
        }

        public void AddEdge(TNode node, TEdge edge)
        {
            if (adjacencyMap.TryGetValue(node, out var list) == false)
            {
                adjacencyMap.Add(node, list = new List<TEdge>() { edge });
            }
            else
            {
                list.Add(edge);
            }
        }

        public void BFS(TNode node) => throw new System.NotImplementedException();
        public void DFS(TNode node) => throw new System.NotImplementedException();
    }
}