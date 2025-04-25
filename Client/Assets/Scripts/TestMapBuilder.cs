using System;
using DataSequence.Tree;
using GameEngine;
using GameEngine.MapGenerator;
using UnityEngine;

namespace DefaultNamespace
{
    public class TestMapBuilder : MonoBehaviour
    {
        [SerializeField] private Vector2Int mapSize;
        [SerializeField] private RoomInfoDisplay display;

        private DataSequence.Tree.TreeNode root;
        
        public void Build()
        {
            MapGenerator generator = new();
            MapRenderer renderer = GetComponentInChildren<MapRenderer>();
            generator.Init(mapSize, renderer);

            root = new(0, 0, mapSize.x, mapSize.y);
            root.Name = "Root";

            generator.DivideTree(root, 0);
            var roomPos = generator.GenerateDungeon(root, 0).GetCenterInt();
            generator.GenerateRoad(root, 0);
            
            display.SetUI(root);

            var player = GameObject.FindObjectOfType<CharacterController>();
            var playerPos = player.transform.position;
            playerPos.x = roomPos.x;
            playerPos.y = roomPos.y;
            player.transform.position = playerPos;
        }

        #if UNITY_EDITOR
        private void Update()
        {
            display.SetUI(root);
        }
        #endif
    }
}