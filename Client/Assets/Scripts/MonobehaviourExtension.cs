using GameEngine.Observable;
using UnityEngine;

namespace GameEngine
{
    public class MonobehaviourExtension : MonoBehaviour
    {
        private Transform m_transform;
        public Transform Transform
        {
            get
            {
                if(m_transform == null)
                    m_transform = transform;
                return m_transform;
            }
        }

        private GameObject m_gameObject;
        public GameObject GameObject
        {
            get
            {
                if (m_gameObject == null)
                    m_gameObject = gameObject;
                return m_gameObject;
            }
        }

        public ReactiveProperty<bool> DestroyState = new();

        protected virtual void OnDestroy()
        {
            DestroyState.Value = true;
        }
    }
}


