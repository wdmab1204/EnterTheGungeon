using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameEngine.UI
{
    public abstract class AutoDisableScrollViewBase<T> : CacheObject, IScrollView<T>
    {
        private ScrollRect scrollRect;
        private List<AutoDisableCellBase<T>> cellList = new();

        [SerializeField] private GameObject cellPrefab;
        protected abstract float CellSize { get; }
        protected virtual Context Context { get; set; } = new();

        public virtual void Initialize()
        {
            scrollRect = GetComponent<ScrollRect>();
            scrollRect.onValueChanged.AddListener(OnScrollChanged);
            cellPrefab.SetActive(false);
        }

        public void UpdateContent(IList<T> list)
        {
            for (int index = 0; index < list.Count; index++)
            {
                var cell = CreateCell();
                cell.Top = Vector2.down * index * CellSize;
                cell.Index = index;
                cell.UpdateContent(list[index]);
            }

            UpdateContentSize(list.Count);
        }
        
        public void SetVisible(bool visible) => gameObject.SetActive(visible);
        
        private void UpdateContentSize(int dataCount)
        {
            float contentHeight = dataCount * CellSize;
            Vector2 sizeDelta = scrollRect.content.sizeDelta;
            sizeDelta.y = contentHeight;
            scrollRect.content.sizeDelta = sizeDelta;
        }
        
        protected virtual AutoDisableCellBase<T> CreateCell()
        {
            GameObject obj = Instantiate(cellPrefab);

            if (obj.TryGetComponent<AutoDisableCellBase<T>>(out var cell) == false)
            {
                Debug.LogError("Cell Component could not found.");
                return null;
            }

            cell.SetVisible(true);
            cell.Context = this.Context;
            cell.MyTransform.SetParent(scrollRect.content.transform, false);
            cellList.Add(cell);

            return cell;
        }

        private void OnScrollChanged(Vector2 vec)
        {
            for (int i = 0; i < cellList.Count; i++)
            {
                var cell = cellList[i];
                if (cell.Bottom.y < -scrollRect.content.anchoredPosition.y &&
                    cell.Top.y > -scrollRect.content.anchoredPosition.y - scrollRect.viewport.rect.height)
                {
                    cell.SetVisible(true);
                }
                else
                    cell.SetVisible(false);
            }
        }
    }
}