using GameEngine.DataSequence.Graph;
using System.Collections.Generic;
using UnityEngine;

namespace GameEngine
{
    public class GameGrid : MonoBehaviour
    {
        public bool isGizmos;

        private GridCell[,] cellArray;
        public BoundsInt gridBoundsInt;
        private int[] xDir = { 1, -1, 0, 0 };
        private int[] yDir = { 0, 0, 1, -1 };
        private Vector3 gridWorldPosition;
        private readonly Vector3 gridLocalPosition = Vector3.zero;
        private int gridCellSize;

        public void CreateGrid(IEnumerable<RoomNode> roomEnumerable, int gridCellSize, BoundsInt boundsInt)
        {
            this.gridBoundsInt = boundsInt;
            this.gridCellSize = gridCellSize;
            gridWorldPosition = this.transform.position;

            cellArray = new GridCell[gridBoundsInt.size.y, gridBoundsInt.size.x];
            foreach (var cellPosition in gridBoundsInt.allPositionsWithin)
            {
                var cellWorldPosition = GetCellWorldPosition(cellPosition);
                cellArray[cellPosition.y, cellPosition.x] = new GridCell(cellWorldPosition, cellPosition);
                cellArray[cellPosition.y, cellPosition.x].IsWalkable = true;
            }

            foreach (var room in roomEnumerable)
            {
                var roomWorldPosition = room.ToVector3();
                var roomCellPosition = GetCellPosition(roomWorldPosition);
                int cellHeight = room.Height / gridCellSize;
                int cellWidth = room.Width / gridCellSize;

                int cellTopRightY = roomCellPosition.y + cellHeight;
                int cellTopRightX = roomCellPosition.x + cellWidth;

                for (int y = roomCellPosition.y; y < cellTopRightY; y++)
                {
                    for (int x = roomCellPosition.x; x < cellTopRightX; x++)
                    {
                        //cellArray[y, x].IsWalkable = false;
                        cellArray[y, x].Weight = 10;
                        cellArray[y, x].CellType = CellType.Room;
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
                    cellArray[j, i].gCost = float.MaxValue;
                }
            }
        }

        public IEnumerable<GridCell> GetNeighbors(GridCell item)
        {
            var cellPosition = item.CellPosition;

            List<GridCell> neighbors = new();
            for (int k = 0; k < 4; k++)
            {
                int nextJ = cellPosition.y + yDir[k];
                int nextI = cellPosition.x + xDir[k];
                if (nextJ < 0 || nextJ >= gridBoundsInt.size.y || nextI < 0 || nextI >= gridBoundsInt.size.x)
                    continue;

                var neighborNode = cellArray[nextJ, nextI];
                neighbors.Add(neighborNode);
            }

            return neighbors;
        }

        public GridCell GetCell(Vector3 cellWorldPosition)
        {
            Vector3Int cellPosition = GetCellPosition(cellWorldPosition);
            return cellArray[cellPosition.y, cellPosition.x];
        }

        public Vector3 GetCellCenter(Vector3Int cellPosition)
        {
            return new Vector3(cellPosition.x + gridCellSize / 2f, cellPosition.y + gridCellSize / 2f);
        }

        public Vector3 GetCellWorldCenter(Vector3 worldPosition)
        {
            return new Vector3(worldPosition.x + gridCellSize / 2f, worldPosition.y + gridCellSize / 2f);
        }

        public Vector3 GetCellWorldPosition(Vector3Int cellPosition)
        {
            Matrix4x4 cellToLocalMatrix = Matrix4x4.TRS(gridLocalPosition, Quaternion.identity, new Vector3(gridCellSize, gridCellSize, gridCellSize));
            Vector3 localPosition = cellToLocalMatrix.MultiplyPoint3x4(cellPosition);
            Matrix4x4 localToWorldMatrix = Matrix4x4.TRS(gridWorldPosition, Quaternion.identity, Vector3.one);
            Vector3 worldPosition = localToWorldMatrix.MultiplyPoint3x4(localPosition);
            return worldPosition;
        }

        public Vector3Int GetCellPosition(Vector3 worldPosition)
        {
            Matrix4x4 worldToLocalMatrix = Matrix4x4.TRS(gridWorldPosition, Quaternion.identity, Vector3.one).inverse;
            Vector3 localPosition = worldToLocalMatrix.MultiplyPoint3x4(worldPosition);
            Matrix4x4 localToCellMatrix = Matrix4x4.TRS(gridLocalPosition, Quaternion.identity, new Vector3(gridCellSize, gridCellSize, gridCellSize)).inverse;
            Vector3 cellPosition = localToCellMatrix.MultiplyPoint3x4(localPosition);
            Vector3Int cellPositionInt = new Vector3Int((int)cellPosition.x, (int)cellPosition.y, (int)cellPosition.z);

            return cellPositionInt;
        }

        private void OnDrawGizmos()
        {
            if (isGizmos == false)
                return;

            Gizmos.color = Color.yellow;

            Vector3 cellSize = new Vector3(gridCellSize, gridCellSize, 0);

            foreach(var cellPosition in gridBoundsInt.allPositionsWithin)
            {
                var cellWorldPosition = GetCellWorldPosition(cellPosition);
                var cellWorldCenter = new Vector3(cellWorldPosition.x + gridCellSize / 2f, cellWorldPosition.y + gridCellSize / 2f);
                Gizmos.DrawWireCube(cellWorldCenter, cellSize);
            }

            if (cellArray == null)
                return;

            foreach (var cell in cellArray)
            {
                if (cell == null)
                    continue;

                if (cell.CellType == CellType.None)
                    continue;

                Vector3Int cellPosition = cell.CellPosition;
                Vector3 cellWorldPosition = GetCellWorldPosition(cellPosition);
                Vector3 cellWorldCenter = GetCellWorldCenter(cellWorldPosition);
                Gizmos.color = cell.CellType == CellType.Room ? Color.black : Color.red;
                Gizmos.DrawCube(cellWorldCenter, cellSize);
            }
        }
    }
}
