using System.Collections;
using System.Linq;
using Common;
using Screeps3D.Effects;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{    
    public class InvaderCoreView : MonoBehaviour, IObjectViewComponent
    {
        [SerializeField] private ScaleAxes _energyDisplay;
        [SerializeField] private Renderer _decayDisplay;
        [SerializeField] private Renderer _core;
        [SerializeField] private Renderer _walls;
        [SerializeField] private Renderer _top;
        [SerializeField] private Transform _rotationRoot;
        private Quaternion _targetRot;
        private InvaderCore _invadeCore;
        private Color _actionColor = Color.red;
        private LineRenderer _lineRenderer;
        private IEnumerator _pulse;
        private bool _pulsing;
        private bool _idle;

        public void Init()
        {
            _lineRenderer = gameObject.GetComponent<LineRenderer>();
        }

        private void pulseEmission() {

            float tSin = Mathf.Sin(Time.time);

            // Texture
            _core.materials[0].SetColor("EmissionColor", _actionColor);
            _core.materials[0].SetFloat("EmissionStrength", 6 + Mathf.Abs(tSin) * 8);

            // decay on top
            _decayDisplay.materials[0].SetFloat("EmissionStrength", 2 + Mathf.Abs(tSin) * 4);         
        }

        public void Load(RoomObject roomObject)
        {
            _invadeCore = roomObject as InvaderCore;
            _actionColor = Color.red;
            _pulsing = false;
            AdjustScale();
        }

        public void Delta(JSONObject data)
        {
            AdjustScale();

            if (_invadeCore != null)
            {
                var action = _invadeCore.Actions.FirstOrDefault(c => !c.Value.IsNull);
                if (action.Value == null)
                {
                    return;
                }
                if(action.Key == "reserveController") {
                    var endPos = PosUtility.Convert(action.Value, _invadeCore.Room);
                    EffectsUtility.Beam(_invadeCore, action.Value, new BeamConfig(_actionColor, 1.8f, 0.8f));
                } else {
                }
                _decayDisplay.materials[0].SetColor("EmissionColor", _actionColor);
            }
        }

        public void Unload(RoomObject roomObject)
        {
        }

        private void AdjustScale()
        {
            if (_invadeCore != null)
            {
                // _energyDisplay.SetVisibility(_invadeCore.TotalResources / _invadeCore.TotalCapacity);
            }
        }

        private void Update()
        {
            if (_invadeCore == null)
            {
                // A ruin tower should not rotate. 
                // TODO: perhaps we want it to point downwards towards the ground?
                return;
            }
            // Debug.Log("Update _invadeCore.Effects.ToString: " +  _invadeCore.Effects.ToString());
            pulseEmission();
            return;
        }

        // private IEnumerator Pulse() {
        //     var targetEmission = 150;
        //     _pulsing = true;
        //     // while(true) {                
        //     // }
        //     _pulsing = false;
        //     Debug.Log("Finished Pulse(), setting _pulsing to " + _pulsing.ToString());
        // }
    }
}