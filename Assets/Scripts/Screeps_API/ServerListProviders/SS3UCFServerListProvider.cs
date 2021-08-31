using System;
using System.Collections.Generic;
using System.IO;
using Screeps_API;
using UnityEngine;
using YamlDotNet.RepresentationModel;

namespace Assets.Scripts.Screeps_API.ServerListProviders
{
    /// <summary>
    /// https://github.com/screepers/screepers-standards/blob/master/SS3-Unified_Credentials_File.md
    /// </summary>
    class SS3UCFServerListProvider : IServerListProvider
    {
        public bool MergeWithCache
        {
            get { return true; }
        }

        public void Load(Action<IEnumerable<IScreepsServer>> callback)
        {
            try
            {
                var servers = SS3UnifiedCredentials.LoadServers();

                callback(servers);
            }
            catch (FileNotFoundException ex)
            {
                Debug.Log($"No SS3 Unified Credentials File found.");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}