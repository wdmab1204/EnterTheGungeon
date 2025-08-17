using UnityEngine;

namespace GameEngine.UI
{
    public class MinimapOveray : MonoBehaviour, IMinimapDisplay
    {
        [SerializeField] private Transform mask;
        [SerializeField] private RectTransform playerIcon;

        private RectTransform minimapLayer;

        public void OnMovePlayer(Vector3 playerPosition)
        {
            minimapLayer.anchoredPosition = -(playerPosition);
        }

        public void SetGameObject(GameObject minimapObj)
        {
            minimapLayer = GameUtility.FindChild<RectTransform>(minimapObj, "Layer Group");

            RectTransform minimapRect = minimapObj.GetComponent<RectTransform>();
            minimapObj.transform.SetParent(mask, true);

            minimapLayer.anchoredPosition = Vector2.zero;
            minimapRect.anchoredPosition = Vector2.zero;
            playerIcon.anchoredPosition = Vector2.zero;
        }


    }
}