using System;
using System.Collections.Generic;
using Screeps3D.RoomObjects;
using Screeps_API;
using UnityEngine;

namespace Screeps3D.Rooms
{
    public class RoomUnpacker
    {
        private Room _room;
        private List<string> _removeList = new List<string>();

        public Action<JSONObject> OnUnpack;

        public void Init(Room room)
        {
            _room = room;
            _room.ObjectStream.OnData += Unpack;
            _room.OnShowObjects += ControlVisibility;
        }

        private void Unpack(JSONObject roomData)
        {
            if (roomData.HasField("gameTime"))
                _room.GameTime = (long) roomData["gameTime"].n;
            UnpackUsers(roomData);
            UnpackFlags(roomData);
            UnpackObjects(roomData);
            
            if (OnUnpack != null)
                OnUnpack(roomData);
        }

        private void UnpackObjects(JSONObject roomData)
        {
            var objectsData = roomData["objects"];

            // add new objects
            foreach (var id in objectsData.keys)
            {
                var objectData = objectsData[id];
                if (objectData.IsNull)
                    continue;
                
                if (!_room.Objects.ContainsKey(id))
                {
                    var roomObject = ObjectManager.Instance.GetInstance(id, objectData);
                    _room.Objects[id] = roomObject;
                }
            }

            // process existing object deltas
            _removeList.Clear();
            foreach (var kvp in _room.Objects)
            {
                var id = kvp.Key;
                var roomObject = kvp.Value;

                JSONObject objectData;
                if (objectsData.HasField(id))
                {
                    objectData = objectsData[id];
                }
                else if (roomObject.Room != _room)
                {
                    _removeList.Add(id);
                    continue;
                }
                else
                {
                    objectData = JSONObject.obj;
                }

                if (objectData.IsNull)
                {
                    roomObject.HideObject(_room);
                    _removeList.Add(id);
                }
                else
                {
                    roomObject.Delta(objectData, _room);
                }
            }

            foreach (var id in _removeList)
            {
                _room.Objects.Remove(id);
            }
        }

        private void UnpackUsers(JSONObject data)
        {
            var usersData = data["users"];
            if (usersData == null)
            {
                return;
            }

            foreach (var id in usersData.keys)
            {
                var userData = usersData[id];
                ScreepsAPI.UserManager.CacheUser(userData);
            }
        }

        private void ControlVisibility(bool showObjects)
        {
            if (!showObjects)
            {
                foreach (var kvp in _room.Objects)
                {
                    var roomObject = kvp.Value;
                    roomObject.HideObject(_room);
                }
                _room.Objects.Clear();
            }
        }

        // "swarm_W3N7s~4~9~25~25|intel_nsa~4~9~25~25|control_W3N7c~4~9~25~25"
        private void UnpackFlags(JSONObject roomData)
        {
            var flagsData = roomData["flags"];
            if (flagsData == null)
                return;

            if (flagsData.IsNull)
            {
                Debug.Log("recieved null flag data");
                return;
            }

            var flagStrings = flagsData.str.Split('|');
            foreach (var flagStr in flagStrings)
            {
                var dataArray = flagStr.Split('~');
                if (dataArray.Length < 5)
                    continue;
                var flag = ObjectManager.Instance.GetFlag(dataArray);
                flag.FlagDelta(dataArray, _room);
                _room.Objects[flag.Name] = flag;
            }
        }
    }
}