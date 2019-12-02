using System.Collections.Generic;

namespace Screeps3D.RoomObjects
{
    /*{
      "_id": "5a0da460ab17fd00012bf0e9",
      "type": "spawn",
      "room": "E2S7",
      "x": 15,
      "y": 19,
      "name": "Spawn1",
      "user": "5a0da017ab17fd00012bf0e7",
      "energy": 9,
      "energyCapacity": 300,
      "hits": 5000,
      "hitsMax": 5000,
      "spawning": {
        "name": "E2S7s_E1S7_swarmMiner0_60",
        "needTime": 21,
        "remainingTime": 12
      },
      "notifyWhenAttacked": true,
      "off": false
    }*/

    public class Spawn : OwnedStoreStructure//, IEnergyObject
    {
        public string SpawningName { get; set; }
        public float SpawningNeedTime { get; set; }
        public float SpawningRemainingTime { get; set; }

        internal override void Unpack(JSONObject data, bool initial)
        {
            base.Unpack(data, initial);

            if (!data.HasField("spawning"))
            {
                SpawningName = null;
                return;
            }

            var spawningData = data["spawning"];
            if (spawningData.HasField("name"))
                SpawningName = spawningData["name"].str;

            if (spawningData.HasField("needTime"))
                SpawningNeedTime = spawningData["needTime"].n;

            if (spawningData.HasField("remainingTime"))
                SpawningRemainingTime = spawningData["remainingTime"].n;
        }
    }
}