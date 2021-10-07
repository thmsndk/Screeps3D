using Common;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class SpawnView : MonoBehaviour, IObjectViewComponent
    {
        [SerializeField] private Renderer _energyBall = default;
        [SerializeField] private ScaleAxes _energyBallScale = default;

        private Spawn _spawn;

        public void Init()
        {
        }

        public void Load(RoomObject roomObject)
        {
            _spawn = roomObject as Spawn;

            _energyBall.materials[0].SetFloat("EmissionStrength", .5f);
            _energyBall.materials[0].SetFloat("xSize", 0.4f);
            _energyBall.materials[0].SetFloat("ySize", 0.4f);
            _energyBall.materials[0].SetTexture("EmissionTexture", Constants.EnergyColorTexture.Get());

        }

        public void Delta(JSONObject data)
        {
        }

        public void Unload(RoomObject roomObject)
        {
            _spawn = null;
        }
        private void AdjustScale()
        {
            if (_spawn != null)
            {
                _energyBallScale.SetVisibility(_spawn.TotalResources / _spawn.TotalCapacity);
            }
        }
    }
}