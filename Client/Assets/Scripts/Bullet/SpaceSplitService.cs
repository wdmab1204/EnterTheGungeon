using UnityEngine;
using System.Collections.Generic;

public class SpaceSplitService
{
    private float cellSize;
    private HashSet<Vector2Int> grid;

    public SpaceSplitService(float cellSize)
    {
        this.cellSize = cellSize;
        grid = new HashSet<Vector2Int>();
    }

    public void Add(Vector2 pos)
    {
        grid.Add(GetCellKey(pos));
    }

    public bool Contains(Vector2 pos)
    {
        return grid.Contains(GetCellKey(pos));
    }

    private Vector2Int GetCellKey(Vector2 pos)
    {
        return new Vector2Int(
            Mathf.FloorToInt(pos.x / cellSize),
            Mathf.FloorToInt(pos.y / cellSize)
        );
    }
}
