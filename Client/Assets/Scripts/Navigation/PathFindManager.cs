using Cysharp.Threading.Tasks;
using System.Threading;
using Unity.Profiling;
using UnityEngine;

namespace GameEngine.Navigation
{
    public static class PathFindManager
    {
        public static DungeonNavigation PathFinder { get; set; }
        private static ProfilerMarker pathFindMarker = new("PathFinding");

        public static PathResult GetPath(Vector3 start, Vector3 end)
        {
            using (pathFindMarker.Auto())
            {
                return PathFinder.FindPath(start, end);
            }
        }

        public static async UniTask<PathResult> GetPathAsync(Vector3 start, Vector3 end, CancellationTokenSource token)
        {
            PathResult result;
            result = await PathFinder.FindPathAsync(start, end, token);
            return result;
        }
    }
}