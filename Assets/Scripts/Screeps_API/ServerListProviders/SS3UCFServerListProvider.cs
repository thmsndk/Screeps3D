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
                        Address = { HostName = host, Port = port },
                        Type = SourceProviderType.SS3_UCF_YAML,
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

            var configPath = GetScreepsConfigFilePath();
            Debug.Log($"Found config at {configPath}");
            var yaml = new YamlStream();

            using (var reader = File.OpenText(configPath))
            {
                yaml.Load(reader);

                var mapping = (YamlMappingNode)yaml.Documents[0].RootNode;

                var servers = (YamlMappingNode)mapping.Children[new YamlScalarNode("servers")];

                foreach (var item in servers.Children)
                {
                    var serverName = ((YamlScalarNode)item.Key).Value;
                    var server = (YamlMappingNode)item.Value;

                    var host = GetValueOrdefault(server, "host");
                    var secure = bool.Parse(GetValueOrdefault(server, "secure") ?? "false");
                    var port = GetValueOrdefault(server, "port") ?? (secure ? "443" : "21025"); // TODO: this default logic belongs in the connection handler.
                    var ptr = bool.Parse(GetValueOrdefault(server, "ptr") ?? "false");
                    var sim = bool.Parse(GetValueOrdefault(server, "sim") ?? "false"); // if true, skip

                    var token = GetValueOrdefault(server, "token");
                    var username = GetValueOrdefault(server, "username");
                    var password = GetValueOrdefault(server, "password");

                    Debug.Log($"{serverName} {host} {port} {secure} {ptr} {sim} {token} {username} {password}");

                    var cachedServer = new ServerCache
                    {
                        Address = { HostName = host, Port = port, Ssl = secure }, 
                        Type = SourceProviderType.SS3_UCF_YAML,
                        Name = serverName,

                    };

                    // TODO: PTR PATH shenanigans belongs another place bool should be enough?
                    if (ptr)
                    {
                        cachedServer.Address.Path = "/ptr";
                    }

                    serverList.Add(cachedServer);
                }

                callback(serverList);
            }
        }

        private static string GetValueOrdefault(YamlMappingNode server, string property)
        {
            var node = new YamlScalarNode(property);
            return server.Children.ContainsKey(node) ? ((YamlScalarNode)server.Children[node]).Value : null;
        }

        private string GetScreepsConfigFilePath()
        {
            //Environment.GetEnvironmentVariable();
            /*
             * 
             * https://docs.unity3d.com/Manual/PlatformDependentCompilation.html
             * UNITY_EDITOR_WIN
             * UNITY_EDITOR_OSX
             * UNITY_EDITOR_LINUX
             * 
             * UNITY_STANDALONE_WIN
             * UNITY_STANDALONE_OSX
             * UNITY_STANDALONE_LINUX
             * 
                Env Variable ($SCREEPS_CONFIG)
                Project Root (Optional) - (project/.screeps.yaml)
                Current Working Directory - (./.screeps.yaml)
                XDG Config Directory - ($XDG_CONFIG_HOME/screeps/config.yaml)
                XDG Config Default Directory - ($HOME/.config/screeps/config.yaml)
                APPDATA (Windows Only) - (%APPDATA%/screeps/config.yaml)
                Home Directory - (~/.screeps.yaml)

                https://assetstore.unity.com/packages/tools/integration/yamldotnet-for-unity-36292
             */

            var configPaths = new List<string>
            {
                Environment.GetEnvironmentVariable("$SCREEPS_CONFIG"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) ?? string.Empty, "screeps/config.yaml"),
                Path.Combine(Environment.CurrentDirectory, ".screeps.yaml"),
                Path.Combine(Environment.GetEnvironmentVariable("$XDG_CONFIG_HOME") ?? string.Empty, "screeps/config.yaml"),
                Path.Combine(Environment.GetEnvironmentVariable("$HOME") ?? string.Empty, ".config/screeps/config.yaml"),
                Path.Combine(Environment.GetEnvironmentVariable("$HOME") ?? string.Empty, ".screeps.yaml"),

            };

            foreach (var file in configPaths)
            {
                Debug.Log(file);
                if (File.Exists(file))
                {
                    return file;
                }
            }

            throw new FileNotFoundException("screeps server config file could not be found.");
        }
    }
}