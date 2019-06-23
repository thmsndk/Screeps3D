using Screeps3D.Rooms;
using UnityEngine;

namespace Screeps3D.RoomObjects
{
    /*
    {
    _id: "3eaba7e08f2f9a3"
    type: "energy"
    x: "12"
    y: "29"
    room: "W5N8"
    energy: "136"
    resourceType: "energy"
    }

    OR
    
    {
    _id: "07c4a7e08f2a95e"
    type: "energy"
    x: "12"
    y: "29"
    room: "W5N8"
    K: "40"
    resourceType: "K"
    }
    */

    public class Resource : RoomObject, IResourceObject
    {
        public float ResourceAmount { get; set; }
        public float ResourceCapacity { get; set; }
        public string ResourceType { get; set; }

        public Resource()
        {
        }

        internal override void Unpack(JSONObject data, bool initial)
        {
            base.Unpack(data, initial);

            if (initial)
            {
                Type = "resource";
            }

            foreach (var resourceType in data.keys)
            {
                if (Constants.ResourcesAll.Contains(resourceType))
                {
                    ResourceType = resourceType;
                    ResourceCapacity = 0;
                    ResourceAmount = data[resourceType].n;
                    break;
                }
            }
        }
        
        internal override void Delta(JSONObject delta, Room room)
        {
            base.Delta(delta, room);

            if (View != null)
                View.Delta(delta);
        }
    }
}