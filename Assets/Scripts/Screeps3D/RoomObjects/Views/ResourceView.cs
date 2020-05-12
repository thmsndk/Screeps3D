using UnityEngine;
using Common;

namespace Screeps3D.RoomObjects.Views
{
    internal class ResourceView : ObjectView
    {
        [SerializeField] private Renderer _renderer = default;
        [SerializeField] private ScaleAxes _scale = default;

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

        private void Update()
        {
            if (_resource == null)
                return;
            
            if (!_initialized)
            {
                if (_resource.ResourceType.Equals("energy"))
                    _renderer.material.color = new Color(1.0f, 0.91f, 0.49f);
                else
                    _renderer.material.color = new Color(1.0f, 1.0f, 1.0f);
                _initialized = true;
            }

            if (_resource.ResourceType.Equals("energy"))
                _scale.SetVisibility(0.6f * Mathf.Min(1000.0f, _resource.ResourceAmount) / 1000.0f);
            else
                _scale.SetVisibility(0.92f * Mathf.Min(1500.0f, _resource.ResourceAmount) / 1500.0f);
        }
    }
}