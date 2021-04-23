using System;
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
        [SerializeField] private Toggle _save = default;
        [SerializeField] private Toggle _ssl = default;
        [SerializeField] private TMP_InputField _port = default;
        [SerializeField] private TMP_InputField _username = default;
        [SerializeField] private TMP_InputField _password = default;
        [SerializeField] private TMP_InputField _token = default;
        [SerializeField] private TMP_Dropdown _serverSelect = default;
        [SerializeField] private Button _connect = default;
        [SerializeField] private FadePanel _panel = default;
        [SerializeField] private Button _addServer = default;
        [SerializeField] private Button _removeServer = default;
        [SerializeField] private Button _editServer = default;
        [SerializeField] private Button _exit = default;
        [SerializeField] private ChooseSS3UnifiedCredentialsFileLocationPopup _chooseSS3UnifiedCredentialsFileLocationPopup = default;
        public Action<Credentials, Address> OnSubmit;
        public string secret = "abc123";

        private List<IScreepsServer> _servers;
        private int _serverIndex;
        private string _savePath = "servers";

        private ServerListTableViewController _serverListTableViewController;

        private List<IServerListProvider> serverListProviders = new List<IServerListProvider>();

        private bool editServer = false;

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
                LoadServers();
                UpdateFieldVisibility();
                UpdateFieldContent();
            }
            else
            {
                _chooseSS3UnifiedCredentialsFileLocationPopup.OnOkClicked += SS3UnifiedCredentialsFileLocationSelected;
                _chooseSS3UnifiedCredentialsFileLocationPopup?.gameObject?.SetActive(true);
            }

            _connect.onClick.AddListener(OnConnect);
            _serverSelect.onValueChanged.AddListener(OnServerChange);
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

            int serverIndex = _servers.IndexOf(server);
            //_serverSelect.value = serverIndex; // Updates dropdown
            OnServerChange(serverIndex);
        }

        private void OnServerChange(int serverIndex)
        {
            // TODO: selection in server list?
            if (_serverIndex != -1)
            {
                // deselect previous server
                var previousServer = _servers[_serverIndex];
                if (previousServer != null)
                {
                    //previousServer.Selected = false;
                }
            }

            // select new server
            var selectedServer = _servers[serverIndex];
            if (selectedServer != null)
            {
                //selectedServer.Selected = true;
            }

            UpdateServerList();

            editServer = false;
            PlayerPrefs.SetInt("serverIndex", serverIndex);
            _serverIndex = serverIndex;
            UpdateFieldVisibility();
            UpdateFieldContent();
        }

        private void SS3UnifiedCredentialsFileLocationSelected()
        {
            _chooseSS3UnifiedCredentialsFileLocationPopup?.gameObject?.SetActive(false);
            LoadServers();
        }

        private void UpdateFieldVisibility()
        {
            if (_serverIndex == -1 || _servers == null)
            {
                _username.gameObject.SetActive(false);
                _password.gameObject.SetActive(false);
                _token.gameObject.SetActive(false);

                _removeServer.gameObject.SetActive(false);
                return;
            }

            var selectedServer = _servers[_serverIndex];
            var usesTokens = selectedServer.Official;

            //_ssl.gameObject.SetActive(!isPublic);
            //_port.gameObject.SetActive(!isPublic);

            var showCredentialInput =
                string.IsNullOrEmpty(!usesTokens ? selectedServer.Credentials.Email : selectedServer.Credentials.Token) ||
                editServer;

            _username.gameObject.SetActive(!usesTokens && showCredentialInput);
            _password.gameObject.SetActive(!usesTokens && showCredentialInput);
            _token.gameObject.SetActive(usesTokens && showCredentialInput);

            _removeServer.gameObject.SetActive(!selectedServer.Official);

            if (!usesTokens && (string.IsNullOrEmpty(selectedServer.Address.Port) || editServer))
            {
                _port.gameObject.SetActive(true);
            }
            else
            {
                _port.gameObject.SetActive(false);
            }
        }

        private void UpdateFieldContent()
        {
            if (_serverIndex == -1)
            {
                return;
            }

            var server = _servers[_serverIndex];
            _port.text = server.Address.Port ?? "21025";
            _username.text = server.Credentials.Email ?? "";
            _token.text = server.Credentials.Token ?? "";
            _password.text = server.Credentials.Password ?? "";
            _ssl.isOn = server.Address.Ssl;
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

        private void LoadServers()
        {
            _servers = new List<IScreepsServer>();

            foreach (var provider in serverListProviders)
            {
                provider.Load(servers =>
                {
                    //Debug.LogError($"{provider.GetType()}");
                    foreach (var server in servers)
                    {
                        if (provider.MergeWithCache)
                        {
                            // TODO: a "display name" or the "name" property in the yaml file can be used to merge the different providers

                            if (!server.HasCredentials)
                            {
                                var existingServer = _servers.FirstOrDefault(server => server.HasCredentials
                                && server.Address.HostName == server.Address.HostName
                                && server.Address.Path == server.Address.Path
                                && server.Address.Port == server.Address.Port);

                                //Debug.LogError($"{server.Name} => {server.Address.Http()}");

                                if (existingServer == null)
                                {
                                    _servers.Add(server);
                                }
                                else
                                {
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

                    UpdateServerList();
                });
            }
        }

        private void QueryAndUpdateServerInfo(IScreepsServer server)
        {
            // Get status of servers, should probably be async for each server and a coroutine.
            // Need to double wrap it to keep a reference to the server
            ScreepsAPI.Server = server; // TODO: Currently all ScreepsAPI.Http calls utilize this server property, we need a ScreepsAPI.Http(server).GetVersion... ability
            server.Online = null;
            Action<string> queryServerInfoCallback = str =>
            {
                UpdateServerVersionInfo(server, str);
                UpdateServerList();
            };

            Action queryServerInfoErrorCallback = () =>
            {
                server.Online = false;

                UpdateServerList();
            };

            var stuff = ScreepsAPI.Http.GetVersion(queryServerInfoCallback, queryServerInfoErrorCallback, noNotification: true);
            //stuff.Current
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

                server.Meta.ShardNames = new List<string>();
                if (shards != null && !shards.IsNull)
                {
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
            }

            server.Online = true;
            // TODO: timestamp of online status?
            server.Meta.Users = users;
            server.Meta.Version = "v" + (package != null ? package.n.ToString() : packageVersion.str);
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
            var cache = _servers[_serverIndex];
            
            QueryAndUpdateServerInfo(cache);

            // TODO: only persist on connect, if save credentials is marked.


            // TODO: persist server info / meta data to a file. mainly containing data from api/version endpoint. likes, shard, last online status and so forth.
            // TODO: persist last connection date
            // TODO: meta data could also contain what shard you where on last time you connected, what room you where loaded into. Will we use PlayerPrefs for meta data?

            // TODO: handle no SS3 credentials file existing, popping up a dialog allowing the user to choose where to save it
            //cache.SaveCredentials = _save.isOn;
            //cache.Address.Port = _port.text;
            //cache.Address.Ssl = _ssl.isOn;

            //if (cache.SaveCredentials)
            //{
                cache.Credentials.Email = _username.text;
                cache.Credentials.Password = _password.text;
                cache.Credentials.Token = _token.text;
            //}

            // TODO: persist credentials to the SS3 credentials file
            //var filteredServers = new CacheList();
            //filteredServers.AddRange(_servers.Where(s => s.HasCredentials && s.Persist));
            
            //SaveManager.Save(_savePath, filteredServers);
            NotifyText.Message("Connecting...");
            _api.Connect(cache);
        }
    }
}