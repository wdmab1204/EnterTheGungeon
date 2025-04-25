using GameEngine.DataSequence.Graph;
using System.Collections.Generic;
using System;

namespace GameEngine.DataSequence.Tree
{
    internal static class SpanningTree
    {
        public static List<TEdge> TransformMininum<TNode, TEdge>(IEnumerable<TNode> Vertices, IEnumerable<TEdge> Edges)
            where TNode : INode, IEquatable<TNode>
            where TEdge : IEdge<TNode>, new()
        {
            List<TEdge> result = new List<TEdge>();
            List<TEdge> sortedEdges = new List<TEdge>(Edges);
            sortedEdges.Sort((e1, e2) => e1.Weight.CompareTo(e2.Weight));

            UnionFind<TNode> uf = new UnionFind<TNode>();
            foreach (var vertex in Vertices)
            {
                uf.AddVertex(vertex);
            }

            foreach (var edge in sortedEdges)
            {
                TNode rootFrom = uf.Find(edge.From);
                TNode rootTo = uf.Find(edge.To);

                if (rootFrom.Equals(rootTo) == false)
                {
                    result.Add(edge);
                    uf.Union(rootFrom, rootTo);
                }
            }

            return result;
        }
    }
}
