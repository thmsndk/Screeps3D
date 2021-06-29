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
        public static string ConfigFilePath { get; private set; }

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

        internal static void SetConfigFile(string configFilepath)
        {
            ConfigFilePath = configFilepath;
        }

        public static string GetScreepsConfigFilePath()
        {
            if (ConfigFilePath != null)
            {
                return ConfigFilePath;
            }

            var configPaths = GetValidConfigPaths();

            foreach (var file in configPaths)
            {
                if (File.Exists(file))
                {
                    ConfigFilePath = file;
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

            var validFileNames = new List<string>
            {
                "config.yaml",
                "config.yml"
            };

            var validFileNamesInAScreepsFolder = validFileNames.Select(f => Path.Combine("screeps", f));

            var validFileNamesWithADot = validFileNames.Select(f => "." + f);

            var configPaths = new List<string>();

            AddConfigPath(configPaths, Environment.GetEnvironmentVariable("SCREEPS_CONFIG"), null);

            AddConfigPath(configPaths, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), validFileNamesInAScreepsFolder);

            AddConfigPath(configPaths, Environment.CurrentDirectory, validFileNamesWithADot);

            /* Linux / Mac*/
            AddConfigPath(configPaths, Environment.GetEnvironmentVariable("XDG_CONFIG_HOME"), validFileNamesInAScreepsFolder);

            AddConfigPath(configPaths, Environment.GetEnvironmentVariable("HOME"), validFileNames.Select(f => Path.Combine(".config/screeps", f)));

            AddConfigPath(configPaths, Environment.GetEnvironmentVariable("HOME"), validFileNamesWithADot);

            return configPaths;

        }

        private static bool AddConfigPath(List<string> paths, string path, IEnumerable<string> fileNames)
        {
            if (Directory.Exists(path))
            {
                if (fileNames == null)
                {
                    paths.Add(path);
                }
                else
                {
                    foreach (var fileName in fileNames)
                    {
                        paths.Add(Path.Combine(path, fileName));
                    }
                }

                return true;
            }

            return false;
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

                // initially, a config path is set, but the file does not exist yet.
                if (!File.Exists(configPath))
                {
                    return result;
                }

                using (var reader = File.OpenText(configPath))
                {
                    var deserializedServers = deserializer.Deserialize<SS3UnifiedCredentialsDocument>(reader);

                    // in case of an empty file, we won't get servers
                    if (deserializedServers == null)
                    {
                        return result;
                    }

                    //Debug.Log($"yaml deserialize found {deserializedServers.Servers.Count} servers");
                    foreach (var server in deserializedServers.Servers)
                    {
                        if (server.Value.Sim.HasValue && server.Value.Sim.Value)
                        {
                            // We don't load sim
                            continue;
                        }

                        //Debug.Log($"{item.Key} => {item.Value.Host}:{item.Value.Port}");
                        var screepsServer = new ScreepsServer(server.Key, server.Value);

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

        public static void SaveServer(IScreepsServer server, string oldKey = null, bool persistCredentials = false)
        {
            var configFilePath = GetScreepsConfigFilePath();

            Debug.Log($"Found config at {configFilePath}"); // TODO: handle a case where there is no config, throw exception?

            var deserializer = new DeserializerBuilder()
                //.WithNamingConvention(new CamelCaseNamingConvention())
                .Build();

            var document = new SS3UnifiedCredentialsDocument() { Servers = new Dictionary<string, SS3UnifiedCredentialsServer>() };

            if (File.Exists(configFilePath))
            {
                using (var reader = File.OpenText(configFilePath))
                {
                    document = deserializer.Deserialize<SS3UnifiedCredentialsDocument>(reader);
                    if (document == null)
                    {
                        document = new SS3UnifiedCredentialsDocument() { Servers = new Dictionary<string, SS3UnifiedCredentialsServer>() };
                    }
                }
            }

            if (!document.Servers.TryGetValue(oldKey ?? server.Key, out var yamlServer))
            {
                yamlServer = new SS3UnifiedCredentialsServer();
                document.Servers.Add(server.Key, yamlServer);
            };

            if (server.Key != oldKey)
            {
                document.Servers.Remove(oldKey);
                document.Servers.Add(server.Key, yamlServer);
            }

            yamlServer.Name = server.Key != server.Name ? server.Name : null;
            yamlServer.Secure = server.Address.Ssl ? true : (bool?)null;
            yamlServer.Host = server.Address.HostName;
            yamlServer.Port = server.Address.Port;

            // Persist port, even if "default"
            //if (server.Address.Port == "443" || server.Address.Port == "21025")
            //{
            //    yamlServer.Port = null;
            //}

            yamlServer.Path = server.Address.Path;
            
            if (server.Address.Path == "/")
            {
                yamlServer.Path = null;
            }

            yamlServer.Ptr = server.Address.Path == "/ptr" ? true : (bool?)null;
            yamlServer.Season = server.Address.Path == "/season" ? true : (bool?)null;
            yamlServer.Sim = server.Address.Path == "/sim" ? true : (bool?)null;

            if (server.Official)
            {
                yamlServer.Token = persistCredentials && !string.IsNullOrEmpty(server.Credentials.Token) ? server.Credentials.Token : null;
            }
            else
            {
                yamlServer.Username = persistCredentials&& !string.IsNullOrEmpty(server.Credentials.Email) ? server.Credentials.Email : null;
                yamlServer.Password = persistCredentials && !string.IsNullOrEmpty(server.Credentials.Password) ? server.Credentials.Password : null;
            }

            var serializer = new SerializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .Build();
            var yaml = serializer.Serialize(document);

            if (File.Exists(configFilePath))
            {
                var filename = Path.GetFileName(configFilePath);
                File.Copy(configFilePath, configFilePath.Replace(filename, filename + $".{DateTime.Now:yyyyMMddHHmm}.bak"));
            }

            File.WriteAllText(configFilePath, yaml);

        }

        public class SS3UnifiedCredentialsDocument
        {
            /// <summary>
            /// A key value pair where the key is a server name / entry
            /// </summary>
            [YamlMember(Alias = "servers", ApplyNamingConventions = false)]
            public Dictionary<string, SS3UnifiedCredentialsServer> Servers { get; set; } // gotta be lowercase to not mess up the seralizer
        }

        public class SS3UnifiedCredentialsServer
        {
            [YamlMember(Alias = "name", ApplyNamingConventions = false)]
            public string Name { get; set; }
            
            [YamlMember(Alias = "host", ApplyNamingConventions = false)]
            public string Host { get; set; }

            [YamlMember(Alias = "secure", ApplyNamingConventions = false)]
            public bool? Secure { get; set; }

            [YamlMember(Alias = "port", ApplyNamingConventions = false)]
            public string Port { get; set; }

            [YamlMember(Alias = "path", ApplyNamingConventions = false)]
            public string Path { get; set; }

            [YamlMember(Alias = "ptr", ApplyNamingConventions = false)]
            public bool? Ptr { get; set; }

            [YamlMember(Alias = "sim", ApplyNamingConventions = false)]
            public bool? Sim { get; set; }

            [YamlMember(Alias = "season", ApplyNamingConventions = false)]
            public bool? Season { get; set; }

            [YamlMember(Alias = "token", ApplyNamingConventions = false)]
            public string Token { get; set; }

            [YamlMember(Alias = "username", ApplyNamingConventions = false)]
            public string Username { get; set; }

            [YamlMember(Alias = "password", ApplyNamingConventions = false)]
            public string Password { get; set; }
        }
    }
}
