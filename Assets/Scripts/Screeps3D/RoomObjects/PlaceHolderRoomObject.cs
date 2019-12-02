using Screeps3D.RoomObjects.Views;

namespace Screeps3D.RoomObjects
{
    public class PlaceHolderRoomObject : OwnedStoreStructure
    {
        protected internal override void AssignView()
        {
            if (Shown && View == null)
            {
                var type = this.Type;

                this.Type = "placeholder";
                View = ObjectViewFactory.Instance.NewView(this);

                this.Type = type;

                if (View)
                {
                    View.Load(this);
                }
            }
        }
    }
}