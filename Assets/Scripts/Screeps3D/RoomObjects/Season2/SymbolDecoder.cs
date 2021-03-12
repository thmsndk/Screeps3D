using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Screeps3D.RoomObjects
{
    /*
    {
	    "_id": "60285f4b4883687220e23b16",
	    "room": "E4N3",
	    "x": 7,
	    "y": 20,
	    "type": "symbolDecoder",
	    "resourceType": "symbol_res",
	    "store": {},
	    "storeCapacityResource": {
		    "symbol_res": 1000000
	    }
    }
    */

    public class SymbolDecoder : StoreStructure, IResourceObject
    {
        public float ResourceAmount { get; set; }
        public float ResourceCapacity { get; set; }
        public string ResourceType { get; set; }

        public SymbolDecoder()
        {
            OverrideTypePath = "Season2/";
        }

        internal override void Unpack(JSONObject data, bool initial)
        {
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
            catch (System.Exception)
            {
                throw;
            }

        }
    }
}