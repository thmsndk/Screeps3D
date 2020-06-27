using Common;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class FactoryView : MonoBehaviour, IObjectViewComponent
    {
        [SerializeField] private ScaleAxes _energyDisplay = default;
        [SerializeField] private Renderer _l1 = default;
        [SerializeField] private Renderer _l2 = default;
        [SerializeField] private Renderer _l3 = default;
        [SerializeField] private Renderer _l4 = default;
        [SerializeField] private Renderer _l5 = default;

        private Factory _factory;
        // TODO: we also need the mineral on the location to get regen time if we want to do something specific in regards to that

        private void setLevelDisplay() {
            int level = _factory.Level != null ? (int)_factory.Level : 0;
            _l1.materials[0].SetFloat("EmissionStrength", level >= 1 ? 0.7f : 0.1f);
            _l2.materials[0].SetFloat("EmissionStrength", level >= 2 ? 0.7f : 0.1f);
            _l3.materials[0].SetFloat("EmissionStrength", level >= 3 ? 0.7f : 0.1f);
            _l4.materials[0].SetFloat("EmissionStrength", level >= 4 ? 0.7f : 0.1f);
            _l5.materials[0].SetFloat("EmissionStrength", level >= 5 ? 0.7f : 0.1f);

            _l1.materials[0].SetColor("EmissionColor", Color.white);
            _l2.materials[0].SetColor("EmissionColor", Color.white);
            _l3.materials[0].SetColor("EmissionColor", Color.white);
            _l4.materials[0].SetColor("EmissionColor", Color.white);
            _l5.materials[0].SetColor("EmissionColor", Color.white);
        }

        public void Init()
        {
        }

        public void Load(RoomObject roomObject)
        {
            _factory = roomObject as Factory;
            setLevelDisplay();
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