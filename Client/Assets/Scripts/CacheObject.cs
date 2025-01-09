using UnityEngine;

namespace GameEngine
{
    public class CacheObject : MonoBehaviour
    {
        private GameObject myGameObject;
        public GameObject MyGameObject
        {
            get
            {
                if (myGameObject == null)
                    myGameObject = this.gameObject;
                return myGameObject;
            }
        }
	
        private Transform myTransform;
        public Transform MyTransform
        {
            get
            {
                if (myTransform == null)
                    myTransform = this.transform;
                return myTransform;
            }
        }
	
        private RectTransform myRT;
        public RectTransform MyRT
        {
            get
            {
                if (myRT == null)
                    myRT = MyTransform as RectTransform;
                return myRT;
            }
        }
    }
}