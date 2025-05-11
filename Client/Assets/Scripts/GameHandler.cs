using GameEngine.DataSequence.Graph;
using GameEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
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
        private RoomNode currentVisitRoom;
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
                    
                    foreach(var mob in layoutData[room].Mobs)
                    {
                        camera.AddTransform(mob.transform);
                        mob.SetActive(true);
                    }

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

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                var ui = Instantiate(minimapUI, minimapUI.transform.parent);
                ui.gameObject.SetActive(true);
                ui.SetData(
                    dungeonGeneratorLevel.Rooms,
                    dungeonGeneratorLevel.RoadEdges,
                    playerMovementController.transform.position,
                    dungeonGeneratorLevel.GridCellSize);
                playerMovementController.onMove += ui.OnMovePlayer;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                var ui = GameObject.Find("Minimap UI(Clone)")?.GetComponent<UI_Minimap>();
                if (ui == null) return;
                playerMovementController.onMove -= ui.OnMovePlayer;
                GameUtil.Destroy(ui.gameObject);
            }
                
        }
    }
}