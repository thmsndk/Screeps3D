using UnityEngine;
using Common;

namespace Screeps3D.RoomObjects.Views
{
    internal class ResourceView : ObjectView
    {
        [SerializeField] private Renderer _baseRenderer;
        [SerializeField] private Renderer _topRenderer;
        [SerializeField] private ScaleAxes baseScale;
        [SerializeField] private ScaleAxes topScale;

        private bool _initialized;
        private Resource _resource;

        internal override void Load(RoomObject roomObject)
        {
            base.Load(roomObject);
            _initialized = false;
            _resource = roomObject as Resource;
            baseScale.SetVisibility(1.0f);
            topScale.SetVisibility(1.0f);
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
                {
                    _baseRenderer.material.color = new Color(1.0f, 0.91f, 0.49f);
                    _topRenderer.material.color = new Color(1.0f, 0.91f, 0.49f);
                }
                else
                {
                    _baseRenderer.material.color = new Color(1.0f, 1.0f, 1.0f);
                    _topRenderer.material.color = new Color(1.0f, 1.0f, 1.0f);
                }
                _initialized = true;
            }

            if (_resource.ResourceType.Equals("energy"))
            {
                baseScale.SetVisibility(0.6f * Mathf.Min(1000.0f, _resource.ResourceAmount) / 1000.0f);
                topScale.SetVisibility(0.6f * Mathf.Min(1000.0f, _resource.ResourceAmount) / 1000.0f);
            }
            else
            {
                baseScale.SetVisibility(0.92f * Mathf.Min(1500.0f, _resource.ResourceAmount) / 1500.0f);
                topScale.SetVisibility(0.92f * Mathf.Min(1500.0f, _resource.ResourceAmount) / 1500.0f);
            }
        }
    }
}