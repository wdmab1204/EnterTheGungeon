using GameEngine.DataSequence.Graph;
using System.Linq;
using UnityEngine;

namespace GameEngine
{
    public class GameHandler : MonoBehaviour
    {
        DungeonGeneratorBase dungeonGenerator;
        CharacterController playerMovementController;
        DungeonGeneratorLevel dungeonGeneratorLevel;

        private void Awake()
        {
            dungeonGenerator = GameObject.Find("Dungeon Generator").GetComponent<DungeonGeneratorBase>();
            dungeonGeneratorLevel = dungeonGenerator.Generate();

            playerMovementController = GameObject.Find("Mine").GetComponent<CharacterController>();
            SetPlayerPosition(playerMovementController.transform);
            playerMovementController.onMove += OnUserMove;
        }

        private void OnUserMove(Vector3 position)
        {
            foreach(var room in dungeonGeneratorLevel.GetRoomEnumerable())
            {
                if(MathUtility.IsPointInRectangle(position, room.ToVector3(), room.GetSize()))
                {
                    Debug.Log(room.ID);
                    return;
                }
            }
        }

        private void SetPlayerPosition(Transform playerTrans)
        {
            Vector3 worldPosition = dungeonGeneratorLevel.GetRoomEnumerable().First().GetCenter();
            playerTrans.position = worldPosition;
        }

        private void CloseDoor(RoomNode room)
        {
            
        }
    }
}