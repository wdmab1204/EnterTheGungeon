using GameEngine.DataSequence.Queue;
using System.Collections.Generic;
using UnityEngine;

namespace GameEngine.Navigation
{
    public delegate PathResult PathDelegate(Vector3 start, Vector3 end);

    public class PathResult
    {
        public bool success;
        public Vector3[] path;

        public PathResult(bool success, Vector3[] path)
        {
            this.success = success;
            this.path = path;
        }
    }

    public class PathFinding
    {
        private List<Node> openSet = new();
        private HashSet<Node> closedSet = new();
        private PriorityQueue<Node> queue = new();
        private Dictionary<Node, Node> parentDict = new();

        private NavGrid navGrid;

        public PathFinding(NavGrid navGrid)
        {
            this.navGrid = navGrid;
        }

        public PathResult FindPath(Vector3 start, Vector3 end)
        {
            int safety = 100_000;

            openSet.Clear();
            closedSet.Clear();
            queue.Clear();

            foreach (var key in parentDict.Keys)
            {
                key.gCost = float.MaxValue;
                key.hCost = 0f;
            }
            parentDict.Clear();

            var startNode = navGrid.GetNode(start);
            var endNode = navGrid.GetNode(end);
            bool success = false;

            if(startNode.IsWalkable == false || endNode.IsWalkable == false)
            {
                Debug.LogError("start or end is not walkable node");
                PathResult result = new PathResult(false, null);
                return result;
            }

            queue.Enqueue(startNode);
            startNode.gCost = 0;

            while (queue.IsEmpty() == false || safety-- > 0)
            {
                var node = queue.Dequeue();
                closedSet.Add(node);

                if(node.Equals(endNode))
                {
                    success = true;
                    break;
                }

                foreach (Node neighbour in navGrid.GetNeighbours(node))
                {
                    if (neighbour.IsWalkable == false || closedSet.Contains(neighbour))
                        continue;

                    float newCostToNeighbour = node.gCost + GetDistance(endNode, neighbour);
                    if(newCostToNeighbour < neighbour.gCost)
                    {
                        neighbour.gCost = newCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, endNode);
                        parentDict[neighbour] = node;

                        if (!queue.Contain(neighbour))
                            queue.Enqueue(neighbour);
                    }
                }
            }


            Vector3[] path = new Vector3[0];
            if (success)
                path = RetracePath(startNode, endNode);

            PathResult pathResult = new PathResult(success, path);
            return pathResult;
        }

        Vector3[] RetracePath(Node startNode, Node endNode)
        {
            List<Vector3> path = new();
            Node curNode = endNode;
            curNode.gCost = float.MaxValue;
            while(!curNode.Equals(startNode))
            {
                var localPosition = curNode.ToVector3();
                var worldPosition = navGrid.GetWorldPositionFromLocal(localPosition, isCenter: true);
                path.Add(worldPosition);

                curNode = parentDict[curNode];
                curNode.gCost = float.MaxValue;
            }

            path.Reverse();
            return path.ToArray();
        }

        float GetDistance(Node nodeA, Node nodeB)
        {
            float dstX = System.Math.Abs(nodeA.X - nodeB.X);
            float dstY = Mathf.Abs((int)nodeA.Y - (int)nodeB.Y);

            if (dstX > dstY)
                return 1.41f * dstY + 1f * (dstX - dstY);
            return 1.41f * dstX + 1f * (dstY - dstX);
        }
    }
}
