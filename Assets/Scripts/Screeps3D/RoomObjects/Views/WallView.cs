using Common;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class WallView: MonoBehaviour, IMapViewComponent
    {
        public const string Path = "Prefabs/RoomObjects/constructedWall";

        [SerializeField] private ScaleVisibility _vis;
        [SerializeField] private MeshRenderer _rend;
        
        public int roomPosX { get; set; }
        public int roomPosY { get; set; }

        public void Show()
        {
            _vis.Show();
            var ls = _rend.transform.localScale;
            _rend.transform.localScale = new Vector3(ls.x, 2, ls.z);
        }

        public void Hide()
        {
            _vis.Hide();
        }
    }
}