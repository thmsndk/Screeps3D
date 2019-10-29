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
            get { return true; }
        }

        public void Load(Action<IEnumerable<ServerCache>> callback)
        {
            var serverList = new List<ServerCache>();

            var publicServer = new ServerCache
            {
                Official = true,
                Type = SourceProviderType.Official,
                Name = "Screeps.com",
                Address = {HostName = "Screeps.com", Path = "/", Port = "", Ssl = true}
            };
            serverList.Add(publicServer);

            var ptr = new ServerCache
            {
                Official = true,
                Type = SourceProviderType.Official,
                Name = "PTR Screeps.com",
                Address = {HostName = "screeps.com", Path = "/ptr", Port = "", Ssl = true}
            };

            serverList.Add(ptr);

            callback(serverList);
        }
    }
}