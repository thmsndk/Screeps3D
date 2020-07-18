using Common;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class ExtensionView: MonoBehaviour, IObjectViewComponent
    {
        [SerializeField] private ScaleAxes _size = default;
        [SerializeField] private MeshFilter _energyBall = default;
        private Extension _extension;

        public void Init()
        {
        }

        public void Load(RoomObject roomObject)
        {
            _extension = roomObject as Extension;
            // random rotation of energy display on load, so all extensions does not have floating texture in one direction
            var euler = transform.eulerAngles;
            euler.y = Random.Range(0.0f, 360.0f);
            _energyBall.transform.eulerAngles = euler;

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