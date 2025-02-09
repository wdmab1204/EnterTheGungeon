using GameEngine.DataSequence.Shape;
using GameEngine.MapGenerator.Room;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameEngine.Pipeline
{
    public class DungeonGeneratorTask : IPipelineTask<DungeonGeneratorPayLoad>
    {
        public DungeonGeneratorPayLoad PayLoad { get; set; }
        private int roomCount;

        public DungeonGeneratorTask(int roomCount)
        {
            this.roomCount = roomCount;
        }

        public IEnumerator Process()
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
                Debug.Log(tilemap.cellBounds);

                float width = room.width;
                float height = room.height;

                Rectangle rect = new(center, width, height);
                if (CanBuild(roomShapes, rect))
                {
                    roomShapes.Add((rect, room));
                    sample--;
                }
                else
                    rand.stdev += .1f;
            }

            PayLoad.RoomShapes = roomShapes;

            yield return null;
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