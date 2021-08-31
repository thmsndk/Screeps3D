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

        public void Load(Action<IEnumerable<IScreepsServer>> callback)
        {
            var serverList = new List<IScreepsServer>();

            var publicServer = new ScreepsServer("main")
            {
                Official = true,
                Name = "Screeps.com",
                Address = {HostName = "screeps.com", Path = "/", Port = "443", Ssl = true}
            };
            serverList.Add(publicServer);

            var ptr = new ScreepsServer("ptr")
            {
                Official = true,
                Name = "PTR Screeps.com",
                Address = { HostName = "screeps.com", Path = "/ptr", Port = "443", Ssl = true }
            };

            serverList.Add(ptr);

            var season = new ScreepsServer("season")
            {
                Official = true,
                Name = "SEASONAL Screeps.com",
                Address = { HostName = "screeps.com", Path = "/season", Port = "443", Ssl = true },
            };

            serverList.Add(season);

            callback(serverList);
        }
    }
}