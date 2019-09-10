using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Screeps_API;

namespace Assets.Scripts.Screeps_API.ServerListProviders
{
    class OfficialCommunityServerListProvider : IServerListProvider
    {
        public bool MergeWithCache
        {
            get
            {
                return true;
            }
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
                    var status = server["status"].str;
                    var likeCount = Convert.ToInt32(server["likeCount"].n);

                    var settings = server["settings"];
                    var host = settings["host"].str;
                    var port = settings["port"].str;

                    var cachedServer = new ServerCache();
                    cachedServer.Address.HostName = host;
                    cachedServer.Address.Port = port;
                    cachedServer.Name = name;
                    cachedServer.LikeCount = likeCount;

                    serverList.Add(cachedServer);

                    //if (cachedServer.Address.HostName.EndsWith(".screepspl.us"))
                    //{
                    //    // WebSocketSharp has issues connecting to SSL
                    //    //cachedServer.Address.Ssl = true;
                    //    //cachedServer.Address.Port = "443";
                    //    cachedServer.Address.Ssl = false;
                    //    cachedServer.Address.Port = port;
                    //}
                }

                callback(serverList);
            };

            Action errorCallBack = () =>
            {
                callback(serverList);
            };


            ScreepsAPI.Http.GetServerList(serverCallback, errorCallBack);
        }

        
    }
}
