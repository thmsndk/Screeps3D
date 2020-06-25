using Common;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class PortalView : MonoBehaviour, IMapViewComponent
    {
        public const string Path = "Prefabs/RoomObjects/portal";

        [SerializeField] private ScaleVisibility _vis = default;
        [SerializeField] private Collider _collider = default;
        [SerializeField] private Light _light;


        public void Update() {            
            _light.intensity = 1f + Mathf.PingPong(Time.time, 3f);
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