namespace Screeps3D.RoomObjects
{
    /*{
        "_id":"594f8d23ea895523478670b4",
        "type":"observer",
        "x":13,
        "y":18,
        "room":"W8S12",
        "notifyWhenAttacked":true,
        "user":"567d9401f60a26fc4c41bd38",
        "hits":500,
        "hitsMax":500,
        "observeRoom":null
    }*/
    public class Observer : OwnedStructure
    {
        internal override void Unpack(JSONObject data, bool initial)
        {
            base.Unpack(data, initial);
        }
    }
}