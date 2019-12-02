using UnityEngine;

namespace Screeps3D.RoomObjects
{

    public class Ruin : PlaceHolderRoomObject /*OwnedStoreStructure*/, ICooldownObject, IDecay
    {
        public float NextDecayTime { get; set; }

        public float Cooldown { get; set; }

        internal override void Unpack(JSONObject data, bool initial)
        {
            base.Unpack(data, initial);
            UnpackUtility.Cooldown(this, data);
            UnpackUtility.Decay(this, data);
        }
    }
}