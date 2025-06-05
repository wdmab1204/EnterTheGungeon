using UnityEngine;

namespace GameEngine.UI
{
    public interface IMinimapDisplay
    {
        void OnMovePlayer(Vector3 playerPosition);
    }
}