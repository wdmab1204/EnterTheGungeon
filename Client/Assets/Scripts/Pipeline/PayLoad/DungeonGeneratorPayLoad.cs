using GameEngine.DataSequence.Random;
using GameEngine.DataSequence.Shape;
using GameEngine.MapGenerator.Room;
using System.Collections.Generic;
using UnityEngine;

namespace GameEngine.Pipeline
{
    public interface INode
    {

    }

    public interface INodeGeomery : INode
    {
        int x { get; set; }
        int y { get; set; }
    }

    public class Graph<TNode> where TNode : INode
    {
        
    }

    public class GeomertyGraph<TNode> : Graph<TNode> where TNode : INodeGeomery
    {

    }

    public class DungeonGeneratorPayLoad
    {
        public NormalDistribution Random { get; set; }
        public List<GameObject> RoomTemplates { get; set; }
        public List<(Rectangle rect, Room room)> RoomShapes { get; set; }
    }
}