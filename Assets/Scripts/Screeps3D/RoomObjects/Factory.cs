using UnityEngine;

namespace Screeps3D.RoomObjects
{

    public class Factory : PlaceHolderRoomObject /*OwnedStoreStructure*/, ICooldownObject
    {
        public float Cooldown { get; set; }

        internal override void Unpack(JSONObject data, bool initial)
        {
            base.Unpack(data, initial);
            UnpackUtility.Cooldown(this, data);
        }
    }
}