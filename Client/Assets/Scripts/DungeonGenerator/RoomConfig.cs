using System.Collections.Generic;
using UnityEngine;

namespace GameEngine
{
    [CreateAssetMenu(fileName = "RoomTemplates", menuName = "Scriptable Object/Room templates")]
    public class RoomTemplates : ScriptableObject
    {
        public List<GameObject> roomList = new();
        public List<GameObject> verticalRoadList = new();
        public List<GameObject> horizontalRoadList = new();
        public GameObject leftdownCorner;
        public GameObject leftupCorner;
        public GameObject rightdownCorner;
        public GameObject rightupCorner;
    }
}