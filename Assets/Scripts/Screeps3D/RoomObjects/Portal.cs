using System.Collections.Generic;
using Screeps3D.Effects;
using Screeps3D.Rooms;
using Screeps_API;
using UnityEngine;
using System.Linq;

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

    internal class Portal : Structure, IDecay, IPortalDestination
    {
        public float NextDecayTime { get; set; }

        public bool Stable { get; set; }

        // PortalDestination
        public string DestinationShard { get;set;}
        public string DestinationRoom { get; set; }
        public string DestinationPosition { get; set; }

        // TODO: Destination

        private List<Creep> creepsTeleporting = new List<Creep>();
        internal Portal()

        {
        }

        internal override void Delta(JSONObject delta, Room room)
        {
            base.Delta(delta, room);
            var creeps = room.Objects.Values.OfType<Creep>();
            var creepsNearbyPortal = creeps.Where(creep => {
                var distance = Vector3.Distance(creep.Position, this.Position);
                // How large is a tile actually in world space? do we have a constant for that?
                // Also need to ceep in mind that the diagonal distance is longer than NSEW
                Debug.Log("distance: " + distance);
                if(distance > 1.5f)
                {
                    creepsTeleporting.Remove(creep);
                    return false;
                }

                return true;
            });

            // ags131
            // Well, heres a fun one for you, inter-shard portals create the creep in a random location, intra-shard portals creates the creep on the portal

            foreach (var creep in creepsNearbyPortal)
            {
                if (!creepsTeleporting.Contains(creep)) {
                    creepsTeleporting.Add(creep);
                    EffectsUtility.Teleport(creep);
                }
            }

            // clean up teleported creeps
            creepsTeleporting.RemoveAll(creep => !creeps.Contains(creep));
        }

        internal override void Unpack(JSONObject data, bool initial)
        {
            base.Unpack(data, initial);

            if (initial)
            {
                UnpackUtility.Decay(this, data);

                this.Stable = this.NextDecayTime == 0f;

                UnpackDestination(data);

                Initialized = true;
            }
        }

        private void UnpackDestination(JSONObject data)
        {
            var destinationData = data["destination"];

            if (destinationData != null)
            {
                var destinationShardData = destinationData["shard"];
                if (destinationShardData != null)
                {
                    DestinationShard = destinationShardData.str;
                }

                var destinationRoomData = destinationData["room"];

                if (destinationRoomData != null)
                {
                    DestinationRoom = destinationRoomData.str;
                }

                var destinationX = destinationData["x"];
                var destinationY = destinationData["y"];

                if (destinationX != null && destinationY != null)
                {
                    DestinationPosition = string.Format("{0}, {1}", destinationX.n, destinationY.n);
                }
            }
        }
    }
}