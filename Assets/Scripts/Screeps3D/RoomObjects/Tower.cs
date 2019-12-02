using System.Collections.Generic;

namespace Screeps3D.RoomObjects
{
    /*{
        "_id":"5945134eea485cae18c518ef",
        "type":"tower",
        "x":7,
        "y":24,
        "room":"W8S12",
        "notifyWhenAttacked":true,
        "user":"567d9401f60a26fc4c41bd38",
        "energy":990,
        "energyCapacity":1000,
        "hits":3000,
        "hitsMax":3000,
        "actionLog":{
            "attack":null,
            "heal":null,
            "repair":null
        }
    }*/

    public class Tower : OwnedStoreStructure/*, IEnergyObject*/, IActionObject
    {
        public Dictionary<string, JSONObject> Actions { get; set; }

        internal Tower()
        {
            Actions = new Dictionary<string, JSONObject>();
        }
        
        internal override void Unpack(JSONObject data, bool initial)
        {
            base.Unpack(data, initial);

            UnpackUtility.ActionLog(this, data);
        }
    }
}