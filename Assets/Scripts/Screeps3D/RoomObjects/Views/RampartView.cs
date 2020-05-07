using Common;
using Screeps_API;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class RampartView : MonoBehaviour, IObjectViewComponent
    {
        [SerializeField] private Renderer renderer;

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

            bool ownedByMe = owner?.UserId != ScreepsAPI.Me.UserId; // TODO: isNPC?;

            var color = new Color(0.000f, 1.000f, 0.297f, 0.053f); // enemy
            var emissionColor = new Color(0.000f, 0.400f, 0.119f, 0.278f); // enemy;


            if (ownedByMe)
            {
                color = new Color(1.000f, 0f, 0f, 0.053f);
                emissionColor = new Color(0.400f, 0f, 0f, 0.278f);
            }

            if (_rampart == null)
            {
                // Should only happen for ruins
                //renderer.material.SetColor("_Color", Color.grey);
                //renderer.material.SetColor("_EmissionColor", Color.grey);
            }

            renderer.material.SetColor("_BaseColor", color);
            
            renderer.material.SetColor("_EmissiveColor", emissionColor);
            //renderer.material.SetColor("_EmissionColor", emissionColor);
        }

        public void Delta(JSONObject data)
        {
        }

        public void Unload(RoomObject roomObject)
        {
        }
    }
}