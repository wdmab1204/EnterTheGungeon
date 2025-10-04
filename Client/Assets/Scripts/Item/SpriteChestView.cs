using UnityEngine;

namespace GameEngine.Item
{
    public interface IChestView
    {
        void ShowOpened();
        void ShowClosed();
    }


    public class SpriteChestView : MonoBehaviour, IChestView
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite openSprite;
        [SerializeField] private Sprite closeSprite;

        public void ShowOpened()
        {
            spriteRenderer.sprite = openSprite;
        }

        public void ShowClosed()
        {
            spriteRenderer.sprite = closeSprite;
        }
    }
}
