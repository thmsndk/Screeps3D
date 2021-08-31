using System;
using System.Collections.Generic;
using Common;
using UnityEngine;

namespace Screeps3D.RoomObjects
{
    [DisallowMultipleComponent]
    public class ObjectManager : BaseSingleton<ObjectManager>
    {
        public Dictionary<string, RoomObject> RoomObjects { get; private set; }
        private ObjectFactory _factory = new ObjectFactory();

        private void Start()
        {
            RoomObjects = new Dictionary<string, RoomObject>();
        }

        internal RoomObject GetInstance(string id, JSONObject data)
        {
            if (RoomObjects.ContainsKey(id))
            {
                return RoomObjects[id];
            }

            var typeData = data["type"];
            if (typeData == null)
            {
                Debug.LogError(data.ToString());
                throw new Exception($"type data was not found for new roomObject with id {id}, there may be a caching problem");
            }

            var type = typeData.str;

            var roomObject = _factory.Get(type);
            RoomObjects[id] = roomObject;

            return roomObject;
        }

        public Flag GetFlag(string[] dataArray)
        {
            var name = dataArray[0];
            if (!RoomObjects.ContainsKey(name))
            {
                RoomObjects[name] = new Flag(name);
            }

            var flag = RoomObjects[name] as Flag;
            return flag;
        }
    }
}