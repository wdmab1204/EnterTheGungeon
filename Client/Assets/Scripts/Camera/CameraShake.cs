using System.Collections;
using UnityEngine;

namespace GameEngine
{
    using UnityEngine;
    using System.Collections;

    public class CameraShake : MonoBehaviour
    {
        FollowPlayer cameraFollow;
        public float duration;
        public float intensity;

        private void Awake()
        {
            cameraFollow = GetComponent<FollowPlayer>();
        }

        public void Shake()
        {
            Shake(this.duration, this.intensity);
        }

        public void Shake(float duration, float intensity)
        {
            StopAllCoroutines();
            StartCoroutine(ShakeCoroutine(duration, intensity));
        }

        private IEnumerator ShakeCoroutine(float duration, float intensity)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * intensity;
                float y = Random.Range(-1f, 1f) * intensity;

                cameraFollow.offset = new Vector3(x, y, 0);

                elapsed += Time.deltaTime;
                yield return null;
            }

            cameraFollow.offset = Vector3.zero;
        }
    }


}
