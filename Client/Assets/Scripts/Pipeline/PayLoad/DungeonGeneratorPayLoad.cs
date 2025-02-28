using GameEngine.DataSequence.Geometry;
using GameEngine.DataSequence.Graph;
using GameEngine.DataSequence.Random;
using GameEngine.MapGenerator.Room;
using System.Collections.Generic;
using UnityEngine;

namespace GameEngine.Pipeline
{
    public class DungeonGeneratorPayLoad
    {
        public NormalDistribution Random { get; set; }
        public List<GameObject> RoomTemplates { get; set; }
        public List<(Rectangle rect, Room room)> RoomShapes { get; set; }
        public DungeonGraph DungeonGraph { get; set; } = new();
        public GameObject RootGameObject { get; set; }
        public RoomTemplates roomTemplates { get; set; }
    }
}