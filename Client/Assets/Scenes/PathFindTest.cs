using GameEngine;
using GameEngine.Navigation;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathFindTest : MonoBehaviour
{
    void Awake()
    {
        var navGrid = GameObject.Find("NavGrid").GetComponent<NavGrid>();
        var floorTilemap = GameObject.Find("Floor").GetComponent<Tilemap>();
        navGrid.CreateGrid(floorTilemap, null);
        PathFinding pathFinder = new(navGrid);
        PathFindManager.PathFinder = pathFinder;
    }
}
