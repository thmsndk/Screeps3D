using System;
using Screeps_API;

namespace Screeps3D.Rooms
{
    public class RoomMapStream
    {
        private Room _room;
        private string _path;
        private bool _subscribed;
        
        public Action<JSONObject> OnData;

        public void Init(Room room)
        {
            _room = room;

            if (ScreepsAPI.Server.Address.HostName.ToLowerInvariant() == "screeps.com")
            {
                _path = string.Format("roomMap2:{0}/{1}", room.ShardName, room.RoomName);
            } else
            {
                _path = string.Format("roomMap2:{0}", room.RoomName);
            }
            // temporarily hooked into OnShowObjects so roads will render
            room.OnShowMap += ManageSubscription;
        }

        private void ManageSubscription(bool showObjects)
        {
            _subscribed = showObjects;
            if (showObjects)
            {
                Subscribe();
            } else
            {
                Unsubscribe();
            }
        }

        private void Subscribe()
        {
            ScreepsAPI.Socket.Subscribe(_path, ReceiveData);
        }

        private void Unsubscribe()
        {
            ScreepsAPI.Socket.Unsub(_path);
        }
        
        private void ReceiveData(JSONObject Data)
        {
            if (OnData != null)
                OnData(Data);
        }
    }
}