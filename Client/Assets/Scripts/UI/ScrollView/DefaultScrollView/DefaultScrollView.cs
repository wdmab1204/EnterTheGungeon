using System.Collections.Generic;
using UnityEngine;

namespace GameEngine.UI
{
    public class DefaultScrollView : MonobehaviourExtension, IScrollView<QuestCellModel>
    {
        public void Initialize()
        {
        }

        public void UpdateContent(IList<QuestCellModel> list)
        {
        }

        public void SetVisible(bool visible) => GameObject.SetActive(visible);
    }
}