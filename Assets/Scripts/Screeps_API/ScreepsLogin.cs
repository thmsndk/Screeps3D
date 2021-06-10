using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Assets.Scripts.Screeps_API;
using Assets.Scripts.Screeps_API.ServerListProviders;
using Assets.Scripts.Screeps3D.Main;
using Common;
using Screeps3D;
using Screeps3D.Menus.ServerList;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Screeps_API
{
    public class ScreepsLogin : MonoBehaviour
    {
        [SerializeField] private ScreepsAPI _api = default;
        [SerializeField] private Button _connect = default;
        [SerializeField] private FadePanel _panel = default;
        [SerializeField] private Button _addServer = default;
        [SerializeField] private Button _removeServer = default;
        [SerializeField] private Button _editServer = default;
        [SerializeField] private Button _exit = default;
        [SerializeField] private ChooseSS3UnifiedCredentialsFileLocationPopup _chooseSS3UnifiedCredentialsFileLocationPopup = default;
        [SerializeField] private EditServerPopup _editServerPopup = default;

        public Action<Credentials, Address> OnSubmit;
        public string secret = "abc123";

        private List<IScreepsServer> _servers;
        private int _serverIndex;
        private string _savePath = "servers";

        private ServerListTableViewController _serverListTableViewController;

        private List<IServerListProvider> serverListProviders = new List<IServerListProvider>();

        private bool editServer = false;

        private IScreepsServer selectedServer;

        private void Start()
        {
            GameManager.OnModeChange += OnModeChange;
            serverListProviders.Add(new SS3UCFServerListProvider()); // Load all servers/credentials the user has supplied, it is important that this is the first entry.
            serverListProviders.Add(new OfficialServerListProvider()); // Add official servers, in case the user does not have any servers
            serverListProviders.Add(new OfficialCommunityServerListProvider()); // Add community servers provided by the official team.
            // TODO: SS3 Unified Credentials File .ini
            // https://screeps.online/ ?
            
            if (HasUnifiedCredentials())
            {
                StartCoroutine(LoadServers());

                UpdateFieldVisibility();
            }
            else
            {
                _chooseSS3UnifiedCredentialsFileLocationPopup.OnOkClicked += SS3UnifiedCredentialsFileLocationSelected;
                _chooseSS3UnifiedCredentialsFileLocationPopup?.gameObject?.SetActive(true);
            }

            _connect.onClick.AddListener(OnConnect);
            _addServer.onClick.AddListener(OnAddServer);
            _removeServer.onClick.AddListener(OnRemoveServer);
            _editServer.onClick.AddListener(OnEditServer);

            _serverListTableViewController = gameObject.GetComponent<ServerListTableViewController>();

            _serverListTableViewController.onServerSelected.AddListener(OnServerSelected);

            _exit.onClick.AddListener(OnExit);
        }

        private void OnExit()
        {
#if UNITY_EDITOR
            // Application.Quit() does not work in the editor so
            // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
            UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
        }

        private void OnModeChange(GameMode mode)
        {
            if (mode == GameMode.Login)
                _panel.Show();
            else
                _panel.Hide();
        }

        private void OnEditServer()
        {
            editServer = true;
            UpdateFieldVisibility();
            ShowEditServerDialog();

        }

        private void ShowEditServerDialog()
        {
            _editServerPopup?.SetServer(selectedServer);
            _editServerPopup?.gameObject?.SetActive(true);
        }

        private void OnRemoveServer()
        {
            if (_serverIndex == 0)
                return;

            _servers.RemoveAt(_serverIndex);
            OnServerChange(_serverIndex - 1);
            UpdateServerList();
            SaveManager.Save(_savePath, _servers);
        }

        //private void UpdateServerDropdown()
        //{
        //    _serverSelect.ClearOptions();
        //    var options = new List<TMP_Dropdown.OptionData>();
        //    foreach (var server in _servers)
        //    {
        //        options.Add(new TMP_Dropdown.OptionData(string.Format("{0} {1}",
        //            server.Name ?? server.Address.HostName,
        //            server.LikeCount > 0 ? string.Format("({0} Likes)", server.LikeCount) : string.Empty)));
        //    }
        //    _serverSelect.AddOptions(options);
        //    _serverSelect.value = _serverIndex;
        //}

        private void OnAddServer()
        {
            PlayerInput.Get("Server Hostname\n<size=12>example: 127.0.0.1</size>", OnSubmitServer);
        }

        private void OnSubmitServer(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return;
            }

            var ss3Server = new ScreepsServer(input);
            ss3Server.Address.HostName = input;
            ss3Server.Address.Port = "21025";

            // split/parse http url and port and assign properly e.g. http://screeps.reggaemuffin.me:21025
            var urlPattern =
                @"(?<protocol>http(?:s?))?(?:\:\/\/)?(?<hostname>(?:[\w]+\.)+[a-zA-Z]+)(?::(?<port>\d{1,5}))?";
            var match = Regex.Match(input, urlPattern);

            if (match.Success)
            {

                var protocol = match.Groups["protocol"].Value;
                var hostname = match.Groups["hostname"].Value;
                var port = match.Groups["port"].Value;

                

                if (!string.IsNullOrEmpty(hostname))
                {
                    ss3Server.Address.HostName = hostname;
                }

                if (protocol.ToLowerInvariant() == "https" || port == "443")
                {
                    port = "443";

                    ss3Server.Address.Ssl = true;
                }

                if (!string.IsNullOrEmpty(port))
                {
                    ss3Server.Address.Port = port;
                }
            }
            else
            {
                // inform the player that adding it failed.
            }

            SS3UnifiedCredentials.SaveServer(ss3Server);

            _servers.Add(ss3Server);

            OnServerChange(_servers.IndexOf(ss3Server));

            UpdateServerList();
        }

        private void OnServerSelected(IScreepsServer server)
        {
            QueryAndUpdateServerInfo(server);

            selectedServer = server;

            int serverIndex = _servers.IndexOf(server);
            //_serverSelect.value = serverIndex; // Updates dropdown
            OnServerChange(serverIndex);
        }

        private void OnServerChange(int serverIndex)
        {
            UpdateServerList();

            editServer = false;
            PlayerPrefs.SetInt("serverIndex", serverIndex); // TODO: persist server key instead
            _serverIndex = serverIndex;
            UpdateFieldVisibility();
        }

        private void SS3UnifiedCredentialsFileLocationSelected()
        {
            _chooseSS3UnifiedCredentialsFileLocationPopup?.gameObject?.SetActive(false);
            StartCoroutine(LoadServers());
        }

        private void UpdateFieldVisibility()
        {
            if (selectedServer == null)
            {
                _removeServer.gameObject.SetActive(false);
                return;
            }

            _removeServer.gameObject.SetActive(!selectedServer.Official);
        }

        private bool HasUnifiedCredentials()
        {
            try
            {
                var ss3ConfigPath = SS3UnifiedCredentials.GetScreepsConfigFilePath();

                if (ss3ConfigPath != null)
                {
                    return true;
                }
            }
            catch (FileNotFoundException)
            {
                return false;
            }
            catch (Exception)
            {
                throw;
            }

            return false;
        }

        private IEnumerator LoadServers()
        {
            _servers = new List<IScreepsServer>();

            foreach (var provider in serverListProviders)
            {
                var finishedLoading = false;

                provider.Load(servers =>
                {
                    //Debug.LogError($"{provider.GetType()}");
                    foreach (var server in servers)
                    {
                        if (provider.MergeWithCache)
                        {
                            // TODO: a "display name" or the "name" property in the yaml file can be used to merge the different providers

                            //Debug.LogError($"{server.Name} => {server.Address.Http()}");

                            if (!server.HasCredentials)
                            {
                                // Official community server does not have any credentials, we can however update entries from SS3 with the server name
                                var existingServer = _servers.FirstOrDefault(s => s.HasCredentials
                                && s.Address.HostName == server.Address.HostName
                                && s.Address.Path == server.Address.Path
                                && s.Address.Port == server.Address.Port);


                                if (existingServer == null)
                                {
                                    _servers.Add(server);
                                }
                                else
                                {

                                    //Debug.LogError($"{server.Name} => {server.Address.Http()} ==> {existingServer.Address.Http()}");

                                    existingServer.Name = server.Name;
                                    existingServer.Meta.LikeCount = server.Meta.LikeCount;

                                    // Update credentials
                                    if (!string.IsNullOrEmpty(server.Credentials.Token))
                                    {
                                        existingServer.Credentials.Token = server.Credentials.Token;
                                    }

                                    if (!string.IsNullOrEmpty(server.Credentials.Email))
                                    {
                                        existingServer.Credentials.Email = server.Credentials.Email;
                                    }

                                    if (!string.IsNullOrEmpty(server.Credentials.Password))
                                    {
                                        existingServer.Credentials.Password = server.Credentials.Password;
                                    }
                                }
                            }
                            else
                            {
                                // Add as a new server
                                _servers.Add(server);
                            }
                        }
                        else
                        {
                            _servers.AddRange(servers);
                            // TODO: server icon
                        }

                        QueryAndUpdateServerInfo(server);
                    }

                    

                    _servers = _servers.OrderByDescending(s => s.Official)
                        .ThenByDescending(s => s.Meta.LikeCount)
                        .ThenBy(s => s.Address.Path)
                        .ThenBy(s => s.Address.HostName).ToList();

                    // preselecting selected server might be an issue when the selected server status is not saved for like SS3
                    //_serverIndex = sortedCache.FindIndex(s => s.Selected);

                    

                    finishedLoading = true;
                });

                while (!finishedLoading)
                {
                    yield return new WaitForSeconds(1);
                }
            }

            UpdateServerList();

        }

        private void QueryAndUpdateServerInfo(IScreepsServer server)
        {
            // Get status of servers, should probably be async for each server and a coroutine.
            // Need to double wrap it to keep a reference to the server

            server.Online = null;

            Action<string> queryServerInfoCallback = str =>
            {
                UpdateServerVersionInfo(server, str);

                // TODO: get world status, that does require us to have credentials available to lookup

                // Do we have admin utils?
                // TODO: /stats, if we have a username in credentials we can find our user stats, if we have token/password we could user other endpoints and just auth, 
                // leaning mostly towards /stats for private servers, and /api/user/overview for official servers

                // TODO: cache our badge? /api/auth/me gives a lot of details, requires us to auth though, and we don't get owned rooms, we do get cpu allocated to shards and their shardnames, as well as credits and resources
                // /api/user/rooms?id={userId} gives us owned rooms but requires us to have the user id

                // TODO: message count?

                // TODO: a tooltip that shows features / welcome message on hover or on click

                UpdateServerList();
            };

            Action queryServerInfoErrorCallback = () =>
            {
                server.Online = false;

                UpdateServerList();
            };

            var stuff = ScreepsAPI.Http.GetVersion(queryServerInfoCallback, queryServerInfoErrorCallback, server, noNotification: true);
            //stuff.Current

            if (server.Credentials.HasCredentials)
            {
                Action<string> queryAuthMeCallback = str =>
                {
                    UpdateServerAuthInfo(server, str);

                    UpdateServerList();
                };

                ScreepsAPI.Http.GetUser(queryAuthMeCallback, server, noNotification: true);

                Action<string> queryWorldStatusCallback = str =>
                {
                    var obj = new JSONObject(str);
                    if (Enum.TryParse<WorldStatus>(obj["status"].str, true, out var worldStatus)) { 
                        server.Meta.WorldStatus = worldStatus;
                    }

                    UpdateServerList();
                };

                ScreepsAPI.Http.GetWorldStatus(queryWorldStatusCallback, server, noNotification: true);
            }
        }
        
        private static void UpdateServerAuthInfo(IScreepsServer server, string str)
        {
            var obj = new JSONObject(str);
            var me = ScreepsAPI.UserManager.CacheUser(obj);
            server.Meta.Me = me;

            var GCL_POW = 2.4;
            var GCL_MULTIPLY = 1000000;

            var gclObject = obj["gcl"];
            if (gclObject != null && !gclObject.IsNull)
            {
                var gcl = (int)obj["gcl"].n;
                var gclLevel = Math.Floor(Math.Pow((gcl) / GCL_MULTIPLY, 1 / GCL_POW)) + 1;
                server.Meta.GlobalControlLevel = gclLevel;
            }
        }

        private static void UpdateServerVersionInfo(IScreepsServer server, string str)
        {
            // {"ok":1,"package":159,"protocol":13,"serverData":{"historyChunkSize":100,"shards":["shard0","shard1","shard2","shard3"]},"users":1606}
            var obj = new JSONObject(str);
            var package = obj["package"]; // MMO
            var packageVersion = obj["packageVersion"]; // Private Server
            var users = Convert.ToInt32(obj["users"].n);
            var serverData = obj["serverData"];

            if (serverData != null && !serverData.IsNull)
            {
                // screeps-admin-utils adds shards, default server does not have it
                var shards = serverData["shards"];

                if (shards != null && !shards.IsNull)
                {
                    server.Meta.ShardNames.Clear();
                    foreach (var shard in shards.list)
                    {
                        if (!shard.IsNull)
                        {
                            server.Meta.ShardNames.Add(shard.str);
                        }
                    }
                }

                if (server.Meta.ShardNames.Count == 0)
                {
                    // if server does not have a shardname set, version seems to return null
                    server.Meta.ShardNames.Add("shard0");
                }

                var features = serverData["features"];
                if (features != null && !features.IsNull)
                {
                    server.Meta.Features.Clear();
                    foreach (var feature in features.list)
                    {
                        if (!feature.IsNull)
                        {
                            server.Meta.Features.Add(feature["name"].ToString().Trim('"'), feature["version"].ToString().Trim('"'));
                        }
                    }
                }
            }

            server.Online = true;
            // TODO: timestamp of online status?
            server.Meta.Users = users;
            server.Meta.Version = "v" + (packageVersion != null ? packageVersion.str : package.n.ToString());
        }

        private void UpdateServerList()
        {
            if (_serverListTableViewController != null)
            {
                _serverListTableViewController.UpdateServerList(_servers);
            }

            UpdateFieldVisibility();
        }

        private void OnConnect()
        {
            var server = _servers[_serverIndex];
            
            QueryAndUpdateServerInfo(server);

            if (!server.HasCredentials)
            {
                ShowEditServerDialog(); // TODO: supply the fact that the server is in "Edit Credentials" mode, only connect after pressing OK
            }
            else
            {
                NotifyText.Message("Connecting...");
                _api.Connect(server);
            }
        }
    }
}