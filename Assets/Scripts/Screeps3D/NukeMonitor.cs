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
        private Dictionary<string, NukeMissileOverlay> _nukes = new Dictionary<string, NukeMissileOverlay>();

        private List<ShardInfo> _shardInfo = new List<ShardInfo>();

        private IEnumerator getNukes;

        private bool nukesInitialized = false;

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

            // Should probably do this lookup on connect
            if (ScreepsAPI.Cache.Type == SourceProviderType.Official)
            {
                ScreepsAPI.Http.Request("GET", $"/api/game/shards/info", null, (jsonShardInfo) =>
                {
                    // tickrates and such, what about private servers?
                    var shardInfo = new JSONObject(jsonShardInfo);
                    var shards = shardInfo["shards"].list;
                    foreach (var shard in shards)
                    {
                        var tickRateString = shard["tick"].n;
                        _shardInfo.Add(new ShardInfo(shard));
                    }
                });
            }
            else
            {
                // PS => /api/game/tick => { "ok": 1, "tick": 1234 }
                ScreepsAPI.Http.Request("GET", $"/api/game/tick", null, (jsonTickInfo) =>
                {
                    var info = new JSONObject(jsonTickInfo);

                    var shard = new JSONObject();
                    shard.AddField("tick", info["tick"].n);

                    _shardInfo.Add(new ShardInfo(shard));
                });
            }
            
            // also it seems like when we get a 404, it keeps trying to call the endpoint due to the "fail" retry logic.
            
        }

        private IEnumerator GetNukes()
        {
            yield return new WaitForSeconds(5);

            while (true)
            {
                if (_shardInfo.Count == 0)
                {
                    Debug.LogWarning("shardinfo not fetched yet, waiting 5 seconds");
                    yield return new WaitForSeconds(5);
                }

                //NotifyText.Message("SCANNING FOR NUKES!", Color.red);

                // We might have an issue if people use custom shard names, so we can't use shardName, because playerposition shardname is shardX
                var shardIndex = PlayerPosition.Instance.ShardLevel;

                // Should probably cache this, and refresh it at an interval to detect new nukes.
                ScreepsAPI.Http.GetExperimentalNukes((jsonString) =>
                {

                    var obj = new JSONObject(jsonString);
                    var status = obj["ok"];
                    var nukes = obj["nukes"];
                    var nukesShardName = nukes.keys[shardIndex];
                    var shardName = ScreepsAPI.Cache.Type == SourceProviderType.Official ? nukesShardName : $"shard{shardIndex}";
                    var shardNukes = nukes[nukesShardName].list;
                    //NotifyText.Message($"{nukesShardName} has {shardNukes.Count} nukes!", Color.red);
                    Debug.Log($"{nukesShardName} has {shardNukes.Count} nukes!");
                    var time = ScreepsAPI.Time;


                    // TODO: getting time should be moved to when logging or switching shards
                    ScreepsAPI.Http.Request("GET", $"/api/game/time?shard={nukesShardName}", null, (jsonTime) =>
                        {
                            var timeData = new JSONObject(jsonTime)["time"];
                            if (timeData != null)
                            {
                                ScreepsAPI.Time = time = (long)timeData.n;
                            }

                            foreach (var nuke in shardNukes)
                            {
                                var id = nuke["_id"].str; // should probably switch to UnPackUtility later.
                                var key = $"{shardName}/{id}"; // shardname breaks something, probably because the same id is stored multiple places?
                                if (!_nukes.TryGetValue(key, out var overlay))
                                {
                                    // TODO: further detection if this was a newly launched nuke. perhaps the progress is at a really low percentage, or between x ticks?
                                    if (nukesInitialized)
                                    {
                                        NotifyText.Message($"{nukesShardName} => Nuclear Launch Detected", Color.red);
                                    }

                                    overlay = new NukeMissileOverlay(id);
                                    _nukes.Add(key, overlay);
                                }

                                // TODO: overlay.Unpack?

                                if (overlay.LaunchRoom == null)
                                {
                                    overlay.LaunchRoom = RoomManager.Instance.Get(nuke["launchRoomName"].str, shardName);

                                }

                                if (overlay.ImpactRoom == null)
                                {
                                    overlay.ImpactRoom = RoomManager.Instance.Get(nuke["room"].str, shardName);
                                    overlay.ImpactPosition = PosUtility.Convert(nuke, overlay.ImpactRoom);
                                }

                                var nukeLandTime = nuke["landTime"];

                                var landingTime = nukeLandTime.IsNumber ? (long)nukeLandTime.n : long.Parse(nukeLandTime.str.Replace("\"", ""));

                                var initialLaunchTick = Math.Max(landingTime - Constants.NUKE_TRAVEL_TICKS, 0);
                                var progress = (float)(time - initialLaunchTick) / Constants.NUKE_TRAVEL_TICKS;

                                overlay.LandingTime = landingTime;
                                overlay.InitialLaunchTick = initialLaunchTick;
                                overlay.Progress = progress;

                                var shard = _shardInfo[shardIndex];
                                if (shard != null && shard.AverageTick.HasValue)
                                {
                                    // TODO: move this to a view component?
                                    var tickRate = shard.AverageTick.Value;

                                    var ticksLeft = landingTime - time; // eta
                                    var etaSeconds = (float)Math.Floor((ticksLeft * tickRate) / 1000f);
                                    var impact = (float)Math.Floor(Math.Floor(landingTime / 100f) * 100);
                                    var diff = (float)Math.Floor(etaSeconds * 0.05);

                                    var now = DateTime.Now;
                                    var eta = now.AddSeconds(etaSeconds);

                                    var etaEarly = eta.AddSeconds(-diff);
                                    var etaLate = eta.AddSeconds(diff);

                                    Debug.Log($"{id} {overlay?.ImpactRoom?.Name} {eta.ToString()} => {etaEarly.ToString()} - {etaLate.ToString()}");
                                    Debug.Log($"TicksLeft:{ticksLeft} ETA:{etaSeconds}s Early:{etaEarly}s Late:{etaLate}s");
                                }
                                else
                                {
                                    Debug.LogError("no shardinfo?");
                                }
                            }

                            // TODO: detect removed nukes and clean up the arc / missile / view

                            if (!this.nukesInitialized) { this.nukesInitialized = true; }
                        });


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

        private class ShardInfo
        {
            public ShardInfo(JSONObject info)
            {
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
        }

        // How do we instantiate the object, it's kinda like RoomObjects that has a view attached, we should probably do something like that
        // except this object can be between rooms, and should be using a "global" position, for placement. probably need to use PosUtility to some regards
        // perhaps a raytrace or something like playerpostion to figure out what room it is in check the update method
        // we also need to convert the impact room to a world position to figure out where to draw the arch / missile path to

        private class NukeData
        {
            public NukeData(JSONObject nuke)
            {

            }
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
