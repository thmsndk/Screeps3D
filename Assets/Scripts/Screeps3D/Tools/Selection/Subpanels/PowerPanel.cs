using System;
using Common;
using Screeps3D.RoomObjects;
using TMPro;
using UnityEngine;

namespace Screeps3D.Tools.Selection.Subpanels
{
    public class PowerPanel : LinePanel
    {
        [SerializeField] private TextMeshProUGUI _label;
        [SerializeField] private ScaleAxes _meter;
        private IPowerObject _powerObject;
        private RoomObject _roomObject;

        public override string Name
        {
            get { return "Power"; }
        }

        public override Type ObjectType
        {
            get { return typeof(IPowerObject); }
        }

        public override void Load(RoomObject roomObject)
        {
            _roomObject = roomObject;
            roomObject.OnDelta += OnDelta;
            _powerObject = roomObject as IPowerObject;
            UpdateLabel();
        }

        private void UpdateLabel()
        {
            _meter.SetVisibility(_powerObject.Power / _powerObject.PowerCapacity);
            _label.text = string.Format("{0:n0} / {1:n0}", _powerObject.Power,
                (long) _powerObject.PowerCapacity);
        }

        private void OnDelta(JSONObject obj)
        {
            var hitsData = obj["power"];
            if (hitsData == null) return;
            UpdateLabel();
        }

        public override void Unload()
        {
            if (_roomObject == null)
                return;
            _roomObject.OnDelta -= OnDelta;
            _roomObject = null;
        }
    }
}