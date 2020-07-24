using Common;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class NukerView : MonoBehaviour, IObjectViewComponent
    {
        public const string Path = "Prefabs/RoomObjects/nuker";
        [SerializeField] private ScaleAxes _energyScale = default;
        [SerializeField] private ScaleAxes _cdScale = default;
        [SerializeField] private Renderer _cdRenderer = default;
        [SerializeField] private ScaleAxes _ghodiumScale = default;
        [SerializeField] private Renderer _nuke = default;
        [SerializeField] private Renderer _shell = default;
        private Nuker _nuker;


        private void showLoadLevels() {
            _ghodiumScale.SetVisibility(_nuker.ResourceAmount / _nuker.ResourceCapacity  );

            var energy = _nuker.Store.ContainsKey(Constants.TypeResource) ? _nuker.Store[Constants.TypeResource] : 0f;
            var energyCapacity = _nuker.Capacity.ContainsKey(Constants.TypeResource) ? _nuker.Capacity[Constants.TypeResource] : 0f;
            _energyScale.SetVisibility(energy / energyCapacity);

            _cdScale.SetVisibility(1 - _nuker.Cooldown / _nuker.maxCooldown);
        }

        private void hideNukeIfCooldown() {
            if (_nuker.Cooldown > 0)
            {
                _nuke.enabled = false;
                _cdRenderer.materials[0].SetColor("EmissionColor", Color.red);
            } else {
                _nuke.enabled = true;
                _cdRenderer.materials[0].SetColor("EmissionColor", Color.green);
                _nuke.materials[2].SetTexture("EmissionTexture", _nuker?.Owner?.Badge);
                _nuke.materials[2].SetFloat("EmissionStrength", .1f);
            }
        }


        public void Init()
        {
        }


        public void Load(RoomObject roomObject)
        {
            _nuker = roomObject as Nuker;
        }

        public void Delta(JSONObject data)
        {
            hideNukeIfCooldown();
            showLoadLevels();
        }

        public void Unload(RoomObject roomObject)
        {
        }

        private void Update()
        {
        }
    }
}