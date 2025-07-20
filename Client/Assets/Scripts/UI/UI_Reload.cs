using System.Collections;
using UnityEngine;

namespace GameEngine.UI
{
    public class UI_Reload : MonoBehaviour
    {
        [SerializeField] Transform handle;
        private float startX;

        private void Start()
        {
            startX = handle.localPosition.x;
            this.gameObject.SetActive(false);

            //GameData.AmmoSize.OnValueChanged += x =>
            //{
            //    if (x == 0)
            //        Play(GameData.EquipGunData.ReloadTime);
            //};
        }

        [ContextMenu("Test Reload")]
        void TestReload()
        {
            Play(5f);
        }

        public void Play(float duration)
        {
            if (this.gameObject.activeSelf == true)
                return;

            this.gameObject.SetActive(true);

            StopAllCoroutines();
            StartCoroutine(CoPlay(duration));
        }

        private IEnumerator CoPlay(float duration)
        {
            var handlePosition = handle.localPosition;
            handlePosition.x = startX;
            handle.localPosition = handlePosition;

            float endX = -startX;

            float t = 0f;
            while(t < duration)
            {
                t += Time.deltaTime;
                float ratio = Mathf.Clamp01(t / duration);
                float lerpX = startX + (endX - startX) * ratio;

                handlePosition.x = lerpX;
                handle.localPosition = handlePosition;

                yield return null;
            }

            this.gameObject.SetActive(false);
        }
    }
}
