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

                        //Debug.Log($"{serverName} {host} {port} {secure} {ptr} {sim} {token} {username} {password}");

                        var cachedServer = new ServerCache
                        {
                            Address = { HostName = host, Port = port, Ssl = secure },
                            Type = SourceProviderType.SS3_UCF_YAML,
                            Name = serverName,
                            Persist = false

                        };

                        // TODO: PTR PATH shenanigans belongs another place bool should be enough?
                        if (ptr)
                        {
                            cachedServer.Address.Path = "/ptr";
                        }

                        // Assist with merging
                        if (host.EndsWith("screeps.com"))
                        {
                            cachedServer.Persist = true;
                            cachedServer.Type = SourceProviderType.Official;
                            cachedServer.Name = $"Screeps.com";
                            if (ptr)
                            {
                                cachedServer.Name = $"PTR " + cachedServer.Name;
                            }
                        }

                        cachedServer.Credentials.Token = token;
                        cachedServer.Credentials.Email = username;
                        cachedServer.Credentials.Password = password;

                        serverList.Add(cachedServer);
                    }

                    callback(serverList);
                }
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
                Environment.GetEnvironmentVariable("SCREEPS_CONFIG"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) ?? string.Empty, "screeps/config.yaml"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) ?? string.Empty, "screeps/config.yml"),
                Path.Combine(Environment.CurrentDirectory, ".screeps.yaml"),
                Path.Combine(Environment.CurrentDirectory, ".screeps.yml"),
                /* Linux / Mac*/
                Path.Combine(Environment.GetEnvironmentVariable("XDG_CONFIG_HOME") ?? string.Empty, "screeps/config.yaml"),
                Path.Combine(Environment.GetEnvironmentVariable("XDG_CONFIG_HOME") ?? string.Empty, "screeps/config.yml"),
                Path.Combine(Environment.GetEnvironmentVariable("HOME") ?? string.Empty, ".config/screeps/config.yaml"),
                Path.Combine(Environment.GetEnvironmentVariable("HOME") ?? string.Empty, ".config/screeps/config.yml"),
                Path.Combine(Environment.GetEnvironmentVariable("HOME") ?? string.Empty, ".screeps.yaml"),
                Path.Combine(Environment.GetEnvironmentVariable("HOME") ?? string.Empty, ".screeps.yml"),

            };

            foreach (var file in configPaths)
            {
                if (File.Exists(file))
                {
                    return file;
                }
            }

            throw new FileNotFoundException("screeps server config file could not be found.");
        }
    }
}