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
        navGrid.CreateGrid(floorTilemap, Vector3.zero);
        DungeonNavigation pathFinder = new(navGrid);
        PathFindManager.PathFinder = pathFinder;
    }
}
