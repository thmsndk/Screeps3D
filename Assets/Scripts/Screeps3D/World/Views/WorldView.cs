using Common;
using UnityEngine;

namespace Screeps3D.World.Views
{
    public class WorldView : MonoBehaviour
    {
        //[SerializeField] private ScaleVisibility _vis = default;

        public WorldOverlay Overlay { get; private set; }
        private IWorldOverlayViewComponent[] _viewComponents;

        public void Init(WorldOverlay overlay)
        {
            //_vis.Show();
            Overlay = overlay;

            _viewComponents = GetComponentsInChildren<IWorldOverlayViewComponent>();
            foreach (var component in _viewComponents)
            {
                component.Init(overlay);
            }
        }
    }
}