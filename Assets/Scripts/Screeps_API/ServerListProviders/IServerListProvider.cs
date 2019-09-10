using Screeps_API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Screeps_API.ServerListProviders
{
    interface IServerListProvider
    {
        bool MergeWithCache { get; }

        void Load(Action<IEnumerable<ServerCache>> callback);
    }
}
