using Screeps_API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Assets.Scripts.Screeps_API
{
    public static class SS3UnifiedCredentials
    {
        // TODO: value types?
        public static string SetValueOrdefault(YamlMappingNode server, string property, string value)
        {
            var node = new YamlScalarNode(property);

            if (!server.Children.ContainsKey(node))
            {
                server.Add(property, value);
            }
            else
            {
                // Update existing nodes value
                var existingNode = ((YamlScalarNode)server.Children[node]);
                existingNode.Value = value;
            }

            return server.Children.ContainsKey(node) ? ((YamlScalarNode)server.Children[node]).Value : null;
        }

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

                var deserializer = new DeserializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .Build();

                using (var reader = File.OpenText(configPath))
                {
                    var deserializedServers = deserializer.Deserialize<SS3UnifiedCredentialsDocument>(reader);

                    //Debug.Log($"yaml deserialize found {deserializedServers.Servers.Count} servers");

                    foreach (var item in deserializedServers.Servers)
                    {
                        //Debug.Log($"{item.Key} => {item.Value.Host}:{item.Value.Port}");
                        var screepsServer = new ScreepsServer(item.Key, item.Value);

                        result.Add(screepsServer);
                    }
                }

                return result;
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

        public static void SaveServer(ScreepsServer server)
        {
            //var configPath = GetScreepsConfigFilePath();

            //Debug.Log($"Found config at {configPath}"); // TODO: handle a case where there is no config, throw exception?

            //var yaml = new YamlStream();

            //using (var stream = File.Open(configPath, FileMode.OpenOrCreate | FileMode.Append))
            //{
            //    yaml.Load(new StreamReader(stream));

            //    var mapping = (YamlMappingNode)yaml.Documents[0].RootNode;

            //    var servers = (YamlMappingNode)mapping.Children[new YamlScalarNode("servers")];

            //    // TODO: find existing node and update it, can we change the key?
            //    var yamlServerEntry = new YamlMappingNode();
            //    servers.Add(server.Name, yamlServerEntry);


            //    yaml.Save()


            //}
        }

        public class SS3UnifiedCredentialsDocument
        {
            /// <summary>
            /// A key value pair where the key is a server name / entry
            /// </summary>
            public Dictionary<string, SS3UnifiedCredentialsServer> Servers { get; set; }
        }

        public class SS3UnifiedCredentialsServer
        {
            public string Name { get; set; }
            public string Host { get; set; }
            public bool? Secure { get; set; }
            public string Port { get; set; }
            public bool? Ptr { get; set; }
            public bool? Sim { get; set; }
            public bool? Season { get; set; }
            public string Path { get; set; }
            public string Token { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}
