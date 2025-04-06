using GameEngine.DataSequence.Graph;
using GameEngine.MapGenerator.Room;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameEngine
{
    public class GameHandler : MonoBehaviour
    {
        DungeonGeneratorBase dungeonGenerator;
        CharacterController playerMovementController;
        DungeonGeneratorLevel dungeonGeneratorLevel;

        private HashSet<RoomNode> visitedRoomSet = new();
        private RoomNode currentVisitRoom;

        private void Awake()
        {
            dungeonGenerator = GameObject.Find("Dungeon Generator").GetComponent<DungeonGeneratorBase>();
            dungeonGeneratorLevel = dungeonGenerator.Generate();

            playerMovementController = GameObject.Find("Mine").GetComponent<CharacterController>();
            SetPlayerPosition(playerMovementController.transform);
            playerMovementController.onMove += OnUserMove;
            OnUserMove(playerMovementController.transform.position);
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
                    currentVisitRoom = room;
                    foreach(var door in layoutData[room].Doors)
                        door.SetActive(true);
                    
                    return;
                }
            }

            if (currentVisitRoom == null)
                return;

            if (IsInRoom(currentVisitRoom, position, 1))
                return;

            foreach(var door in layoutData[currentVisitRoom].Doors)
                door.SetActive(false);

            currentVisitRoom = null;

            bool IsInRoom(RoomNode room, Vector3 position, int width)
            {
                Vector3 roomWorldPosition = room.ToVector3();
                roomWorldPosition.x += width;
                roomWorldPosition.y += width;

                Vector3 roomArea = room.GetSize();
                roomArea.x -= width * 2;
                roomArea.y -= width * 2;

                bool result = MathUtility.IsPointInRectangle(position, roomWorldPosition, roomArea);
                if (result)
                    Debug.Log(roomWorldPosition + ", " + roomArea);
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