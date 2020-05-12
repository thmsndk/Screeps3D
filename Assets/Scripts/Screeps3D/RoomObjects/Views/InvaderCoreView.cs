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
        private bool _idle;
        private float _time;
        private float _nextRot;
        private bool _rotating;
        private InvaderCore _invadeCore;
        private Color _actionColor;
        private LineRenderer _lineRenderer;
        private IEnumerator _powerUp;
        private IEnumerator _rotator;

        public void Init()
        {
            _lineRenderer = gameObject.GetComponent<LineRenderer>();
        }

        private void setEmission(Color color, float strength) {

            _top.material.SetFloat("EmissionStrength", strength);
            _top.material.SetColor("EmissionColor", color);

            _decayDisplay.material.SetFloat("EmissionStrength", strength);
            _decayDisplay.material.SetColor("EmissionColor", color);            
        }

        public void Load(RoomObject roomObject)
        {
            _invadeCore = roomObject as InvaderCore;
            _time = 0f;
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
                    _idle = true;
                    return; // Early
                }
                _idle = false;
                if (_rotator != null) StopCoroutine(_rotator);

                var endPos = PosUtility.Convert(action.Value, _invadeCore.Room);
                _rotationRoot.rotation = Quaternion.LookRotation(endPos - _invadeCore.Position);
                _actionColor = action.Key == "attack" ? Color.blue : action.Key == "heal" ? Color.green : Color.yellow;
                EffectsUtility.Beam(_invadeCore, action.Value, new BeamConfig(_actionColor, 0.6f, 0.3f));
                
                _powerUp = PowerUp();
                StartCoroutine(_powerUp);
            }
            // StartCoroutine(Beam.Draw(_invadeCore, action.Value, _lineRenderer, new BeamConfig(color, 0.6f, 0.3f)));
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

            if(_idle) {
                setEmission(Color.black, 0f);
            }

            if (!_idle || _rotating || !(Time.time > _nextRot ))  {
                return; // Early
            }            

            _rotator = Rotate();
            StartCoroutine(_rotator);
        }

        private IEnumerator Rotate()
        {
            var direction = Random.value > 0.5 ? 1 : -1;
            _targetRot = _rotationRoot.rotation * Quaternion.Euler(0, 180 * Random.value * direction, 0);
            _rotating = true;
            while (_rotationRoot.rotation != _targetRot)
            {
                _rotationRoot.rotation = Quaternion.Slerp(_rotationRoot.rotation, _targetRot, Time.deltaTime);
                yield return null;
            }
            _nextRot = Time.time + Random.value + 1;
            _rotating = false;
        }

        private IEnumerator PowerUp() {
            var targetEmission = 150;
            setEmission(_actionColor, 0f);
            // powerUp - brightness up
            while (_core.material.GetFloat("EmissionStrength") < targetEmission)
            {    
                setEmission(_actionColor, _core.material.GetFloat("EmissionStrength") +15f);
                yield return null;
            }
            // powerUp - wind down
            while (_core.material.GetFloat("EmissionStrength") > 0)
            {    
                setEmission(_actionColor, _core.material.GetFloat("EmissionStrength") -5f);
                yield return null;
            }
            setEmission(Color.black, 0f);
        }
    }
}