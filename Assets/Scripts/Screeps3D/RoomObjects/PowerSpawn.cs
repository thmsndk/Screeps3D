using System.Collections.Generic;
using Common;
using UnityEngine;

namespace Screeps3D.RoomObjects
{
    /*{
        "_id":"594fd4a759b455ab3f6aa147",
        "type":"powerSpawn",
        "x":8,
        "y":21,
        "room":"W8S12",
        "notifyWhenAttacked":true,
        "user":"567d9401f60a26fc4c41bd38",
        "power":69,
        "powerCapacity":100,
        "energy":4450,
        "energyCapacity":5000,
        "hits":5000,
        "hitsMax":5000
    }*/
    public class PowerSpawn : OwnedStoreStructure
    {
        public int power { get; set; }
        public int powerCap { get; set; }
        public int energy { get; set; }
        public int energyCap { get; set; }
        public float ResourceAmount { get; set; }
        public float ResourceCapacity { get; set; }
        public string ResourceType { get; set; }

        internal PowerSpawn()
        {
            ResourceType = "power";
        }
        
        internal override void Unpack(JSONObject data, bool initial)
        {
            var power = data["power"];
            var powerCap = data["powerCapacity"];
            // Convert pre-store update to post-store update
            if (!data.HasField("store") && data.keys.Count > 0)
            {


                // if (minAmountData != null)
                // {
                //     store.AddField(ResourceType, minAmountData.n);
                // }

                // if (minCapacityData != null)
                // {
                //     storeCapacityResource.AddField(ResourceType, minCapacityData.n);
                // }

            }

            base.Unpack(data, initial);

            // ResourceCapacity = this.Capacity.ContainsKey(ResourceType) ? this.Capacity[ResourceType] : 0;
            // ResourceAmount = this.Store.ContainsKey(ResourceType) ? this.Store[ResourceType] : 0;
        }
    }
}