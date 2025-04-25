using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameEngine
{
    [CreateAssetMenu(fileName = "RoomTemplates", menuName = "Scriptable Object/Room templates")]
    public class RoomTemplates : ScriptableObject
    {
        public List<RoomData> roomList;

        public GameObject horizonRoad;
        public GameObject verticalRoad;
        public GameObject leftTopRoad;
        public GameObject leftBottomRoad;
        public GameObject rightTopRoad;
        public GameObject rightBottomRoad;

        public GameObject horizonDoor;
        public GameObject verticalDoor;
    }

    [System.Serializable]
    public class RoomData
    {
        public GameObject prefab;
        public int guaranteedCount;
    }

    [System.Serializable]
    public class RoadData
    {
        public GameObject horizonRoad;
        public GameObject verticalRoad;
        public GameObject leftTopRoad;
        public GameObject leftBottomRoad;
        public GameObject rightTopRoad;
        public GameObject rightBottomRoad;
    }

    [System.Serializable]
    public class DoorData
    {
        public GameObject horizonDoor;
        public GameObject verticalDoor;
    }
}