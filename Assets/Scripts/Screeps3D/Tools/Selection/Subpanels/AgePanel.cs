using System;
using Common;
using Screeps3D.RoomObjects;
using TMPro;
using UnityEngine;

namespace Screeps3D.Tools.Selection.Subpanels
{
    public class AgePanel : LinePanel
    {
        [SerializeField] private TextMeshProUGUI _label = default;
        [SerializeField] private ScaleAxes _meter = default;
        
        private Creep _creep;

        public override string Name
        {
            get { return "Age"; }
        }

        public override Type ObjectType
        {
            get { return typeof(Creep); }
        }

        public override void Load(RoomObject roomObject)
        {
            _creep = roomObject as Creep;
            if (_creep == null)
                return;
            
            _creep.OnDelta += OnTick;
            UpdateLabel();
        }

        private void OnTick(JSONObject obj)
        {
            UpdateLabel();
        }

        private void UpdateLabel()
        {
            var ttl = _creep.AgeTime - _creep.Room.GameTime;
            _label.text = string.Format("{0:n0}", ttl);
            _meter.SetVisibility((float) ttl / 1500);
        }

        public override void Unload()
        {
            _creep.OnDelta -= OnTick;
            _creep = null;
        }
    }
}