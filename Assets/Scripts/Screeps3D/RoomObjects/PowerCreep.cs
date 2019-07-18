using System.Collections.Generic;
using Screeps3D.Effects;
using Screeps3D.Rooms;
using Screeps_API;
using UnityEngine;

namespace Screeps3D.RoomObjects
{
    /*
     {
	        "_id": "5cccd296b0d5ab7bb6ab8117",
	        "name": "OP_2",
	        "className": "operator",
	        "user": "598d908641d70d5a4845c623",
	        "level": 25,
	        "hitsMax": 26000,
	        "energyCapacity": 2600,
	        "spawnCooldownTime": null,
	        "powers": {
		        "1": {
			        "level": 5,
			        "cooldownTime": 8738803
		        },
		        "2": {
			        "level": 3,
			        "cooldownTime": 8737320
		        },
		        "5": {
			        "level": 5
		        },
		        "6": {
			        "level": 3,
			        "cooldownTime": 8737447
		        },
		        "8": {
			        "level": 3,
			        "cooldownTime": 7091739
		        },
		        "13": {
			        "level": 5,
			        "cooldownTime": 8738855
		        },
		        "16": {
			        "level": 1,
			        "cooldownTime": 8699186
		        }
	        },
	        "shard": "shard3",
	        "deleteTime": null,
	        "type": "powerCreep",
	        "room": "W3S5",
	        "x": 39,
	        "y": 40,
	        "hits": 26000,
	        "ageTime": 8740670,
	        "actionLog": {
		        "spawned": null,
		        "attack": null,
		        "attacked": null,
		        "healed": null,
		        "power": {"id":1,"x":13,"y":27},
		        "say": null
	        },
	        "notifyWhenAttacked": true,
	        "ops": 616,
	        "energy": 500
        }
    */

    // TODO: we need to map powers per class, I mean, what is power 1,8, 16?

    internal class PowerCreep : RoomObject, INamedObject, IHitpointsObject, IOwnedObject, IStoreObject, IActionObject, IBump
    {
        public string UserId { get; set; }
        public ScreepsUser Owner { get; set; }
        //public CreepBody Body { get; private set; }
        public string Name { get; set; }
        public Dictionary<string, JSONObject> Actions { get; set; }
        public float Hits { get; set; }
        public float HitsMax { get; set; }
        public float Fatigue { get; set; }
        public int TTL { get; set; }
        public long AgeTime { get; set; }
        public Vector3 PrevPosition { get; protected set; }
        public Vector3 BumpPosition { get; private set; }
        public Quaternion Rotation { get; private set; }
        public Dictionary<string, float> Store { get; private set; }
        public float StoreCapacity { get; set; }
        public float TotalResources { get; set; }

        internal PowerCreep()
        {
            //Body = new CreepBody();
            Actions = new Dictionary<string, JSONObject>();
            Store = new Dictionary<string, float>();
        }

        internal override void Unpack(JSONObject data, bool initial)
        {
            base.Unpack(data, initial);
            if (initial)
            {
                UnpackUtility.Owner(this, data);
                UnpackUtility.Name(this, data);
            }
            
            UnpackUtility.HitPoints(this, data);
            UnpackUtility.Store(this, data);
            UnpackUtility.ActionLog(this, data);

            var ageData = data["ageTime"];
            if (ageData != null)
            {
                AgeTime = (long) ageData.n;
            }
            
            var fatigueData = data["fatigue"];
            if (fatigueData != null)
            {
                Fatigue = fatigueData.n;
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
            AssignBumpPosition();
            AssignRotation();
            
            if (Actions.ContainsKey("say") && !Actions["say"].IsNull)
                EffectsUtility.Speech(this, Actions["say"]["message"].str);
            
            if (View != null)
                View.Delta(delta);
            
            RaiseDeltaEvent(delta);
        }

        private void AssignBumpPosition()
        {
            if (Room == null)
                return;
            BumpPosition = default(Vector3);
            foreach (var kvp in Constants.ContactActions)
            {
                if (!kvp.Value)
                    continue;
                var action = kvp.Key;
                if (!Actions.ContainsKey(action))
                    continue;
                var actionData = Actions[action];
                if (actionData.IsNull)
                    continue;
                BumpPosition = PosUtility.Convert(actionData, Room);
            }
        }

        private void AssignRotation()
        {
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