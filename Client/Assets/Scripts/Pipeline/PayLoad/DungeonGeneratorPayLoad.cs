using GameEngine.DataSequence.Graph;
using GameEngine.DataSequence.Random;
using System.Collections.Generic;
using UnityEngine;

namespace GameEngine.Pipeline
{
    public class DungeonGeneratorPayLoad
    {
        public NormalDistribution Random { get; set; }
        public List<GameObject> RoomTemplates { get; set; }
        public GameObject HorizonRoad { get; set; }
        public GameObject VerticalRoad { get; set; }
        public GameObject LeftTopRoad { get; set; }
        public GameObject LeftBottomRoad { get; set; }
        public GameObject RightTopRoad { get; set; }
        public GameObject RightBottomRoad { get; set; }
        public GameObject HorizonDoor { get; set; }
        public GameObject VerticalDoor { get; set; }
        public DungeonGraph DungeonGraph { get; set; } = new();
        public GameObject RootGameObject { get; set; }
        public int GridCellSize;
    }
}