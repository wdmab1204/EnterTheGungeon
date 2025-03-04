using GameEngine.DataSequence.Graph;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameEngine
{
    public class GameGrid : MonoBehaviour
    {
        public bool isGizmos;

        private RoadTileNode[,] nodes;
        public BoundsInt gridBoundsInt;
        private Grid grid;
        private int[] xDir = { 1, -1, 0, 0 };
        private int[] yDir = { 0, 0, 1, -1 };

        public void CreateGrid(Grid grid, IEnumerable<RoomNode> roomEnumerable)
        {
            this.grid = grid;
            Tilemap[] childTilemaps = gameObject.GetComponentsInChildren<Tilemap>();
            gridBoundsInt = GameUtil.GetBoundsIntFromTilemaps(childTilemaps);
            nodes = new RoadTileNode[gridBoundsInt.size.y, gridBoundsInt.size.x];
            foreach (var tilePos in gridBoundsInt.allPositionsWithin)
            {
                int j = tilePos.y - gridBoundsInt.yMin;
                int i = tilePos.x - gridBoundsInt.xMin;
                var tileWorldPosition = grid.GetCellCenterWorld(tilePos);
                nodes[j, i] = new RoadTileNode(tileWorldPosition, tilePos);
                nodes[j, i].IsWalkable = true;
            }

            foreach(var room in roomEnumerable)
            {
                var center = grid.WorldToCell(room.GetCenter());

                for(int y = (int)room.Y; y < (int)(room.Y + room.Height); y++)
                {
                    for (int x = (int)room.X; x < (int)(room.X + room.Width); x++)
                    {
                        if (x == center.x || y == center.y)
                            continue;

                        int j = y - gridBoundsInt.yMin;
                        int i = x - gridBoundsInt.xMin;
                        try
                        {
                            nodes[j, i].IsWalkable = false;
                            nodes[j, i].Weight = 10;
                        }
                        catch
                        {
                            Debug.LogError($"grid Size : {gridBoundsInt.size}, index : {i},{j}");
                        }
                    }
                }
            }
        }

        public void Clear()
        {
            for(int j=0; j< gridBoundsInt.size.y; j++)
            {
                for(int i=0; i < gridBoundsInt.size.x; i++)
                {
                    nodes[j, i].gCost = float.MaxValue;
                }
            }
        }

        public IEnumerable<RoadTileNode> GetNeighbors(RoadTileNode item)
        {
            var tilePos = item.CellPosition;
            int j = tilePos.y - gridBoundsInt.yMin;
            int i = tilePos.x - gridBoundsInt.xMin;

            List<RoadTileNode> neighbors = new();
            for (int k = 0; k < 4; k++)
            {
                int nextJ = j + yDir[k];
                int nextI = i + xDir[k];
                if (nextJ < 0 || nextJ >= gridBoundsInt.size.y || nextI < 0 || nextI >= gridBoundsInt.size.x)
                    continue;

                var neighborNode = nodes[nextJ, nextI];
                neighbors.Add(neighborNode);
            }

            return neighbors;
        }

        public RoadTileNode GetTile(Vector3 tileWorldPos)
        {
            var tilePos = grid.WorldToCell(tileWorldPos);
            int j = tilePos.y - gridBoundsInt.yMin;
            int i = tilePos.x - gridBoundsInt.xMin;

            try
            {
                RoadTileNode tileNode = nodes[j, i];
                return tileNode;
            }
            catch
            {
                Debug.LogError($"grid Size : {gridBoundsInt.size}, index : {i},{j}");
            }
            return null;
            
        }

        private void OnDrawGizmos()
        {
            if (isGizmos == false)
                return;

            Gizmos.DrawWireCube(transform.position + gridBoundsInt.center, gridBoundsInt.size);
            if (nodes == null)
                return;

            foreach (var roadTileNode in nodes)
            {
                if (roadTileNode == null)
                    continue;

                if (roadTileNode.IsWalkable == false)
                    Gizmos.color = Color.white;
                else
                    Gizmos.color = Color.black;

                Gizmos.DrawCube(roadTileNode.ToVector3(), Vector3.one);
            }
        }
    }
}
