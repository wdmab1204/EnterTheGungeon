using System.Collections.Generic;

namespace GameEngine.DataSequence.Graph
{
    public class Graph<TNode, TEdge>
        where TNode : INode
        where TEdge : IEdge<TNode>, new()
    {
        protected HashSet<TNode> nodeSet = new();
        protected Dictionary<TNode, List<TEdge>> adjacencyMap = new();

        public virtual TNode AddNode(TNode node)
        {
            nodeSet.Add(node);
            return node;
        }

        public virtual void AddEdge(TNode from, TNode to, bool isTwoWay = false)
        {
            TEdge edge = new();
            edge.From = from;
            edge.To = to;

            AddEdge(from, edge);

            if (isTwoWay)
                AddEdge(to, from, false);
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

        public List<TEdge> AllGetEdges()
        {
            List<TEdge> edgeList = new();
            foreach (var edges in adjacencyMap.Values)
                edgeList.AddRange(edges);

            return edgeList;
        }

        public void BFS(TNode node) => throw new System.NotImplementedException();
        public void DFS(TNode node) => throw new System.NotImplementedException();
    }
}