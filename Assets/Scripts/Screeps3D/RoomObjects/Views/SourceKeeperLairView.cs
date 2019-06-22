using Common;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class SourceKeeperLairView : MonoBehaviour, IObjectViewComponent
    {
        private SourceKeeperLair _sourceKeeperLair;

        public void Init()
        {
        }

        public void Load(RoomObject roomObject)
        {
            _sourceKeeperLair = roomObject as SourceKeeperLair;
        }

        public void Delta(JSONObject data)
        {
        }

        public void Unload(RoomObject roomObject)
        {
        }
    }
}