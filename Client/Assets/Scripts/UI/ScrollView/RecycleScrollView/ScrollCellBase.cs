using UnityEngine;

namespace GameEngine.UI
{
    public abstract class ScrollCellBase<T> : MonobehaviourExtension
    {
        private Vector3[] corners = new Vector3[4];
        
        public virtual void SetVisible(bool visible) => GameObject.SetActive(visible);
        
        public int Index { get; set; }
        
        public virtual Context Context { get; set; }
        
        public abstract void UpdateContent(T item);
        
        public Vector2 Top
        {
            get
            {
                RectTransform.GetLocalCorners(corners);
                return RectTransform.anchoredPosition + new Vector2(0.0f, corners[1].y);
            }
            set
            {
                RectTransform.GetLocalCorners(corners);
                RectTransform.anchoredPosition = value - new Vector2(0.0f, corners[1].y);
            }
        }

        public Vector2 Bottom
        {
            get
            {
                RectTransform.GetLocalCorners(corners);
                return RectTransform.anchoredPosition + new Vector2(0.0f, corners[3].y);
            }
            set
            {
                RectTransform.GetLocalCorners(corners);
                RectTransform.anchoredPosition = value - new Vector2(0.0f, corners[3].y);
            }
        }
    }
}