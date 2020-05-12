using System;
using Screeps3D.RoomObjects;
using TMPro;
using UnityEngine;

namespace Screeps3D.Tools.Selection.Subpanels
{
    public class FatiguePanel : LinePanel
    {
        [SerializeField] private TextMeshProUGUI _label = default;
        
        private Creep _creep;

        public override string Name
        {
            get { return "Fatigue"; }
        }

        public override Type ObjectType
        {
            get { return typeof(Creep); }
        }

        public override void Load(RoomObject roomObject)
        {
            _creep = roomObject as Creep;
            _creep.OnDelta += OnDelta;
            UpdateDisplay();
        }

        private void OnDelta(JSONObject obj)
        {
            var fatigueData = obj["fatigue"];
            if (fatigueData == null)
                return;
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            _label.text = string.Format("{0}", _creep.Fatigue);
        }

        public override void Unload()
        {
            _creep.OnDelta -= OnDelta;
            _creep = null;
        }
    }
}