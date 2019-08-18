using System.Collections.Generic;
using Screeps3D.Effects;
using Screeps3D.Rooms;
using Screeps_API;
using UnityEngine;

namespace Screeps3D.RoomObjects
{
    /*
        {
	        "_id": "5c0e406c504e0a34e3d61d22",
	        "type": "portal",
	        "room": "E20S40",
	        "x": 33,
	        "y": 36,
	        "destination": {
		        "room": "E20S40",
		        "shard": "shard2"
	        }
        }

    */

    internal class Portal : Structure, IDecay
    {
        public float NextDecayTime { get; set; }

        public bool Stable { get; set; }

        // TODO: Destination
        internal Portal()
        {
        }

        internal override void Unpack(JSONObject data, bool initial)
        {
            base.Unpack(data, initial);

            if (initial)
            {
                UnpackUtility.Decay(this, data);

                this.Stable = this.NextDecayTime == 0f;

                Initialized = true;
            }
        }
    }
}