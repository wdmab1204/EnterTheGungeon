using GameEngine.DataSequence.Geometry;
using GameEngine.DataSequence.Graph;
using GameEngine.DataSequence.Tree;
using GameEngine.MapGenerator.Room;
using System;
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
        private const int roomPadding = 1;

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
            int gridCellSize = PayLoad.GridCellSize;

            while (sample > 0)
            {
                var randPosX = rand.NextDouble();
                var randPosY = rand.NextDouble();

                int randCellPosX = (int)(Math.Round(randPosX / gridCellSize) * gridCellSize);
                int randCellPosY = (int)(Math.Round(randPosY / gridCellSize) * gridCellSize);

                Vector2 cellPosition = new(randCellPosX, randCellPosY);
                var room = roomList[UnityEngine.Random.Range(0, roomList.Count)].GetComponent<Room>();
                var tilemaps = room.GetComponentsInChildren<Tilemap>();
                foreach (var tilemap in tilemaps)
                    tilemap.CompressBounds();
                Vector3Int size = GameUtil.GetBoundsIntFromTilemaps(tilemaps).size;

                Vector3Int roomBottomLeft = new((int)(cellPosition.x - roomPadding), (int)(cellPosition.y - roomPadding));
                Vector3Int roomTopRight = new((int)(cellPosition.x + size.x + roomPadding), (int)(cellPosition.y + size.y + roomPadding));

                if (CanBuild(DungeonGraph.Vertices, roomBottomLeft, roomTopRight))
                {
                    RoomNode roomNode = new(cellPosition, size.x, size.y, room.gameObject);
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

        private bool CanBuild(IEnumerable<RoomNode> vertices, Vector3Int roomBottomLeft, Vector3Int roomTopRight)
        {
            bool canCollide = false;

            Vector3Int min = roomBottomLeft;
            Vector3Int max = roomTopRight;
            foreach (var otherNode in vertices)
            {
                Vector3Int otherMin = otherNode.ToVector3Int();
                otherMin.x -= roomPadding;
                otherMin.y -= roomPadding;
                Vector3Int otherMax = new Vector3Int(otherMin.x + otherNode.Width + roomPadding, otherMin.y + otherNode.Height + roomPadding);
                if (MathUtility.AABB(min, max, otherMin, otherMax))
                    canCollide = true;
            }

            return canCollide == false;
        }
    }
}