using UnityEngine;

namespace GameEngine.Item
{
    public abstract class Interactable : MonobehaviourExtension
    {
        protected GameObject player;

        private void Awake()
        {
            player = GameData.Player.gameObject;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.gameObject == player)
                DoInteractable();
        }

        protected abstract void DoInteractable();
    }
}
