using Common;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class PowerSpawnView : MonoBehaviour, IObjectViewComponent
    {
        [SerializeField] private ScaleAxes _energyDisplay = default;
        [SerializeField] private ScaleAxes _powerDisplay = default;
        private PowerSpawn _powerSpawn;

        public void Init()
        {
        }

        public void Load(RoomObject roomObject)
        {
            _powerSpawn = roomObject as PowerSpawn;
            AdjustScale();
        }

        public void Delta(JSONObject data)
        {            
        if (data.HasField("store") && data.keys.Count > 0) {
                AdjustScale();
            }
        }

        public void Unload(RoomObject roomObject)
        {
        }

        private void AdjustScale()
        {
            if (_powerSpawn != null)
            {
                if(_powerSpawn.Store["energy"] != null) {
                    _energyDisplay.SetVisibility(_powerSpawn.Store["energy"] / 5000);
                }
                if(_powerSpawn.Store["power"] != null) {
                _powerDisplay.SetVisibility(_powerSpawn.Store["power"] / 100);
                }
            }
        }
    }
}