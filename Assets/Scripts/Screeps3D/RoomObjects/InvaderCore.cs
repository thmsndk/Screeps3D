using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Screeps3D.RoomObjects;

namespace Screeps3D.RoomObjects
{
    /*
     {
	"_id": "5ebaa3c353d6bb4881bef8a6",
	"type": "invaderCore",
	"level": 5,
	"strongholdBehavior": "bunker5",
	"room": "W24N16",
	"x": 14,
	"y": 18,
	"user": "2",
	"templateName": "bunker5",
	"hits": 100000,
	"hitsMax": 100000,
	"nextExpandTime": 1.821286E+07,
	"depositType": "silicon",
	"deployTime": null,
	"strongholdId": "W24N16_18165263",
	"effects": [{
			"effect": 1001,
			"power": 1001,
			"endTime": 1.817026E+07,
			"duration": 5000
		}, {
			"effect": 1002,
			"power": 1002,
			"endTime": 1.824889E+07,
			"duration": 78629
		}
	],
	"actionLog": {
		"transferEnergy": null,
		"reserveController": null,
		"attackController": null,
		"upgradeController": null
	},
	"decayTime": 1.824889E+07,
	"population": {
		"0": {
			"body": "fortifier",
			"behavior": "fortifier"
		},
		"1": {
			"body": "fullBoostedRanger",
			"behavior": "coordinated"
		},
		"2": {
			"body": "fullBoostedMelee",
			"behavior": "coordinated"
		},
		"3": {
			"body": "fullBoostedRanger",
			"behavior": "coordinated"
		},
		"4": {
			"body": "fullBoostedMelee",
			"behavior": "coordinated"
		},
		"5": {
			"body": "fullBoostedRanger",
			"behavior": "coordinated"
		},
		"6": {
			"body": "fortifier",
			"behavior": "fortifier"
		},
		"7": {
			"body": "fullBoostedRanger",
			"behavior": "coordinated"
		},
		"8": {
			"body": "fullBoostedRanger",
			"behavior": "coordinated"
		}
	},
	"spawning": null
}

     */

    // TODO: Add ILevel, IDecay, add support for "nextExpandTime,depositType,strongholdBehavior,templateName, spawning (check out spawn / sk lair)"
    public class InvaderCore : OwnedStructure, ICooldownObject, IActionObject, IEffectObject, ILevel
    {
        // TODO: Effects
        public float Cooldown { get; set; }
		public int Level { get; set; }
		public int LevelMax { get; set; }
        public List<EffectDto> Effects { get; set; }
        public Dictionary<string, JSONObject> Actions { get; set; }

        internal InvaderCore()
        {
            Actions = new Dictionary<string, JSONObject>();
            Effects = new List<EffectDto>();
        }

        internal override void Unpack(JSONObject data, bool initial)
        {
            base.Unpack(data, initial);

            UnpackUtility.Cooldown(this, data);
            UnpackUtility.ActionLog(this, data);
            UnpackUtility.Effects(this, data);
			UnpackUtility.Level(this, data);
        }
    }
}