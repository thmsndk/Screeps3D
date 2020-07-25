using System.Collections.Generic;
using Common;
using UnityEngine;

namespace Screeps3D.RoomObjects
{
    /*{
        "_id":"594c4187c46642cc2ce46dff",
        "type":"nuker",
        "x":14,
        "y":18,
        "room":"W2S12",
        "notifyWhenAttacked":true,
        "user":"567d9401f60a26fc4c41bd38",
        "energy":300000,
        "energyCapacity":300000,
        "G":5000,
        "GCapacity":5000,
        "hits":1000,
        "hitsMax":1000,
        "cooldownTime":2.247301E+07
    }*/

    public class Nuker : OwnedStoreStructure, IResourceObject, ICooldownTime
    {
        public float ResourceAmount { get; set; }
        public float ResourceCapacity { get; set; }
        public string ResourceType { get; set; }
        public long CooldownTime { get; set; }
        public float maxCooldown = 100000f;

        internal Nuker()
        {
            ResourceType = "G";
        }
        
        internal override void Unpack(JSONObject data, bool initial)
        {
            base.Unpack(data, initial);
            UnpackUtility.Cooldown(this, data);
            
            ResourceCapacity = this.Capacity.ContainsKey(ResourceType) ? this.Capacity[ResourceType] : 0;
            ResourceAmount = this.Store.ContainsKey(ResourceType) ? this.Store[ResourceType] : 0;
        }
    }
}