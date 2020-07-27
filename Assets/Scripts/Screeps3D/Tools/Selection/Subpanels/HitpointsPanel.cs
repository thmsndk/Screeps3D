using System;
using Common;
using Screeps3D.RoomObjects;
using TMPro;
using UnityEngine;

namespace Screeps3D.Tools.Selection.Subpanels
{
    public class HitpointsPanel : LinePanel
    {
        [SerializeField] private TextMeshProUGUI _label = default;
        [SerializeField] private ScaleAxes _meter = default;
        private IHitpointsObject _hitsObject;
        private RoomObject _roomObject;

        public override string Name
        {
            get { return "Hits"; }
        }

        public override Type ObjectType
        {
            get { return typeof(IHitpointsObject); }
        }

        public override void Load(RoomObject roomObject)
        {
            _roomObject = roomObject;
            _hitsObject = roomObject as IHitpointsObject;
            roomObject.OnDelta += OnDelta;
            UpdateLabel();
        }

        public override bool IsPanelAvailabelForObject(RoomObject roomObject)
        {
            return base.IsPanelAvailabelForObject(roomObject) && !roomObject.Type.Equals(Constants.TypeController);
        }

        private void UpdateLabel()
        {
            _meter.SetVisibility(_hitsObject.Hits / _hitsObject.HitsMax);
            if((long) _hitsObject.HitsMax > 0) {
                _label.text = string.Format("{0:n0} / {1:n0}", _hitsObject.Hits, (long) _hitsObject.HitsMax);
            } else {
                _label.text = string.Format("Indestructible");
            }
        }

        private void OnDelta(JSONObject obj)
        {
            var hitsData = obj["hits"];
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