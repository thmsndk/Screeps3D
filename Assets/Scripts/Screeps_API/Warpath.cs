using Assets.Scripts.Screeps_API.ConsoleClientAbuse;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Screeps_API
{
    /// <summary>
    /// Responsible for gathering battle stats and classifications from the admin utils warpath mod on private servers, and LOAN battles endpoint from official servers
    /// </summary>
    public class Warpath : MonoBehaviour
    {

        private Queue<JSONObject> queue = new Queue<JSONObject>();

        public List<WarpathRoom> Rooms { get; set; } = new List<WarpathRoom>();

        public Action OnClassificationsUpdated;

        private void Start()
        {
            ScreepsAPI.OnConnectionStatusChange += OnConnectionStatusChange;
        }

        private void OnConnectionStatusChange(bool connected)
        {
            if (connected)
            {
                // TODO: on official we need to start a timer that pulls data from LOAN e.g. https://www.leagueofautomatednations.com/vk/battles.json

                ScreepsAPI.Socket.Subscribe(string.Format("warpath:battles", ScreepsAPI.Me.UserId), RecieveData);
                // TODO: subscribe to server-message to display server messages in console
            }
        }

        private void RecieveData(JSONObject obj)
        {
            queue.Enqueue(obj);
        }

        private void Update()
        {
            if (queue.Count == 0)
                return;
            UnpackData(queue.Dequeue());
        }

        private void UnpackData(JSONObject obj)
        {
            if (!ScreepsAPI.Cache.Official)
            {
                UnpackWarpathData(obj);
            }
            // TODO: LOAN parsing.
        }

        private void UnpackWarpathData(JSONObject obj)
        {
            var rooms = obj.list;
            /*
             * {
	                "channel": "warpath",
	                "id": "battles",
	                "type": "warpath"
	                "data": [{
			                "room": "W14S6",
			                "shard": "screepsplus2",
			                "classification": 0,
			                "defender": "false or username",
			                "attackers": ["username1", "username2"],
			                "lastPvpTime": 3328660,
			                "powerCreeps": [{
					                "name": "x",
					                "className": "operator",
					                "level": 1,
					                "powers": {
						                "power": "PWR",
						                "level": 0
					                }
				                }
			                ],
			                "stronghold": 0,
		                }
	                ],

                }
             * */
            Debug.Log($"Warpath: Recieved {rooms.Count} rooms");
            foreach (var roomClassification in rooms)
            {
                var roomName = roomClassification["room"].str;
                var shardName = roomClassification["shard"].str;

                int classification = (int)roomClassification["classification"].n;

                var defender = roomClassification["defender"].str; // username

                var attackers = roomClassification["attackers"].list; // list of usernames

                var lastPvpTime = (int)roomClassification["lastPvpTime"].n;

                var powerCreeps = roomClassification["powerCreeps"].list; // list of power creeps with

                var stronghold = (int)roomClassification["stronghold"].n; // stronghold level

                // Try and find room, else make a new one.
                var room = Rooms.SingleOrDefault(r => r.RoomName == roomName && r.Shard == shardName);

                if (room == null)
                {
                    room = new WarpathRoom(shardName, roomName);
                    Rooms.Add(room);
                }

                // TODO: make event and raise classification has gone up.
                room.Classification = (Classification)classification;

                if (room.Defender == null || room.Defender.Username != defender)
                {
                    room.Defender = ScreepsAPI.UserManager.GetUserByName(defender);
                }

                room.Attackers.Clear();
                foreach (var attacker in attackers)
                {
                    var username = attacker.str;
                    var user = ScreepsAPI.UserManager.GetUserByName(username);
                    if (user != null)
                    {
                        room.Attackers.Add(user);
                    }
                }

                room.LastPvpTime = lastPvpTime;

                // TODO: Power creeps

                room.StrongholdLevel = stronghold;
                
            }

            OnClassificationsUpdated?.Invoke();

        }

        public class WarpathRoom
        {
            public WarpathRoom(string shardName, string roomName)
            {
                Shard = shardName;
                RoomName = roomName;
                Attackers = new List<ScreepsUser>();
            }

            public string RoomName { get; internal set; }
            public string Shard { get; internal set; }
            public Classification Classification { get; internal set; }
            public ScreepsUser Defender { get; internal set; }

            public List<ScreepsUser> Attackers { get; internal set; }
            public int LastPvpTime { get; internal set; }
            public int StrongholdLevel { get; internal set; }
        }

        /// <summary>
        /// https://github.com/ScreepsMods/screepsmod-admin-utils/blob/master/warpath-classifications.md
        /// </summary>
        public enum Classification
        {
            /// <summary>
            /// Any PVP actions that do not meet above qualifications
            /// </summary>
            Class0 = 0,
            /// <summary>
            /// Reserved or Undefnded Room
            /// Small Squad (Minimal Healers and no Boosts)​OR​
            /// Neutral Room
            /// Boosted Creeps and/or Massive Amount of Healers​​
            /// </summary>
            Class1 = 1,
            /// <summary>
            /// Reserved Room
            /// Boosted Creeps and/or Massive Amount of Healers​OR​
            /// Low Level (Undefended)
            /// Small Squad(Minimal Healers and no Boosts)​​
            /// </summary>
            Class2 = 2,
            /// <summary>
            /// High Level (>= Established) Room
            ///  Small Squad (Minimal Healers and no Boosts)​OR​
            ///  Low Level(Developing) Room
            /// Boosted Creeps and/or Massive Amount of Healers​OR​
            /// Reserved Room
            /// Boosted Creeps and/or Massive Amount of Healers
            /// Multiple Attacking Players​​
            /// </summary>
            Class3 = 3,
            /// <summary>
            /// High Level Room (>= Established)
            /// Boosted Creeps and/or Massive Amount of Healers​​
            /// </summary>
            Class4 = 4,
            /// <summary>
            /// High Level Room (Fortified)
            /// Multiple Attacking Players and/or 10+ Attacker Creeps
            /// Boosted Creeps and/or Massive Amount of Healers​​
            /// </summary>
            Class5 = 5,
            /// <summary>
            /// Any classification is automatically upgraded to the next level (including from 5 to 6) when a nuclear device is about to go off or a PowerCreep is present.​​
            /// </summary>
            Class6 = 6,
        }


    }
}