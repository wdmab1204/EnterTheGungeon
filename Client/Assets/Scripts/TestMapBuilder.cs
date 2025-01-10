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
        
        private void Awake()
        {
            MapGenerator generator = new();
            MapRenderer renderer = GetComponentInChildren<MapRenderer>();
            generator.Init(mapSize, renderer);

            TreeNode root = new(0, 0, mapSize.x, mapSize.y);

            generator.DivideTree(root, 0);
            var roomPos = generator.GenerateDungeon(root, 0).GetCenterInt();
            generator.GenerateRoad(root, 0);

            var player = GameObject.FindObjectOfType<CharacterController>();
            var playerPos = player.transform.position;
            playerPos.x = roomPos.x;
            playerPos.y = roomPos.y;
            player.transform.position = playerPos;
        }
    }
}