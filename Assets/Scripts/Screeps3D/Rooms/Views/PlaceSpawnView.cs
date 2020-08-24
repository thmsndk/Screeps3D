using Screeps3D;
using Screeps3D.Rooms;
using Screeps3D.Rooms.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Screeps3D.Rooms.Views
{
    public class PlaceSpawnView : MonoBehaviour, IRoomViewComponent
    {
        public static bool EnableOverlay { get; set; } = true;
        private bool _overlayDisabled = false;

        [SerializeField] private GameObject _ProhibitedRoomProjector = default;

        private Room room;
        private RoomInfo _roomInfo;

        private bool _subscribingToOnMapStatsUpdated = false;
        public void Init(Room room)
        {
            _subscribingToOnMapStatsUpdated = false;

            this.room = room;
            room.MapStream.OnData += OnMapData;
            room.OnShowObjects += OnShowObjects;

            _roomInfo = MapStatsUpdater.Instance.GetRoomInfo(room.ShardName, room.RoomName);

            SpawnProhibited(false);

            WorldStatusUpdater.Instance.OnWorldStatusChanged += OnWorldStatusChanged;
            if (WorldStatusUpdater.Instance.WorldStatus == WorldStatus.Empty)
            {
                if (!_subscribingToOnMapStatsUpdated)
                {
                    MapStatsUpdater.Instance.OnMapStatsUpdated += OnMapStatsUpdated; // TODO: how to unsubscribe?
                    _subscribingToOnMapStatsUpdated = true;
                }

                UpdateRespawnProhibited();
            }
        }

        private bool ShouldRenderProhibitedRespawn()
        {
            if (_roomInfo == null)
            {
                return false;
            }

            if (_roomInfo.Status == RoomInfo.STATUS_OUT_OF_BORDERS)
            {
                return true;
            }
            // TODO: we should render time left untill open somehow
            if (int.TryParse(_roomInfo.OpenTime, out var openTime) && DateTimeOffset.FromUnixTimeMilliseconds(openTime) > DateTimeOffset.UtcNow)
            {
                return true;
            }

            var prohibited = IsCenter(room.XCoord, room.YCoord)
                || !(!IsBus(room.XCoord) && !IsBus(room.YCoord))
                || (/* TODO: check respawn prohibited rooms endpoint result ||*/
                    _roomInfo.User != null
                    );

            return prohibited;
        }

        private bool IsCenter(int x, int y)
        {
            return (x < 0 && Math.Abs(x + 1) % 10 >= 4 && Math.Abs(x + 1) % 10 <= 6 || x >= 0 && Math.Abs(x) % 10 >= 4 && Math.Abs(x) % 10 <= 6) 
                && (y < 0 && Math.Abs(y + 1) % 10 >= 4 && Math.Abs(y + 1) % 10 <= 6 || y >= 0 && Math.Abs(y) % 10 >= 4 && Math.Abs(y) % 10 <= 6);
        }

        private bool IsBus(int coord)
        {
            return coord < 0 && (coord + 1) % 10 == 0 || coord > 0 && coord % 10 == 0 || 0 == coord;
        }

        private void OnWorldStatusChanged(WorldStatus previous, WorldStatus current)
        {
            if (current == WorldStatus.Empty)
            {
                if (!_subscribingToOnMapStatsUpdated)
                {
                    MapStatsUpdater.Instance.OnMapStatsUpdated += OnMapStatsUpdated; // TODO: how to unsubscribe?
                    _subscribingToOnMapStatsUpdated = true;
                }

                UpdateRespawnProhibited();
            }
            else
            {
                SpawnProhibited(false);
            }
        }

        private void Update()
        {
            if (_roomInfo == null)
            {
                if (room != null)
                {
                    
                    _roomInfo = MapStatsUpdater.Instance.GetRoomInfo(room.ShardName, room.RoomName);
                    UpdateRespawnProhibited();

                }

                return;
            }

            // when we toggle spectate on, disable the overlay.
            if (!EnableOverlay)
            {
                _overlayDisabled = true;
                SpawnProhibited(false);
                return;
            }

            // when we toggle spectate off, re-enable the overlay.
            if (EnableOverlay && _overlayDisabled)
            {
                _overlayDisabled = false;
                UpdateRespawnProhibited();
                return;
            }
        }

        private void UpdateRespawnProhibited()
        {
            

            if (WorldStatusUpdater.Instance.WorldStatus == WorldStatus.Empty)
            {
                if (!_subscribingToOnMapStatsUpdated)
                {
                    MapStatsUpdater.Instance.OnMapStatsUpdated += OnMapStatsUpdated; // TODO: how to unsubscribe?
                    _subscribingToOnMapStatsUpdated = true;
                }

                SpawnProhibited(ShouldRenderProhibitedRespawn());
            }
        }

        private void OnShowObjects(bool show)
        {

        }

        private void OnMapData(JSONObject data)
        {

        }

        private void OnMapStatsUpdated()
        {
            if (_roomInfo != null)
            {
                // Can't spawn in owned rooms (Players, SK rooms). should not spawn in reserved rooms.
                if (_roomInfo.User != null || _roomInfo.IsReserved)
                {
                    //SpawnProhibited(true);
                }
            };
        }

        public void SpawnProhibited(bool prohibited)
        {
            if (_ProhibitedRoomProjector.activeSelf != prohibited)
            {
                _ProhibitedRoomProjector.SetActive(prohibited);
            }
        }
    }
}
