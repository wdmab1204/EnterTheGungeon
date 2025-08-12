using GameEngine.DataSequence.Graph;
using GameEngine.DataSequence.Queue;
using System.Collections.Generic;
using UnityEngine;

namespace GameEngine.DataSequence.PathFinding
{
    //A star with minimum turns
    internal class DungeonRoadBuilder
    {
        private GameGrid grid;
        private GridCell destination;

        private PriorityQueue<GridCell> pq;
        protected int safety;

        public DungeonRoadBuilder(IComparer<GridCell> comparer, GameGrid grid)
        {
            this.grid = grid;
            this.safety = grid.gridBoundsInt.size.x * grid.gridBoundsInt.size.y * 10;
            pq = new PriorityQueue<GridCell>(comparer);
        }

        public IEnumerable<GridCell> GetMinPath(Vector3 from, Vector3 to)
        {
            grid.Clear();

            GridCell source = grid.GetCellFromWorldPosition(from);
            destination = grid.GetCellFromWorldPosition(to);
            return GetMinPath(source, destination);
        }

        //Get Shortest Path at minimum turns and ignore src and dst.
        public IEnumerable<GridCell> GetMinPath(GridCell src, GridCell dst)
        {
            //if (src.IsWalkable == false || dst.IsWalkable == false)
            //{
            //    Debug.LogError($"Source or Destination is Blocked : src : {src.ToVector3()}, dst : {dst.ToVector3()}");
            //    return null;
            //}

            List<GridCell> pathList = new();
            Dictionary<GridCell, GridCell> parentMap = new();
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
                        if (curNode.CellType == CellType.None)
                        {
                            curNode.CellType = CellType.Road;
                            curNode.IsWalkable = false;
                            pathList.Add(curNode);
                        }
                            
                        curNode = parentMap[curNode];
                    }

                    //pathList.Add(src);
                    pathList.Reverse();
                    return pathList;
                }

                foreach (var next in grid.GetNeighbors(curNode))
                {
                    if (pq.Contain(next))
                        continue;

                    if (next.IsWalkable == false)
                        continue;

                    //출발지점의 방과, 도착지점의 방을 제외한 모든 방은 탐색 대상에서 제외
                    if (next.CellType == CellType.Room && next.ID != src.ID && next.ID != dst.ID) 
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

            UnityEngine.Debug.LogError($"Can not found destination. src :  {src.ToVector3()}, dst : {dst.ToVector3()}");
            return null;
        }

        private bool IsTurning(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            float rad = MathUtility.GetRadianFrom3Points(v1, v2, v3);
            return rad < 2.96f && rad > 0.17f;
        }
    }
}
