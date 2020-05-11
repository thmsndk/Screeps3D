using System;
using Common;
using UnityEngine;
using UnityEngine.UI;

namespace Screeps3D.Tools
{
    public class ToolChooser : BaseSingleton<ToolChooser>
    {
        [SerializeField] private Toggle _selectionToggle = default;
        [SerializeField] private Toggle _flagToggle = default;
        [SerializeField] private Toggle _constructionToggle = default;
        
        private IVisibilityMod _vis;
        public ToolType CurrentTool { get; private set; }
        public Action<ToolType> OnToolChange;

        private void Start()
        {
            _vis = GetComponent<IVisibilityMod>();
            _selectionToggle.onValueChanged.AddListener(isOn => ToggleInput(isOn, ToolType.Selection));
            _flagToggle.onValueChanged.AddListener(isOn => ToggleInput(isOn, ToolType.Flag));
            _constructionToggle.onValueChanged.AddListener(isOn => ToggleInput(isOn, ToolType.Construction));

            GameManager.OnModeChange += OnModeChange;
            
            _vis.Hide(true);
        }

        private void OnModeChange(GameMode mode)
        {
            _vis.SetVisibility(mode == GameMode.Room);
        }

        private void ToggleInput(bool isOn, ToolType toolType)
        {
            if (!isOn)
                return;
            CurrentTool = toolType;
            if (OnToolChange != null)
                OnToolChange(toolType);
        }
    }

    public enum ToolType
    {
        Selection,
        Flag,
        Construction
    }
}