using System.Collections.Generic;

namespace Screeps3D.RoomObjects
{
    public class OwnedStoreStructure : OwnedStructure, IStoreObject
    {
        public float TotalCapacity { get; set; }
        public float TotalResources { get; set; }

        public Dictionary<string, float> Store { get; private set; }
        public Dictionary<string, float> Capacity { get; private set; }

        public OwnedStoreStructure()
        {
            Store = new Dictionary<string, float>();
            Capacity = new Dictionary<string, float>();
        }

        internal override void Unpack(JSONObject data, bool initial)
        {
            base.Unpack(data, initial);

            UnpackUtility.Store(this, data);
        }
    }
}