using System.Collections.Generic;

namespace Screeps3D.RoomObjects
{
    /*{
        "_id":"594816d4dc09e4106f2b782f",
        "type":"link",
        "x":20,
        "y":8,
        "room":"W8S12",
        "notifyWhenAttacked":true,
        "user":"567d9401f60a26fc4c41bd38",
        "energy":779,
        "energyCapacity":800,
        "cooldown":0,
        "hits":1000,
        "hitsMax":1000,
        "actionLog":{
            "transferEnergy":null
        }
    }*/
    public class Link : OwnedStoreStructure, ICooldownObject, IActionObject//, IEnergyObject
    {
        public Dictionary<string, JSONObject> Actions { get; set; }
        public float Cooldown { get; set; }

        internal Link()
        {
            Actions = new Dictionary<string, JSONObject>();
        }

        internal override void Unpack(JSONObject data, bool initial)
        {
            base.Unpack(data, initial);

            UnpackUtility.Store(this, data);
            UnpackUtility.Cooldown(this, data);
            UnpackUtility.ActionLog(this, data);
        }

    }
}