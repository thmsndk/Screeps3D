using System;
using System.Collections.Generic;
using Common;
using Screeps3D.Rooms;
using Screeps_API;
using UnityEngine;
using System.Linq;
using Screeps3D.Player;
using System.Collections;
using System.Text.RegularExpressions;

namespace Screeps3D
{
    // This seems more like "room info" in regards to status of the room
    public class MapStatsUpdater : BaseSingleton<MapStatsUpdater>
    {
        private void Start()
        {
            StartCoroutine(Scan());
        }

        public Dictionary<string, RoomInfo> RoomInfo { get; } = new Dictionary<string, RoomInfo>();

        // TODO
        public RoomInfo GetRoomInfo(string roomName) => RoomInfo.ContainsKey(roomName) ? RoomInfo[roomName] : null; // Do we need shardname support?

        public IEnumerator Scan()
        {
            while (true)
            {
                yield return new WaitForSeconds(5);

                Debug.Log($"scanning sectors");
                List<string> rooms = ScanSectorsAroundPlayer();
                ////if (RoomInfo.Count == 0)
                ////{
                ////    Debug.Log($"scanning sectors"); // 0 seconds
                ////    rooms = ScanSectors();
                ////}
                ////else
                ////{
                ////    Debug.Log($"using existing roominfo keys"); // 0 seconds
                ////    rooms = RoomInfo.Keys.ToList();
                ////    rooms.AddRange(RoomManager.Instance.Rooms.Select(r => r.RoomName).ToList());
                ////    rooms = rooms.Distinct().ToList();
                ////    ////var rooms = RoomManager.Instance.Rooms.Select(r => r.RoomName).ToList();
                ////}

                Debug.Log($"Getting mapstats {rooms.Count}"); // 10 seconds
                // what shard should we be getting info from? had the same issue in the nuker
                // We might have an issue if people use custom shard names, so we can't use shardName, because playerposition shardname is shardX
                var shardName = PlayerPosition.Instance.ShardName;
                // TODO: could probably switch to "mineral0" we still recieve owner info,  but also info about what mineral, seems like we also recieve openTime and respawn area
                //yield return 
                ScreepsAPI.Http.GetMapStats(rooms, shardName, "owner0", GetMapStatsCallback);

                Debug.Log($"Waiting 60 seconds");
                // https://docs.screeps.com/auth-tokens.html#Rate-Limiting
                // POST /api/game/map-stats	60 / hour
                yield return new WaitForSecondsRealtime(60);
            }
        }

        private void GetMapStatsCallback(string jsonString)
        {
            StartCoroutine(UnpackMapStatsData(jsonString));
        }

        private IEnumerator UnpackMapStatsData(string jsonString)
        {
            Debug.Log($"Converting to jsonObject");
            var result = new JSONObject(jsonString); // 1 second
            Debug.Log($"Unpacking users {result["users"]?.keys?.Count}");
            yield return StartCoroutine(UnpackUsers(result)); // 14 seconds
            Debug.Log("Unpacking users done");

            var stats = result["stats"];
            Debug.Log($"Unpacking rooms {stats?.keys?.Count}"); // 14 seconds
            var index = 0;
            if (stats == null)
            {
                Debug.Log("stats null, no rooms");

                yield break;
            }

            foreach (var roomName in stats.keys)
            {
                index++;

                if (index % 100 == 0)
                {
                    //Debug.Log(index);
                    yield return new WaitForSeconds(0.1f);
                }


                // should we store this info on the room so it is available for others?
                var roomInfo = RoomInfo.ContainsKey(roomName) ? RoomInfo[roomName] : null;
                if (roomInfo == null)
                {
                    roomInfo = new RoomInfo(roomName);
                    RoomInfo[roomName] = roomInfo;
                }

                roomInfo.Unpack(stats[roomName]);

                roomInfo.Time = (long)result["gameTime"].n;
            }
            Debug.Log("Unpacking rooms done");
        }

        private IEnumerator UnpackUsers(JSONObject data)
        {
            var usersData = data["users"];
            if (usersData == null)
            {
                yield break;
            }

            if (usersData == null)
            {
                Debug.Log("usersData null, no users");
                yield break;
            }

            var index = 0;
            foreach (var id in usersData.keys)
            {
                index++;

                if (index % 10 == 0)
                {
                    //Debug.Log(index);
                    yield return new WaitForSeconds(0.1f);
                }

                var userData = usersData[id];
                try
                {
                    ScreepsAPI.UserManager.CacheUser(userData);
                }
                catch (Exception ex)
                {
                    //Debug.LogError(ex.Message);
                    Debug.Log(userData.ToString());
                }
            }
        }

        private List<string> ScanSectorsAroundPlayer()
        {
            // TODO: scan less sectors
            var currentRoom = PlayerPosition.Instance.Room;
            // Generate roomnames allowing sectors to surround current room

            (int cX, int cY) = SectorCenterXYFromRoom(currentRoom.RoomName);

            var gridRange = 2;
            var sectorWidth = 10;

            var topY = cY - (5 + gridRange * sectorWidth);
            var bottomY = cY + (4 + gridRange * sectorWidth);

            var leftX = cX - (5 + gridRange * sectorWidth);
            var rightX = cX + (4 + gridRange * sectorWidth);

            var rooms = new List<string>();
            for (var yo = topY; yo <= bottomY; yo++)
            {
                for (var xo = leftX; xo <= rightX; xo++)
                {
                    var room = XYToRoomName(xo, yo);
                    rooms.Add(room);
                }
            }

            return rooms;
        }

        private List<string> ScanSectors()
        {
            var rooms = new List<string>();
            for (var yo = -10; yo <= 10; yo++)
            {
                for (var xo = -10; xo <= 10; xo++)
                {
                    var room = XYToRoomName((xo * 10) + 5, (yo * 10) + 5);
                    rooms.Add(room);
                    rooms.AddRange(RoomsInSector(room));
                }
            }

            return rooms;
        }

        private List<string> RoomsInSector(string roomName)
        {
            var rooms = new List<string>();
            ////var roomName = jsonRoom["id"].str;

            (int x, int y) = XYFromRoom(roomName);
            for (var xx = 0; xx < 12; xx++)
            {
                for (var yy = 0; yy < 12; yy++)
                {
                    var roomName2 = XYToRoomName(x + xx - 6, y + yy - 6);
                    rooms.Add(roomName2);
                }
            }

            return rooms;
        }

        // We already do something like this in RoomManager, parts of it probably belongs there
        private string XYToRoomName(int x, int y)
        {
            var dx = "E";
            var dy = "S";
            if (x < 0)
            {
                x = -x - 1;
                dx = "W";
            }
            if (y < 0)
            {
                y = -y - 1;
                dy = "N";
            }
            return $"{dx}{x}{dy}{y}";
        }

        // TODO: maybe we should not regex?
        private (int, int) XYFromRoom(string room)
        {
            var match = Regex.Match(room, @"^(?<dx>[WE])(?<x>\d+)(?<dy>[NS])(?<y>\d+)$");

            var dx = match.Groups["dx"].Value;
            var x = int.Parse(match.Groups["x"].Value);

            var dy = match.Groups["dy"].Value;
            var y = int.Parse(match.Groups["y"].Value);
            if (dx == "W") x = -x - 1;
            if (dy == "N") y = -y - 1;
            return (x, y);
        }

        private string SectorCenterFromRoom(string room)
        {
            int split = room.IndexOf('N');
            if (split == -1)
            {
                split = room.IndexOf('S');
            }
            string X = room.Substring(0, split - 1);
            string Y = room.Substring(split, room.Length - split - 1);

            return $"{X}5{Y}5";
        }

        private (int, int) SectorCenterXYFromRoom(string room)
        {
            int split = room.IndexOf('N');
            if (split == -1)
            {
                split = room.IndexOf('S');
            }
            string x = room.Substring(0, split - 1).Substring(1);
            string y = room.Substring(split, room.Length - split - 1).Substring(1);

            return (int.Parse($"{x}5"), int.Parse($"{y}5"));
        }
    }

    public class RoomInfo
    {
        /// <summary>
        /// Amount of ticks in gameTime
        /// </summary>
        public long Time { get; set; }

        /// <summary>
        /// status can at least be "normal" or "out of borders" 
        /// </summary>
        public string Status { get; set; }

        public bool IsNoviceZone { get; set; }
        public string NoviceTime { get; set; }

        public bool IsRespawnArea { get; set; }
        public string RespawnAreaTime { get; set; }

        public JSONObject Sign { get; set; } // adjust at later date

        public ScreepsUser User { get; set; }

        /// <summary>
        /// the actual time on the safemode is on the controller, so we only know if safemode is up or not.
        /// </summary>
        public bool HasSafeMode { get; set; }

        /// <summary>
        /// Unix timestamp for when it opens if a sector is pending opening
        /// </summary>
        public string OpenTime { get; set; } // TODO: convert to datetime
        public int? Level { get; private set; }
        public bool IsReserved { get; set; }
        public string RoomName { get; }

        public RoomInfo(string roomName)
        {
            RoomName = roomName;
        }

        internal void Unpack(JSONObject roomStats)
        {
            var status = roomStats["status"];
            var own = roomStats["own"];
            if (own != null && !own.IsNull)
            {
                var user = own["user"];
                if (user != null && !user.IsNull)
                {
                    var userId = user.str;
                    this.User = ScreepsAPI.UserManager.GetUser(userId);
                }

                this.Level = (int)own["level"].n;

                this.IsReserved = this.Level == 0;
            }

            this.Status = status.str;

            var openTime = roomStats["openTime"];
            this.OpenTime = openTime != null && !openTime.IsNull ? openTime.str : null;

            var novice = roomStats["novice"];
            this.IsNoviceZone = novice != null && !novice.IsNull;
            this.NoviceTime = this.IsNoviceZone ? novice.n.ToString() : null;

            var respawnArea = roomStats["respawnArea"];
            this.IsRespawnArea = respawnArea != null && !respawnArea.IsNull;
            this.RespawnAreaTime = this.IsRespawnArea ? respawnArea.n.ToString() : null;

            var safeMode = roomStats["safeMode"];
            this.HasSafeMode = safeMode != null && !safeMode.IsNull && safeMode.b;

            // could also store mineral type and density for future use in map overview
            // hardSign indicates a planned novice or respawn area
        }
    }
}