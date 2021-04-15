using Screeps_API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using YamlDotNet.RepresentationModel;

namespace Assets.Scripts.Screeps_API
{
    public static class SS3UnifiedCredentials
    {
        public static string GetValueOrdefault(YamlMappingNode server, string property)
        {
            var node = new YamlScalarNode(property);
            return server.Children.ContainsKey(node) ? ((YamlScalarNode)server.Children[node]).Value : null;
        }

        public static string GetScreepsConfigFilePath()
        {
            var configPaths = GetValidConfigPaths();

            foreach (var file in configPaths)
            {
                if (File.Exists(file))
                {
                    return file;
                }
            }

            throw new FileNotFoundException("screeps server config file could not be found.");

        }
        public static List<string> GetValidConfigPaths()
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

            // TODO: iterate paths and find valid ones for the choose save location dialog

            return configPaths;

        }

        public static List<ScreepsServer> LoadServers(string configPath = null)
        {
            try
            {
                var result = new List<ScreepsServer>();
                if (configPath == null)
                {
                    configPath = GetScreepsConfigFilePath();
                }

                Debug.Log($"Found config at {configPath}");

                var yaml = new YamlStream();

                using (var reader = File.OpenText(configPath))
                {
                    yaml.Load(reader);

                    var mapping = (YamlMappingNode)yaml.Documents[0].RootNode;

                    var servers = (YamlMappingNode)mapping.Children[new YamlScalarNode("servers")];

                    foreach (var item in servers.Children)
                    {
                        var serverKey = ((YamlScalarNode)item.Key).Value;
                        var server = (YamlMappingNode)item.Value;

                        var name = GetValueOrdefault(server, "name") ?? serverKey;
                        var host = GetValueOrdefault(server, "host");
                        var secure = bool.Parse(GetValueOrdefault(server, "secure") ?? "false");
                        var port = GetValueOrdefault(server, "port") ?? (secure ? "443" : "21025"); // TODO: this default logic belongs in the connection handler.
                        var ptr = bool.Parse(GetValueOrdefault(server, "ptr") ?? "false");
                        var sim = bool.Parse(GetValueOrdefault(server, "sim") ?? "false"); // if true, skip
                        var season = bool.Parse(GetValueOrdefault(server, "season") ?? "false"); // if true, skip

                        var path = GetValueOrdefault(server, "path");

                        var token = GetValueOrdefault(server, "token");
                        var username = GetValueOrdefault(server, "username");
                        var password = GetValueOrdefault(server, "password");

                        //Debug.Log($"{serverName} {host} {port} {secure} {ptr} {sim} {token} {username} {password}");

                        // TODO: attempt to find the screeps server in case of multiple registrations / names
                        // TODO: the hostname and port identifies a unique server. we should prefer the SS3 name.
                        // TODO: In case of multiple entries to the same server, where do we store the alias? perhaps as a duplicate with the alias applies as a parenthesis
                        // TODO: how do we merge servers from different sources? If we have credentials for a server, assume that we use thoose credentials, but allow a user to change them later for that specific server. only persist them in edit mode, not when connecting.
                        // TODO: a "display name" or the "name" property in the yaml file can be used to merge the different providers

                        var screepsServer = new ScreepsServer
                        {
                            Address = { HostName = host, Port = port, Ssl = secure },
                            Name = name,
                        };

                        // TODO: What if they have supplied a path, but it is not equal to the bools?
                        if (ptr)
                        {
                            screepsServer.Address.Path = "/ptr";
                        }

                        if (season)
                        {
                            screepsServer.Address.Path = "/season";
                        }

                        // Assist with merging
                        if (host.ToLowerInvariant().EndsWith("screeps.com"))
                        {
                            screepsServer.Name = $"Screeps.com";
                            if (ptr || path == "/ptr")
                            {
                                screepsServer.Name = $"PTR " + screepsServer.Name;
                            }

                            if (season || path == "/season")
                            {
                                screepsServer.Name = $"SEASONAL " + screepsServer.Name;
                            }
                        }

                        screepsServer.Credentials.Token = token;
                        screepsServer.Credentials.Email = username;
                        screepsServer.Credentials.Password = password;

                        result.Add(screepsServer);
                    }

                    return result;
                }
            }
            catch (FileNotFoundException ex)
            {
                Debug.LogError($"No SS3 Unified Credentials File found.");
                throw;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                throw;
            }
        }
    }
}
