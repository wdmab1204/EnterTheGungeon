using GameEngine.DataSequence.Geometry;
using GameEngine.DataSequence.Random;
using GameEngine.MapGenerator.Room;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEngine.NormalDistributionMapBuild
{
    public class NormalDistributionMapBuild : MonoBehaviour
    {
        [SerializeField] List<Room> roomList = new();
        [SerializeField] int count = 50;
        List<(Rectangle rect, Room room)> rects = new();

        private IEnumerator Start()
        {
            NormalDistribution rand = new(new System.Random(), 0, 8);

            int sample = count;
            while(sample > 0)
            {
                Vector2 center = new((float)rand.NextDouble(), (float)rand.NextDouble());
                var room = roomList[Random.Range(0, roomList.Count)];
                float width = room.width;
                float height = room.height;

                Rectangle rect = new(center, width, height);
                if (CanBuild(rect))
                {
                    rects.Add((rect, room));
                    sample--;
                }
                else
                    rand.stdev += .1f;
            }

            foreach(var pair in rects)
            {
                var room = pair.room;
                var rect = pair.rect;

                var clone = Instantiate(room.gameObject);
                clone.SetActive(true);
                clone.transform.position = rect.Center;
                yield return null;
            }
        }

        private bool CanBuild(Rectangle rect)
        {
            if (rects == null || rects.Count == 0)
                return true;

            foreach (var other in rects)
            {
                if (rect.IsColliding(other.rect))
                    return false;
            }

            return true;
        }
    }
}
