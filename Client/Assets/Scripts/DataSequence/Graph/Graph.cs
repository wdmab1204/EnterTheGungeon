using System;
using System.Collections.Generic;
using System.Linq;

namespace GameEngine.DataSequence.Graph
{
    public class Graph<TNode, TEdge>
        where TNode : INode
        where TEdge : IEdge<TNode>, new()
    {
        public HashSet<TNode> Vertices { get; private set; } = new();
        public IEnumerable<TEdge> Edges => adjacencyMap.Values.SelectMany(x => x);
        protected Dictionary<TNode, HashSet<TEdge>> adjacencyMap = new();

        public virtual TNode AddNode(TNode node)
        {
            Vertices.Add(node);
            return node;
        }

        public virtual void AddEdge(TNode from, TNode to, int weight, bool isTwoWay = false)
        {
            TEdge edge = new();
            edge.From = from;
            edge.To = to;

            AddEdge(from, edge);

            if (isTwoWay)
                AddEdge(to, from, weight, false);
        }

        public void AddEdge(TNode node, TEdge edge)
        {
            if (adjacencyMap.TryGetValue(node, out var list) == false)
            {
                adjacencyMap.Add(node, list = new HashSet<TEdge>() { edge });
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
    }
}