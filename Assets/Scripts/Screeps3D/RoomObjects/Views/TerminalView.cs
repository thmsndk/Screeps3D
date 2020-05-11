using Common;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class TerminalView : MonoBehaviour, IObjectViewComponent
    {
        [SerializeField] private ScaleAxes _energyDisplay = default;
        private Terminal _terminal;

        public void Init()
        {
        }

        public void Load(RoomObject roomObject)
        {
            _terminal = roomObject as Terminal;
            AdjustScale();
        }

        public void Delta(JSONObject data)
        {
            AdjustScale();
        }

        public void Unload(RoomObject roomObject)
        {
        }

        private void AdjustScale()
        {
            if (_terminal != null)
            {
                _energyDisplay.SetVisibility(_terminal.TotalResources / _terminal.TotalCapacity);
            }
        }
    }
}