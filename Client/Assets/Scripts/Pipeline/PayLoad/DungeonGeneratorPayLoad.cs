using GameEngine.DataSequence.Graph;
using GameEngine.DataSequence.Random;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameEngine.Pipeline
{
    public class DungeonGeneratorPayLoad
    {
        public int Seed { get; set; }
        public List<RoomData> RoomTemplates { get; set; }
        public GameObject HorizonRoad { get; set; }
        public GameObject VerticalRoad { get; set; }
        public GameObject LeftTopRoad { get; set; }
        public GameObject LeftBottomRoad { get; set; }
        public GameObject RightTopRoad { get; set; }
        public GameObject RightBottomRoad { get; set; }
        public GameObject HorizonDoor { get; set; }
        public GameObject VerticalDoor { get; set; }
        public DungeonGraph DungeonGraph { get; set; }
        public GameObject RootGameObject { get; set; }
        public GameGrid GameGrid { get; set; }
        public List<(Tilemap[] tilemaps, Vector3 worldPosition)> TilemapRenderTaskList { get; set; } = new();
        public int GridCellSize { get; set; }
        public bool ShowGizmos { get; set; }

        public Dictionary<RoomNode, RoomInstance> LayoutData { get; set; }

    }
}