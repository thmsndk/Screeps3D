using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Screeps3D.Rooms;
using UnityEngine;

namespace Screeps3D.Player
{
    public class PlayerTransporter : BaseSingleton<PlayerTransporter>
    {
        [SerializeField] private RoomChooser _chooser;

        [SerializeField] private PlayerGaze _playerGaze;

        private Stack<Room> _loadStack = new Stack<Room>();

        private void Start()
        {
            _chooser.OnChooseRoom += OnChoose;

        }

        private void OnChoose(Room room)
        {
            _loadStack.Push(room);

            if (_playerGaze != null)
            {
                _playerGaze.allowFocus = false;
                CameraRig.Instance.OnTargetReached += OnTargetReached;
                // disable player gaze while traveling to new room looks like we are subscribing to all the rooms we are scrolling past.
                // while this prevents the subscription of too many rooms, it also breaks the subscriptions in general so they are not behaving as expected   
                //CameraRig.Instance.OnTargetReached += OnTargetReached;
                //_playerGaze.enabled = false;
            }
            
            TransportPlayer(room.Position);
            

            room.Show(true);
            
        }

        private void OnTargetReached()
        {
            Debug.Log("target reached!");
            
            _playerGaze.allowFocus = true;
            _playerGaze.enabled = true;
            CameraRig.Instance.OnTargetReached -= OnTargetReached;
        }

        private void TransportPlayer(Vector3 pos)
        {
            CameraRig.Position = pos + new Vector3(25, 0, 25);
        }
    }
}