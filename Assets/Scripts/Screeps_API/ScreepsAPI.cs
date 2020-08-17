using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Screeps_API;
using Common;
using Screeps3D;
using UnityEngine;

namespace Screeps_API
{
    [RequireComponent(typeof(ScreepsHTTP))]
    [RequireComponent(typeof(ScreepsSocket))]
    public class ScreepsAPI : BaseSingleton<ScreepsAPI>
    {
        public static ServerCache Cache { get; /*private*/ set; }
        public static ScreepsHTTP Http { get; private set; }
        public static ScreepsSocket Socket { get; private set; }
        public static ScreepsUser Me { get; private set; }
        public static BadgeManager Badges { get; private set; }
        public static UserManager UserManager { get; private set; }
        public static CpuMonitor Monitor { get; private set; }
        public static ScreepsConsole Console { get; private set; }

        public static ServerMessageMonitor ServerMessageMonitor { get; private set; }

        public static Warpath Warpath { get; private set; }

        public static ResourceMonitor Resources { get; private set; }

        public static ShardInfoMonitor ShardInfo { get; private set; }

        public static long Time { get; internal set; }
        public static bool IsConnected { get; private set; }
        
        public static event Action<bool> OnConnectionStatusChange;
        public static event Action<long> OnTick;
        public static event Action OnShutdown;

        public static List<string> WorldStartRooms { get; private set; }

        private string _token;

        public override void Awake()
        {
            base.Awake();

            Http = GetComponent<ScreepsHTTP>();
            Socket = GetComponent<ScreepsSocket>();
            Badges = GetComponent<BadgeManager>();
            Monitor = GetComponent<CpuMonitor>();
            Console = GetComponent<ScreepsConsole>();
            UserManager = new UserManager();
            ServerMessageMonitor = GetComponent<ServerMessageMonitor>();
            Warpath = GetComponent<Warpath>();
            Resources = GetComponent<ResourceMonitor>();
            ShardInfo = GetComponent<ShardInfoMonitor>();
        }

        // Use this for initialization
        public void Connect(ServerCache cache)
        {
            Cache = cache;
            
            // configure HTTP
            Http.Auth(o =>
            {
                NotifyText.Message("Success", Color.green, 1);
                Socket.Connect();
                Http.GetUser(AssignUser);
            }, () =>
            {
                Debug.Log("login failed");
            });
        }

        public void Disconnect()
        {
            SetConnectionStatus(false);
            Badges.Reset();
            UserManager.Reset();
            Socket.Disconnect();
        }

        internal void UpdateTime(long currentTime)
        {
            if(currentTime == Time)
            {
                return;
            }
            if(currentTime - Time > 1)
            {
                Debug.LogWarning("Lost ticks count: " + (currentTime - Time));
            }
            Time = currentTime;
            if (OnTick != null)
                OnTick(Time);
        }

        internal void AuthFailure()
        {
            SetConnectionStatus(false);
        }

        private void AssignUser(string str)
        {
            var obj = new JSONObject(str);
            Me = UserManager.CacheUser(obj);

            SetConnectionStatus(true);
            // Request URL: https://screeps.com/api/user/world-start-room
            // {"ok":1,"room":["shard3/E19S38"]}

            //Http.Request("GET", "/api/game/time", null, SetTime);
            
            // Initialize things related to shard....
            // TOOD: world start room is called to figure out what room to load in case we never selected a room :thinking: but what if you had selected a shard already in roomchooser?
            Http.Request("GET", "/api/user/world-start-room", null, GetShardSetTime);
            // GET /api/user/world-start-room?shard=shard0 is called when connecting to a private server after world-start-room is called

            // call room-status, but why?
            // call GET /api/user/respawn-prohibited-rooms but why? is it not only relevant if we have died?
            // GET /api/user/world-status is called at an interval of 6 seconds to detect status {"ok":1,"status":"normal"}
            // if we changed world status, or just connected with non normal status, we should pop up a dialog a bout respawning
            // status empty is when we have called POST /api/user/respawn
            // api/game/place-spawn is called when we place the initial first spawn.

            // we need a world status updater, kinda like map status updater is this a thing that belongs in the 3D space though? should mapstats belong in the api as well?
            // can't spawn in already owned rooms, can't spawn in rooms without a controller (SK rooms, highways) we check that from map stats data

            // after calling respawn, we should probably call get respawn prohibited rooms. we should also call it if status is empty
        }
        private void GetShardSetTime(string obj)
        {
            //Debug.Log(obj);
            var worldStartRooms = new JSONObject(obj)["room"];
            if (worldStartRooms != null)
            {
                var firstRoom = worldStartRooms.list.FirstOrDefault();
                var firstRoomInfo = firstRoom.str.Split('/'); // on PS we don't recieve a shard, only the room name... so shard will be roomName...
                var shard = firstRoomInfo[0];

                WorldStartRooms = worldStartRooms.list.Select(x => x.str).ToList();


                Http.Request("GET", $"/api/game/time?shard={shard}", null, SetTime);
            }
        }

        private void SetTime(string obj)
        {
            //Debug.Log(obj);
            var timeData = new JSONObject(obj)["time"];
            if (timeData != null)
            {
                Time = (int) timeData.n;
                //Debug.Log($"time/tick is now {Time}");
            }
        }

        private void OnDestroy()
        {
            if (OnShutdown != null)
                OnShutdown();
        }

        private void SetConnectionStatus(bool isConnected)
        {
            IsConnected = isConnected;
            if (OnConnectionStatusChange != null) OnConnectionStatusChange(isConnected);
        }
    }
}