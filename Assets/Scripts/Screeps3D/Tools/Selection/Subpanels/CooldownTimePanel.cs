using System;
using Screeps3D.RoomObjects;
using Screeps_API;
using TMPro;
using UnityEngine;

namespace Screeps3D.Tools.Selection.Subpanels
{
    public class CooldownTimePanel : LinePanel
    {
        [SerializeField] private TextMeshProUGUI _cooldownLabel;

        private ICooldownTime _cooldown;

        public override string Name
        {
            get { return "CooldownTime"; }
        }

        public override Type ObjectType
        {
            get { return typeof(ICooldownTime); }
        }

        public override void Load(RoomObject roomObject)
        {
            _cooldown = roomObject as ICooldownTime;
            UpdateLabel();
            ScreepsAPI.OnTick += OnTick;
        }

        private void OnTick(long obj)
        {
            UpdateLabel();
        }

        public override void Unload()
        {
            _cooldown = null;
            ScreepsAPI.OnTick -= OnTick;
        }

        private void UpdateLabel()
        {
            var cooldown = _cooldown.CooldownTime - _cooldown.Room.GameTime;

            if (cooldown <= 0)
            {
                Hide();
                return;
            }

            Show();

            _cooldownLabel.text = cooldown > 0 ? string.Format("{0:n0}", cooldown) : string.Empty;
        }
    }
}