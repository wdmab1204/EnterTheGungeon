using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameEngine.UI
{
    public class AutoDisableScrollView : AutoDisableScrollViewBase<QuestCellModel>
    {
        [SerializeField] private float cellSize;

        protected override float CellSize => cellSize;

        public override void Initialize()
        {
            base.Initialize();
            Context.OnClickCell = OnClickClaimButton;
        }
        
        private void OnClickClaimButton(string msg)
        {
            Debug.Log($"[AutoDisableScrollView] : {msg}");
        }
    }
}