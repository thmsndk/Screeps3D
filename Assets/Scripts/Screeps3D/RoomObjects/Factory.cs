using UnityEngine;

namespace Screeps3D.RoomObjects
{
    /**
      {
		"_id":"5ed2633254ff224b98225c2f",
		"type":"factory",
		"x":21,
		"y":19,
		"room":"E21N59",
		"notifyWhenAttacked":true,
		"user":"5dae93298cf7c402af7ce749",
		"store":{"energy":1974,"battery":4449,"keanium_bar":15,"mist":700,"condensate":0,"reductant":1644,"concentrate":0},
		"storeCapacity":50000,
		"hits":1000,
		"hitsMax":1000,
		"cooldown":0,
		"actionLog":{"produce":null},
		"cooldownTime":1,938689E+07,
		"effects":{
			"0":{
				"effect":19,
				"power":19,
				"level":1,
				"endTime":1,898205E+07
				}
			},
		"level":1
		}
     */
    public class Factory : OwnedStoreStructure, ICooldownTime, ILevel
    {
        public long CooldownTime { get; set; }
        public int Level { get; set; }
        public int LevelMax { get; set; }

        internal override void Unpack(JSONObject data, bool initial)
        {
            base.Unpack(data, initial);
            UnpackUtility.Cooldown(this, data);
            UnpackUtility.Level(this, data);
        }
    }
}