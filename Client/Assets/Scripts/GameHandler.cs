using GameEngine.DataSequence.Graph;
using GameEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameEngine
{
    public class GameHandler : MonoBehaviour
    {
        DungeonGeneratorBase dungeonGenerator;
        DungeonGeneratorLevel dungeonGeneratorLevel;
        CharacterController playerMovementController;

        [SerializeField] private UI_Minimap minimapUI;

        private HashSet<RoomNode> visitedRoomSet = new();
        private RoomNode CurrentVisitRoom
        {
            get => GameData.CurrentVisitRoom.Value;
            set => GameData.CurrentVisitRoom.Value = value;
        }
        private new FollowPlayer camera;

        private void Awake()
        {
            dungeonGenerator = GameObject.Find("Dungeon Generator").GetComponent<DungeonGeneratorBase>();
            dungeonGeneratorLevel = dungeonGenerator.Generate();

            playerMovementController = GameObject.Find("Mine").GetComponent<CharacterController>();
            SetPlayerPosition(playerMovementController.transform);
            playerMovementController.onMove += OnUserMove;
            OnUserMove(playerMovementController.transform.position);

            camera = GameObject.Find("Main Camera").GetComponent<FollowPlayer>();
            camera.transform.position = playerMovementController.transform.position;
            camera.AddTransform(playerMovementController.transform);

            minimapUI.Render(
                dungeonGeneratorLevel.Rooms,
                dungeonGeneratorLevel.RoadEdges,
                () => playerMovementController.transform.position,
                dungeonGeneratorLevel.GridCellSize);
            playerMovementController.onMove += minimapUI.OnMovePlayer;
        }

        private void OnUserMove(Vector3 position)
        {
            var layoutData = dungeonGeneratorLevel.LayoutData;

            foreach (RoomNode room in dungeonGeneratorLevel.Rooms)
            {
                if (visitedRoomSet.Contains(room))
                    continue;

                if(IsInRoom(room, position, 1))
                {
                    Debug.Log($"Current Visit Room : {room.ID}");
                    visitedRoomSet.Add(room);
                    GameData.CurrentVisitRoom.Value = room;
                    foreach(var door in layoutData[room].Doors)
                        door.SetActive(true);
                    
                    foreach(var mob in layoutData[room].Mobs)
                    {
                        camera.AddTransform(mob.transform);
                        mob.SetActive(true);
                    }

                    return;
                }
            }

            if (CurrentVisitRoom == null)
                return;

            if (IsInRoom(CurrentVisitRoom, position, 1))
                return;

            foreach(var door in layoutData[CurrentVisitRoom].Doors)
                door.SetActive(false);

            CurrentVisitRoom = null;

            bool IsInRoom(RoomNode room, Vector3 position, int width)
            {
                Vector3 roomWorldPosition = room.ToVector3();
                roomWorldPosition.x += width;
                roomWorldPosition.y += width;

                Vector3 roomArea = room.GetSize();
                roomArea.x -= width * 2;
                roomArea.y -= width * 2;

                bool result = MathUtility.IsPointInRectangle(position, roomWorldPosition, roomArea);
                return result;
            }
        }

        private void SetPlayerPosition(Transform playerTrans)
        {
            RoomNode startRoom = dungeonGeneratorLevel.Rooms.FirstOrDefault(room => room.ID == 1000);
            Vector3 worldPosition = startRoom.GetCenter();
            playerTrans.position = worldPosition;
            visitedRoomSet.Add(startRoom);
        }
    }
}