using Common;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class StorageView : MonoBehaviour, IObjectViewComponent
    {
        [SerializeField] private ScaleAxes _energyDisplay = default;

        private Storage _energyObject;

        public void Init()
        {
        }

        public void Load(RoomObject roomObject)
        {
            _energyObject = roomObject as Storage;
            AdjustScale();
        }

        public void Delta(JSONObject data)
        {
            AdjustScale();
        }

        public void Unload(RoomObject roomObject)
        {
        }

        private void AdjustScale()
        {
            if (_energyObject != null)
            {
                _energyDisplay.SetVisibility(_energyObject.TotalResources / _energyObject.TotalCapacity);
            }
        }
    }
}