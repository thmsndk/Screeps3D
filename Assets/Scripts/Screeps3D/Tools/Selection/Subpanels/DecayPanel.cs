using System;
using Screeps3D.RoomObjects;
using Screeps_API;
using TMPro;
using UnityEngine;

namespace Screeps3D.Tools.Selection.Subpanels
{
    public class DecayPanel : LinePanel
    {
        [SerializeField] private TextMeshProUGUI _label;
        
        private IDecay _decay;

        public override string Name
        {
            get { return "Decay"; }
        }

        public override Type ObjectType
        {
            get { return typeof(IDecay); }
        }

        public override void Load(RoomObject roomObject)
        {
            _decay = roomObject as IDecay;
            UpdateLabel();
            ScreepsAPI.OnTick += OnTick;
        }

        private void OnTick(long obj)
        {
            UpdateLabel();
        }

        public override void Unload()
        {
            _decay = null;
            ScreepsAPI.OnTick -= OnTick;
        }

        private void UpdateLabel()
        {
            if (_decay.NextDecayTime == 0f)
            {
                Hide();
                return;
            }

            _label.text = string.Format("{0:n0}", _decay.NextDecayTime - _decay.Room.GameTime);
        }
    }
}