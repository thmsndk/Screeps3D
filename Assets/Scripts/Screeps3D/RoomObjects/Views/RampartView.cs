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

            // Enemy rampart detection
            if (_rampart.Owner.UserId != ScreepsAPI.Me.UserId) // TODO: isNPC?
            {
                renderer.material.SetColor("_Color", new Color(1.000f, 0f, 0f, 0.053f));
                renderer.material.SetColor("_EmissionColor", new Color(0.400f, 0f, 0f, 0.278f));

            }
            else
            {
                // Owned rampart color, extracted from debug.log
                renderer.material.SetColor("_Color", new Color(0.000f, 1.000f, 0.297f, 0.053f));
                renderer.material.SetColor("_EmissionColor", new Color(0.000f, 0.400f, 0.119f, 0.278f));
            }
        }

        public void Delta(JSONObject data)
        {
        }

        public void Unload(RoomObject roomObject)
        {
        }
    }
}