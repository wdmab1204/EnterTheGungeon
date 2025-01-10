using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace GameEngine.UI
{
    public abstract class RecycleScrollViewBase<TItem> : CacheObject, IScrollView<TItem>
    {
        private ScrollRect scrollRect;
        private LinkedList<RecycleScrollCellBase<TItem>> poolDeque = new();
        private IList<TItem> dataList;
        private Vector2 prevScrollDirection;

        protected abstract float CellSize { get; }
        protected abstract GameObject CellPrefab { get; }
        protected virtual Context Context { get; } = new();

        public virtual void Initialize()
        {
            CellPrefab.gameObject.SetActive(false);
            scrollRect = GetComponent<ScrollRect>();
            scrollRect.onValueChanged.AddListener(OnScrollChanged);
        }

        private void OnScrollChanged(Vector2 scrollDirection)
        {
            UpdateCells((scrollDirection.y < prevScrollDirection.y) ? 1 : -1);
            prevScrollDirection = scrollDirection;
        }

        private void UpdateCells(int scrollDirection)
        {
            //위로 스크롤 할 때
            if (scrollDirection > 0)
            {
                var firstCell = poolDeque.First.Value;
                var lastCell = poolDeque.Last.Value;
                
                //맨위의 Cell이 Viewport바깥에 있는가?
                if (firstCell.Bottom.y > -scrollRect.content.anchoredPosition.y &&
                    lastCell.Index + 1 < dataList.Count)
                {
                    //맨위의 Cell을 맨 아래로 이동
                    firstCell.Top = lastCell.Bottom;
                    firstCell.Index = lastCell.Index + 1;

                    UpdateCell(firstCell, dataList[lastCell.Index + 1]);

                    poolDeque.AddLast(firstCell);
                    poolDeque.RemoveFirst();
                }
            }
            //아래로 스크롤  할 때
            else if (scrollDirection < 0)
            {
                var firstCell = poolDeque.First.Value;
                var lastCell = poolDeque.Last.Value;
                
                //맨 아래의 Cell이 Viewport바깥에 있는가?
                if (lastCell.Top.y < -scrollRect.content.anchoredPosition.y - scrollRect.viewport.rect.height &&
                    firstCell.Index - 1 >= 0)
                {
                    //맨 아래의 Cell을 맨 위로 이동
                    lastCell.Bottom = firstCell.Top;
                    lastCell.Index = firstCell.Index - 1;

                    UpdateCell(lastCell, dataList[firstCell.Index - 1]);

                    poolDeque.AddFirst(lastCell);
                    poolDeque.RemoveLast();
                }
            }
        }

        public void UpdateContent(IList<TItem> list)
        {
            dataList = list;
            var scrollRectSizeDelta = scrollRect.GetComponent<RectTransform>().sizeDelta;

            int index = 0;
            for (float i = 0f; i <= scrollRectSizeDelta.y + CellSize; i += CellSize)
            {
                var cell = CreateCell();
                cell.Index = index;
                cell.Top = Vector2.down * index * CellSize;

                UpdateCell(cell, list[index]);
                index += 1;
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

        protected virtual RecycleScrollCellBase<TItem> CreateCell()
        {
            GameObject obj = Instantiate(CellPrefab);

            if (obj.TryGetComponent<RecycleScrollCellBase<TItem>>(out var cell) == false)
            {
                Debug.LogError("Cell Component could not found.");
                return null;
            }

            cell.SetVisible(true);
            cell.Context = this.Context;
            cell.MyTransform.SetParent(scrollRect.content.transform, false);
            poolDeque.AddLast(cell);

            return cell;
        }

        private void UpdateCell(RecycleScrollCellBase<TItem> cell, TItem data)
        {
            cell.UpdateContent(data);
        }
    }
}