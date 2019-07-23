using Common;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class SourceView : MonoBehaviour, IObjectViewComponent, IMapViewComponent
    {
        public const string Path = "Prefabs/RoomObjects/source";

        [SerializeField] private ScaleVisibility _vis;
        [SerializeField] private Collider _collider;
        private Source _source;
        

        public void Init()
        {
        }

        public void Load(RoomObject roomObject)
        {
            _source = roomObject as Source;
        }

        public void Delta(JSONObject data)
        {
            var percentage = _source.Energy / _source.EnergyCapacity;

            var minVisibility = 0.001f; /*to keep it visible and selectable, also allows the resource to render again when regen hits*/
            var maxVisibility = 1f;

            // http://james-ramsden.com/map-a-value-from-one-number-scale-to-another-formula-and-c-code/
            float minimum = Mathf.Log(minVisibility);
            float maximum = Mathf.Log(maxVisibility);

            // Scale the visibility in such a way that a lot of the model is rendered above 50% energy

            float current = Mathf.Log(percentage == 0 ? minVisibility : percentage);

            // Map range to visibility range
            var visibility = minVisibility + (maxVisibility - minVisibility) * ((current - minimum) / (maximum - minimum));

            _vis.SetVisibility(visibility);
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