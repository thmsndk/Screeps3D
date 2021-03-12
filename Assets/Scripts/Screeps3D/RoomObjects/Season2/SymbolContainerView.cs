using Common;
using UnityEngine;
using System.Linq;
using Screeps_API;
using System.Collections.Generic;
using Assets.Scripts.Screeps3D.RoomObjects.Season2;

namespace Screeps3D.RoomObjects.Views
{
    public class SymbolContainerView : MonoBehaviour, IObjectViewComponent
    {
        private SymbolContainer _sContainer;
        [SerializeField] private Renderer _base = default;
        [SerializeField] private Light _light = default;
        public void Init()
        {
        }

        public void Load(RoomObject roomObject)
        {
            _sContainer = roomObject as SymbolContainer;
            _light.gameObject.SetActive(true);
            string symbol = new List<string>(_sContainer.Store.Keys)[0];
            Color32 c = Season2Constants.SymbolToColor(symbol);

            // _base.materials[1].SetFloat("EmissionStrength", 0.35f);
            _base.materials[1].SetFloat("EmissionStrength", 1.5f);
            _base.materials[1].SetColor("EmissionColor", c);
            _light.color = c;
        }

        public void Delta(JSONObject data)
        { }

        public void Unload(RoomObject roomObject)
        {
            _sContainer = null;
            _light.gameObject.SetActive(false);
        }
    }
}