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
            // Ŭ���ϸ� �巡�� ����
            isDragging = true;

            // Ŭ���� ��ġ�� UI ������Ʈ�� �Ÿ� ����
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
            // �巡�� ����
            isDragging = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (isDragging)
            {
                // �巡�� ���� �� ������Ʈ �̵�
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