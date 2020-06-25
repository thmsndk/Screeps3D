using System;
using System.Collections.Generic;
using Common;
using Screeps3D.Rooms;
using Screeps_API;
using UnityEngine;
using System.Linq;
using Screeps3D.Player;
using System.Collections;
using System.Text.RegularExpressions;
using Screeps3D.Tools;
using Assets.Scripts.Screeps3D.Tools.ConstructionSite;
using Assets.Scripts.Screeps3D.Menus.Respawn;

namespace Screeps3D
{
    public class RespawnManager : BaseSingleton<RespawnManager>
    {

        [SerializeField] private ToolChooser _toolChooser = default;
        [SerializeField] private ChooseConstructionSite _chooseConstruction = default;
        [SerializeField] private LostSpawnPopup _lostSpawnPopup = default;

        private void Awake()
        {
            // subscripe to world status updates
            // render oops
            // render respawn warning

            // call api for respawn
            // trigger place-spawn tool
            WorldStatusUpdater.Instance.OnWorldStatusChanged += OnWorldStatusChanged;
            MapStatsUpdater.Instance.OnMapStatsUpdated += OnMapStatsUpdated;


        }

        private Dictionary<string, Room> _prohibitedRooms = new Dictionary<string, Room>();
        private void OnMapStatsUpdated()
        {
            if (MapStatsUpdater.Instance.RoomInfo.TryGetValue(PlayerPosition.Instance.ShardName, out var shardRoomInfo))
            {
                foreach (var roomInfo in shardRoomInfo)
                {
                    if (roomInfo.RoomName == "W14N3")
                    {
                        Debug.LogError($"W14N3 user {roomInfo.User != null} reserved {roomInfo.IsReserved}");
                    }

                    // Can't spawn in owned or reserved rooms.
                    if (roomInfo.User != null || roomInfo.IsReserved)
                    {
                        if (!_prohibitedRooms.TryGetValue(roomInfo.RoomName, out var room))
                        {
                            room = RoomManager.Instance.Get(roomInfo.RoomName, PlayerPosition.Instance.ShardName);
                            if (room != null)
                            {
                                _prohibitedRooms.Add(roomInfo.RoomName, room);
                            }

                            // TODO: this is wrong to do... we can't unregister the event.
                            // room aint initialized yet, wait for show. but we also need to remove the prohibed event....

                            if (!room.InitializedView)
                            {
                                room.OnShow += (show) =>
                                {
                                    room.View.SpawnProhibited(true);
                                };
                            }
                            else
                            {
                                room.View.SpawnProhibited(true);
                            }

                            Debug.LogError($"{roomInfo.RoomName} {PlayerPosition.Instance.ShardName} should be prohibited");

                        }

                        continue;
                    }

                    // TODO: Can't spawn in SK rooms
                    // TODO: Can't spawn in highway rooms
                }
            };
        }

        private void Start()
        {
            _lostSpawnPopup.OnCancel += LostSpawnPopupCancelClicked;
            _lostSpawnPopup.OnRespawn += LostSpawnPopupRespawnClicked;
        }

        private void OnDestroy()
        {
            WorldStatusUpdater.Instance.OnWorldStatusChanged -= OnWorldStatusChanged;
            _lostSpawnPopup.OnCancel -= LostSpawnPopupCancelClicked;
            _lostSpawnPopup.OnRespawn -= LostSpawnPopupRespawnClicked;
        }

        private void LostSpawnPopupCancelClicked()
        {
            _lostSpawnPopup?.gameObject?.SetActive(false);
        }

        private void LostSpawnPopupRespawnClicked()
        {
            _lostSpawnPopup?.gameObject?.SetActive(false);
            // TODO: call respawn, activate emmpty mode
            ScreepsAPI.Http.Respawn((jsonResponse) =>
            {
                var result = new JSONObject(jsonResponse);
                var ok = result["ok"];

                if (ok != null && ok.n == 1)
                {
                    WorldStatusUpdater.Instance.SetWorldStatus(WorldStatus.Empty);
                }
            });
        }

        private void OnWorldStatusChanged(WorldStatus previous, WorldStatus current)
        {
            switch (current)
            {
                case WorldStatus.None:
                    break;
                case WorldStatus.Normal:
                    if (previous == WorldStatus.Empty)
                    {
                        _toolChooser?.Show(ToolType.Flag);
                        _toolChooser?.Show(ToolType.Construction);
                        _toolChooser?.Hide(ToolType.Spawn);
                    }
                    break;
                case WorldStatus.Lost:
                    if (previous != WorldStatus.Lost)
                    {
                        _lostSpawnPopup?.gameObject?.SetActive(true);
                    }

                    break;
                case WorldStatus.Empty:
                    _toolChooser?.Hide(ToolType.Flag);
                    _toolChooser?.Hide(ToolType.Construction);
                    _toolChooser?.Show(ToolType.Spawn);

                    // Get respawn prohibited rooms
                    // Get mapstats to determine invalid rooms for spawning. should probably trigger a coroutine that updates and calculates what rooms a prohibited.
                    // Toggle spawn overlay on



                    break;
                default:
                    break;
            }

            Debug.Log($"[WorldStatus] {previous} => {current}");
        }
    }
}