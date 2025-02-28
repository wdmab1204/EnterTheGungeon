using Assets.Scripts.DataSequence.Queue;
using GameEngine.DataSequence.Graph;
using System;
using System.Collections.Generic;

namespace GameEngine.DataSequence.PathFinding
{
    public interface IPathFinder<T>
    {
        IEnumerable<T> GetPath(T src, T dst);
    }
}
