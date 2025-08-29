using GameEngine.DataSequence.Graph;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameEngine.Navigation
{
    public class Node : IGeomertyNode, IPathNode, IEquatable<Node>, IComparable<Node>
    {
        public bool IsWalkable { get; set; } = false;
        public float gCost { get; set; } = float.MaxValue;
        public float hCost;
        public float fCost => gCost + hCost;

        public int ID { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public int ProcessID { get; set; }

        public Node(Vector2Int position)
        {
            X = position.x;
            Y = position.y;
            ID = position.GetHashCode();
        }

        public bool Equals(Node other)
        {
            return this.X == other.X && this.Y == other.Y;
        }

        public int CompareTo(Node other)
        {
            int compare = fCost.CompareTo(other.fCost);
            if (compare == 0)
                compare = hCost.CompareTo(other.hCost);

            return compare;
        }
    }

    public class NavGrid : MonobehaviourExtension
    {
        private Node[,] grid;
        private int width;
        private int height;
        private Dictionary<Node, List<Node>> neighbourCache = new();
        public bool IsGizmos { get; set; }

        public void CreateGrid(Tilemap floorTilemap, Tilemap collideableTilemap)
        {
            BoundsInt bounds = floorTilemap.cellBounds;
            Debug.Log("Floor Bounds : " + bounds);
            width = bounds.position.x + bounds.size.x;
            height = bounds.position.y + bounds.size.y;
            grid = new Node[height, width];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector2Int cellPos2 = new(x + bounds.xMin - bounds.position.x, y + bounds.yMin - bounds.position.y);
                    Vector3Int cellPos3 = (Vector3Int)cellPos2;

                    bool hasFloor = floorTilemap.HasTile(cellPos3);
                    //bool hasWall = collideableTilemap.HasTile(cellPos3);

                    grid[y, x] = new Node(cellPos2);
                    grid[y, x].IsWalkable = hasFloor;

                }
            }
        }

        public Node GetNode(int localY, int localX)
        {
            if (localY < 0 || localY >= height || localX < 0 || localX >= width)
                return null;

            return grid[localY, localX];
        }

        public Node GetNode(Vector3 worldPosition)
        {
            var localPosition = GetLocalFromWorld(worldPosition);
            return GetNode((int)localPosition.y, (int)localPosition.x);
        }

        public Vector3 GetLocalFromWorld(Vector3 worldPosition)
        {
            var worldToLocalMatrix = Matrix4x4.TRS(Transform.position, Transform.rotation, Transform.localScale).inverse;
            var localPosition = worldToLocalMatrix.MultiplyPoint3x4(worldPosition);
            return localPosition;
        }

        public Vector3 GetWorldPositionFromLocal(Vector3 localPosition, bool isCenter = false)
        {
            var localToWorldMatrix = Matrix4x4.TRS(Transform.position, Transform.rotation, Transform.localScale);
            var worldPosition = localToWorldMatrix.MultiplyPoint3x4(localPosition);

            if (isCenter)
            {
                worldPosition.x += .5f;
                worldPosition.y += .5f;
            }

            return worldPosition;
        }

        public IEnumerable<Node> GetNeighbours(Node node)
        {
            List<Node> neighbours = neighbourCache.GetValueOrDefault(node, null);
            if (neighbours != null)
            {
                return neighbours;
            }

            neighbours = new();

            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    int neighborX = (int)node.X + x;
                    int neighborY = (int)node.Y + y;

                    if (neighborX < 0 || neighborX >= width || neighborY < 0 || neighborY >= height)
                        continue;

                    neighbours.Add(grid[neighborY, neighborX]);
                }
            }
            neighbourCache[node] = neighbours;

            return neighbours;
        }

        //private void OnDrawGizmos()
        //{
        //    if (grid == null)
        //        return;

        //    foreach (var tile in grid)
        //    {
        //        if (tile == null)
        //            continue;

        //        if(tile.gCost > 200)
        //        {
        //            Gizmos.color = tile.IsWalkable ? Color.blue : Color.red;
        //            Gizmos.DrawWireCube(GetWorldPositionFromLocal(tile.ToVector3(), true), Vector3.one);
        //        }
        //        else
        //        {
        //            Gizmos.color = Color.Lerp(Color.white, Color.black, tile.gCost / 200);
        //            Gizmos.DrawCube(GetWorldPositionFromLocal(tile.ToVector3(), true), Vector3.one);
        //        }
        //    }
        //}
    }
}
