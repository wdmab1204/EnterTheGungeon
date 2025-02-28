using GameEngine.DataSequence.Graph;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameEngine
{
    public class GameGrid : MonoBehaviour
    {
        private RoadTileNode[,] nodes;
        public BoundsInt gridBoundsInt;
        private Grid grid;
        private int[] xDir = { 1, -1, 0, 0 };
        private int[] yDir = { 0, 0, 1, -1 };

        public void CreateGrid(Grid grid)
        {
            this.grid = grid;
            Tilemap[] childTilemaps = gameObject.GetComponentsInChildren<Tilemap>();
            gridBoundsInt = GetBoundsIntFromTilemaps(childTilemaps);
            nodes = new RoadTileNode[gridBoundsInt.size.y, gridBoundsInt.size.x];
            foreach (var tilePos in gridBoundsInt.allPositionsWithin)
            {
                int j = tilePos.y - gridBoundsInt.yMin;
                int i = tilePos.x - gridBoundsInt.xMin;
                var tileWorldPosition = grid.GetCellCenterWorld(tilePos);
                nodes[j, i] = new RoadTileNode(tileWorldPosition, tilePos);
                nodes[j, i].IsWalkable = true;
            }

            //foreach (var collidableTilemap in childTilemaps.Where(tilemap => tilemap.GetComponent<TilemapCollider2D>() != null))
            //{
            //    var cellBounds = collidableTilemap.cellBounds;
            //    foreach (var tilePos in cellBounds.allPositionsWithin)
            //    {
            //        if (collidableTilemap.HasTile(tilePos) == false)
            //            continue;

            //        int j = tilePos.y - gridBoundsInt.yMin;
            //        int i = tilePos.x - gridBoundsInt.xMin;
            //        nodes[j, i].IsWalkable = false;
            //    }
            //}
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

        private BoundsInt GetBoundsIntFromTilemaps(IEnumerable<Tilemap> tilemaps)
        {
            BoundsInt totalBounds = default;
            foreach (var tilemap in tilemaps)
            {
                totalBounds.xMin = Mathf.Min(totalBounds.xMin, tilemap.cellBounds.xMin);
                totalBounds.yMin = Mathf.Min(totalBounds.yMin, tilemap.cellBounds.yMin);
                totalBounds.xMax = Mathf.Max(totalBounds.xMax, tilemap.cellBounds.xMax);
                totalBounds.yMax = Mathf.Max(totalBounds.yMax, tilemap.cellBounds.yMax);
            }
            totalBounds.zMin = 0;
            totalBounds.zMax = 1;

            return totalBounds;
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

            Gizmos.DrawWireCube(transform.position + gridBoundsInt.center, gridBoundsInt.size);
            if (nodes == null)
                return;

            //foreach (var roadTileNode in nodes)
            //{
            //    if (roadTileNode == null)
            //        continue;

            //    if (roadTileNode.IsWalkable)
            //        Gizmos.color = Color.white;
            //    else
            //        Gizmos.color = Color.black;

            //    Gizmos.DrawCube(roadTileNode.ToVector3(), Vector3.one);
            //}
        }
    }
}
