using Common;
using Screeps_API;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class RampartView : MonoBehaviour, IObjectViewComponent
    {
        [SerializeField] private Renderer rend = default;
        private Rampart _rampart;

        public void Init()
        {
        }

        public void Load(RoomObject roomObject)
        {
            _rampart = roomObject as Rampart;

            var owner = _rampart?.Owner;

            if (_rampart == null)
            {
                var ownedObject = roomObject as IOwnedObject;
                owner = ownedObject?.Owner;
            }

            bool ownedByMe = owner.Username == ScreepsAPI.Me.Username; // TODO: isNPC?;

            // var emissionColor = new Color(1.000f, 0f, 0.297f, 0.053f);
            var emissionColor = new Color32(255, 111, 111, 0); // enemy

            if (ownedByMe)
            {
                // emissionColor = new Color(0f, 1.000f, 0.297f, 0.053f); // me
                emissionColor = new Color32(65, 140, 65, 0); // me
            }

            if (_rampart == null)
            {
                // Should only happen for ruins
                //renderer.material.SetColor("_Color", Color.grey);
                //renderer.material.SetColor("_EmissionColor", Color.grey);
            }            
            rend.materials[0].SetColor("EmissionColor", emissionColor);
        }

        public void Delta(JSONObject data)
        {
        }

        public void Unload(RoomObject roomObject)
        {
        }
    }
}