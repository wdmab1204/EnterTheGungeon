using System.Collections.Generic;
using GameEngine;
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
        
        public string Name { get; set; }

        public bool HasChild() => LeftNode != null && RightNode != null;

        public Vector2Int GetCenterInt()
        {
            if (HasChild())
                return RightNode.GetCenterInt();
            else
                return DungeonSize.GetCenterInt();
        }

        public TreeNode(int x, int y, int width, int height)
        {
            RoomSize.x = x;
            RoomSize.y = y;
            RoomSize.width = width;
            RoomSize.height = height;
        }
    }
}