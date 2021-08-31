using System;
using System.Collections.Generic;
using System.IO;
using Screeps_API;
using UnityEngine;

namespace Assets.Scripts.Screeps_API.ServerListProviders
{
    class OfficialCommunityServerListProvider : IServerListProvider
    {
        private const string CACHE_FILE = "screeps.com.servers.list.json";

        public bool MergeWithCache
        {
            get { return true; }
        }

        public void Load(Action<IEnumerable<IScreepsServer>> callback)
        {
            var serverList = new List<IScreepsServer>();

            Action<string> serverCallback = str =>
            {
                UnpackServers(str, serverList);

                // Persist serverlist in case of no response later.
                File.WriteAllText(Path.Combine(Application.persistentDataPath, CACHE_FILE), str);

                callback(serverList);
            };

            Action errorCallBack = () => {

                // Load servers from cache
                var json = File.ReadAllText(Path.Combine(Application.persistentDataPath, CACHE_FILE));
                UnpackServers(json, serverList);

                callback(serverList); 
            };

            ScreepsAPI.Http.GetServerList(serverCallback, errorCallBack);
        }

        private static void UnpackServers(string str, List<IScreepsServer> serverList)
        {
            var obj = new JSONObject(str);
            var servers = obj["servers"].list;

            foreach (var server in servers)
            {
                var name = server["name"].str;
                //TODO implement status
                var status = server["status"].str;
                var likeCount = Convert.ToInt32(server["likeCount"].n);

                var settings = server["settings"];
                var host = settings["host"].str;
                var port = settings["port"].str;

                var cachedServer = new ScreepsServer(name)
                {
                    Address = { HostName = host, Port = port },
                    Name = name,
                };

                cachedServer.Meta.LikeCount = likeCount;

                serverList.Add(cachedServer);

                if (cachedServer.Address.HostName.EndsWith(".screepspl.us"))
                {
                    cachedServer.Address.Ssl = true;
                    cachedServer.Address.Port = "443";
                }
            }
        }
    }
}