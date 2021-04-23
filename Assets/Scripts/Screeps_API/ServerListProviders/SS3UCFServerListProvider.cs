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

        public void Load(Action<IEnumerable<ServerCache>> callback)
        {
            var serverList = new List<ServerCache>();

            try
            {
                var servers = SS3UnifiedCredentials.LoadServers();

                foreach (var server in servers)
                {
                    var cachedServer = new ServerCache
                    {
                        Address = server.Address,
                        Type = SourceProviderType.SS3_UCF_YAML,
                        Name = server.Name,
                        Credentials = server.Credentials
                    };

                    serverList.Add(cachedServer);
                }

                callback(serverList);
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

        private string GetScreepsConfigFilePath()
        {
            return SS3UnifiedCredentials.GetScreepsConfigFilePath();
        }

        private string GetValueOrdefault(YamlMappingNode server, string property)
        {
            return SS3UnifiedCredentials.GetValueOrdefault(server, property);
        }
    }
}