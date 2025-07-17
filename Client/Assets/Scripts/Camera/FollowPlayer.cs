using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace GameEngine
{
    public class FollowPlayer : MonoBehaviour
    {
        public Transform player;
        public Vector3 offset;
        public float smoothSpeed = 0.125f;
        private HashSet<Transform> transformList = new(), removeList = new();

        public void AddTransform(Transform transform) => transformList.Add(transform);

        void LateUpdate()
        {
            Vector3 avrPosition = Vector3.zero;
            int validCount = 0;

            foreach (Transform t in transformList)
            {
                if (t == null || t.IsDestroyed())
                {
                    removeList.Add(t);
                }
                else
                {
                    avrPosition += t.position;
                    validCount++;
                }
            }

            if (validCount > 0)
            {
                avrPosition /= validCount;
                Vector3 smoothedPosition = Vector3.Lerp(transform.position, avrPosition, smoothSpeed * Time.deltaTime);
                transform.position = smoothedPosition + offset;
            }

            foreach (var t in removeList)
                transformList.Remove(t);
            removeList.Clear();
        }
    }
}
