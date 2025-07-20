using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GameEngine.UI
{
    public class UI_Ammo : MonoBehaviour
    {
        [SerializeField] private Text text;
        [SerializeField] private Slider slider;

        private IEnumerator Start()
        {
            yield return new WaitUntil(()=>GameData.EquipGunData != null);

            GameData.AmmoSize.OnValueChanged += OnAmmoDataChanged;
            GameData.AmmoCount.OnValueChanged += OnAmmoDataChanged;

            OnAmmoDataChanged(0);
        }

        private void OnAmmoDataChanged(int _)
        {
            Set(GameData.AmmoSize.Value, GameData.EquipGunData.AmmoSize, GameData.AmmoCount.Value);
        }

        public void Set(int ammoSize, int maxAmmoSize, int ammoCount)
        {
            text.text = $"{ammoSize} / {(ammoCount == -1 ? "Inf" : ammoCount.ToString())}";
            RectTransform rectTransform = (RectTransform)slider.transform;
            Vector2 sizeDelta = rectTransform.sizeDelta;
            sizeDelta.y = (maxAmmoSize <= 10 ? 10 : 5) * maxAmmoSize;
            rectTransform.sizeDelta = sizeDelta;

            slider.maxValue = maxAmmoSize;
            slider.value = ammoSize;
        }
    }
}
