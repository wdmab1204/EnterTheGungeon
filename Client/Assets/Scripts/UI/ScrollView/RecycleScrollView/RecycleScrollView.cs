using System.Collections.Generic;
using UnityEngine;

namespace GameEngine.UI
{
    public class RecycleScrollView : RecycleScrollViewBase<QuestCellModel>
    {
        [SerializeField] private float cellSize;
        [SerializeField] private GameObject cellPrefab;

        protected override float CellSize => cellSize;

        protected override GameObject CellPrefab => cellPrefab;

        public override void Initialize()
        {
            base.Initialize();
            Context.OnClickCell = OnClickClaimButton;
        }

        private void OnClickClaimButton(string msg)
        {
            Debug.Log($"[RecycleScrollView] : {msg}");
        }
    }
}