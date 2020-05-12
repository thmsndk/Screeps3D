using System;
using Screeps3D.RoomObjects;
using TMPro;
using UnityEngine;

namespace Screeps3D.Tools.Selection.Subpanels
{
    public class PositionPanel : LinePanel
    {
        [SerializeField] private TextMeshProUGUI _label = default;
        
        private RoomObject _roomObject;
        private Creep _creep;

        public override string Name
        {
            get { return "Pos"; }
        }

        public override Type ObjectType
        {
            get { return typeof(RoomObject); }
        }

        public override void Load(RoomObject roomObject)
        {
            _roomObject = roomObject;
            _creep = roomObject as Creep;
            if (_creep != null)
                _creep.OnDelta += OnDelta;
            UpdateLabel();
        }

        private void OnDelta(JSONObject data)
        {
            UpdateLabel();
        }

        public override void Unload()
        {
            if (_creep != null)
                _creep.OnDelta -= OnDelta;
            _roomObject = null;
            _creep = null;
        }

        private void UpdateLabel()
        {
            _label.text = string.Format("{0}, {1}", _roomObject.X, _roomObject.Y);
        }
    }
}