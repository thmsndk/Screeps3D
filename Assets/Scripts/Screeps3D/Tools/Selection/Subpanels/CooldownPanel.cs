using System;
using Screeps3D.RoomObjects;
using TMPro;
using UnityEngine;

namespace Screeps3D.Tools.Selection.Subpanels
{
    public class CooldownPanel : LinePanel
    {
        [SerializeField] private TextMeshProUGUI _label = default;

        private RoomObject _roomObject;
        private ICooldownObject _cooldownObject;

        public override string Name
        {
            get { return "Cooldown"; }
        }

        public override Type ObjectType
        {
            get { return typeof(ICooldownObject); }
        }

        public override void Load(RoomObject roomObject)
        {
            _roomObject = roomObject;
            _cooldownObject = roomObject as ICooldownObject;
            _roomObject.OnDelta += OnDelta;
            UpdateDisplay();
        }

        private void OnDelta(JSONObject obj)
        {
            var cooldownData = obj["cooldown"];
            if (cooldownData == null)
                return;
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            _label.text = string.Format("{0}", _cooldownObject.Cooldown);
        }

        public override void Unload()
        {
            _roomObject.OnDelta -= OnDelta;
            _roomObject = null;
            _cooldownObject = null;
        }
    }
}