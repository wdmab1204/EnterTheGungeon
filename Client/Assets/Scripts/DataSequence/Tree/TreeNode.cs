using System.Collections.Generic;
using UnityEngine;

namespace DataSequence.Tree
{
    public class TreeNode : ITreeNode
    {
        public TreeNode LeftNode { get; set; }
        public TreeNode RightNode { get; set; }
        public TreeNode ParentNode { get; set; }
        public RectInt RoomSize;
        public RectInt DungeonSize { get; set; }

        public List<Vector2Int> DoorList { get; set; } = new();

        public TreeNode(int x, int y, int width, int height)
        {
            RoomSize.x = x;
            RoomSize.y = y;
            RoomSize.width = width;
            RoomSize.height = height;
        }
    }
}