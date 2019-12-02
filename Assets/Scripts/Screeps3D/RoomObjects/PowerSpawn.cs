using System.Collections.Generic;

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
    public class PowerSpawn : OwnedStoreStructure, IResourceObject//, IEnergyObject
    {
        public float ResourceAmount { get; set; }
        public float ResourceCapacity { get; set; }
        public string ResourceType { get; set; }

        internal PowerSpawn()
        {
            ResourceType = "power";
        }
        
        internal override void Unpack(JSONObject data, bool initial)
        {
            // Convert pre-store update to post-store update
            if (!data.HasField("store") && data.keys.Count > 0)
            {
                var store = new JSONObject();
                data.AddField("store", store);

                var storeCapacityResource = new JSONObject();
                data.AddField("storeCapacityResource", store);

                var energyCapData = data["energyCapacity"];
                if (energyCapData)
                {
                    if (energyCapData != null)
                    {
                        storeCapacityResource.AddField(Constants.TypeResource, energyCapData.n);
                    }
                }

                var energyData = data["energy"];
                if (energyData != null)
                {
                    if (energyData != null)
                    {
                        store.AddField(Constants.TypeResource, energyData.n);
                    }
                }

                var minAmountData = data["power"];

                var minCapacityData = data["powerCapacity"];

                if (minAmountData != null)
                {
                    store.AddField(ResourceType, minAmountData.n);
                }

                if (minCapacityData != null)
                {
                    storeCapacityResource.AddField(ResourceType, minCapacityData.n);
                }

            }

            base.Unpack(data, initial);

            ResourceCapacity = this.Capacity.ContainsKey(ResourceType) ? this.Capacity[ResourceType] : 0;
            ResourceAmount = this.Store.ContainsKey(ResourceType) ? this.Store[ResourceType] : 0;
        }
    }
}