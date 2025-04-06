using GameEngine.Pipeline;
using System.Collections.Generic;
using UnityEngine;

namespace GameEngine
{
    public abstract class DungeonGeneratorBase : MonoBehaviour
    {
        public RoomTemplates RoomTemplates;

        public bool ShowGizmos;

        public int RoomCount;

        private PipelineRunner<DungeonGeneratorPayLoad> pipelineRunner = new();

        public DungeonGeneratorLevel Generate()
        {
            var payLoad = new DungeonGeneratorPayLoad()
            {
                Random = new DataSequence.Random.NormalDistribution(new System.Random(), 0, 8),
                RoomTemplates = RoomTemplates.roomList,
                VerticalRoad = RoomTemplates.verticalRoad,
                HorizonRoad = RoomTemplates.horizonRoad,
                LeftBottomRoad = RoomTemplates.leftBottomRoad,
                LeftTopRoad = RoomTemplates.leftTopRoad,
                RightTopRoad = RoomTemplates.rightTopRoad,
                RightBottomRoad = RoomTemplates.rightBottomRoad,
                VerticalDoor = RoomTemplates.verticalDoor,
                HorizonDoor = RoomTemplates.horizonDoor,
                RootGameObject = this.gameObject,
                GridCellSize = 5,
                ShowGizmos = ShowGizmos
            };

            var rootTransform = payLoad.RootGameObject.transform;

            if (Application.isPlaying)
            {
                foreach (Transform child in rootTransform)
                {
                    GameUtil.Destroy(child.gameObject);
                }
            }
            else
            {
                while (rootTransform.childCount != 0)
                {
                    GameUtil.Destroy(rootTransform.GetChild(0).gameObject);
                }
            }



            var pipelineItems = GetPipelineItems();

            System.Diagnostics.Stopwatch sw = new();
            sw.Start();
            pipelineRunner.Run(pipelineItems, payLoad);
            sw.Stop();

            Debug.Log($"Total Seconds : {sw.ElapsedMilliseconds / 1000f}s");

            DungeonGeneratorLevel dungeonGeneratorLevel = new()
            {
                Rooms = payLoad.DungeonGraph.Vertices,
                LayoutData = payLoad.LayoutData,
            };

            return dungeonGeneratorLevel;
        }

        protected virtual List<IPipelineTask<DungeonGeneratorPayLoad>> GetPipelineItems()
        {
            var pipelineItems = new List<IPipelineTask<DungeonGeneratorPayLoad>>()
            {
                new DungeonGeneratorTask(RoomCount),
                new TilemapRenderingTask(),
                //new GraphRenderingTask(),
            };
            return pipelineItems;
        }
    }
}