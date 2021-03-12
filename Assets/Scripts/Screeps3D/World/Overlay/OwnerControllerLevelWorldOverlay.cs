using System;
using System.Collections.Generic;
using Assets.Scripts.Screeps3D;
using Assets.Scripts.Screeps3D.World.Views;
using Common;
using Screeps3D.Player;
using Screeps3D.Rooms;
using Screeps3D.Rooms.Views;
using UnityEngine;

namespace Screeps3D.World.Views
{
    // We want an overlay manager of sorts that can initialized room specific overlay and toggle them on / off
    // we need to initialize worldoverlay views for each room,

    public class OwnerControllerLevelWorldOverlay : MonoBehaviour
    {
        private Dictionary<string, WorldView> _views = new Dictionary<string, WorldView>();

        // TODO: we will initialize a list of OwnerControllerLevelViewData
        public OwnerControllerLevelWorldOverlay()
        {

            // TODO: we want to initialize 1 view, this view is then responsible for world wide rendering
            // inside this view, we might want object pooling, to render "sub views" over each room, in this case an OwnerControllerLevelPrefab for each room
            // we should be able to control how many of theese sub views we render, for example based on distance from player.
            // other kind of world views would be some sort of strategic view, intel, map visuals and such.


            // TODO: for each view, or well actually inside the view
            //_label.text = string.Format("{0}", _selected.Owner.Username);
            //_badge.sprite = Sprite.Create(_selected.Owner.Badge,
            //    new Rect(0.0f, 0.0f, BadgeManager.BADGE_SIZE, BadgeManager.BADGE_SIZE), new Vector2(.5f, .5f));
        }

        private void Awake()
        {
            MapStatsUpdater.Instance.OnMapStatsUpdated += Instance_OnMapStatsUpdated;
        }

        public const float OverlayDistance = 200;
        private void Instance_OnMapStatsUpdated()
        {
            LoadViewsByPlayerPosition(); // TODO: add property to trigger "update" of data.
        }

        private enum LookingDirection
        {
            North,
            East,
            South,
            West
        }

        private LookingDirection _lookingDirection = LookingDirection.North;

        private void Update()
        {
            //Debug.Log(CameraRig.Position);
            //Debug.Log(PlayerPosition.Instance.transform.position);
            //Debug.Log(Vector3.Angle(CameraRig.Position, PlayerPosition.Instance.transform.position));
            //Debug.Log(CameraRig.Rotation);
            // dpending if you rotate left or right, the y value will be negative or positive
            // it increases and increases and increases in either direction
            if (CameraRig.Rotation.y >= -50 && CameraRig.Rotation.y <= 50)
            {
                _lookingDirection = LookingDirection.North;
            }
            else if (CameraRig.Rotation.y >= 50 && CameraRig.Rotation.y <= 145)
            {
                _lookingDirection = LookingDirection.East;
            }
            else if (CameraRig.Rotation.y >= 145 || CameraRig.Rotation.y <= 225)
            {
                _lookingDirection = LookingDirection.South;
            }
            else if (CameraRig.Rotation.y >= 225 || CameraRig.Rotation.y <= 320)
            {
                _lookingDirection = LookingDirection.West;
            }
            //Debug.Log(_lookingDirection);

            LoadViewsByPlayerPosition();
        }

        private void LoadViewsByPlayerPosition()
        {
            if (string.IsNullOrEmpty(PlayerPosition.Instance.ShardName))
            {
                // we don't know where we are yet, we can't load views
                return;
            }

            var playerPosition = PlayerPosition.Instance.transform.position;
            foreach (var col in Physics.OverlapSphere(playerPosition, OverlayDistance, 1 << 10))
            {
                var roomView = col.GetComponent<RoomView>();
                if (roomView == null || roomView.Room == null || Vector3.Distance(playerPosition, roomView.Room.Position) > OverlayDistance)
                {
                    continue;
                }

                var roomInfo = MapStatsUpdater.Instance.GetRoomInfo(PlayerPosition.Instance.ShardName, roomView.Room.RoomName);

                if (roomInfo == null)
                {
                    continue;
                }

                if (!_views.TryGetValue(roomInfo.RoomName, out var view))
                {
                    _views[roomInfo.RoomName] = null; // Add it with a null value to prevent repated queueing

                    Scheduler.Instance.Add(() =>
                    {
                        var room = RoomManager.Instance.Get(roomInfo.RoomName, PlayerPosition.Instance.ShardName);
                        var data = new OwnerControllerLevelData(room, roomInfo);
                        var o = WorldViewFactory.GetInstance(data);
                        o.name = $"{roomInfo.RoomName}:RoomOwnerInfoView";
                        _views[roomInfo.RoomName] = o;
                    });
                }

                if (view != null)
                {
                // TODO: trigger an update?

                }
            }
        }
    }

    public class OwnerControllerLevelData : WorldViewData
    {
        public OwnerControllerLevelData(Room room, RoomInfo roomInfo)
        {
            Type = "RoomOwnerInfo";
            Room = room;
            RoomInfo = roomInfo;
        }

        public Room Room { get; }
        public RoomInfo RoomInfo { get; }
    }
}