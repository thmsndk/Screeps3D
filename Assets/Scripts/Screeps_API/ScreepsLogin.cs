using System;
using System.Collections.Generic;
using System.Linq;
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
        public Action<Credentials, Address> OnSubmit;
        public string secret = "abc123";
        private CacheList _servers;
        private int _serverIndex;
        private string _savePath = "servers";

        private ServerListTableViewController _serverListTableViewController;

        private void Start()
        {
            GameManager.OnModeChange += OnModeChange;
            
            LoadCache();
            //UpdateServerDropdown();
            UpdateFieldVisibility();
            UpdateFieldContent();
            
            _connect.onClick.AddListener(OnClick);
            _serverSelect.onValueChanged.AddListener(OnServerChange);
            _addServer.onClick.AddListener(OnAddServer);
            _removeServer.onClick.AddListener(OnRemoveServer);

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

        private void OnRemoveServer()
        {
            if (_serverIndex == 0)
                return;
            
            _servers.RemoveAt(_serverIndex);
            OnServerChange(_serverIndex - 1);
            UpdateServerList();
            SaveManager.Save(_savePath, _servers);
        }

        private void UpdateServerDropdown()
        {
            _serverSelect.ClearOptions();
            var options = new List<TMP_Dropdown.OptionData>();
            foreach (var server in _servers)
            {
                options.Add(new TMP_Dropdown.OptionData(string.Format("{0} {1}", 
                    server.Name ?? server.Address.HostName, 
                    server.LikeCount > 0 ? string.Format("({0} Likes)",server.LikeCount) : string.Empty)));
            }
            _serverSelect.AddOptions(options);
            _serverSelect.value = _serverIndex;
        }

        private void OnAddServer()
        {
            PlayerInput.Get("Server Hostname\n<size=12>example: 127.0.0.1</size>", OnSubmitServer);
        }

        private void OnSubmitServer(string hostName)
        {
            if (hostName == null)
                return;
            
            var server = new ServerCache();
            server.Address.HostName = hostName;
            _servers.Add(server);
            OnServerChange(_servers.IndexOf(server));
            UpdateServerList();
            SaveManager.Save(_savePath, _servers);
        }

        private void OnServerSelected(ServerCache server)
        {
            int serverIndex = _servers.IndexOf(server);
            _serverSelect.value = serverIndex;
            OnServerChange(serverIndex);
            
        }

        private void OnServerChange(int serverIndex)
        {
            PlayerPrefs.SetInt("serverIndex", serverIndex);
            _serverIndex = serverIndex;
            UpdateFieldVisibility();
            UpdateFieldContent();
        }

        private void UpdateFieldVisibility()
        {
            var selectedServer = _servers[_serverIndex];
            var isPublic = selectedServer.MMO;

            //_ssl.gameObject.SetActive(!isPublic);
            //_port.gameObject.SetActive(!isPublic);

            var showCredentialInput = string.IsNullOrEmpty(!isPublic ? selectedServer.Credentials.Email : selectedServer.Credentials.Token);

            _username.gameObject.SetActive(!isPublic && showCredentialInput);
            _password.gameObject.SetActive(!isPublic && showCredentialInput);
            _token.gameObject.SetActive(isPublic && showCredentialInput);

            _removeServer.gameObject.SetActive(!selectedServer.MMO);

            
        }

        private void UpdateFieldContent()
        {
            var cache = _servers[_serverIndex];
            _port.text = cache.Address.Port ?? "";
            _username.text = cache.Credentials.Email ?? "";
            _token.text = cache.Credentials.Token ?? "";
            _password.text = cache.Credentials.Password ?? "";
            _ssl.isOn = cache.Address.Ssl;
            _save.isOn = cache.SaveCredentials;
        }

        private void LoadCache()
        {
            _servers = SaveManager.Load<CacheList>(_savePath); //TODO: we are loading cached terrain for ALL servers? seems like something that should be loaded when connecting to the selected server
            if (_servers == null)
            {
                _servers = new CacheList();
                var publicServer = new ServerCache();
                publicServer.MMO = true;
                publicServer.Name = "Screeps.com";
                publicServer.Address.HostName = "Screeps.com";
                publicServer.Address.Ssl = true;
                _servers.Add(publicServer);
            }

            var ptr = _servers.SingleOrDefault(cache => cache.MMO && cache.Address.HostName == "Screeps.com/ptr");
            if (ptr == null)
            {
                var publicServer = new ServerCache();
                publicServer.MMO = true;
                publicServer.Name = "PTR Screeps.com";
                publicServer.Address.HostName = "screeps.com";
                publicServer.Address.Ssl = true;
                publicServer.Address.Path = "/ptr";
                _servers.Add(publicServer);
            }

            var sortedCache = new CacheList();
            sortedCache.AddRange(_servers.OrderByDescending(s => s.MMO).ThenBy(s => s.Address.Path).ThenBy(s => s.Address.HostName));
            _servers = sortedCache;
            

            // Fetch servers from other sources
            // If we just append theese servers to the list, when the server list is saved, they will suddenly appear twice, 
            // but we still want to cache the server terrain for next time we connect.
            // TODO: move to a "generic" "server load" component, that can have different ways of getting servers.
            Action<string> serverCallback = str =>
            {
                var obj = new JSONObject(str);
                var servers = obj["servers"].list;

                var cachedOfficialServers = new List<ServerCache>();
                foreach (var server in servers)
                {
                    var name = server["name"].str;
                    var status = server["status"].str;
                    var likeCount = Convert.ToInt32(server["likeCount"].n);

                    var settings = server["settings"];
                    var host = settings["host"].str;
                    var port = settings["port"].str;

                    var cachedServer = _servers.SingleOrDefault(cache => cache.Address.HostName == host);
                    if (cachedServer == null)
                    {
                        cachedServer = new ServerCache();
                        cachedServer.Address.HostName = host;
                        cachedServer.Address.Port = port;
                        //officialServerListServer.Address.Ssl = true; // not sure how to determine if ssl or not
                        cachedOfficialServers.Add(cachedServer);
                    }

                    cachedServer.Name = name;
                    cachedServer.LikeCount = likeCount;

                    if (cachedServer.Address.HostName.EndsWith(".screepspl.us"))
                    {
                        // WebSocketSharp has issues connecting to SSL
                        //cachedServer.Address.Ssl = true;
                        //cachedServer.Address.Port = "443";
                        cachedServer.Address.Ssl = false;
                        cachedServer.Address.Port = port;
                    }
                }

                _servers.AddRange(cachedOfficialServers.OrderByDescending(s => s.LikeCount));

                // TODO: likes
                // all of this and the above needs to be wrapped in a coroutine that does not finish before everything is fetched.
                UpdateServerList();
            };

            

            var officialServer = _servers.SingleOrDefault(s => s.Address.HostName == "Screeps.com");
            if (officialServer != null)
            {
                // convert database
                officialServer.MMO = true;

                if (!string.IsNullOrEmpty(officialServer.Credentials.Token))
                {
                    ScreepsAPI.Cache = officialServer; // Allow calling api endpoint without having connected.
                    ScreepsAPI.Http.GetServerList(serverCallback);
                    
                }
            }


            // TODO: SS3 Unified Credentials File .yml
            // TODO: SS3 Unified Credentials File .ini

            // Get status of servers, should probably be async for each server and a coroutine.
            // TODO: I really feel this parsing of the response belongs inside the api 🤔

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

                    var cachedServer = _servers.SingleOrDefault(cache => cache.Address.HostName == server.Address.HostName);
                    if (cachedServer != null)
                    {
                        cachedServer.Online = true;
                        // TODO: timestamp of online status?
                        cachedServer.Users = users;
                        cachedServer.Version = "v"+ (cachedServer.MMO ? package.n.ToString() : packageVersion.str);
                    }

                    UpdateServerList();
                };

                Action queryServerInfoErrorCallback = () =>
                {
                    var cachedServer = _servers.SingleOrDefault(cache => cache.Address.HostName == server.Address.HostName);
                    if (cachedServer != null)
                    {
                        cachedServer.Online = false;
                    }

                    UpdateServerList();
                };
                // TODO: silent, make it silent so no notification is made on timeout.
                var stuff = ScreepsAPI.Http.GetVersion(queryServerInfoCallback, queryServerInfoErrorCallback);
                //stuff.Current
            };

            var currentAPIServer = ScreepsAPI.Cache;
            foreach (var server in _servers)
            {
                queryServerInfo(server);
            }

            ScreepsAPI.Cache = currentAPIServer;
        }

        private void UpdateServerList()
        {
            if (_serverListTableViewController != null)
            {
                _serverListTableViewController.UpdateServerList(_servers);
            }

            UpdateFieldVisibility();
        }

        private void OnClick()
        {
            var cache = _servers[_serverIndex];
            cache.SaveCredentials = _save.isOn;
            //cache.Address.Port = _port.text;
            //cache.Address.Ssl = _ssl.isOn;
            
            cache.SaveCredentials = _save.isOn;
            if (cache.SaveCredentials)
            {
                cache.Credentials.Email = _username.text;
                cache.Credentials.Password = _password.text;
                cache.Credentials.Token = _token.text;
            }

            // TODO: When saving servers, we do not wish to persist servers we've gotten from third party sources, 
            // UNLESS we have saved credentials for them that we did not get from the third party source.
            // If we however already have credentials from the third party source, then we don't want to save it either.

            // TODO: We also wish to load the terrain cache from disk when we connect to a server.

            // Sources column
            // Official, UCF, Custom

            // TODO: look into SSL

            SaveManager.Save(_savePath, _servers);
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
    
    [Serializable]
    public class CacheList : List<ServerCache> { } 
    // I'm not sure why it is necessary to use this class rather than just the list, but the binary formatter
    // seems to require it
}