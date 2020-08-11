using UnityEngine;
using Common;
using System.Collections.Generic;

namespace Screeps3D.RoomObjects.Views
{
    internal class ResourceView : ObjectView
    {
        [SerializeField] private Renderer _renderer = default;
        [SerializeField] private ScaleAxes _scale = default;
        [SerializeField] private ParticleSystem _ps = default;

        [SerializeField] private Material[] _materials = default;
        private const int energyMaterialIndex = 0;
        private const int powerMaterialIndex = 1;
        private const int commonMaterialIndex = 2;

        private bool _initialized;
        private Resource _resource;

        internal override void Load(RoomObject roomObject)
        {
            base.Load(roomObject);
            _initialized = false;
            _resource = roomObject as Resource;
            _scale.SetVisibility(1.0f);
        }

        internal override void Delta(JSONObject data)
        {
            base.Delta(data);
        }

        private void scaleMesh() {
            if (_resource.ResourceType.Equals("energy")) {
                _scale.SetVisibility(0.6f * Mathf.Min(1000.0f, _resource.ResourceAmount) / 1000.0f);
            }
            else {
                _scale.SetVisibility(0.92f * Mathf.Min(1500.0f, _resource.ResourceAmount) / 1500.0f);
            }
        }

        private void initialize() {
            var main = _ps.main;
            switch(_resource.ResourceType) {
                case "energy":
                    break;
                case "power":
                    _renderer.materials[0].SetColor("EmissionColor", new Color(.3f, .0f, .0f));
                    main.startColor = new Color(1f, 0f, 0f);
                    break;
                default:
                    _renderer.materials[0].SetColor("EmissionColor", new Color(.3f, .3f, .3f));
                    main.startColor = new Color(1f, 1f, 1f);
                    break;
            }

            _initialized = true;
        }

        private void Update()
        {
            if (_resource == null){
                return;
            }

            scaleMesh();

            if(_initialized) {
                initialize();
            }

        }
    }
}