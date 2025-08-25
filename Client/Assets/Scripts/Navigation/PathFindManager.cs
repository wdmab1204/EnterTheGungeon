using Unity.Profiling;
using UnityEngine;

namespace GameEngine.Navigation
{
    public static class PathFindManager
    {
        public static PathFinding PathFinder { get; set; }
        private static ProfilerMarker pathFindMarker = new("PathFinding");

        public static PathResult GetPath(Vector3 start, Vector3 end)
        {
            using (pathFindMarker.Auto())
            {
                return PathFinder.FindPath(start, end);
            }
            
        }
    }
}