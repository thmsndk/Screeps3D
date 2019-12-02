using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Assets.Scripts.Screeps_API.ServerListProviders;
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
        [SerializeField] private ScreepsAPI _api;
        [SerializeField] private Toggle _save;
        [SerializeField] private Toggle _ssl;
        [SerializeField] private TMP_InputField _port;
        [SerializeField] private TMP_InputField _username;
        [SerializeField] private TMP_InputField _password;
        [SerializeField] private TMP_InputField _token;
        [SerializeField] private TMP_Dropdown _serverSelect;
        [SerializeField] private Button _connect;
        [SerializeField] private FadePanel _panel;
        [SerializeField] private Button _addServer;
        [SerializeField] private Button _removeServer;
        [SerializeField] private Button _editServer;
        public Action<Credentials, Address> OnSubmit;
        public string secret = "abc123";
        private CacheList _servers;
        private int _serverIndex;
        private string _savePath = "servers";

        private ServerListTableViewController _serverListTableViewController;

        private List<IServerListProvider> serverListProviders = new List<IServerListProvider>();

        private bool editServer = false;

        private void Start()
        {
            GameManager.OnModeChange += OnModeChange;
            serverListProviders.Add(new OfficialServerListProvider());
            serverListProviders.Add(new OfficialCommunityServerListProvider());
            // TODO: SS3 Unified Credentials File .yml
            // TODO: SS3 Unified Credentials File .ini
            // https://screeps.online/ ?

            LoadServers();
            //UpdateServerDropdown();
            UpdateFieldVisibility();
            UpdateFieldContent();

            _connect.onClick.AddListener(OnConnect);
            _serverSelect.onValueChanged.AddListener(OnServerChange);
            _addServer.onClick.AddListener(OnAddServer);
            _removeServer.onClick.AddListener(OnRemoveServer);
            _editServer.onClick.AddListener(OnEditServer);

            _serverListTableViewController = gameObject.GetComponent<ServerListTableViewController>();

            _serverListTableViewController.onServerSelected.AddListener(OnServerSelected);
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

            var server = new ServerCache
                {Type = SourceProviderType.Custom, Address = {HostName = input, Port = "21025"}};

            // split/parse http url and port and assign properly e.g. http://screeps.reggaemuffin.me:21025
            var urlPattern =
                @"(?<protocol>http(?:s?))?(?:\:\/\/)?(?<hostname>(?:[\w]+\.)+[a-zA-Z]+)(?::(?<port>\d{1,5}))?";
            var match = Regex.Match(input, urlPattern);

            if (match.Success)
            {
                var protocol = match.Groups["protocol"].Value;
                var hostName = match.Groups["hostname"].Value;
                var port = match.Groups["port"].Value;

                if (!string.IsNullOrEmpty(hostName))
                {
                    server.Address.HostName = hostName;
                }

                if (protocol.ToLowerInvariant() == "https" || port == "443")
                {
                    port = "443";
                    server.Address.Ssl = true;
                }

                if (!string.IsNullOrEmpty(port))
                {
                    server.Address.Port = port;
                }
            }

            _servers.Add(server);
            OnServerChange(_servers.IndexOf(server));
            UpdateServerList();
            SaveManager.Save(_savePath, _servers);
        }

        private void OnServerSelected(ServerCache server)
        {
            int serverIndex = _servers.IndexOf(server);
            //_serverSelect.value = serverIndex; // Updates dropdown
            OnServerChange(serverIndex);
        }

        private void OnServerChange(int serverIndex)
        {
            if (_serverIndex != -1)
            {
                // deselect previous server
                var previousServer = _servers[_serverIndex];
                if (previousServer != null)
                {
                    previousServer.Selected = false;
                }
            }

            // select new server
            var selectedServer = _servers[serverIndex];
            if (selectedServer != null)
            {
                selectedServer.Selected = true;
            }

            UpdateServerList();

            editServer = false;
            PlayerPrefs.SetInt("serverIndex", serverIndex);
            _serverIndex = serverIndex;
            UpdateFieldVisibility();
            UpdateFieldContent();
        }

        private void UpdateFieldVisibility()
        {
            if (_serverIndex == -1)
            {
                _username.gameObject.SetActive(false);
                _password.gameObject.SetActive(false);
                _token.gameObject.SetActive(false);

                _removeServer.gameObject.SetActive(false);
                return;
            }

            var selectedServer = _servers[_serverIndex];
            var isPublic = selectedServer.Official;

            //_ssl.gameObject.SetActive(!isPublic);
            //_port.gameObject.SetActive(!isPublic);

            var showCredentialInput =
                string.IsNullOrEmpty(!isPublic ? selectedServer.Credentials.Email : selectedServer.Credentials.Token) ||
                editServer;

            _username.gameObject.SetActive(!isPublic && showCredentialInput);
            _password.gameObject.SetActive(!isPublic && showCredentialInput);
            _token.gameObject.SetActive(isPublic && showCredentialInput);

            _removeServer.gameObject.SetActive(!selectedServer.Official);

            if (!isPublic && (string.IsNullOrEmpty(selectedServer.Address.Port) || editServer))
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

            var cache = _servers[_serverIndex];
            _port.text = cache.Address.Port ?? "21025";
            _username.text = cache.Credentials.Email ?? "";
            _token.text = cache.Credentials.Token ?? "";
            _password.text = cache.Credentials.Password ?? "";
            _ssl.isOn = cache.Address.Ssl;
            _save.isOn = cache.SaveCredentials;
        }

        private void LoadServers()
        {
            //SaveManager.Save(_savePath, new CacheList()); // clear servers

            _servers = SaveManager.Load<CacheList>(_savePath) ?? new CacheList();


            // Get status of servers, should probably be async for each server and a coroutine.
            // Need to double wrap it to keep a reference to the server
            Action<ServerCache> queryServerInfo = server =>
            {
                ScreepsAPI.Cache = server;
                server.Online = null;
                Action<string> queryServerInfoCallback = str =>
                {
                    // {"ok":1,"package":159,"protocol":13,"serverData":{"historyChunkSize":100,"shards":["shard0","shard1","shard2","shard3"]},"users":1606}
                    var obj = new JSONObject(str);
                    var package = obj["package"]; // MMO
                    var packageVersion = obj["packageVersion"]; // Private Server
                    var users = Convert.ToInt32(obj["users"].n);

                    server.Online = true;
                    // TODO: timestamp of online status?
                    server.Users = users;
                    server.Version = "v" + (server.Official ? package != null ? package.n.ToString() : string.Empty : packageVersion.str);
                    UpdateServerList();
                };

                Action queryServerInfoErrorCallback = () =>
                {
                    server.Online = false;

                    UpdateServerList();
                };

                var stuff = ScreepsAPI.Http.GetVersion(queryServerInfoCallback, queryServerInfoErrorCallback, noNotification: true);
                //stuff.Current
            };

            // query saved servers, should probably only query the ones where source == custom cause the providers will query the others
            foreach (var server in _servers)
            {
                queryServerInfo(server);
            }

            foreach (var provider in serverListProviders)
            {
                provider.Load(servers =>
                {
                    foreach (var server in servers)
                    {
                        if (provider.MergeWithCache)
                        {
                            var cachedServer = _servers.SingleOrDefault(cache =>
                                cache.Address.HostName == server.Address.HostName
                                && cache.Address.Path == server.Address.Path
                                && cache.Address.Port == server.Address.Port);

                            if (cachedServer == null)
                            {
                                _servers.Add(server);
                            }
                            else
                            {
                                
                                cachedServer.Name = server.Name;
                                cachedServer.LikeCount = server.LikeCount;

                                //Backwards compatibility
                                cachedServer.Official = server.Official;
                                cachedServer.Type = server.Type;
                            }
                        }
                        else
                        {
                            _servers.AddRange(servers);
                            // TODO: servers also need to be marked if they should be saved to the cachelist or not. e.g. SS3 should not be persisted, they already contain passwords
                            // TODO: server icon
                        }

                        queryServerInfo(server);
                    }

                    var sortedCache = new CacheList();
                    sortedCache.AddRange(_servers.OrderByDescending(s => s.Type == SourceProviderType.Official)
                        .ThenByDescending(s => s.LikeCount)
                        .ThenBy(s => s.Address.Path)
                        .ThenBy(s => s.Address.HostName));
                    _servers = sortedCache;

                    // preselecting selected server might be an issue when the selected server status is not saved for like SS3
                    _serverIndex = sortedCache.FindIndex(s => s.Selected);

                    UpdateServerList();
                });
            }
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
            cache.SaveCredentials = _save.isOn;
            //cache.Address.Port = _port.text;
            //cache.Address.Ssl = _ssl.isOn;

            if (cache.SaveCredentials)
            {
                cache.Credentials.Email = _username.text;
                cache.Credentials.Password = _password.text;
                cache.Credentials.Token = _token.text;
            }

            // TODO: When saving servers, we do not wish to persist servers we've gotten from third party sources, 
            // UNLESS we have saved credentials for them that we did not get from the third party source.
            // If we however already have credentials from the third party source, then we don't want to save it either.

            // Sources column
            // Official, UCF, Custom

            // TODO: look into SSL

            var filteredServers = new CacheList();
            filteredServers.AddRange(_servers.Where(s => s.HasCredentials));


            SaveManager.Save(_savePath, filteredServers);
            NotifyText.Message("Connecting...");
            _api.Connect(cache);
        }
    }

    [Serializable]
    public class Credentials
    {
        public string Token;
        public string Email;
        public string Password;
    }

    [Serializable]
    public class Address
    {
        public bool Ssl;
        public string HostName;
        public string Port;
        public string Path = "/";

        public string Http(string path = "")
        {
            if (path.StartsWith("/") && Path.EndsWith("/"))
            {
                path = path.Substring(1);
            }

            var protocol = Ssl ? "https" : "http";
            var port = HostName.ToLowerInvariant() == "screeps.com" ? "" : string.Format(":{0}", this.Port);
            var url = string.Format("{0}://{1}{2}{3}{4}", protocol, HostName, port, this.Path, path);
            //Debug.Log(url);
            return url;
        }
    }

    // The Binary Formatter checks for the serializable attribute, thus this workaround
    [Serializable]
    public class CacheList : List<ServerCache>
    {
    }
}