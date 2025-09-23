using GameEngine.DataSequence.DIContainer;
using UnityEngine;

namespace GameEngine.Item
{
    public abstract class Interactable : MonobehaviourExtension
    {
        protected GameObject player => DIContainer.Resolve<ICharacterController>().GameObject;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.gameObject == player)
                DoInteractable();
        }

        protected abstract void DoInteractable();
    }
}
