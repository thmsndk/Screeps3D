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
        
        // TODO: the portal effect should visualize if it is unstable, e.g. if it has a decaytime it should become more and more unstable the closer it gets to the decay
        public bool Stable { get; set; }

        // PortalDestination
        public string DestinationShard { get;set;}
        public string DestinationRoom { get; set; }
        public string DestinationPosition { get; set; }

        private List<Creep> creepsTeleporting = new List<Creep>();

        // this is insane to keep track of in each portal, unless we reduce it to creepId, still kinda insane though
        private List<Creep> previousCreeps = new List<Creep>(); 

        internal Portal()
        {
        }

        internal override void Delta(JSONObject delta, Room room)
        {
            base.Delta(delta, room);
            var creeps = room.Objects.Values.OfType<Creep>();
            var newCreeps = creeps.Where(c => !previousCreeps.Contains(c)).ToList(); // new creeps appeared in the room
            previousCreeps = creeps.ToList();

            var creepsNearbyPortal = creeps.Where(creep => {
                var distance = Vector3.Distance(creep.Position, this.Position);
                // How large is a tile actually in world space? do we have a constant for that?
                // Also need to ceep in mind that the diagonal distance is longer than NSEW
                if(distance > 1.5f)
                {
                    creepsTeleporting.Remove(creep);
                    return false;
                }

                return true;
            });

            foreach (var creep in newCreeps)
            {
                creep.OnPosition += Creep_OnPosition;
            }

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

        private void Creep_OnPosition(RoomObject creep, Vector3 obj)
        {
            //Debug.Log("creep" + creep.Position);
            // squareish distance u.238
            // need a function like var diff = pos1 - pos2; return Math.Max(Math.Abs(diff.X), Math.Abs(diff.Y));
            var distance = Vector3.Distance(creep.Position, this.Position);
            //Debug.Log("newCreep distance to spawn: " + distance);
            // I have no idea how close to a teleporter creeps spawn, heck I guess they can spawn in a room where there is no teleporter?
            // using a distance of ~5 but diagnoally is more than 5
            if (distance <= 7.5f) 
            {
                // TODO: we also need to detect if the creep is on the edge. e.g. something like isNearEdge

                // TODO: we might want to make the teleporter do a "pulse" due to spawning something.
                // visualize that a creep just teleported in here
                // this appears to not be working, I assume because creep.View is null
                //Debug.Log("Spawning teleport at " + creep.Position);
                EffectsUtility.TeleportSpawn(creep.Position);
                //EffectsUtility.Teleport(creep);
                // TODO: limit the teleport/spawn effect in time, should be a TeleportSpawn effect instead I guess where the animation goes downwards, could just rotate the element
            }

            // unregister
            creep.OnPosition -= Creep_OnPosition;
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