using System;
using System.Collections.Generic;
using Screeps_API;

namespace Assets.Scripts.Screeps_API.ServerListProviders
{
    class OfficialCommunityServerListProvider : IServerListProvider
    {
        public bool MergeWithCache
        {
            get { return true; }
        }

        public void Load(Action<IEnumerable<ServerCache>> callback)
        {
            var serverList = new List<ServerCache>();

            Action<string> serverCallback = str =>
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

                    var cachedServer = new ServerCache
                    {
                        Address = {HostName = host, Port = port},
                        Type = SourceProviderType.Community,
                        Name = name,
                        LikeCount = likeCount
                    };

                    serverList.Add(cachedServer);

                    if (cachedServer.Address.HostName.EndsWith(".screepspl.us"))
                    {
                        cachedServer.Address.Ssl = true;
                        cachedServer.Address.Port = "443";
                    }
                }

                callback(serverList);
            };

            Action errorCallBack = () => { callback(serverList); };


            ScreepsAPI.Http.GetServerList(serverCallback, errorCallBack);
        }
    }
}