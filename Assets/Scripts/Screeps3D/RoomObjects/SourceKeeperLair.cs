using Screeps_API;

namespace Screeps3D.RoomObjects
{
    /*
        {
            "_id": "5d0185da1b95a4a5d67d8bad",
            "room": "E4S6",
            "type": "keeperLair",
            "x": 37,
            "y": 7,
            "nextSpawnTime": 1304183
        }
    */

    public class SourceKeeperLair : RoomObject, ISpawningInObject, IOwnedObject
    {
        const string SourceKeeperUserId = "3";

        public float NextSpawnTime { get; set; }
        public string UserId { get; set; }
        public ScreepsUser Owner { get; set; }

        internal override void Unpack(JSONObject data, bool initial)
        {
            base.Unpack(data, initial);
            
            Owner = ScreepsAPI.UserManager.GetUser(SourceKeeperUserId);
            UserId = Owner.Username;
            var nextSpawnTime = data["nextSpawnTime"];
            if (nextSpawnTime != null)
            {
                NextSpawnTime = nextSpawnTime.n;
            }
        }
    }
}