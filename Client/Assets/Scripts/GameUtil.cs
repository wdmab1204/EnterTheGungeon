using System;
using System.Linq;
using UnityEngine;

namespace GameEngine
{
    public static class GameUtil
    {
        public static Vector2Int GetCenterInt(this RectInt rect)
        {
            return new Vector2Int(rect.x + rect.width / 2, rect.y + rect.height / 2);
        }

        public static Vector2Int GetSize(this RectInt rect)
        {
            return new Vector2Int(rect.width, rect.height);
        }

        public static string GetString(this byte[] bytes)
        {
            var bytesInStr = String.Join("", bytes.Select(b => String.Format("{0:X02}", b)));
            var bytesInStrHex = $"0x{bytesInStr}";
            return bytesInStrHex;
        }
    }
}