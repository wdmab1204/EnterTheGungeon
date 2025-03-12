using System.Collections.Generic;
using UnityEngine;

namespace GameEngine
{
    [CreateAssetMenu(fileName = "RoomTemplates", menuName = "Scriptable Object/Room templates")]
    public class RoomTemplates : ScriptableObject
    {
        public List<GameObject> roomList = new();
        public GameObject horizonRoad;
        public GameObject verticalRoad;
        public GameObject leftTopRoad;
        public GameObject leftBottomRoad;
        public GameObject rightTopRoad;
        public GameObject rightBottomRoad;
    }
}