using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Assets.Scripts.DataSequence.Queue
{
    internal class PriorityQueue<T>
    {
        private SortedSet<T> sortedSet;

        public PriorityQueue()
        {
            sortedSet = new SortedSet<T>(Comparer<T>.Default);
        }

        public PriorityQueue(IComparer<T> comparer)
        {
            sortedSet = new SortedSet<T>(comparer);
        }

        public void Enqueue(T item)
        {
            if (sortedSet.Contains(item))
                UnityEngine.Debug.LogError($"Dup Item!!");
            sortedSet.Add(item);
        }

        public T Dequeue()
        {
            if (IsEmpty())
                throw new System.InvalidOperationException("PQ is empty");

            var highPriorityItem = sortedSet.Min;
            sortedSet.Remove(highPriorityItem);
            return highPriorityItem;
        }

        public bool IsEmpty() => sortedSet.Count <= 0;

        public bool Contain(T item)
        {
            return sortedSet.Contains(item);
        }

        public void Clear()
        {
            sortedSet.Clear();
        }
    }
}
