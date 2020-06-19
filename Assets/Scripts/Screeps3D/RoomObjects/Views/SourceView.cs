using Common;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class SourceView : MonoBehaviour, IObjectViewComponent, IMapViewComponent
    {
        public const string Path = "Prefabs/RoomObjects/source";

        [SerializeField] private ScaleVisibility _vis = default;
        [SerializeField] private Collider _collider = default;
        [SerializeField] private Renderer _sourceRend;
        private Source _source;
        private Component[] _lights;
        private Light _l;

        private float _lightMax = 1.5f;
        private float _emissionMax = 5f;
        

        public void Init()
        {
            _lights = GetComponentsInChildren(typeof(Light));
        }

        public void Load(RoomObject roomObject)
        {
            _source = roomObject as Source;
        }

        public void Delta(JSONObject data)
        {
            var percentage = _source.Energy / _source.EnergyCapacity;
            foreach (Light l in _lights)
            {                
                l.intensity = _lightMax * percentage;
            }
            if(percentage == 0 ) {
                _sourceRend.materials[0].SetFloat("EmissionEV", 0f);
            } else {
                _sourceRend.materials[0].SetFloat("EmissionEV", _emissionMax);
            }
            // var minVisibility = 0.001f; /*to keep it visible and selectable, also allows the resource to render again when regen hits*/
            // var maxVisibility = 1f;

            // // http://james-ramsden.com/map-a-value-from-one-number-scale-to-another-formula-and-c-code/
            // float minimum = Mathf.Log(minVisibility);
            // float maximum = Mathf.Log(maxVisibility);

            // // Scale the visibility in such a way that a lot of the model is rendered above 50% energy

            // float current = Mathf.Log(percentage == 0 ? minVisibility : percentage);

            // // Map range to visibility range
            // var visibility = minVisibility + (maxVisibility - minVisibility) * ((current - minimum) / (maximum - minimum));

            // _vis.SetVisibility(maxVisibility);
        }

        public void Unload(RoomObject roomObject)
        {
        }
        
        // IMapViewComponent *****************
        public int roomPosX { get; set; }
        public int roomPosY { get; set; }
        public void Show()
        {
            _vis.Show();
            _collider.enabled = false;
        }
        public void Hide()
        {
            _vis.Hide();
            _collider.enabled = true;
        }
    }
}