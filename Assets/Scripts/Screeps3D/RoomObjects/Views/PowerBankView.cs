using Common;
using Screeps3D.RoomObjects;
using UnityEngine;

namespace Assets.Scripts.Screeps3D.RoomObjects.Views
{
    class PowerBankView : MonoBehaviour, IObjectViewComponent
    {
        [SerializeField] private ScaleVisibility _powerScaleVisibility;
        private PowerBank _powerBank;

        [SerializeField] private float _Power;

        public void Init()
        {
        }

        public void Load(RoomObject roomObject)
        {
            _powerBank = roomObject as PowerBank;
        }

        public void Delta(JSONObject data)
        {
            var power = _Power > 0 ? _Power : _powerBank.Power;
            var percentage = power / _powerBank.PowerCapacity;

            var minVisibility = 0.001f; /*to keep it visible and selectable, also allows the resource to render again when regen hits*/
            
            float visibility = percentage == 0 ? minVisibility : percentage;

            _powerScaleVisibility.SetVisibility(visibility);
        }

        public void Unload(RoomObject roomObject)
        {
        }
    }
}
