using Assets.Scripts.Screeps3D.World.Views;
using Common;
using UnityEngine;

namespace Screeps3D.World.Views
{
    public class WorldView : MonoBehaviour
    {
        //[SerializeField] private ScaleVisibility _vis = default;

        public WorldViewData Data { get; private set; }
        private IWorldOverlayViewComponent[] _viewComponents;

        public void Init(WorldViewData data)
        {
            //_vis.Show();
            Data = data;

            _viewComponents = GetComponentsInChildren<IWorldOverlayViewComponent>();
            foreach (var component in _viewComponents)
            {
                component.Init(data);
            }
        }
    }
}