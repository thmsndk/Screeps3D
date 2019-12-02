using UnityEngine;

namespace Screeps3D.RoomObjects
{
    /*{
        "_id":"5949e649dd5848a527eaafd6",
        "type":"extractor",
        "x":9,
        "y":13,
        "room":"W8S12",
        "notifyWhenAttacked":true,
        "user":"567d9401f60a26fc4c41bd38",
        "hits":500,
        "hitsMax":500,
        "cooldown":0
    }*/
    public class Extractor : OwnedStructure, ICooldownObject, IOwnedObject
    {
        public float Cooldown { get; set; }

        internal override void Unpack(JSONObject data, bool initial)
        {
            base.Unpack(data, initial);
            UnpackUtility.Cooldown(this, data);
        }
    }
}