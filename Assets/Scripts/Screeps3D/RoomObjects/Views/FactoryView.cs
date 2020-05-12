using Common;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class FactoryView : MonoBehaviour, IObjectViewComponent
    {
        [SerializeField] private ScaleAxes _energyDisplay = default;

        private Factory _factory;
        // TODO: we also need the mineral on the location to get regen time if we want to do something specific in regards to that

        public void Init()
        {
        }

        public void Load(RoomObject roomObject)
        {
            _factory = roomObject as Factory;
            AdjustScale();
        }

        public void Delta(JSONObject data)
        {
            AdjustScale();
        }

        public void Unload(RoomObject roomObject)
        {
            _factory = null;
        }

        private void Update()
        {
            if (_factory == null)
                return;
            
            // TODO: actions, like creep
        }
        private void AdjustScale()
        {
            if (_factory != null)
            {
                _energyDisplay.SetVisibility(_factory.TotalResources / _factory.TotalCapacity);
            }
        }
    }
}