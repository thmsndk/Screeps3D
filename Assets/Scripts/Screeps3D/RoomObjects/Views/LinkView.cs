using Screeps3D.Effects;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class LinkView : MonoBehaviour, IObjectViewComponent
    {
        private LineRenderer _lineRenderer;
        private Link _link;

        public void Init()
        {
            _lineRenderer = gameObject.GetComponent<LineRenderer>();
        }

        public void Load(RoomObject roomObject)
        {
            _link = roomObject as Link;
        }

        public void Delta(JSONObject data)
        {
            if (_link == null)
            {
                return;
            }

            if (_link.Actions.TryGetValue("transferEnergy", out var action))
            {
                if (action.IsNull) return;

                EffectsUtility.Beam(_link, action, new BeamConfig(Color.yellow, 0.5f, 0.5f));
                // StartCoroutine(Beam.Draw(_link, action, _lineRenderer, new BeamConfig(Color.yellow, 0.5f, 0.5f)));
            }
        }

        public void Unload(RoomObject roomObject)
        {
        }
    }
}