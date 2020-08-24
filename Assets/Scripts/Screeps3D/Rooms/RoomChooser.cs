using Assets.Scripts.Common;
using Assets.Scripts.Screeps_API.ConsoleClientAbuse;
using Assets.Scripts.Screeps3D;
using Assets.Scripts.Screeps3D.Rooms.Views;
using Common;
using Screeps_API;
using Screeps3D.Player;
using Screeps3D.RoomObjects;
using Screeps3D.Tools.Selection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Screeps3D.Rooms
{
    public class RoomChooser : MonoBehaviour
    {
        public Action<Room> OnChooseRoom;

        [SerializeField] private TMP_Dropdown _shardInput = default;
        [SerializeField] private TMP_InputField _roomInput = default;
        [SerializeField] private Toggle _pvpSpectateToggle = default;
        [SerializeField] private Toggle _SpectateToggle = default;
        //[SerializeField] private GameObject _roomList = default;
        [SerializeField] private VerticalPanelElement _roomList = default;
        private bool showRoomList;
        [SerializeField] private GameObject _roomListContent = default;

        private readonly string _prefSpectateToggle = "SpectateToggle";
        private readonly string _prefPvpSpectateToggle = "PvpSpectateToggle";
        private readonly string _prefShard = "shard";
        private readonly string _prefRoom = "room";

        private List<string> _shards = new List<string>();
        private System.Random random;

        /// <summary>
        /// A list of owned room names on each shard
        /// </summary>
        private Dictionary<string, List<string>> _shardRooms = new Dictionary<string, List<string>>();

        private IEnumerator _findPvpRooms;
        private IEnumerator _findPlayerOwnedRooms;

        private bool pausedBecauseOfTwitchGoto = false;

        private void Start()
        {
            random = new System.Random();
            _pvpSpectateToggle.isOn = PlayerPrefs.GetInt(_prefPvpSpectateToggle, 1) == 1;
            _SpectateToggle.isOn = PlayerPrefs.GetInt(_prefSpectateToggle, 1) == 1;
            _shardInput.onValueChanged.AddListener(OnSelectedShardChanged);
            _roomInput.onSubmit.AddListener(OnSelectedRoomChanged);
            _roomInput.onSelect.AddListener(OnToggleRoomList);
            _roomInput.onDeselect.AddListener(OnToggleRoomList);
            _pvpSpectateToggle.onValueChanged.AddListener(OnTogglePvpSpectate);
            _SpectateToggle.onValueChanged.AddListener(OnToggleSpectate);

            if (ScreepsAPI.IsConnected)
            {
                ScreepsAPI.Http.GetRooms(ScreepsAPI.Me.UserId, InitializeChooser);
            }
            else
            {
                throw new Exception("RoomChooser assumes ScreepsAPI.IsConnected == true at start of scene");
            }

            _roomList.Hide();
        }

        private void Instance_OnGoToRoom(object sender, GoToRoomEventArgs e)
        {
            StartCoroutine(TwitchGotoRoom(e));
        }

        private IEnumerator TwitchGotoRoom(GoToRoomEventArgs e)
        {
            Debug.Log($"Twitch told me to go to {e.RoomName}");
            this.GetAndChooseRoom(e.RoomName);

            if (_pvpSpectateToggle.isOn)
            {
                // TODO: what if people constantly swap rooms?
                Debug.Log($"Pausing pvp spectate for {e.Seconds}s");
                pausedBecauseOfTwitchGoto = true;
                _pvpSpectateToggle.isOn = false;
                yield return new WaitForSeconds(e.Seconds);
                _pvpSpectateToggle.isOn = true;
                pausedBecauseOfTwitchGoto = false;
            }
        }

        private void OnTogglePvpSpectate(bool isOn)
        {
            PlayerPrefs.SetInt(_prefPvpSpectateToggle, isOn ? 1 : 0);

            if (isOn)
            {
                PlaceSpawnView.EnableOverlay = false;
                _findPvpRooms = FindPvpRoom();
                StartCoroutine(_findPvpRooms);
            }
            else
            {
                PlaceSpawnView.EnableOverlay = pausedBecauseOfTwitchGoto ? false : true;
                if (_findPvpRooms != null)
                {
                    StopCoroutine(_findPvpRooms);
                }
            }

            Debug.Log($"PlaceSpawnView.EnableOverlay {PlaceSpawnView.EnableOverlay}");
        }

        private void OnToggleSpectate(bool isOn)
        {
            PlayerPrefs.SetInt(_prefSpectateToggle, isOn ? 1 : 0);

            if (isOn)
            {
                PlaceSpawnView.EnableOverlay = false;
                // TODO: Find player owned rooms
                _findPlayerOwnedRooms = FindPlayerOwnedRoom();
                StartCoroutine(_findPlayerOwnedRooms);
            }
            else
            {
                PlaceSpawnView.EnableOverlay = true;
                if (_findPlayerOwnedRooms != null)
                {
                    StopCoroutine(_findPlayerOwnedRooms);
                }
            }
        }

        private IEnumerator FindPvpRoom()
        {

            while (true)
            {
                // How to get all rooms? :thinking:
                // Lets see if we can just get random room navigation to work, no clue how the experimental pvp stuff from ags works xD
                try
                {
                    // We need to ignore "walls", what determines a "Wall" ?
                    //var room = RoomManager.Instance.Rooms.ElementAt(random.Next(RoomManager.Instance.Rooms.Count()));
                    //this.GetAndChooseRoom(room.RoomName);

                    this.RoomSwap();
                }
                catch (Exception)
                {

                    throw;
                }

                // Wait for either the default 60 seconds or whatever value was passed in on the command args
                yield return new WaitForSeconds(CmdArgs.PvPTimerSwitch);
            }
        }

        private IEnumerator FindPlayerOwnedRoom()
        {
            while (true)
            {
                if (MapStatsUpdater.Instance.RoomInfo.TryGetValue(PlayerPosition.Instance.ShardName, out var shardRoomInfo))
                {
                    var ownedRooms = shardRoomInfo.Where(r => r.Value.User != null && r.Value.User.UserId == ScreepsAPI.Me.UserId).Select(r=> r.Value);

                    if (ownedRooms.Any())
                    {
                        var random = new System.Random();
                        var room = ownedRooms.ElementAt(random.Next(ownedRooms.Count()));
                        var roomName = room?.RoomName;

                        Debug.Log($"Going to room {roomName} owned by {room?.User?.Username}");
                        _roomInput.text = roomName;
                        this.GetAndChooseRoom(roomName);
                    }
                    else
                    {
                        Debug.Log($"Could not find any of your owned rooms :/");
                    }
                }

                // Wait for either the default 60 seconds or whatever value was passed in on the command args
                yield return new WaitForSeconds(CmdArgs.SpectateTimerSwitch);
            }
        }

        internal void GetAndChooseRandomWorldStartRoom()
        {
            var random = new System.Random();
            var room = ScreepsAPI.WorldStartRooms.ElementAt(random.Next(ScreepsAPI.WorldStartRooms.Count()));
            var shardAndRoom = room.Split('/');
            if (shardAndRoom.Length == 2)
            {
                room = shardAndRoom[1];
            }

            Debug.Log($"Going to random world start room {room}");
            GetAndChooseRoom(room);
        }

        // shamelessly "stolen" / given by ags131
        private void RoomSwap()
        {
            if (!ScreepsAPI.IsConnected)
            {
                return;
            }

            //ChooseRandomOwnedRoom();
            ChooseRoomWithPVPOrOwnedRoom();
        }

        private float _pvpSpectateBias = 0;
        private void ChooseRoomWithPVPOrOwnedRoom()
        {
            // loop rooms and add a list of rooms, mark the selected one bold
            // https://twitchtv.desk.com/customer/en/portal/articles/2884064-twitch-app-s-chat-message-formatting
            // the pvp list probably belongs in a twitch extension https://www.twitch.tv/p/extensions/
            // Still a little spammy with every 30 seconds, should probably collect pvp details in a warpath fashion and put the message on a "warpath" timer

            if (ScreepsAPI.Cache.Official)
            {
                //// requires screepsmod-admin-utils
                //https://botarena.screepspl.us/api/experimental/pvp?interval=100
                var body = new RequestBody();
                body.AddField("interval", "100");
                ScreepsAPI.Http.Request("GET", "/api/experimental/pvp", body, (jsonString) =>
                {
                    var obj = new JSONObject(jsonString);
                    var rooms = obj["pvp"][_shardInput.value]["rooms"].list;

                    rooms.Sort((a, b) =>
                    {
                        return (int)b.GetField("lastPvpTime").n - (int)a.GetField("lastPvpTime").n;
                    });

                    // TODO: get the room viewer to work, so it renders the room you have "selected"
                    if (rooms.Count > 0)
                    {
                        var room = rooms.ElementAt(random.Next(rooms.Count));
                        var roomName = room.GetField("_id").str;
                        _roomInput.text = roomName;
                        this.GetAndChooseRoom(roomName);
                    }
                    else
                    {
                        ChooseRandomOwnedRoom();
                    }


                    /*
                     * {
                        "ok": 1,
                        "time": 43584,
                        "pvp": {
                            "shardname": {
                                "rooms": [{
                                        "_id": "E1S7",
                                        "lastPvpTime": 43113
                                    }]
                                }
                            }
                        }
                     */
                });

            }
            else
            {
                // TODO: verify if the warpath feature is enabled, admin utils gives us this though, so we don't really need the experimental endpoint?
                // TODO: perhaps we need to still call the experimental endpoint initially to get an initial list to choose from?
                // Sorting inspired from screeps-cap https://github.com/ags131/screeps-cap/blob/bb9d3954fbd69992b1fa4532ecaf3fe6d797c650/index.js#L260-L261
                var rooms = ScreepsAPI.Warpath.Rooms
                    .OrderByDescending(r => ((int)r.Classification * 1000) - (ScreepsAPI.Time - r.LastPvpTime))
                    .Where(r => r.LastPvpTime > ScreepsAPI.Time - 20);

                if (rooms.Count() > 0)
                {
                    var index = Mathf.FloorToInt(random.Next(rooms.Count()) * Math.Min(_pvpSpectateBias, rooms.Count()));
                    Debug.Log($"warpath room index {index}");

                    if (index >= rooms.Count())
                    {
                        index -= 1;
                    }

                    var room = rooms.ElementAt(index);
                    if (PlayerPosition.Instance.RoomName == room.RoomName)
                    {
                        _pvpSpectateBias += 0.5f;
                    }
                    else
                    {
                        _pvpSpectateBias = 0;
                    }

                    var roomName = room.RoomName;
                    _roomInput.text = roomName;
                    Debug.Log($"Going to room {roomName} bias: {_pvpSpectateBias} Classification {room.Classification}, Defender: {room.Defender?.Username} , Attackers: {string.Join(",", room.Attackers.Select(a => a.Username))}");
                    var swappingRooms = false;
                    if (PlayerPosition.Instance.RoomName != roomName)
                    {
                        // Only swap room if it is a new one
                        swappingRooms = true;
                        this.GetAndChooseRoom(roomName);
                    }

                    var sb = new StringBuilder();
                    //var fontSize
                    var messageColor = Color.white;
                    switch (room.Classification)
                    {
                        case Warpath.Classification.Class2:
                        case Warpath.Classification.Class3:
                            messageColor = Color.yellow;
                            break;
                        case Warpath.Classification.Class4:
                        case Warpath.Classification.Class5:
                        case Warpath.Classification.Class6:
                            messageColor = Color.red;
                            break;
                        default:
                            break;
                    }

                    sb.Append("<size=20>");
                    if (swappingRooms)
                    {
                        sb.AppendLine($"Going to {room.RoomName}");
                    }
                    else
                    {
                        sb.AppendLine($"Staying in {room.RoomName}");
                    }

                    sb.AppendLine($"Class {(int)room.Classification}");
                    sb.AppendLine($"Defender {room.Defender?.Username}");
                    sb.AppendLine($"Attackers {string.Join(",", room.Attackers.Select(a => a.Username))}");
                    sb.Append("</size>");

                    NotifyText.Message(sb.ToString(), messageColor, 2.5f);
                }
                else
                {
                    ChooseRandomOwnedRoom();
                }

            }
        }

        private void ChooseRandomOwnedRoom()
        {
            if (!string.IsNullOrEmpty(PlayerPosition.Instance.ShardName) && MapStatsUpdater.Instance.RoomInfo.TryGetValue(PlayerPosition.Instance.ShardName, out var shardRoomInfo))
            {
                var ownedRooms = shardRoomInfo.Where(r => r.Value.User != null && r.Value.Level.HasValue && r.Value.Level > 0)
                    // remove invaders and source keepers
                    .Where(r => r.Value.User.UserId != Constants.InvaderUserId).Select(r => r.Value);

                if (ownedRooms.Any())
                {
                    var random = new System.Random();
                    var room = ownedRooms.ElementAt(random.Next(ownedRooms.Count()));
                    var roomName = room?.RoomName;

                    Debug.Log($"Going to room {roomName} owned by {room?.User?.Username}");
                    _roomInput.text = roomName;
                    this.GetAndChooseRoom(roomName);
                }
                else
                {
                    Debug.Log($"Could not find any owned rooms :/");
                }
            }
        }

        public void OnSelectedShardChanged(string shardName)
        {
            var shardIndex = _shardInput.options.FindIndex(s => s.text == shardName);
            _shardInput.value = shardIndex;
            ClearAndPropulateRoomList(shardName);
        }

        private void ClearAndPropulateRoomList(string shardName)
        {
            // clear available rooms
            Debug.Log($"clearing rooms {_roomListContent.transform.childCount}");
            foreach (Transform child in _roomListContent.transform)
            {
                child.SetParent(null);
                //child.parent = null;
                PoolLoader.Return("Prefabs/RoomList/roomName", child.gameObject);
            }

            var roomList = _shardRooms[shardName];
            foreach (var room in roomList)
            {
                AddRoomToRoomListGameObject(shardName, room);
            }

            // adjust height of content, cause content fitters and such apparently can't set it properly
            var rect = _roomListContent.transform.parent.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, Math.Max(roomList.Count * 6, 60));
        }

        public void OnSelectedShardChanged(int shardIndex)
        {
            PlayerPrefs.SetInt(GetServerPrefKey(_prefShard), shardIndex);
            ClearAndPropulateRoomList(_shards[shardIndex]);
        }

        public void OnSelectedRoomChanged(string roomName)
        {
            if (!string.IsNullOrEmpty(roomName))
            {
                PlayerPrefs.SetString(GetServerPrefKey(_prefRoom), roomName);
            }

            this.GetAndChooseRoom(roomName);
        }
        public void OnToggleRoomList(string roomName)
        {
            StartCoroutine(DelayToggleRoomList());
        }

        /// <summary>
        /// Delay room toggle to allow clicking a room link
        /// </summary>
        /// <returns></returns>
        private IEnumerator DelayToggleRoomList()
        {
            //if (_roomList.activeSelf){
            if (showRoomList)
            {
                yield return new WaitForSeconds(0.25f);
            }

            if (_roomListContent.transform.childCount > 0)
            {
                showRoomList = !showRoomList;
                _roomList.Show(showRoomList);
                //_roomList.SetActive(!_roomList.activeSelf);
            }
        }

        public void GetAndChooseRoom(string roomName)
        {
            // _roomInput.text
            var room = RoomManager.Instance.Get(roomName, _shards[_shardInput.value]);
            if (room == null)
            {
                Debug.Log("invalid room");
                return;
            }

            CameraRig.Instance.OnTargetReached += OnTargetReached;

            if (OnChooseRoom != null) OnChooseRoom.Invoke(room);


        }

        private void OnTargetReached()
        {
            var room = PlayerPosition.Instance.Room;
            Debug.Log("target reached");
            if (room != null)
            {
                Debug.Log("and we have a room!");
                room.OnShowObjects += RoomShown;

                void RoomShown(bool show)
                {
                    Debug.Log($"{room.Name} should be shown {show}");
                    if (show)
                    {
                        room.RoomUnpacker.OnUnpack += RoomUnpacked;
                    }
                }
            }

            CameraRig.Instance.OnTargetReached -= OnTargetReached;
        }


        private void RoomUnpacked(Room room, JSONObject roomData)
        {
            Debug.Log($"{room.Name} should be unpacked we have {room.Objects.Count} objects");

            var controller = room.Objects.SingleOrDefault(ro => ro.Value.Type == Constants.TypeController);
            if (controller.Value != null)
            {
                //Debug.Log("and a controller!");
                //controller.Value.OnShow += SelectOnShow;
                Selection.Instance.SelectObject(controller.Value);
            }

            var storage = room.Objects.SingleOrDefault(ro => ro.Value.Type == Constants.TypeStorage);
            if (storage.Value != null)
            {
                //Debug.Log("and a storage!");
                //storage.Value.OnShow += SelectOnShow;
                Selection.Instance.SelectObject(storage.Value);
            }

            var terminal = room.Objects.SingleOrDefault(ro => ro.Value.Type == Constants.TypeTerminal);
            if (terminal.Value != null)
            {
                //Debug.Log("and a terminal!");
                //terminal.Value.OnShow += SelectOnShow;
                Selection.Instance.SelectObject(terminal.Value);
            }

            room.RoomUnpacker.OnUnpack -= RoomUnpacked;
        }

        private void SelectOnShow(RoomObject roomObject, bool show)
        {
            Debug.Log($"{roomObject.Type} selectonshow {show}");
            if (show)
            {
                Selection.Instance.SelectObject(roomObject);
                roomObject.OnShow -= SelectOnShow;
            }
        }

        private void InitializeChooser(string str)
        {
            var obj = new JSONObject(str);
            int? defaultShardIndex = null;
            string defaultRoom = ""; // TODO: get starter room endpoint if we have no rooms

            var shardObj = obj["shards"];
            if (shardObj != null)
            {
                _shardInput.gameObject.SetActive(true);

                _shards.Clear();
                var shardIndex = 0;
                var shardNames = shardObj.keys;
                foreach (var shardName in shardNames)
                {
                    _shards.Add(shardName);

                    var shardRooms = new List<string>();
                    _shardRooms.Add(shardName, shardRooms);

                    var roomList = shardObj[shardName].list;
                    if (roomList.Count > 0 && _roomInput.text.Length == 0)
                    {

                        defaultShardIndex = shardIndex;
                        defaultRoom = roomList[0].str;

                        foreach (var room in roomList)
                        {
                            shardRooms.Add(room.str);
                            AddRoomToRoomListGameObject(shardName, room.str);
                        }
                    }

                    shardIndex++;
                }
            }
            else
            {
                const string shardName = "shard0";

                _shardInput.gameObject.SetActive(false);
                _shards.Clear();
                _shards.Add(shardName);
                _shardInput.value = 0;

                var shardRooms = new List<string>();
                _shardRooms.Add(shardName, shardRooms);

                var roomObj = obj["rooms"];
                if (roomObj != null && roomObj.list.Count > 0)
                {
                    var roomList = roomObj.list;
                    defaultRoom = roomList[0].str;

                    foreach (var room in roomList)
                    {
                        shardRooms.Add(room.str);
                        AddRoomToRoomListGameObject(shardName, room.str);
                    }
                }
            }

            _shardInput.ClearOptions();
            _shardInput.AddOptions(_shards);

            var savedShard = PlayerPrefs.GetInt(GetServerPrefKey(_prefShard), -1);
            var savedRoom = PlayerPrefs.GetString(GetServerPrefKey(_prefRoom));
            _roomInput.text = !string.IsNullOrEmpty(savedRoom) ? savedRoom : defaultRoom;
            _shardInput.value = savedShard != -1 ? savedShard : defaultShardIndex.HasValue ? defaultShardIndex.Value : 0;

            if (!string.IsNullOrEmpty(_roomInput.text))
            {
                OnSelectedRoomChanged(_roomInput.text);
            }

            if (_pvpSpectateToggle.isOn)
            {
                this.OnTogglePvpSpectate(_pvpSpectateToggle.isOn);
            }

            // TODO: we need to look into this, TwitchClient has not been initialized at this point, and Instance throws an error.
            // We register it here, cause we are lazy, and hopefully the twitch client is initialized.
            TwitchClient.Instance.OnGoToRoom += Instance_OnGoToRoom;
        }

        private void AddRoomToRoomListGameObject(string shardName, string romName)
        {
            var go = PoolLoader.Load("Prefabs/RoomList/roomName");
            var text = go.GetComponent<TMP_Text>();
            text.text = RoomLink.FormatTMPLink(shardName, romName, $"{romName}");
            go.transform.SetParent(_roomListContent.transform);
        }

        private string GetServerPrefKey(string prefKey)
        {
            var hostname = ScreepsAPI.Cache.Address.HostName;
            var port = ScreepsAPI.Cache.Address.Port;

            return string.Format("{0} {1} {2}", hostname, port, prefKey);
        }
    }
}

/*{"ok":1,"shards":{"shard0":["W2S12","E22S24","E23S15"],"shard1":[],"shard2":[]}}*/
