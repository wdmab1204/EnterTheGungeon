using GameEngine.Pipeline;
using System.Collections.Generic;
using UnityEngine;

namespace GameEngine
{
    public abstract class DungeonGeneratorBase : MonoBehaviour
    {
        public RoomTemplates RoomTemplates;

        public int RoomCount;

        private PipelineRunner<DungeonGeneratorPayLoad> pipelineRunner = new();

        public void Generate()
        {
            var payLoad = new DungeonGeneratorPayLoad()
            {
                Random = new DataSequence.Random.NormalDistribution(new System.Random(), 0, 32),
                RoomTemplates = RoomTemplates.roomList,
                RootGameObject = this.gameObject,
            };

            var rootTransform = payLoad.RootGameObject.transform;
            while(rootTransform.childCount != 0)
            {
                GameUtil.Destroy(rootTransform.GetChild(0).gameObject);
            }

            var pipelineItems = GetPipelineItems();

            System.Diagnostics.Stopwatch sw = new();
            sw.Start();
            pipelineRunner.Run(pipelineItems, payLoad);
            sw.Stop();

            Debug.Log($"Total Seconds : {sw.ElapsedMilliseconds / 1000f}s");
        }

        protected virtual List<IPipelineTask<DungeonGeneratorPayLoad>> GetPipelineItems()
        {
            var pipelineItems = new List<IPipelineTask<DungeonGeneratorPayLoad>>()
            {
                new DungeonGeneratorTask(RoomCount),
                new TilemapRenderingTask(),
                new GraphRenderingTask(),
            };
            return pipelineItems;
        }
    }
}