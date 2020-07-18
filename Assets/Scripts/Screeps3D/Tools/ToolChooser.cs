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
        [SerializeField] private Toggle _spawnToggle = default;

        private IVisibilityMod _vis;
        public ToolType CurrentTool { get; private set; }
        public Action<ToolType> OnToolChange;

        private void Start()
        {
            _vis = GetComponent<IVisibilityMod>();
            _selectionToggle.onValueChanged.AddListener(isOn => ToggleInput(isOn, ToolType.Selection));
            _flagToggle.onValueChanged.AddListener(isOn => ToggleInput(isOn, ToolType.Flag));
            _constructionToggle.onValueChanged.AddListener(isOn => ToggleInput(isOn, ToolType.Construction));
            _spawnToggle.onValueChanged.AddListener(isOn => ToggleInput(isOn, ToolType.Spawn));

            GameManager.OnModeChange += OnModeChange;
            
            _vis.Hide(true);
        }

        private void OnModeChange(GameMode mode)
        {
            _vis.SetVisibility(mode == GameMode.Room);
        }

        public void SelectTool(ToolType toolType)
        {
            Toggle toggle = GetToggleButton(toolType);
            toggle.isOn = true;

        }

        private void ToggleInput(bool isOn, ToolType toolType)
        {
            if (!isOn)
                return;
            CurrentTool = toolType;
            if (OnToolChange != null)
                OnToolChange(toolType);
        }

        internal void Show(ToolType tool)
        {
            SetToggleActive(tool, true);
        }

        internal void Hide(ToolType tool)
        {
            SetToggleActive(tool, false);
        }

        private void SetToggleActive(ToolType tool, bool show)
        {
            Toggle toggle = GetToggleButton(tool);

            toggle?.gameObject.SetActive(show);
        }

        private Toggle GetToggleButton(ToolType tool)
        {
            Toggle toggle = null;
            switch (tool)
            {
                case ToolType.Selection:
                    toggle = _selectionToggle;
                    break;
                case ToolType.Flag:
                    toggle = _flagToggle;
                    break;
                case ToolType.Construction:
                    toggle = _constructionToggle;
                    break;
                case ToolType.Spawn:
                    toggle = _spawnToggle;
                    break;
            }

            return toggle;
        }
    }

    public enum ToolType
    {
        Selection,
        Flag,
        Construction,
        Spawn
    }
}