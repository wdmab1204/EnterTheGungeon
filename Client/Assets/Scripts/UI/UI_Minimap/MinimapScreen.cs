using UnityEngine;

namespace GameEngine.UI
{
    public class MinimapScreen : MonoBehaviour, IMinimapDisplay
    {
        [SerializeField] private RectTransform playerIcon;
        [SerializeField] private RectTransform zoomGroup;

        private float zoomValue = 3f;
        private float minZoomValue, maxZoomValue;

        private void Start()
        {
            minZoomValue = zoomValue / 2f;
            maxZoomValue = zoomValue * 2f;
        }

        public void SetGameObject(GameObject minimapObj)
        {
            RectTransform minimapRect = minimapObj.GetComponent<RectTransform>();
            RectTransform layerGroup = GameUtil.FindChild<RectTransform>(minimapObj, "Layer Group");

            minimapObj.transform.SetParent(zoomGroup);
            minimapRect.anchoredPosition = Vector2.zero;
            layerGroup.anchoredPosition = Vector2.zero;
            playerIcon.anchoredPosition = Vector2.zero;
        }

        public void OnMovePlayer(Vector3 playerPosition)
        {
            playerIcon.anchoredPosition = playerPosition;
        }

        private void OnDisable()
        {
            zoomGroup.anchoredPosition = Vector3.zero;
        }

        private void Update()
        {
            float scrollDelta = Input.GetAxis("Mouse ScrollWheel");

            zoomValue += scrollDelta * 0.5f;
            zoomValue = Mathf.Clamp(zoomValue + scrollDelta * 0.5f, Mathf.Max(minZoomValue, 0.5f), maxZoomValue);

            zoomGroup.localScale = Vector3.one * zoomValue;
        }
    }
}
