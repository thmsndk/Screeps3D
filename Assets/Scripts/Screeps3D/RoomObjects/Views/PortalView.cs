using Common;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class PortalView : MonoBehaviour, IMapViewComponent
    {
        public const string Path = "Prefabs/RoomObjects/portal";

        [SerializeField] private ScaleVisibility _vis;
        [SerializeField] private Collider _collider;
        
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