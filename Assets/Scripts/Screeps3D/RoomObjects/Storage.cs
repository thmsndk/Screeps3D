using System.Collections.Generic;
using System.Diagnostics;

namespace Screeps3D.RoomObjects
{
    /*{
        "_id":"59469528473155f701d2e6d1",
        "type":"storage",
        "x":8,
        "y":19,
        "room":"W8S12",
        "notifyWhenAttacked":true,
        "user":"567d9401f60a26fc4c41bd38",
        "energy":247268,
        "energyCapacity":1000000,
        "hits":10000,
        "hitsMax":10000,
        "H":7450,
        "power":0,
        "ZK":0,
        "XUH2O":1000,
        "XKHO2":1000,
        "XLH2O":0,
        "XLHO2":0,
        "XZH2O":1000,
        "XZHO2":0,
        "XGHO2":0,
        "G":0,
        "XGH2O":0,
        "K":0,
        "L":0,
        "Z":0,
        "U":0
    }*/

    public class Storage : OwnedStoreStructure
    {
        internal override void Unpack(JSONObject data, bool initial)
        {
            base.Unpack(data, initial);
        }
    }
}