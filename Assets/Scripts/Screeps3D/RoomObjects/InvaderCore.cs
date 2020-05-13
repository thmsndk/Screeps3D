using UnityEngine;
using System.Collections.Generic;

namespace Screeps3D.RoomObjects
{

    public class InvaderCore : OwnedStoreStructure, ICooldownObject, IActionObject , IEffectObject
    {
        // TODO: Effects
        public float Cooldown { get; set; }
        public JSONObject[] Effects { get; set; }
        public Dictionary<string, JSONObject> Actions { get; set; }

        internal InvaderCore()
        {
            Actions = new Dictionary<string, JSONObject>();
        }

        internal override void Unpack(JSONObject data, bool initial)
        {
            base.Unpack(data, initial);
            Debug.Log(data.ToString());
            UnpackUtility.Cooldown(this, data);
            UnpackUtility.ActionLog(this, data);
            // UnpackUtility.Effects(this, data);
        }
    }
}