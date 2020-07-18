using Common;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class WallView: MonoBehaviour, IMapViewComponent
    {
        public const string Path = "Prefabs/RoomObjects/constructedWall";

        [SerializeField] private ScaleVisibility _vis = default;
        [SerializeField] private MeshRenderer _rend = default;
        [SerializeField] private Collider _collider = default;
        
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