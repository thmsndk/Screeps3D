using Common;
using UnityEngine;
using System.Linq;
using Screeps_API;
using System.Collections.Generic;
using Assets.Scripts.Screeps3D.RoomObjects.Season2;

namespace Screeps3D.RoomObjects.Views
{
    public class SymbolDecoderView : MonoBehaviour, IObjectViewComponent
    {
        private SymbolDecoder _sDecoder;
        [SerializeField] private Renderer _base = default;
        [SerializeField] private Light _light = default;
        public void Init()
        {
        }

        public void Load(RoomObject roomObject)
        {
            _sDecoder = roomObject as SymbolDecoder;
            Color32 c = Season2Constants.SymbolToColor(_sDecoder.ResourceType);

            // _base.materials[1].SetFloat("EmissionStrength", 0.35f);
            _base.materials[1].SetFloat("EmissionStrength", 1.5f);
            _base.materials[1].SetColor("EmissionColor", c);
            _light.color = c;
        }

        public void Delta(JSONObject data)
        { }

        public void Unload(RoomObject roomObject)
        {
            _sDecoder = null;
        }
    }
}