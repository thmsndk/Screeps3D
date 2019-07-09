using Common;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class WallView: MonoBehaviour, IMapViewComponent
    {
        public const string Path = "Prefabs/RoomObjects/constructedWall";

        [SerializeField] private ScaleVisibility _vis;
        
        public void Show()
        {
            _vis.Show();
            //SetScaleY(1.0f);
        }

        public void Hide()
        {
            _vis.Hide();
        }
    }
}