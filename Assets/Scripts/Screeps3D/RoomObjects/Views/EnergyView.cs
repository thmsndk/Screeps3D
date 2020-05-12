using Common;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    // TODO: do we actually have anything that uses energyview anymore? should this script be replaced by storeview on most gameobjects?
    public class EnergyView : MonoBehaviour, IObjectViewComponent
    {
        [SerializeField] private ScaleVisibility _energyDisplay = default;

        private IEnergyObject _energyObject;
        private IStoreObject _storeObject;

        public void Init()
        {
        }

        public void Load(RoomObject roomObject)
        {
            _energyObject = roomObject as IEnergyObject;
            _storeObject = roomObject as IStoreObject;

            AdjustScale(true);
        }

        public void Delta(JSONObject data)
        {
            AdjustScale(false);
        }

        public void Unload(RoomObject roomObject)
        {
        }

        private void AdjustScale(bool instant)
        {
            var visibility = 0f;
            if (_energyObject != null)
            {
                visibility = _energyObject.Energy / _energyObject.EnergyCapacity;
            }
            else if (_storeObject != null)
            {
                float energy;
                _storeObject.Store.TryGetValue(Constants.TypeResource, out energy);

                visibility = energy / _storeObject.TotalCapacity;
            }

            _energyDisplay.SetVisibility(visibility, instant);
        }
    }
}