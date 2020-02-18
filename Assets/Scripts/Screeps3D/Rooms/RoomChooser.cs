using Assets.Scripts.Screeps_API.ConsoleClientAbuse;
using Common;
using Screeps_API;
using Screeps3D.Player;
using Screeps3D.RoomObjects;
using Screeps3D.Tools.Selection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Screeps3D.Rooms
{
    public class RoomChooser : MonoBehaviour
    {
        public Action<Room> OnChooseRoom;

        [SerializeField] private TMP_Dropdown _shardInput;
        [SerializeField] private TMP_InputField _roomInput;
        [SerializeField] private Toggle _pvpSpectateToggle;
        //[SerializeField] private GameObject _roomList;
        [SerializeField] private VerticalPanelElement _roomList;
        private bool showRoomList;
        [SerializeField] private GameObject _roomListContent;
        

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

        private void Start()
        {
            random = new System.Random();
            _pvpSpectateToggle.isOn = PlayerPrefs.GetInt(_prefPvpSpectateToggle, 1) == 1;
            _shardInput.onValueChanged.AddListener(OnSelectedShardChanged);
            _roomInput.onSubmit.AddListener(OnSelectedRoomChanged);
            _roomInput.onSelect.AddListener(OnToggleRoomList);
            _roomInput.onDeselect.AddListener(OnToggleRoomList);
            _pvpSpectateToggle.onValueChanged.AddListener(OnTogglePvpSpectate);

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

        private void OnTogglePvpSpectate(bool isOn)
        {
            PlayerPrefs.SetInt(_prefPvpSpectateToggle, isOn ? 1 : 0);

            if (isOn)
            {
                _findPvpRooms = FindPvpRoom();
                StartCoroutine(_findPvpRooms);
            }
            else
            {
                StopCoroutine(_findPvpRooms);
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

                yield return new WaitForSeconds(30);
            }
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

        private void ChooseRoomWithPVPOrOwnedRoom()
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

        private void ChooseRandomOwnedRoom()
        {
                var ownedRooms = MapStatsUpdater.Instance.RoomInfo.Values.Where(r => r.User != null && r.Level.HasValue && r.Level > 0);
                var random = new System.Random();
                var room = ownedRooms.ElementAt(random.Next(ownedRooms.Count()));
                var roomName = room?.RoomName;

                Debug.Log($"Going to room {roomName} owned by {room?.User?.Username}");
                _roomInput.text = roomName;
                this.GetAndChooseRoom(roomName);
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
        }

        private void AddRoomToRoomListGameObject(string shardName, string romName)
        {
            var go = PoolLoader.Load("Prefabs/RoomList/roomName");
            var text = go.GetComponent<TMP_Text>();
            text.text = RoomLink.FormatTMPLink(shardName, romName, $"{romName}");
            go.transform.parent = _roomListContent.transform;
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
