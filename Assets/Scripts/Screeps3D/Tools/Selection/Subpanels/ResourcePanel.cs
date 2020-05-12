using System;
using Common;
using Screeps3D.RoomObjects;
using TMPro;
using UnityEngine;
using WebSocketSharp;

namespace Screeps3D.Tools.Selection.Subpanels
{
    public class ResourcePanel : LinePanel
    {
        [SerializeField] private TextMeshProUGUI _typeLabel = default;
        [SerializeField] private TextMeshProUGUI _label = default;
        [SerializeField] private ScaleAxes _meter = default;
        private IResourceObject _resourceObject;
        private RoomObject _roomObject;

        public override string Name
        {
            get { return "Resource"; }
        }

        public override Type ObjectType
        {
            get { return typeof(IResourceObject); }
        }

        public override void Load(RoomObject roomObject)
        {
            _roomObject = roomObject;
            roomObject.OnDelta += OnDelta;
            _resourceObject = roomObject as IResourceObject;
            UpdateLabel();
        }

        private void UpdateLabel()
        {
            var label = _resourceObject.ResourceType.IsNullOrEmpty() ? "Resource" : _resourceObject.ResourceType;
            if (label[0] == 'p') label = "Power";
            _typeLabel.text = string.Format("{0}:", label);
            _meter.SetVisibility(_resourceObject.ResourceAmount / _resourceObject.ResourceCapacity);
            _label.text = string.Format("{0:n0} / {1:n0}", _resourceObject.ResourceAmount,
                (long) _resourceObject.ResourceCapacity);
        }

        private void OnDelta(JSONObject obj)
        {
            if (obj.keys.Count == 0) return;
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