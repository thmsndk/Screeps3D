using System;
using Common;
using Screeps3D.RoomObjects;
using TMPro;
using UnityEngine;

namespace Screeps3D.Tools.Selection.Subpanels
{
    public class CapacityPanel : LinePanel
    {
        [SerializeField] private TextMeshProUGUI _label = default;
        [SerializeField] private ScaleAxes _meter = default;
        private IStoreObject _storeObject;
        private RoomObject _roomObject;

        public override string Name
        {
            get { return "Capacity"; }
        }

        public override Type ObjectType
        {
            get { return typeof(IStoreObject); }
        }

        public override void Load(RoomObject roomObject)
        {
            _roomObject = roomObject;
            roomObject.OnDelta += OnDelta;
            _storeObject = roomObject as IStoreObject;
            UpdateLabel();
        }

        private void UpdateLabel()
        {
            _meter.SetVisibility(_storeObject.TotalResources / _storeObject.TotalCapacity);
            _label.text = string.Format("{0:n0} / {1:n0}", _storeObject.TotalResources,
                (long) _storeObject.TotalCapacity);
        }

        private void OnDelta(JSONObject obj)
        {
            if (obj.Count > 0)
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