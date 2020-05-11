using System.Collections.Generic;
using System.Linq;
using Common;
using Screeps3D.RoomObjects;
using UnityEngine;

namespace Screeps3D.Tools.Selection
{
    public class SelectionUI : BaseSingleton<SelectionUI>
    {
        [SerializeField] private VerticalPanelGroup _panelGroup = default;

        private Dictionary<string, SelectionPanel> _panels = new Dictionary<string, SelectionPanel>();

        private void Start()
        {
            Selection.Instance.OnSelect += OnSelect;
            Selection.Instance.OnDeselect += OnDeselect;
        }

        private void OnSelect(RoomObject obj)
        {
            if (_panels.ContainsKey(obj.Id))
                return;
            
            Scheduler.Instance.Add(() =>
            {
                if (!Selection.Instance.Selections.ContainsKey(obj.Id))
                    return;
                
                var panel = SelectionPanel.GetInstance();
                panel.Load(obj);
                _panelGroup.AddElement(panel);
                _panels[obj.Id] = panel;
            });
        }

        private void OnDeselect(RoomObject obj)
        {
            if (!_panels.ContainsKey(obj.Id))
                return;
            var panel = _panels[obj.Id];

            panel.Unload();
            _panelGroup.RemoveElement(panel);
            _panels.Remove(obj.Id);
        }
    }
}