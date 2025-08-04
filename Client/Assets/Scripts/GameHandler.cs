using GameEngine.DataSequence.Graph;
using GameEngine.Pipeline;
using GameEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameEngine
{
    public class GameHandler : MonoBehaviour
    {
        [SerializeField] private UI_Minimap minimapUI;

        private DungeonGeneratorLevel dungeonGeneratorLevel;
        private CharacterController playerController;
        private new FollowPlayer camera;
        private HashSet<RoomNode> visitedRooms = new();
        private int currentMobCount;

        private RoomNode CurrentVisitRoom
        {
            get => GameData.CurrentVisitRoom.Value;
            set => GameData.CurrentVisitRoom.Value = value;
        }

        private void Awake()
        {
            InitializeComponents();
            InitializePlayer();
            InitializeMinimap();
        }

        private void InitializeComponents()
        {
            var generatorBase = GameObject.Find("Dungeon Generator").GetComponent<DungeonGeneratorBase>();
            dungeonGeneratorLevel = generatorBase.Generate();

            playerController = GameObject.Find("Mine").GetComponent<CharacterController>();
            playerController.onMove += OnUserMove;

            camera = GameObject.Find("Main Camera").GetComponent<FollowPlayer>();
        }

        private void InitializePlayer()
        {
            SetPlayerPosition(playerController.transform);
            camera.transform.position = playerController.transform.position;
            camera.AddTransform(playerController.transform);
            OnUserMove(playerController.transform.position);
        }

        private void InitializeMinimap()
        {
            minimapUI.Render(
                dungeonGeneratorLevel.Rooms,
                dungeonGeneratorLevel.RoadEdges,
                () => playerController.transform.position,
                dungeonGeneratorLevel.GridCellSize
            );
            playerController.onMove += minimapUI.OnMovePlayer;
        }

        private bool IsInRoom(RoomNode room, Vector3 position, int padding)
        {
            var pos = room.ToVector3() + new Vector3(padding, padding);
            var size = room.GetSize() - new Vector3(padding * 2, padding * 2);
            return MathUtility.IsPointInRectangle(position, pos, size);
        }

        private void OnUserMove(Vector3 position)
        {
            var layout = dungeonGeneratorLevel.LayoutData;

            RoomNode newRoom = dungeonGeneratorLevel.Rooms.FirstOrDefault(room =>
                !visitedRooms.Contains(room) && IsInRoom(room, position, 1));

            if (newRoom != null)
            {
                EnterRoom(newRoom, layout);
                return;
            }

            if (CurrentVisitRoom != null && !IsInRoom(CurrentVisitRoom, position, 1))
            {
                CurrentVisitRoom = null;
            }
        }

        private void EnterRoom(RoomNode room, Dictionary<RoomNode, RoomInstance> layout)
        {
            Debug.Log($"Current Visit Room : {room.ID}");
            visitedRooms.Add(room);
            CurrentVisitRoom = room;

            if (room.HasMob)
            {
                foreach (var door in layout[room].Doors)
                    door.SetActive(true);
            }

            currentMobCount = layout[room].Mobs.Count;
            foreach (var mob in layout[room].Mobs)
            {
                camera.AddTransform(mob.transform);
                mob.SetActive(true);
                mob.GetComponent<UnitAbility>().DestroyState.OnValueChanged += isDestroyed =>
                {
                    if (!isDestroyed) return;
                    currentMobCount--;
                    OnMobCountChanged(currentMobCount);
                };
            }
        }

        private void OnMobCountChanged(int mobCount)
        {
            if (mobCount <= 0 && CurrentVisitRoom != null)
            {
                foreach (var door in dungeonGeneratorLevel.LayoutData[CurrentVisitRoom].Doors)
                    door.SetActive(false);
            }
        }

        private void SetPlayerPosition(Transform playerTrans)
        {
            RoomNode startRoom = dungeonGeneratorLevel.Rooms.FirstOrDefault(room => room.ID == 1000);
            Vector3 worldPosition = startRoom.GetCenter();
            playerTrans.position = worldPosition;
            visitedRooms.Add(startRoom);
        }
    }
}