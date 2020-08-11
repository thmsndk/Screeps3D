using Assets.Scripts.Screeps3D.World.Views;
using Common;
using Screeps3D.World.Views;
using UnityEngine;

namespace Assets.Scripts.Screeps3D.World.Overlay
{
    // Q: Will we be using this for WorldMap overlays?
    public class SingleNukeWorldOverlay : WorldViewData
    {
        //public bool Shown { get; private set; }

        public WorldView View { get; private set; }

        public SingleNukeWorldOverlay()
        {
            this.Type = "nukeMissile";

            Scheduler.Instance.Add(AssignView); // Should probably be done based on Show, like Room, this would allow us to show / hide objects based on how far a player can actually see and possible help with performance.
        }

        protected internal virtual void AssignView()
        {
            View = WorldViewFactory.GetInstance(this);
        }

        private void OnFinishedAnimation(bool isVisible)
        {
            if (isVisible)
                return;

            ////foreach (var component in components)
            ////{
            ////    component.Unload(RoomObject);
            ////}

            ////RoomObject.OnShow -= Show;
            ////RoomObject.DetachView();
            WorldViewFactory.Instance.AddToPool(this.View);
            View = null;
            ////RoomObject = null;
        }
    }
}