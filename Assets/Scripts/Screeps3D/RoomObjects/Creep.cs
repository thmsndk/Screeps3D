using System.Collections.Generic;
using Screeps3D.Effects;
using Screeps3D.Rooms;
using Screeps_API;
using UnityEngine;

namespace Screeps3D.RoomObjects
{
    /*{
        
        "body":[
            {
                "type":"work",
                "hits":100
            },
            {
                "type":"work",
                "hits":100
            },
            {
                "type":"carry",
                "hits":100
            },
            {
                "type":"carry",
                "hits":100
            },
            {
                "type":"move",
                "hits":100
            }
        ],
        "energy":0,
        "energyCapacity":100,
        "type":"creep",
        "room":"E2S7",
        "user":"5a0da017ab17fd00012bf0e7",
        "hits":500,
        "hitsMax":500,
        "spawning":false,
        "fatigue":0,
        "notifyWhenAttacked":true,
        "ageTime":8598,
        "actionLog":{
            "attacked":null,
            "healed":null,
            "attack":null,
            "rangedAttack":null,
            "rangedMassAttack":null,
            "rangedHeal":null,
            "harvest":null,
            "heal":null,
            "repair":null,
            "build":null,
            "say":null,
            "upgradeController":null,
            "reserveController":null
        }
    }*/

    // TODO TeleportView?

    internal class Creep : StoreObject, INamedObject, IHitpointsObject, IOwnedObject, IActionObject, IBump, ICreepBody//, IEnergyObject Do we want to visualize energy seperately from store?
    {
        public string UserId { get; set; }
        public ScreepsUser Owner { get; set; }
        public CreepBody Body { get; private set; }
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

        internal Creep()
        {
            Body = new CreepBody();
            Actions = new Dictionary<string, JSONObject>();
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
            
            Body.Unpack(data, initial);
        }
        
        internal override void Delta(JSONObject delta, Room room)
        {
            if (!Initialized)
            {
                Unpack(delta, true);
                Initialized = true;
            }
            else
            {
                Unpack(delta, false);
            }
            
            if (Room != room || !Shown)
            {
                EnterRoom(room);
            }

            // Acquire previous position before updating it.
            PrevPosition = Position; // TODO: this really belongs before unpacking, pretty sure whatever PrevPosition stuff was supposed to do, it aint working like this.

            SetPosition(); 
            AssignBumpPosition();
            AssignRotation();
            
            if (Actions.ContainsKey("say") && !Actions["say"].IsNull)
                EffectsUtility.Speech(this, Actions["say"]["message"].str, Actions["say"]["isPublic"].b);
            
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
            // checking for 0,0,0 so creeps won't look away from world center when initially placed
            if (PrevPosition != Position && PrevPosition != Vector3.zero)
                newForward = PrevPosition - Position;

            if (newForward != Vector3.zero)
                Rotation = Quaternion.LookRotation(newForward);
        }
    }
}