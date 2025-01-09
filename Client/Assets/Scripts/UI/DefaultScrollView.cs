using System.Collections.Generic;
using UnityEngine;

namespace GameEngine.UI
{
    public class DefaultScrollView : CacheObject, IScrollView<QuestCellModel>
    {
        public void Initialize()
        {
        }

        public void UpdateContent(IList<QuestCellModel> list)
        {
        }

        public void SetVisible(bool visible) => gameObject.SetActive(visible);
    }
}