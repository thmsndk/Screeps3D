using UnityEngine;
using System.Collections.Generic;
using System;
using Common;

namespace Screeps3D.Rooms.Views
{
    public class RoomZoneView : MonoBehaviour, IRoomViewComponent
    {
        [SerializeField] private ParticleSystem _north;
        [SerializeField] private ParticleSystem _south;
        [SerializeField] private ParticleSystem _west;
        [SerializeField] private ParticleSystem _east;

        private Room room;
        private RoomInfo _roomInfo;

        // "cached" states of room info zone
        private bool showSafeMode;

        public void Init(Room room)
        {
            this.room = room;
            showSafeMode = false;

            _roomInfo = MapStatsUpdater.Instance.GetRoomInfo(room.RoomName);
        }


        private void Update()
        {
            // detect changes and update zone accordingly
            if (_roomInfo == null) {

                if (room != null)
                {
                    _roomInfo = MapStatsUpdater.Instance.GetRoomInfo(room.RoomName);
                }

                return;
            }

            if (showSafeMode != _roomInfo.HasSafeMode)
            {
                showSafeMode = _roomInfo.HasSafeMode;

                // update safe mode
                if (_roomInfo.HasSafeMode)
                {
                    // should be a prefab that we load and position and rotate correctly.
                    SetColor(_north, Color.yellow);
                    SetColor(_south, Color.yellow);
                    SetColor(_east, Color.yellow);
                    SetColor(_west, Color.yellow);
                    _north.Play();
                    _south.Play();
                    _east.Play();
                    _west.Play();
                }
                else
                {
                    _north.Stop();
                    _south.Stop();
                    _east.Stop();
                    _west.Stop();
                }
            }
        }

        private void SetColor(ParticleSystem particleSystem, Color color)
        {
            var main = particleSystem.main;
            main.startColor = color;
        }
    }
}