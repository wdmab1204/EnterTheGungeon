using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameEngine.DataSequence.Graph
{
    class UnionFind<T> where T : IEquatable<T>
    {
        private Dictionary<T, T> parentMap = new Dictionary<T, T>();
        private Dictionary<T, int> rankMap = new Dictionary<T, int>();

        public void AddVertex(T vertex)
        {
            parentMap[vertex] = vertex;
            rankMap[vertex] = 0;
        }

        public T Find(T vertex)
        {
            if (parentMap[vertex].Equals(vertex) == false)
            {
                parentMap[vertex] = Find(parentMap[vertex]);
            }
            return parentMap[vertex];
        }

        public void Union(T vertex1, T vertex2)
        {
            T root1 = Find(vertex1);
            T root2 = Find(vertex2);

            if (root1.Equals(root2) == false)
            {
                if (rankMap[root1] > rankMap[root2])
                {
                    parentMap[root2] = root1;
                }
                else if (rankMap[root1] < rankMap[root2])
                {
                    parentMap[root1] = root2;
                }
                else
                {
                    parentMap[root2] = root1;
                    rankMap[root1]++;
                }
            }
        }
    }
}
