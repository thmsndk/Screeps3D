using System.Linq;
using Common;
using Screeps3D.Effects;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class LabView : MonoBehaviour, IObjectViewComponent
    {
        [SerializeField] private ScaleAxes _mineral = default;
        [SerializeField] private ScaleAxes _energy = default;
        private LineRenderer _lineRenderer;
        private Lab _lab;

        public void Init()
        {
            _lineRenderer = gameObject.GetComponent<LineRenderer>();
        }

        public void Load(RoomObject roomObject)
        {
            _lab = roomObject as Lab;
            UpdateDisplays();
        }

        public void Delta(JSONObject data)
        {
            UpdateDisplays();
        }

        public void Unload(RoomObject roomObject)
        {
            _lab = null;
        }

        private void UpdateDisplays()
        {
            if (_lab != null)
            {

                _mineral.SetVisibility(_lab.ResourceAmount / _lab.ResourceCapacity /*3000*/);
                var energy = _lab.Store.ContainsKey(Constants.TypeResource) ? _lab.Store[Constants.TypeResource] : 0f;
                var energyCapacity = _lab.Capacity.ContainsKey(Constants.TypeResource) ? _lab.Capacity[Constants.TypeResource] : 0f;
                _energy.SetVisibility(energy / energyCapacity);


                var action = _lab.Actions.FirstOrDefault(c => !c.Value.IsNull);
                if (action.Value == null)
                    return; // Early

                var data = action.Value;

                var start1 = PosUtility.Convert((int)data["x1"].n, (int)data["y1"].n, _lab.Room) + Vector3.up * .6f;
                var start2 = PosUtility.Convert((int)data["x2"].n, (int)data["y2"].n, _lab.Room) + Vector3.up * .6f;
                var endPos = _lab.Position + Vector3.up * .3f;
                EffectsUtility.Beam(start1, endPos, Color.white); // TODO: color beams based on reactions being made?
                EffectsUtility.Beam(start2, endPos, Color.white);
            }
            // StartCoroutine(Beam.Draw(_lab, action.Value, _lineRenderer, new BeamConfig(Color.white, 0.6f, 0.3f)));
        }
    }
}