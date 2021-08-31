using Common;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class TerminalView : MonoBehaviour, IObjectViewComponent
    {
        [SerializeField] private ScaleAxes _energyDisplay = default;
        [SerializeField] private Renderer _terminalStore = default;
        private Terminal _terminal;

        public void Init()
        {
        }

        public void Load(RoomObject roomObject)
        {
            _terminal = roomObject as Terminal;
            AdjustScale();
            _terminalStore.materials[0].SetFloat("EmissionStrength", .05f);
            _terminalStore.materials[0].SetTexture("EmissionTexture", _terminal.CreateStorageTexture());

            _terminalStore.materials[0].SetFloat("EmissionStrength", .05f);
            _terminalStore.materials[0].SetTexture("EmissionTexture", _terminal.CreateStorageTexture());

        }

        public void Delta(JSONObject data)
        {
            AdjustScale();
            _terminalStore.materials[0].SetFloat("EmissionStrength", .05f);
            _terminalStore.materials[0].SetTexture("EmissionTexture", _terminal.CreateStorageTexture());

            _terminalStore.materials[0].SetFloat("EmissionStrength", .05f);
            _terminalStore.materials[0].SetTexture("EmissionTexture", _terminal.CreateStorageTexture());
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