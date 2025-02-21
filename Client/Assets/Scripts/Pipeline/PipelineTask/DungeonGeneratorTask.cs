using GameEngine.DataSequence.Geometry;
using GameEngine.DataSequence.Graph;
using GameEngine.DataSequence.Tree;
using GameEngine.MapGenerator.Room;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameEngine.Pipeline
{
    public class DungeonGeneratorTask : IPipelineTask<DungeonGeneratorPayLoad>
    {
        public DungeonGeneratorPayLoad PayLoad { get; set; }
        private int roomCount;

        private DungeonGraph DungeonGraph => PayLoad.DungeonGraph;

        public DungeonGeneratorTask(int roomCount)
        {
            this.roomCount = roomCount;
        }

        public IEnumerator Process()
        {
            CreateRandomRoom();
            yield return null;

            AutoBuildEdges();
            yield return null;
        }

        private void CreateRandomRoom()
        {
            var rand = PayLoad.Random;
            int sample = roomCount;
            var roomList = PayLoad.RoomTemplates;

            List<(Rectangle rect, Room room)> roomShapes = new();

            while (sample > 0)
            {
                Vector2 center = new((int)rand.NextDouble(), (int)rand.NextDouble());
                var room = roomList[Random.Range(0, roomList.Count)].GetComponent<Room>();
                var tilemap = room.GetComponentInChildren<Tilemap>();
                tilemap.CompressBounds();

                float width = room.width;
                float height = room.height;

                Rectangle rect = new(center, width, height);
                if (CanBuild(roomShapes, rect))
                {
                    roomShapes.Add((rect, room));
                    DungeonGraph.AddNode(center.x, center.y);
                    sample--;
                }
                else
                    rand.stdev += .1f;
            }

            PayLoad.RoomShapes = roomShapes;
        }

        private void AutoBuildEdges()
        {
            //var triangles = Triangulation.TriangulatePoints(DungeonGraph.Vertices.Select(v => new Vertex(v.ToVector3())).ToList());
            var triangles = Triangulation.TriangulateByFlippingEdges(DungeonGraph.Vertices.Select(v => v.ToVector3()).ToList());
            HashSet<RoomEdge> edges = new();
            var graph = PayLoad.DungeonGraph;
            foreach(var tri in triangles)
            {
                var v1Pos = tri.v1.position;
                var v2Pos = tri.v2.position;
                var v3Pos = tri.v3.position;

                graph.AddNode(new (v1Pos));
                graph.AddNode(new (v2Pos));
                graph.AddNode(new (v3Pos));

                edges.Add(new RoomEdge(new(v1Pos), new(v2Pos), Vector3.Distance(v1Pos, v2Pos)));
                edges.Add(new RoomEdge(new(v2Pos), new(v1Pos), Vector3.Distance(v1Pos, v2Pos)));
                edges.Add(new RoomEdge(new(v2Pos), new(v3Pos), Vector3.Distance(v2Pos, v3Pos)));
                edges.Add(new RoomEdge(new(v3Pos), new(v2Pos), Vector3.Distance(v2Pos, v3Pos)));
                edges.Add(new RoomEdge(new(v3Pos), new(v1Pos), Vector3.Distance(v1Pos, v3Pos)));
                edges.Add(new RoomEdge(new(v1Pos), new(v3Pos), Vector3.Distance(v1Pos, v3Pos)));
            }

            var treeEdges =  SpanningTree.TransformMininum(graph.Vertices, edges);
            foreach(var edge in treeEdges)
            {
                graph.AddEdge(edge.From, edge);
            }
        }

        private bool CanBuild(List<(Rectangle rect, Room room)> roomShapes, Rectangle rect)
        {
            if (roomShapes == null || roomShapes.Count == 0)
                return true;

            foreach (var other in roomShapes)
            {
                if (rect.IsColliding(other.rect))
                    return false;
            }

            return true;
        }
    }
}