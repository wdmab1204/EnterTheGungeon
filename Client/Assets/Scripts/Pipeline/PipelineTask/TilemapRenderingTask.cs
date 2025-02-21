using System.Collections;
using UnityEngine;

namespace GameEngine.Pipeline
{
    public class TilemapRenderingTask : IPipelineTask<DungeonGeneratorPayLoad>
    {
        public DungeonGeneratorPayLoad PayLoad { get; set; }

        public IEnumerator Process()
        {
            yield return null;

            foreach (var pair in PayLoad.RoomShapes)
            {
                var room = pair.room;
                var rect = pair.rect;

                var clone = Object.Instantiate(room.gameObject);
                clone.transform.parent = PayLoad.RootGameObject.transform;
                clone.SetActive(true);
                clone.transform.position = rect.Center;

                yield return null;
            }
        }
    }
}