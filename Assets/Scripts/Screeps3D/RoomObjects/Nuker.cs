using System.Collections.Generic;

namespace Screeps3D.RoomObjects
{
    /*{
        "_id":"594c4187c46642cc2ce46dff",
        "type":"nuker",
        "x":14,
        "y":18,
        "room":"W2S12",
        "notifyWhenAttacked":true,
        "user":"567d9401f60a26fc4c41bd38",
        "energy":300000,
        "energyCapacity":300000,
        "G":5000,
        "GCapacity":5000,
        "hits":1000,
        "hitsMax":1000,
        "cooldownTime":2.247301E+07
    }*/

    public class Nuker : OwnedStoreStructure, IResourceObject//, IEnergyObject
    {
        public float ResourceAmount { get; set; }
        public float ResourceCapacity { get; set; }
        public string ResourceType { get; set; }

        internal Nuker()
        {
            ResourceType = "G";
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

                var minAmountData = data["G"];

                var minCapacityData = data["GCapacity"];

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