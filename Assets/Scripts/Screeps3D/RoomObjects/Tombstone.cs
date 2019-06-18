﻿using System.Collections.Generic;
using Screeps3D.Effects;
using Screeps3D.Rooms;
using Screeps_API;
using UnityEngine;

namespace Screeps3D.RoomObjects
{
    /*
        
         {
	        "_id": "5d095e952d48024e40b88c80",
	        "type": "tombstone",
	        "room": "E8S7",
	        "x": 26,
	        "y": 7,
	        "user": "5d027ee76aaed99e2675583b",
	        "deathTime": 756542,
	        "decayTime": 756552,
	        "creepId": "5d095b6724f8af4e7189d6dc",
	        "creepName": "r8970_1557",
	        "creepTicksToLive": 1,
	        "creepBody": ["ranged_attack", "move"],
	        "creepSaying": null
        }

    */

    internal class Tombstone : RoomObject, /*ICreepReferenceObject,*/ IOwnedObject 
    {
        public string UserId { get; set; }
        public ScreepsUser Owner { get; set; }
        //public CreepBody Body { get; private set; } // We only have the body parts, they have no health left?
        public string Name { get; set; } 
        public int TTL { get; set; } // How is TTL set on a creep?
        //public long AgeTime { get; set; }
        public long DeathTime { get; set; }
        public long DecayTime { get; set; }
        public Vector3 PrevPosition { get; protected set; }
        public Vector3 BumpPosition { get; private set; }
        public Quaternion Rotation { get; private set; }

        internal Tombstone()
        {
            //Body = new CreepBody();
        }

        internal override void Unpack(JSONObject data, bool initial)
        {
            base.Unpack(data, initial);

            if (initial)
            {
                UnpackUtility.Owner(this, data);
                //UnpackUtility.Name(this, data);
                var nameData = data["creepName"];
                if (nameData != null)
                {
                    Name = nameData.str;
                }
            }

            var deathData = data["deathTime"];
            if (deathData != null)
            {
                DeathTime = (long)deathData.n;
            }

            var decayData = data["decayTime"];
            if (deathData != null)
            {
                DecayTime = (long)deathData.n;
            }

            //Body.Unpack(data);
        }
        
        internal override void Delta(JSONObject delta, Room room)
        {
            if (!Initialized)
            {
                Unpack(delta, true);
            }
            else
            {
                Unpack(delta, false);
            }
            
            if (Room != room || !Shown)
            {
                EnterRoom(room);
            }

            PrevPosition = Position;
            SetPosition();
            AssignRotation();
            
            if (View != null)
                View.Delta(delta);
            
            RaiseDeltaEvent(delta);
        }

        private void AssignRotation()
        {
            // TODO: Figure out how tombstone should be rotated.
            Vector3 newForward = Vector3.zero;
            if (BumpPosition != default(Vector3))
                newForward = Position - BumpPosition;
            if (PrevPosition != Position)
                newForward = PrevPosition - Position;

            if (newForward != Vector3.zero)
                Rotation = Quaternion.LookRotation(newForward);
        }
    }
}