using Common;
using Screeps_API;
using Screeps3D;
using Screeps3D.Player;
using Screeps3D.Rooms;
using Screeps3D.World.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Screeps3D
{
    /*  https://docs.screeps.com/api/#StructureNuker
     *  A Nuke roomObject is created in the target room, so impact handling can be done by that roomObject
     *  But do we want to see a visual explosion in the distance? perhaps impact should be handled on a general case?
     *  
     *  A nuke has a range of 10 rooms
     *  It takes 50.000 ticks to land
     *  
     *  TODO: get "inspired" by https://github.com/ags131/nuke-announcer/blob/master/index.js
     *  Figure out if the nuke is a hostile nuke or your own nuke.
     *  Figure out Attacker and Defender from map-stats.
     *  ETA in real world time
     **/

    public class NukeMonitor : BaseSingleton<NukeMonitor>
    {
        private Dictionary<string, ShardInfoDto> ShardInfo { get; set; } = new Dictionary<string, ShardInfoDto>();

        private IEnumerator getNukes;

        // Should we just have the raw data available? and then only generate overlays on the current shard?
        public Dictionary<string, List<NukeData>> Nukes { get; } = new Dictionary<string, List<NukeData>>();

        // TODO: does the "overlay" belong here? - probably belongs in another map-overlay component that subscribes to nukemonitor updates.
        public Dictionary<string, NukeMissileOverlay> CurrentShardNukes { get; } = new Dictionary<string, NukeMissileOverlay>();

        public Action OnNukesRefreshed;

        private Dictionary<string, bool> nukesInitialized = new Dictionary<string, bool>();

        private void Start()
        {
            //PlayerPosition.Instance.OnRoomChange += OnRoomChange; // this triggers twice, we might need a more reliable way to detect when loaded
            StartCoroutine(GetShardInfo());
            getNukes = GetNukes();
            StartCoroutine(getNukes);
        }

        /// <summary>
        /// Used to get shard tickrates
        /// </summary>
        /// <returns></returns>
        private IEnumerator GetShardInfo()
        {
            while (!ScreepsAPI.IsConnected)
            {
                yield return new WaitForSeconds(5);
            }

            // TODO: Should probably do this lookup on connect
            yield return ScreepsAPI.Http.Request("GET", $"/api/game/shards/info", null, (jsonShardInfo) =>
            {
                // tickrates and such, what about private servers?
                var shardInfo = new JSONObject(jsonShardInfo);
                var shards = shardInfo["shards"].list;
                foreach (var shard in shards)
                {
                    var tickRateString = shard["tick"].n;
                    // TODO: find existing shard info and update it.
                    ShardInfo.Add(shard["name"].str, new ShardInfoDto(shard));
                }
            });
        }

        private IEnumerator GetNukes()
        {
            yield return new WaitForSeconds(5);

            while (true)
            {
                if (ShardInfo.Count == 0)
                {
                    Debug.LogWarning("shardinfo not fetched yet, waiting 5 seconds");
                    yield return new WaitForSeconds(5);
                }

                //NotifyText.Message("SCANNING FOR NUKES!", Color.red);

                // Should probably cache this, and refresh it at an interval to detect new nukes.
                ScreepsAPI.Http.GetExperimentalNukes((jsonString) =>
                {
                    var obj = new JSONObject(jsonString);
                    var status = obj["ok"];
                    var nukesObject = obj["nukes"];

                    foreach (var nukesShardName in nukesObject.keys)
                    {
                        if (!nukesInitialized.TryGetValue(nukesShardName, out _))
                        {
                            nukesInitialized[nukesShardName] = false;
                        }



                        var shardNukes = nukesObject[nukesShardName].list;
                        Debug.Log($"{nukesShardName} has {shardNukes.Count} nukes!");

                        //var shardName = ScreepsAPI.Cache.Official? nukesShardName : $"shard{shardIndex}";
                        //NotifyText.Message($"{nukesShardName} has {shardNukes.Count} nukes!", Color.red);
                        var time = ScreepsAPI.Time;

                        // TODO: getting time should be moved to when logging or switching shards? - we do need the time for every shard to render this though.
                        ScreepsAPI.Http.Request("GET", $"/api/game/time?shard={nukesShardName}", null, (jsonTime) =>
                        {
                            var roomsToGetMapStatsFrom = new List<string>(); // TODO: handle getting map-stats from other shards

                            var timeData = new JSONObject(jsonTime)["time"];
                            if (timeData != null)
                            {
                                time = (long)timeData.n;
                            }

                            if (!Nukes.TryGetValue(nukesShardName, out var nukes))
                            {
                                nukes = new List<NukeData>();
                                Nukes.Add(nukesShardName, nukes);
                            }

                            
                            if (ShardInfo.TryGetValue(nukesShardName, out var shardInfo))
                            {
                                shardInfo.Time = time;
                            }
                            else
                            {
                                // Handle cases where server has not updated to latest admin-util yet. this should really be in a "ShardInfo" component, that is initialized on connect, and updated with realtime average ticks like the serverinfo box top left uses.
                                shardInfo = new ShardInfoDto(null);
                                ShardInfo.Add(nukesShardName, shardInfo);
                                shardInfo.Time = time;
                                shardInfo.AverageTick = 1000;
                            }

                            shardInfo.TimeUpdated = DateTime.Now;

                            var shardXName = nukesShardName;
                            // Temp fix because RoomFactory expects shards to be named shardX
                            if (!nukesShardName.StartsWith("shard"))
                            {
                                shardXName = "shard0";
                            }

                            foreach (var shardNuke in shardNukes)
                            {
                                var id = shardNuke["_id"].str; // should probably switch to UnPackUtility later.
                                var key = id;

                                var nuke = nukes.SingleOrDefault(n => n.Id == id);

                                if (nuke == null)
                                {
                                    // TODO: further detection if this was a newly launched nuke. perhaps the progress is at a really low percentage, or between x ticks?
                                    if (nukesInitialized[nukesShardName])
                                    {
                                        NotifyText.Message($"{nukesShardName} => Nuclear Launch Detected", Color.red);
                                    }

                                    nuke = new NukeData(shardInfo);
                                    nuke.Id = id;
                                    nuke.Shard = shardXName;//nukesShardName;
                                    nukes.Add(nuke);

                                    // TODO: initialize overlays for current shard
                                    CurrentShardNukes.Add(key, new NukeMissileOverlay(nuke));
                                }

                                // TODO: overlay.Unpack?

                                if (nuke.LaunchRoom == null)
                                {
                                    var launchRoomName = shardNuke["launchRoomName"].str;
                                    roomsToGetMapStatsFrom.Add(launchRoomName);
                                    nuke.LaunchRoom = RoomManager.Instance.Get(launchRoomName, shardXName/*nukesShardName*/);
                                    nuke.LaunchRoomName = launchRoomName;
                                    StartCoroutine(GetRoomTexture(nuke.Shard, launchRoomName, (roomTexture) =>
                                    {
                                        nuke.LaunchRoomTexture = roomTexture;

                                        OnNukesRefreshed?.Invoke();
                                    }));
                                }

                                if (nuke.ImpactRoom == null)
                                {
                                    var impactRoomName = shardNuke["room"].str;
                                    roomsToGetMapStatsFrom.Add(impactRoomName);
                                    nuke.ImpactRoom = RoomManager.Instance.Get(impactRoomName, shardXName/*nukesShardName*/);
                                    nuke.ImpactRoomName = impactRoomName;
                                    nuke.ImpactPosition = PosUtility.Convert(shardNuke, nuke.ImpactRoom);
                                    StartCoroutine(GetRoomTexture(nuke.Shard, impactRoomName, (roomTexture) =>
                                    {
                                        nuke.ImpactRoomTexture = roomTexture;

                                        OnNukesRefreshed?.Invoke();

                                    }));
                                }

                                var nukeLandTime = shardNuke["landTime"];

                                var landingTime = nukeLandTime.IsNumber ? (long)nukeLandTime.n : long.Parse(nukeLandTime.str.Replace("\"", ""));

                                var initialLaunchTick = Math.Max(landingTime - Constants.NUKE_TRAVEL_TICKS, 0);
                                var progress = (float)(time - initialLaunchTick) / Constants.NUKE_TRAVEL_TICKS;

                                nuke.LandingTime = landingTime;
                                nuke.InitialLaunchTick = initialLaunchTick;
                                nuke.Progress = progress;


                                if (shardInfo != null && shardInfo.AverageTick.HasValue)
                                {
                                    // TODO: move this to a view component?
                                    var tickRate = shardInfo.AverageTick.Value;

                                    var ticksLeft = landingTime - time; // eta
                                    var etaSeconds = (float)Math.Floor((ticksLeft * tickRate) / 1000f);
                                    var impact = (float)Math.Floor(Math.Floor(landingTime / 100f) * 100);
                                    var diff = (float)Math.Floor(etaSeconds * 0.05);

                                    var now = DateTime.Now;
                                    var eta = now.AddSeconds(etaSeconds);

                                    var etaEarly = eta.AddSeconds(-diff);
                                    var etaLate = eta.AddSeconds(diff);

                                    nuke.EtaEarly = etaEarly;
                                    nuke.EtaLate = etaLate;

                                    Debug.Log($"{id} {nuke?.ImpactRoom?.Name} {eta.ToString()} => {etaEarly.ToString()} - {etaLate.ToString()}");
                                    Debug.Log($"TicksLeft:{ticksLeft} ETA:{etaSeconds}s Early:{etaEarly}s Late:{etaLate}s");
                                }
                                else
                                {
                                    Debug.LogError("no shardinfo?");
                                }
                            }

                            // TODO: detect removed nukes and clean up the arc / missile / view
                            if (!nukesInitialized[nukesShardName]) { nukesInitialized[nukesShardName] = true; }

                            if (roomsToGetMapStatsFrom.Count > 0)
                            {
                                Debug.Log($"[{nukesShardName}] Nuke monitor requested {roomsToGetMapStatsFrom.Count} rooms to be scanned");
                                MapStatsUpdater.Instance.ScanRooms(shardXName/*nukesShardName*/, roomsToGetMapStatsFrom, (json) =>
                                {
                                    OnNukesRefreshed?.Invoke();
                                });
                            }
                        });
                    }

                    OnNukesRefreshed?.Invoke();

                    /* Example
                     *  {
                            "ok": 1,
                            "nukes": {
                                "shard0": [],
                                "shard1": [],
                                "shard2": [{
                                        "_id": "5d26127173fcd27b55a7ef39",
                                        "type": "nuke",
                                        "room": "W23S15",
                                        "x": 12,
                                        "y": 37,
                                        "landTime": 17300541,
                                        "launchRoomName": "W31S18"
                                    }, {
                                        "_id": "5d26aa11385277180e5c2187",
                                        "type": "nuke",
                                        "room": "W22S22",
                                        "x": 23,
                                        "y": 22,
                                        "landTime": 17311981,
                                        "launchRoomName": "W17S28"
                                    }
                                ],
                                "shard3": []
                            }
                        }
                     */

                });

                yield return new WaitForSeconds(60);
            }
        }

        private IEnumerator GetRoomTexture(string shard, string roomName, Action<Texture> response)
        {
            var roomTextureUrl = $"https://d3os7yery2usni.cloudfront.net/map/{shard}/{roomName}.png";

            if (ScreepsAPI.Cache.Type != SourceProviderType.Official)
            {
                // Private servers runs with a different url.
                roomTextureUrl = ScreepsAPI.Cache.Address.Http($"/assets/map/{roomName}.png");
            }

            UnityWebRequest www = UnityWebRequestTexture.GetTexture(roomTextureUrl);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                //Texture myTexture = DownloadHandlerTexture.GetContent(www);
                response(myTexture);
            }
        }

        public class ShardInfoDto
        {
            public ShardInfoDto(JSONObject info)
            {
                if (info == null)
                {
                    return;
                }

                // should be a float, but it seems like something is wrong when parsing json?
                var tickRateString = info["tick"].n.ToString();
                if (float.TryParse(tickRateString, out var tickRate))
                {
                    this.AverageTick = tickRate; // for some reason .n in the jsonobject returns a really really wonky float.. :S
                }
            }

            /// <summary>
            /// Average length of a tick (in milliseconds)
            /// </summary>
            public float? AverageTick { get; internal set; }
            public long Time { get; internal set; }
            public DateTime TimeUpdated { get; internal set; }
        }

        // How do we instantiate the object, it's kinda like RoomObjects that has a view attached, we should probably do something like that
        // except this object can be between rooms, and should be using a "global" position, for placement. probably need to use PosUtility to some regards
        // perhaps a raytrace or something like playerpostion to figure out what room it is in check the update method
        // we also need to convert the impact room to a world position to figure out where to draw the arch / missile path to

        public class NukeData
        {
            public NukeData(ShardInfoDto shardInfo)
            {
                ShardInfo = shardInfo;
            }

            public string Id { get; internal set; }
            public Room LaunchRoom { get; internal set; }
            public string LaunchRoomName { get; internal set; }
            public Room ImpactRoom { get; internal set; }
            public Vector3 ImpactPosition { get; internal set; }
            public long LandingTime { get; internal set; }
            public long InitialLaunchTick { get; internal set; }
            public float Progress { get; internal set; }
            public DateTime EtaEarly { get; internal set; }
            public DateTime EtaLate { get; internal set; }
            public string ImpactRoomName { get; internal set; }
            public string Shard { get; internal set; }
            public ShardInfoDto ShardInfo { get; }
            public Texture LaunchRoomTexture { get; internal set; }
            public Texture ImpactRoomTexture { get; internal set; }
        }
        //private ObjectView NewInstance(string type)
        //{
        //    var go = PrefabLoader.Load(string.Format("{0}{1}", _path, type));
        //    if (go == null)
        //        return null;
        //    var view = go.GetComponent<ObjectView>();
        //    view.transform.SetParent(_objectParent);
        //    view.Init();
        //    return view;
        //}
        //protected void EnterRoom(Room room)
        //{
        //    Room = room;

        //    if (View == null)
        //    {
        //        Scheduler.Instance.Add(AssignView);
        //    }

        //    Shown = true;
        //    if (OnShow != null)
        //        OnShow(this, true);
        //}
    }
}
