using Screeps_API;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Screeps_API
{
    public class ShardInfoMonitor : MonoBehaviour
    {
        public Dictionary<string, ShardInfoDto> ShardInfo { get; set; } = new Dictionary<string, ShardInfoDto>();

        public Dictionary<string, IEnumerator> ShardTicker { get; set; } = new Dictionary<string, IEnumerator>();

        public int Count { get { return ShardInfo.Count; } }

        private void Start()
        {
            Debug.Log("ShardInfoMonitor Started");
            StartCoroutine(GetShardInfo());
        }

        public ShardInfoDto this[string shardName]
        {
            get
            {
                if (ShardInfo.TryGetValue(shardName, out var shardInfo))
                {
                    return shardInfo;
                }

                return null;
            }
        }

        private IEnumerator GetShardInfo()
        {
            while (!ScreepsAPI.IsConnected)
            {
                yield return new WaitForSeconds(5);
            }

            while (ScreepsAPI.IsConnected)
            {
                // Yield return here seems broken, is it because of the callback? or because i've made a faulty return type on requests?
                ScreepsAPI.Http.Request("GET", $"/api/game/shards/info", null, (jsonShardInfo) =>
                {
                    // tickrates and such, what about private servers?
                    var shardInfoData = new JSONObject(jsonShardInfo);
                    var shardsData = shardInfoData["shards"];
                    if (shardsData == null)
                    {
                        Debug.LogWarning("no shards foound? " + jsonShardInfo); // we recieved an empty jsonShardInfo object
                        return;
                    }

                    var shards = shardsData.list;
                    foreach (var shard in shards)
                    {
                        var tickRateString = shard["tick"].n;

                        var shardName = shard["name"].str;
                        if (!ShardInfo.TryGetValue(shardName, out var shardInfo))
                        {
                            shardInfo = new ShardInfoDto();
                            ShardInfo.Add(shardName, shardInfo);
                        }

                        shardInfo.Update(shard);

                        var time = ScreepsAPI.Time;

                        if (!ShardTicker.TryGetValue(shardName, out var ticker))
                        {
                            ticker = SimulateTick(shardName, shardInfo);
                            ShardTicker.Add(shardName, ticker);
                            StartCoroutine(ticker);
                        }

                        ScreepsAPI.Http.Request("GET", $"/api/game/time?shard={shardName}", null, (jsonTime) =>
                        {
                            var timeData = new JSONObject(jsonTime)["time"];
                            if (timeData != null)
                            {
                                time = (long)timeData.n;
                            }

                            if (ShardInfo.TryGetValue(shardName, out var shardInfo2))
                            {
                                shardInfo2.Time = time;
                            }
                            else
                            {
                                // Handle cases where server has not updated to latest admin-util yet.
                                shardInfo2 = new ShardInfoDto();
                                ShardInfo.Add(shardName, shardInfo2);
                                shardInfo2.Time = time;
                                shardInfo2.AverageTick = 1000;
                            }
                        });
                    }
                });

                yield return new WaitForSecondsRealtime(60);
            }
        }

        private IEnumerator SimulateTick(string shardName, ShardInfoDto shardInfo)
        {
            while (true)
            {
                shardInfo.Time += 1;

                yield return new WaitForSecondsRealtime((float)shardInfo.AverageTick / 1000);
            }
        }
    }

    public class ShardInfoDto
    {
        /// <summary>
        /// Average length of a tick (in milliseconds)
        /// </summary>
        public float? AverageTick { get; internal set; }
        public long Time { get; internal set; }
        public DateTime TimeUpdated { get; internal set; }

        internal void Update(JSONObject info)
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

            this.TimeUpdated = DateTime.Now;
        }
    }
}
