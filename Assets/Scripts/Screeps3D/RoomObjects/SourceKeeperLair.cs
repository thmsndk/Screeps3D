using UnityEngine;

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

    public class SourceKeeperLair : RoomObject
    {
        public float NextSpawnTime { get; set; }


        internal override void Unpack(JSONObject data, bool initial)
        {
            base.Unpack(data, initial);

            var nextSpawnTime = data["nextSpawnTime"];
            if (nextSpawnTime != null)
            {
                NextSpawnTime = nextSpawnTime.n; // is this the game tick of spawning?
                Debug.Log(this.Id + "" + NextSpawnTime);
            }
        }
    }
}