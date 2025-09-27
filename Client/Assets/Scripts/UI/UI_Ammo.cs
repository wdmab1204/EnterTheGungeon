using GameEngine.DataSequence.EventBus;
using UnityEngine;
using UnityEngine.UI;

namespace GameEngine.UI
{
    public class UI_Ammo : MonoBehaviour
    {
        [SerializeField] private Text ammosizeText;
        [SerializeField] private Text ammocountText;
        [SerializeField] private Slider slider;

        private void OnEnable()
        {
            //yield return new WaitUntil(()=>GameData.EquipGunData != null);

            //GameData.AmmoSize.OnValueChanged += OnAmmoDataChanged;
            //GameData.AmmoCount.OnValueChanged += OnAmmoDataChanged;
            EventBus.Subscribe<int>("AmmoSize", OnAmmoSize);
            EventBus.Subscribe<int>("AmmoSizeMax", OnMaxAmmoSize);
            EventBus.Subscribe<int>("AmmoCount", OnAmmoCount);

            //OnAmmoDataChanged(0);
        }

        private void OnDisable()
        {
            EventBus.UnSubScribe<int>("AmmoSize", OnAmmoSize);
            EventBus.UnSubScribe<int>("AmmoSizeMax", OnMaxAmmoSize);
            EventBus.UnSubScribe<int>("AmmoCount", OnAmmoCount);
        }



        private void OnAmmoSize(int ammoSize)
        {
            ammosizeText.text = ammoSize.ToString();
            slider.value = ammoSize;
        }

        private void OnMaxAmmoSize(int maxAmmoSize)
        {
            RectTransform rectTransform = (RectTransform)slider.transform;
            Vector2 sizeDelta = rectTransform.sizeDelta;
            sizeDelta.y = (maxAmmoSize <= 10 ? 10 : 5) * maxAmmoSize;
            rectTransform.sizeDelta = sizeDelta;

            slider.maxValue = maxAmmoSize;
        }

        private void OnAmmoCount(int ammoCount)
        {
            ammocountText.text = $"/ {(ammoCount == -1 ? "Inf" : ammoCount.ToString())}";
        }

        private void OnAmmoDataChanged(int _)
        {
            Set(GameData.AmmoSize.Value, GameData.EquipGunData.AmmoSize, GameData.AmmoCount.Value);
        }

        public void Set(int ammoSize, int maxAmmoSize, int ammoCount)
        {
            ammosizeText.text = $"{ammoSize} / {(ammoCount == -1 ? "Inf" : ammoCount.ToString())}";
            RectTransform rectTransform = (RectTransform)slider.transform;
            Vector2 sizeDelta = rectTransform.sizeDelta;
            sizeDelta.y = (maxAmmoSize <= 10 ? 10 : 5) * maxAmmoSize;
            rectTransform.sizeDelta = sizeDelta;

            slider.maxValue = maxAmmoSize;
            slider.value = ammoSize;
        }
    }
}
