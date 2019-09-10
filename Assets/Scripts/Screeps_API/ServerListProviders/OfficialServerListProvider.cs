using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Screeps_API;

namespace Assets.Scripts.Screeps_API.ServerListProviders
{
    public class OfficialServerListProvider : IServerListProvider
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

            var publicServer = new ServerCache();
            publicServer.MMO = true;
            publicServer.Name = "Screeps.com";
            publicServer.Address.HostName = "Screeps.com";
            publicServer.Address.Path = "/";
            publicServer.Address.Port = "";
            publicServer.Address.Ssl = true;
            serverList.Add(publicServer);

            var ptr = new ServerCache();
            ptr.MMO = true;
            ptr.Name = "PTR Screeps.com";
            ptr.Address.HostName = "screeps.com";
            ptr.Address.Path = "/ptr";
            ptr.Address.Port = "";
            ptr.Address.Ssl = true;

            serverList.Add(ptr);

            callback(serverList);

        }
    }
}
