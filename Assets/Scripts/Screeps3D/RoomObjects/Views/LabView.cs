using System.Diagnostics;
using System.Linq;
using Common;
using Screeps3D.Effects;
using UnityEngine;
using Screeps_API;

namespace Screeps3D.RoomObjects.Views
{
    public class LabView : MonoBehaviour, IObjectViewComponent
    {
        [SerializeField] private ScaleAxes _mineral = default;
        [SerializeField] private Renderer _mineralMesh = default;
        [SerializeField] private ScaleAxes _energy = default;
        [SerializeField] private Renderer _energyMesh = default;
        [SerializeField] private ParticleSystem _reactionSmoke = default;
        private LineRenderer _lineRenderer;
        private Lab _lab;
        private Color _reactionColor;

        public void Init()
        {
            _lineRenderer = gameObject.GetComponent<LineRenderer>();
        }

        public void Load(RoomObject roomObject)
        {
            _lab = roomObject as Lab;
            UpdateEnergyStore();
            UpdateMineralStore();
        }

        public void Delta(JSONObject data)
        {
            if (_lab != null)
            {
                UpdateEnergyStore();
                UpdateMineralStore();
                ShowReaction();
            }
        }

        public void Unload(RoomObject roomObject)
        {
            _lab = null;
        }

        private void UpdateMineralStore()
        {
            _lab.SetSingleResourceTexture(_lab.ResourceType);
            _mineral.SetVisibility(_lab.ResourceAmount / _lab.ResourceCapacity /*3000*/);
            _mineralMesh.materials[0].SetFloat("EmissionStrength", .05f);
            _mineralMesh.materials[0].SetFloat("xSize", 0.3f);
            _mineralMesh.materials[0].SetFloat("ySize", 0.3f * _lab.ResourceAmount / _lab.ResourceCapacity);
            _mineralMesh.materials[0].SetTexture("EmissionTexture", _lab._storeTexture);
        }

        private void UpdateEnergyStore()
        {
            var energy = _lab.Store.ContainsKey(Constants.TypeResource) ? _lab.Store[Constants.TypeResource] : 0f;
            var energyCapacity = _lab.Capacity.ContainsKey(Constants.TypeResource) ? _lab.Capacity[Constants.TypeResource] : 0f;
            _energy.SetVisibility(energy / energyCapacity);

            _lab.SetSingleResourceTexture("energy");
            _energyMesh.materials[0].SetFloat("EmissionStrength", .15f);
            _energyMesh.materials[0].SetFloat("xSize", 0.2f);
            _energyMesh.materials[0].SetFloat("ySize", 0.2f * energy / energyCapacity);
            _energyMesh.materials[0].SetTexture("EmissionTexture", Constants.EnergyColorTexture.Get());
        }

        private void ShowReaction()
        {
            if (_lab.CooldownTime > ScreepsAPI.Time)
            {
                if (_reactionColor != null)
                {
                    var main = _reactionSmoke.main;
                    main.startColor = _reactionColor;
                }
                _reactionSmoke.Play();
            }
            else
            {
                _reactionSmoke.Stop();
            }

            var action = _lab.Actions.FirstOrDefault(c => !c.Value.IsNull);
            if (action.Value == null)
                return; // Early

            var data = action.Value;
            _reactionColor = Constants.GetComplexResourceColor(_lab.ResourceType);

            var start1 = PosUtility.Convert((int)data["x1"].n, (int)data["y1"].n, _lab.Room) + Vector3.up * .6f;
            var start2 = PosUtility.Convert((int)data["x2"].n, (int)data["y2"].n, _lab.Room) + Vector3.up * .6f;
            var endPos = _lab.Position + Vector3.up * .3f;
            EffectsUtility.Beam(start1, endPos, _reactionColor);
            EffectsUtility.Beam(start2, endPos, _reactionColor);
            // StartCoroutine(Beam.Draw(_lab, action.Value, _lineRenderer, new BeamConfig(Color.white, 0.6f, 0.3f)));
        }
    }
}