using Screeps_API;

namespace Screeps3D.RoomObjects
{
    public class OwnedRoomObject : RoomObject, IOwnedObject
    {
        public string UserId { get; set; }
        public ScreepsUser Owner { get; set; }

        internal override void Unpack(JSONObject data, bool initial)
        {
            base.Unpack(data, initial);

            if (initial)
            {
                UnpackUtility.Owner(this, data);
            }
        }
    }
}