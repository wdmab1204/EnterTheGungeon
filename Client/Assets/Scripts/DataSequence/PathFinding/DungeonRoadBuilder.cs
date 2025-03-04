using Assets.Scripts.DataSequence.Queue;
using GameEngine.DataSequence.Graph;
using System.Collections.Generic;
using UnityEngine;

namespace GameEngine.DataSequence.PathFinding
{
    //A star with minimum turns
    internal class DungeonRoadBuilder
    {
        private GameGrid grid;
        private RoadTileNode destination;
        private Dictionary<RoadTileNode, Vector3> directions = new();

        private PriorityQueue<RoadTileNode> pq;
        protected int safety;

        public DungeonRoadBuilder(IComparer<RoadTileNode> comparer, GameGrid grid)
        {
            this.grid = grid;
            this.safety = grid.gridBoundsInt.size.x * grid.gridBoundsInt.size.y * 10;
            pq = new PriorityQueue<RoadTileNode>(comparer);
        }

        public IEnumerable<RoadTileNode> GetMinPath(Vector3 from, Vector3 to)
        {
            directions.Clear();
            grid.Clear();

            RoadTileNode source = grid.GetTile(from);
            destination = grid.GetTile(to);
            directions[source] = Vector3.zero;
            return GetMinPath(source, destination);
        }

        //Get Shortest Path at minimum turns and ignore src and dst.
        public IEnumerable<RoadTileNode> GetMinPath(RoadTileNode src, RoadTileNode dst)
        {
            if (src.IsWalkable == false || dst.IsWalkable == false)
            {
                Debug.LogError($"Source or Destination is Blocked : src : {src.ToVector3()}, dst : {dst.ToVector3()}");
                return null;
            }

            List<RoadTileNode> pathList = new();
            Dictionary<RoadTileNode, RoadTileNode> parentMap = new();
            pq.Clear();

            src.gCost = 0;
            pq.Enqueue(src);

            while (!pq.IsEmpty() && safety > 0)
            {
                var curNode = pq.Dequeue();

                if (curNode.Equals(dst))
                {
                    curNode = parentMap[curNode];
                    while (!curNode.Equals(src))
                    {
                        pathList.Add(curNode);
                        curNode = parentMap[curNode];
                    }

                    pathList.Reverse();
                    return pathList;
                }

                foreach (var next in grid.GetNeighbors(curNode))
                {
                    if (pq.Contain(next))
                        continue;

                    if (next.IsWalkable == false)
                        continue;

                    float nextHCost = Vector3.Distance(dst.ToVector3(), next.ToVector3());
                    if (IsTurning(parentMap.GetValueOrDefault(curNode, curNode).ToVector3(), curNode.ToVector3(), next.ToVector3()))
                    {
                        nextHCost += 50f;
                    }
                    float nextGCost = curNode.gCost + nextHCost + next.Weight;
                    if (nextGCost < next.gCost)
                    {
                        next.gCost = nextGCost;
                        next.hCost = nextHCost;
                        parentMap[next] = curNode;
                        if (!pq.Contain(next))
                            pq.Enqueue(next);
                    }
                }

                safety--;
            }

            if (safety <= 0)
                UnityEngine.Debug.LogError("Over Count Safety");

            UnityEngine.Debug.LogError("Can not found destination");
            return null;
        }

        private bool IsTurning(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            float x1 = v1.x;
            float x2 = v2.x;
            float x3 = v3.x;

            float y1 = v1.y;
            float y2 = v2.y;
            float y3 = v3.y;

            Vector2 AB = new Vector2(x2 - x1, y2 - y1);

            Vector2 BC = new Vector2(x3 - x2, y3 - y2);

            float dot = Vector2.Dot(AB, BC);
            float det = AB.x * BC.y - AB.y * BC.x;
            float angle = Mathf.Abs(Mathf.Atan2(det, dot));

            return angle < 2.96f && angle > 0.17f;
        }
    }
}
