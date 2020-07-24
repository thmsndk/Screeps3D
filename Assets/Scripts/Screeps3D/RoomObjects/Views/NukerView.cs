using Common;
using UnityEngine;
using Screeps_API;

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
        private float _cooldownLeft = 0f;


        private void showLoadLevels() {
            _ghodiumScale.SetVisibility(_nuker.ResourceAmount/_nuker.ResourceCapacity);

            var energy = _nuker.Store.ContainsKey(Constants.TypeResource) ? _nuker.Store[Constants.TypeResource] : 0f;
            var energyCapacity = _nuker.Capacity.ContainsKey(Constants.TypeResource) ? _nuker.Capacity[Constants.TypeResource] : 0f;
            _energyScale.SetVisibility(energy / energyCapacity);
            _cdScale.SetVisibility(1 - (_cooldownLeft / _nuker.maxCooldown));
        }

        private void hideNukeIfCooldown() {
            if (_cooldownLeft > 0) {
                _nuke.enabled = false;
                _cdRenderer.materials[0].SetColor("EmissionColor", Color.red);
            } else {
                _nuke.enabled = true;
                _cdRenderer.materials[0].SetColor("EmissionColor", Color.green);
                _nuke.materials[1].SetTexture("EmissionTexture", _nuker?.Owner?.Badge);
                _nuke.materials[1].SetFloat("EmissionStrength", .1f);
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
            _cooldownLeft = Mathf.Max(0, _nuker.CooldownTime - ScreepsAPI.Time);
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