using Common;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class ExtensionView: MonoBehaviour, IObjectViewComponent
    {
        [SerializeField] private ScaleAxes _size;
        private Extension _extension;

        public void Init()
        {
        }

        public void Load(RoomObject roomObject)
        {
            _extension = roomObject as Extension;
        }

        public void Delta(JSONObject data)
        {
            if (_extension.EnergyCapacity >= 200)
                _size.SetVisibility(0.85f);
            else if (_extension.EnergyCapacity >= 100)
                _size.SetVisibility(0.65f);
            else
                _size.SetVisibility(0.5f);
        }

        public void Unload(RoomObject roomObject)
        {
            _extension = null;
        }
    }
}