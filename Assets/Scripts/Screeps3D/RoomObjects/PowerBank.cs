using System.Collections.Generic;
using System.Diagnostics;

namespace Screeps3D.RoomObjects
{
    /*
       {
	        "_id": "5d1944d618769d42ddabaaf2",
	        "type": "powerBank",
	        "x": 25,
	        "y": 35,
	        "room": "E0S25",
	        "power": 1962,
	        "hits": 2000000,
	        "hitsMax": 2000000,
	        "decayTime": 8206678
        }

        // https://docs.screeps.com/api/#StructurePowerBank
        Hits	2,000,000
        Return damage	50%
        Capacity	500 — 10,000
        Decay	5,000 ticks
    */

    public class PowerBank : Structure, IDecay, IPowerObject
    {
        public float NextDecayTime { get; set; }

        public float Power { get; set; }

        /// <summary>
        /// The maximum power a bank can spawn with
        /// </summary>
        public float PowerCapacity { get; set; }

        internal PowerBank()
        {
            PowerCapacity = 10000; // Q: move to constants?
        }

        internal override void Unpack(JSONObject data, bool initial)
        {
            var powerData = data["power"];
            if (powerData != null)
            {
                this.Power = powerData.n;
            }

            UnpackUtility.Decay(this, data);

            base.Unpack(data, initial);
        }
    }
}