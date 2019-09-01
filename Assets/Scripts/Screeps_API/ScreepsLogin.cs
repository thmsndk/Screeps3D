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
        [SerializeField] private TMP_InputField _email;
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
            UpdateServerDropdown();
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
            UpdateServerDropdown();
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
            UpdateServerDropdown();
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
            var isPublic = _servers[_serverIndex].Address.HostName.ToLowerInvariant() == "screeps.com";
            _ssl.gameObject.SetActive(!isPublic);
            _port.gameObject.SetActive(!isPublic);
            _email.gameObject.SetActive(!isPublic);
            _password.gameObject.SetActive(!isPublic);
            _removeServer.gameObject.SetActive(_serverIndex != 0);
            _token.gameObject.SetActive(isPublic);
        }

        private void UpdateFieldContent()
        {
            var cache = _servers[_serverIndex];
            _port.text = cache.Address.Port ?? "";
            _email.text = cache.Credentials.Email ?? "";
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
                publicServer.Name = "Screeps.com";
                publicServer.Address.HostName = "Screeps.com";
                publicServer.Address.Ssl = true;
                _servers.Add(publicServer);
            }

            var sortedCache = new CacheList();
            sortedCache.AddRange(_servers.OrderByDescending(s => s.Address.HostName == "Screeps.com").ThenBy(s => s.Address.HostName));
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
                }

                _servers.AddRange(cachedOfficialServers.OrderByDescending(s => s.LikeCount));

                // TODO: likes
                _serverListTableViewController.UpdateServerList(_servers);

                UpdateServerDropdown();
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

        }

        private void OnClick()
        {
            var cache = _servers[_serverIndex];
            cache.SaveCredentials = _save.isOn;
            cache.Address.Port = _port.text;
            cache.Address.Ssl = _ssl.isOn;
            
            cache.SaveCredentials = _save.isOn;
            if (cache.SaveCredentials)
            {
                cache.Credentials.Email = _email.text;
                cache.Credentials.Password = _password.text;
                cache.Credentials.Token = _token.text;
            }

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
            // Debug.Log(string.Format("{0}://{1}{2}{3}{4}", protocol, hostName, port, this.path, path));
            return string.Format("{0}://{1}{2}{3}{4}", protocol, HostName, port, this.Path, path);
        }
    }
    
    [Serializable]
    public class CacheList : List<ServerCache> { } 
    // I'm not sure why it is necessary to use this class rather than just the list, but the binary formatter
    // seems to require it
}