using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class AttackView : CreepPartView
    {
        [SerializeField] private Animator _anim = default;

        public override void Load(RoomObject roomObject)
        {
            base.Load(roomObject);
            AdjustSize("attack", .5f, .5f);
        }

        public override void Delta(JSONObject delta)
        {
            AdjustSize("attack", .5f, .5f);

            if (creep.Actions.ContainsKey("attack") && !creep.Actions["attack"].IsNull)
            {
                var rotation = Quaternion.LookRotation(GetActionVector(creep.Actions["attack"]));
                _anim.SetTrigger("activate");
            }
        }
    }
}