using UnityEngine;
using UnityEngine.EventSystems;

namespace GameEngine.UI
{
    public class DragPanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        public Transform targetTransform; 

        private bool isDragging = false;
        private Vector2 dragOffset;

        void Start()
        {
            if (targetTransform == null)
            {
                targetTransform = transform;  
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // 클릭하면 드래그 시작
            isDragging = true;

            // 클릭한 위치와 UI 오브젝트의 거리 저장
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                transform.parent as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPointerPos
            );
            dragOffset = (Vector2)targetTransform.localPosition - localPointerPos;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            // 드래그 종료
            isDragging = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (isDragging)
            {
                // 드래그 중일 때 오브젝트 이동
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    transform.parent as RectTransform,
                    eventData.position,
                    eventData.pressEventCamera,
                    out Vector2 localPointerPos
                ))
                {
                    targetTransform.localPosition = localPointerPos + dragOffset;
                }
            }
        }
    }

}