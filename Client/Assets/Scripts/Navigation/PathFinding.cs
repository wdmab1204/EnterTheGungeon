using Cysharp.Threading.Tasks;
using GameEngine.DataSequence.Queue;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace GameEngine.Navigation
{
    public delegate UniTask<PathResult> PathDelegate(Vector3 start, Vector3 end, CancellationTokenSource token);

    public class PathRequest
    {
        public Vector3 start;
        public Vector3 end;
        public BoxCollider2D collider;
    }

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

    public struct NodeValue : IComparable<NodeValue>, IEquatable<NodeValue>
    {
        public float gCost;
        public float hCost;
        public float fCost => gCost + hCost;
        public Vector3 pos;
        public bool IsWalkable;

        public float X => pos.x;
        public float Y => pos.y;

        public int CompareTo(NodeValue other)
        {
            int compare = fCost.CompareTo(other.fCost);
            if (compare == 0)
                compare = hCost.CompareTo(other.hCost);

            return compare;
        }

        public bool Equals(NodeValue other)
        {
            return this.pos == other.pos;
        }

        public override int GetHashCode()
        {
            return pos.GetHashCode();
        }
    }

    public class PathFinding
    {
        private NavGrid navGrid;

        public PathFinding(NavGrid navGrid)
        {
            this.navGrid = navGrid;
        }

        public UniTask<PathResult> FindPathAsync(Vector3 start, Vector3 end, CancellationTokenSource token)
            => FindPathImpl(start, end, token);

        public IEnumerator FindPathToCoroutine(Vector3 start, Vector3 end, Action<PathResult> callback)
        {
            PathResult result = null;

            var it = FindPathImpl(start, end).ToCoroutine(r => result = r);
            while (it.MoveNext())
                yield return null;

            callback(result);
        }

        public PathResult FindPath(Vector3 start, Vector3 end)
        {
            PathResult result = null;

            UniTask<PathResult> task = FindPathImpl(start, end);
            var coroutine = task.ToCoroutine();
            while (coroutine.MoveNext()) { }

            if (task.Status == UniTaskStatus.Succeeded)
                result = task.GetAwaiter().GetResult();

            return result;
        }

        private async UniTask<PathResult> FindPathImpl(Vector3 start, Vector3 end, CancellationTokenSource token = null)
        {
            int safety = 100_000;

            HashSet<NodeValue> closedSet = new();
            Dictionary<Vector3, NodeValue> nodeContainer = new();
            PriorityQueue<NodeValue> queue = new();
            Dictionary<NodeValue, NodeValue> parentDict = new();

            var startNode = GetNode(start, nodeContainer);
            var endNode = GetNode(end, nodeContainer);
            bool success = false;

            if (startNode.IsWalkable == false || endNode.IsWalkable == false)
            {
                Debug.LogError("start or end is not walkable node");
                var result = new PathResult(false, null);
                return result;
            }

            NodeValue value = startNode;
            value.gCost = 0;
            nodeContainer[value.pos] = value;
            queue.Enqueue(value);
            DateTime cur = DateTime.UtcNow;
            while (queue.IsEmpty() == false && safety-- > 0)
            {
                var node = queue.Dequeue();
                closedSet.Add(node);

                if (node.Equals(endNode))
                {
                    success = true;
                    break;
                }

                foreach (NodeValue raw in GetNeighbours(node, nodeContainer))
                {
                    var neighbour = raw;

                    if (neighbour.IsWalkable == false || closedSet.Contains(neighbour))
                        continue;

                    // 대각선 체크
                    float dx = neighbour.X - node.X;
                    float dy = neighbour.Y - node.Y;

                    // 대각선 이동이라면 양 옆 직선 방향이 뚫려있는지 확인
                    if (dx != 0 && dy != 0)
                    {
                        Node side1 = navGrid.GetNode((int)node.Y, (int)(node.X + dx));     // 가로 방향
                        Node side2 = navGrid.GetNode((int)(node.Y + dy), (int)node.X);     // 세로 방향

                        if (side1 == null || side2 == null || !side1.IsWalkable || !side2.IsWalkable)
                            continue; // 코너 잘라먹기 방지
                    }

                    float distance = GetDistance(endNode, neighbour);
                    float newCostToNeighbour = node.gCost + distance;
                    if (nodeContainer.ContainsKey(neighbour.pos) == false || newCostToNeighbour < neighbour.gCost)
                    {
                        neighbour.gCost = newCostToNeighbour;
                        neighbour.hCost = distance;
                        parentDict[neighbour] = node;
                        nodeContainer[neighbour.pos] = neighbour;

                        if (!queue.Contain(neighbour))
                            queue.Enqueue(neighbour);
                    }

                    //await Yield(token);
                }
                await Yield(token);
            }
            //Debug.Log((DateTime.UtcNow - cur).TotalSeconds +$"[{100_000-safety}], {Time.deltaTime}");

            List<Vector3> path = null;
            if (success)
            {
                path = RetracePath(startNode, endNode, parentDict);
                path.Add(end); //도착지점 보정
            }

            PathResult pathResult = new PathResult(success, success ? path.ToArray() : null);
            return pathResult;
        }

        NodeValue GetNode(Vector3 worldPosition, Dictionary<Vector3, NodeValue> nodeContainer)
        {
            var localPosition = navGrid.GetLocalFromWorld(worldPosition);
            return GetNode((int)localPosition.x, (int)localPosition.y, nodeContainer);
        }

        NodeValue GetNode(int localX, int localY,  Dictionary<Vector3, NodeValue> nodeContainer)
        {
            Vector3 localPosition = new(localX, localY);
            if (nodeContainer.TryGetValue(localPosition, out NodeValue nodeValue))
                return nodeValue;

            var node = navGrid.GetNode(localY, localX);
            return CreateNode(node, nodeContainer);
        }

        NodeValue CreateNode(Node node, Dictionary<Vector3, NodeValue> nodeContainer)
        {
            if(node == null)
                throw new NullReferenceException(nameof(node));

            NodeValue nodeValue = new()
            {
                gCost = node.gCost,
                hCost = node.hCost,
                pos = node.ToVector3(),
                IsWalkable = node.IsWalkable,
            };

            nodeContainer.Add(node.ToVector3(), nodeValue);

            return nodeValue;
        }

        async UniTask Yield(CancellationTokenSource token)
        {
            if (token == null || token.IsCancellationRequested)
            {
                await UniTask.Yield();
                return;
            }

            await UniTask.Yield(token.Token);
        }

        IEnumerable<NodeValue> GetNeighbours(NodeValue nodeValue, Dictionary<Vector3, NodeValue> nodeContainer)
        {
            var node = navGrid.GetNode((int)nodeValue.Y, (int)nodeValue.X);
            return navGrid.GetNeighbours(node).Select(x => GetNode((int)x.X, (int)x.Y, nodeContainer));
        }

        List<Vector3> RetracePath(NodeValue startNode, NodeValue endNode, Dictionary<NodeValue, NodeValue> parentDict)
        {
            List<Vector3> path = new();
            NodeValue curNode = endNode;
            curNode.gCost = float.MaxValue;
            while(!curNode.Equals(startNode))
            {
                var localPosition = curNode.pos;
                var worldPosition = navGrid.GetWorldPositionFromLocal(localPosition, isCenter: true);
                path.Add(worldPosition);

                curNode = parentDict[curNode];
            }

            path.Reverse();
            return path;
        }

        float GetDistance(NodeValue nodeA, NodeValue nodeB)
        {
            float dstX = System.Math.Abs(nodeA.X - nodeB.X);
            float dstY = Mathf.Abs((int)nodeA.Y - (int)nodeB.Y);

            if (dstX > dstY)
                return 1.41f * dstY + 1f * (dstX - dstY);
            return 1.41f * dstX + 1f * (dstY - dstX);
        }
    }
}
