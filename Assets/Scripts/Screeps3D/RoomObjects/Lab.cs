using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Screeps3D.RoomObjects
{
    /*{
        "_id":"594a260005d0cca1799cfc76",
        "type":"lab",
        "x":10,
        "y":19,
        "room":"W8S12",
        "notifyWhenAttacked":true,
        "user":"567d9401f60a26fc4c41bd38",
        "hits":500,
        "hitsMax":500,
        "mineralAmount":0,
        "cooldown":0,
        "mineralType":null,
        "mineralCapacity":3000,
        "energy":1080,
        "energyCapacity":2000,
        "actionLog":{
            "runReaction":null || "runReaction":{"x1":19,"y1":31,"x2":18,"y2":32}
        }
    }*/
    public class Lab : OwnedStoreStructure, IActionObject, IResourceObject, ICooldownObject//, IEnergyObject
    {
        public float ResourceAmount { get; set; }
        public float ResourceCapacity { get; set; }
        public string ResourceType { get; set; }

        public float Cooldown { get; set; }
        public Dictionary<string, JSONObject> Actions { get; set; }

        public Lab()
        {
            Actions = new Dictionary<string, JSONObject>();
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

                var minAmountData = data["mineralAmount"];

                var minCapacityData = data["mineralCapacity"];

                var minTypeData = data["mineralType"];
                if (minTypeData != null && !minTypeData.IsNull)
                {
                    if (minAmountData != null && !minAmountData.IsNull)
                    {
                        store.AddField(minTypeData.str, minAmountData.n);
                    }

                    if (minCapacityData != null && !minCapacityData.IsNull)
                    {
                        storeCapacityResource.AddField(minTypeData.str, minCapacityData.n);
                    }
                }
                else
                {
                    var storeCapacity = 0f;

                    if (energyCapData != null && !energyCapData.IsNull)
                    {
                        storeCapacity += energyCapData.n;
                    }

                    if (minCapacityData != null && !minCapacityData.IsNull)
                    {
                        storeCapacity += minCapacityData.n;
                    }

                    if (storeCapacity > 0f)
                    {
                        data.AddField("storeCapacity", storeCapacity);
                    }
                }
            }

            base.Unpack(data, initial);

            // unpack from store format
            try
            {
                var potentialResourceType = this.Capacity.FirstOrDefault(s => s.Key != Constants.TypeResource && s.Value > 0);
                if (potentialResourceType.Key != null && potentialResourceType.Key != Constants.TypeResource)
                {
                    ResourceType = potentialResourceType.Key;

                    ResourceCapacity = this.Capacity.ContainsKey(ResourceType) ? this.Capacity[ResourceType] : 0;
                    ResourceAmount = this.Store.ContainsKey(ResourceType) ? this.Store[ResourceType] : 0;
                }
            }
            catch (System.Exception ex)
            {
                throw;
            }

            UnpackUtility.Cooldown(this, data);

            UnpackUtility.ActionLog(this, data);
        }
    }
}