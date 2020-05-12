using Common;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class ExtensionView: MonoBehaviour, IObjectViewComponent
    {
        [SerializeField] private ScaleAxes _size = default;
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
            if (_extension?.TotalCapacity >= 200)
                _size.SetVisibility(0.85f);
            else if (_extension?.TotalCapacity >= 100)
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