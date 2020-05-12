using System;
using Common;
using Screeps3D.RoomObjects;
using TMPro;
using UnityEngine;

namespace Screeps3D.Tools.Selection.Subpanels
{
    public class EnergyPanel : LinePanel
    {
        [SerializeField] private TextMeshProUGUI _label = default;
        [SerializeField] private ScaleAxes _meter = default;
        private IEnergyObject _energyObject;
        private RoomObject _roomObject;

        public override string Name
        {
            get { return "Energy"; }
        }

        public override Type ObjectType
        {
            get { return typeof(IEnergyObject); }
        }

        public override void Load(RoomObject roomObject)
        {
            _roomObject = roomObject;
            roomObject.OnDelta += OnDelta;
            _energyObject = roomObject as IEnergyObject;
            UpdateLabel();
        }

        private void UpdateLabel()
        {
            _meter.SetVisibility(_energyObject.Energy / _energyObject.EnergyCapacity);
            _label.text = string.Format("{0:n0} / {1:n0}", _energyObject.Energy,
                (long) _energyObject.EnergyCapacity);
        }

        private void OnDelta(JSONObject obj)
        {
            var hitsData = obj["energy"];
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