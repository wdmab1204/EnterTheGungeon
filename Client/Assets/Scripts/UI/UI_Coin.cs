using UnityEngine;
using UnityEngine.UI;

namespace GameEngine.UI
{
    public class UI_Coin : MonoBehaviour
    {
        [SerializeField] private Text text;

        private void OnEnable()
        {
            var coin = GameData.Coin;
            coin.OnValueChanged += OnCoinChanged;
            OnCoinChanged(coin.Value);
        }

        private void OnDisable()
        {
            GameData.Coin.OnValueChanged -= OnCoinChanged;
        }

        private void OnCoinChanged(int coin)
        {
            text.text = coin.ToString();
        }
    }

}

