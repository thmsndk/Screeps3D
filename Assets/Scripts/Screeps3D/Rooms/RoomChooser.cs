using Screeps_API;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        private readonly string _prefPvpSpectateToggle = "PvpSpectateToggle";
        private readonly string _prefShard = "shard";
        private readonly string _prefRoom = "room";

        private List<string> _shards = new List<string>();
        private System.Random random;

        private IEnumerator _findPvpRooms;

        private void Start()
        {
            random = new System.Random();
            _pvpSpectateToggle.isOn = PlayerPrefs.GetInt(_prefPvpSpectateToggle, 1) == 1;
            _shardInput.onValueChanged.AddListener(OnSelectedShardChanged);
            _roomInput.onSubmit.AddListener(OnSelectedRoomChanged);
            _pvpSpectateToggle.onValueChanged.AddListener(OnTogglePvpSpectate);

            if (ScreepsAPI.IsConnected)
            {
                ScreepsAPI.Http.GetRooms(ScreepsAPI.Me.UserId, InitializeChooser);
            }
            else
            {
                throw new Exception("RoomChooser assumes ScreepsAPI.IsConnected == true at start of scene");
            }
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
                    //  use the mapStats endpoint to get owned rooms and randomly select one
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

            /*
             async function roomSwap() {
              // return setRoom('E7N5')
              while(true) {
                try {
                  const { pvp } =  await api.raw.experimental.pvp(100)
                  const [shard='shard0'] = Object.keys(pvp)
                  let { [shard]: { rooms } } = pvp
                  state.pvp.rooms = rooms
                  rooms.sort((a,b) => b.lastPvpTime - a.lastPvpTime)
                  const now = Date.now()
                  const append = rooms.filter(r => r.lastPvpTime > state.gameTime - 50).map(r => `${now},${r._id},${r.lastPvpTime}\n`).join('')
                  fs.appendFile('pvp.csv', append, () => {})
                  rooms = rooms.filter(r => r.lastPvpTime > state.gameTime - 10)
                  let room = ''
                  if (chatRoom && chatRoomTimeout > Date.now()) {
                    room = chatRoom
                  } else if (rooms.length) {
                    const { _id, lastPvpTime: time } = rooms[Math.floor(Math.random() * rooms.length)]
                    room = _id
                  } else {
                    const { stats } = await api.raw.game.mapStats(roomList, 'owner0')
                    for(let k in stats) {
                      const r = stats[k]
                      if (r.own && r.own.level) {
                        rooms.push(k)
                      }
                    }
                    console.log(stats)
                    room = rooms[Math.floor(Math.random() * rooms.length)]
                  }
                  await setRoom(room)
                } catch(e) { console.error('roomSwap', e) }
                await sleep(ROOM_SWAP_INTERVAL)
              }
            }
             * */
        }
        public void OnSelectedShardChanged(string shardName)
        {
            var shardIndex = _shardInput.options.FindIndex(s => s.text == shardName);
            _shardInput.value = shardIndex;
        }
        public void OnSelectedShardChanged(int shardIndex)
        {
            PlayerPrefs.SetInt(GetServerPrefKey(_prefShard), shardIndex);
        }

        public void OnSelectedRoomChanged(string roomName)
        {
            if (!string.IsNullOrEmpty(roomName))
            {
                PlayerPrefs.SetString(GetServerPrefKey(_prefRoom), roomName);
            }
            
            this.GetAndChooseRoom(roomName);
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
            if (OnChooseRoom != null) OnChooseRoom.Invoke(room);
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
                    var roomList = shardObj[shardName].list;
                    if (roomList.Count > 0 && _roomInput.text.Length == 0)
                    {

                        defaultShardIndex = shardIndex;
                        defaultRoom = roomList[0].str;
                        
                    }

                    shardIndex++;
                }
            }
            else
            {
                _shardInput.gameObject.SetActive(false);
                _shards.Clear();
                _shards.Add("shard0");
                _shardInput.value = 0;

                var roomObj = obj["rooms"];
                if (roomObj != null && roomObj.list.Count > 0)
                {
                    defaultRoom = roomObj.list[0].str;
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

        private string GetServerPrefKey(string prefKey)
        {
            var hostname = ScreepsAPI.Cache.Address.HostName;
            var port = ScreepsAPI.Cache.Address.Port;

            return string.Format("{0} {1} {2}", hostname, port, prefKey);
        }
    }
}

/*{"ok":1,"shards":{"shard0":["W2S12","E22S24","E23S15"],"shard1":[],"shard2":[]}}*/
