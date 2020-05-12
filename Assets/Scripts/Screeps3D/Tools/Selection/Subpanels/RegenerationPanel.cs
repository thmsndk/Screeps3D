using System;
using Screeps3D.RoomObjects;
using TMPro;
using UnityEngine;

namespace Screeps3D.Tools.Selection.Subpanels
{
    public class RegenerationPanel : LinePanel
    {
        [SerializeField] private TextMeshProUGUI _label = default;
        private IRegenerationObject _regenObject;
        private RoomObject _roomObject;

        public override string Name
        {
            get { return "Regeneration"; }
        }

        public override Type ObjectType
        {
            get { return typeof(IRegenerationObject); }
        }

        public override void Load(RoomObject roomObject)
        {
            roomObject.OnDelta += OnDelta;
            _regenObject = roomObject as IRegenerationObject;
            UpdateLabel();
        }

        private void UpdateLabel()
        {
            if (_regenObject.NextRegenerationTime == 0)
                _label.text = string.Format("{0:n0}", 0);
            else
                _label.text = string.Format("{0:n0}", _regenObject.NextRegenerationTime - _regenObject.Room.GameTime);
        }

        private void OnDelta(JSONObject obj)
        {
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