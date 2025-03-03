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

            while (sample > 0)
            {
                Vector2 pos = new((int)rand.NextDouble() - .5f, (int)rand.NextDouble() - .5f);
                var room = roomList[Random.Range(0, roomList.Count)].GetComponent<Room>();
                var tilemaps = room.GetComponentsInChildren<Tilemap>();
                var size = GameUtil.GetBoundsIntFromTilemaps(tilemaps).size;

                Rectangle rect = new(pos, size.x, size.y);
                if (CanBuild(DungeonGraph.Vertices, rect))
                {
                    RoomNode roomNode = new(pos, size.x, size.y, room.gameObject);
                    DungeonGraph.AddNode(roomNode);
                    sample--;
                }
                else
                    rand.stdev += .1f;
            }
        }

        private void AutoBuildEdges()
        {
            var vertices = DungeonGraph.Vertices;

            if (vertices.Count == 2)
            {
                var firstNode = vertices.First();
                var lastNode = vertices.Last();
                RoomEdge edge = new(firstNode, lastNode, Vector3.Distance(firstNode.ToVector3(), lastNode.ToVector3()));
                DungeonGraph.AddEdge(firstNode, edge);
                return;
            }

            //var triangles = Triangulation.TriangulatePoints(DungeonGraph.Vertices.Select(v => new Vertex(v.ToVector3())).ToList());
            var triangles = Triangulation.TriangulateByFlippingEdges(DungeonGraph.Vertices.Select(v => v.ToVector3()).ToList());
            HashSet<RoomEdge> edges = new();

            foreach(var tri in triangles)
            {
                var v1Pos = tri.v1.position;
                var v2Pos = tri.v2.position;
                var v3Pos = tri.v3.position;

                var node1 = DungeonGraph.GetNodeFromPos(v1Pos);
                var node2 = DungeonGraph.GetNodeFromPos(v2Pos);
                var node3 = DungeonGraph.GetNodeFromPos(v3Pos);

                edges.Add(new RoomEdge(node1, node2, Vector3.Distance(v1Pos, v2Pos)));
                edges.Add(new RoomEdge(node2, node1, Vector3.Distance(v1Pos, v2Pos)));
                edges.Add(new RoomEdge(node2, node3, Vector3.Distance(v2Pos, v3Pos)));
                edges.Add(new RoomEdge(node3, node2, Vector3.Distance(v2Pos, v3Pos)));
                edges.Add(new RoomEdge(node3, node1, Vector3.Distance(v1Pos, v3Pos)));
                edges.Add(new RoomEdge(node1, node3, Vector3.Distance(v1Pos, v3Pos)));
            }

            var treeEdges =  SpanningTree.TransformMininum(DungeonGraph.Vertices, edges);
            foreach(var edge in treeEdges)
            {
                DungeonGraph.AddEdge(edge.From, edge);
            }
        }

        private bool CanBuild(IEnumerable<RoomNode> vertices, Rectangle rect)
        {
            bool canCollide = false;

            foreach (var other in vertices)
            {
                if (rect.IsColliding(new Rectangle(other.ToVector3(), other.Width, other.Height)))
                {
                    canCollide = true;
                    break;
                }
            }

            return canCollide == false;
        }
    }
}