using UnityEngine;

namespace GameEngine.Navigation
{
    public static class PathFindManager
    {
        public static PathFinding PathFinder { get; set; }

        public static PathResult GetPath(Vector3 start, Vector3 end) => PathFinder.FindPath(start, end);
    }
}