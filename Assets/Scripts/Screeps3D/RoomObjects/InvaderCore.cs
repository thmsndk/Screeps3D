using UnityEngine;
using System.Collections.Generic;

namespace Screeps3D.RoomObjects
{

    public class InvaderCore : OwnedStoreStructure, ICooldownObject//, IActionObject
    {
        // TODO: Effects
        public float Cooldown { get; set; }
        // public Dictionary<string, JSONObject> Actions { get; set; }

        internal InvaderCore()
        {
            // Actions = new Dictionary<string, JSONObject>();
        }

        internal override void Unpack(JSONObject data, bool initial)
        {
            base.Unpack(data, initial);
            UnpackUtility.Cooldown(this, data);
        }
    }
}