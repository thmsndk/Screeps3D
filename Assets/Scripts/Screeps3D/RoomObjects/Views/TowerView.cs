using System.Collections;
using System.Linq;
using Common;
using Screeps3D.Effects;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class TowerView : ObjectView, IObjectViewComponent
    {
        [SerializeField] private ScaleAxes _energyDisplay;
        [SerializeField] private Renderer _body;
        [SerializeField] private Transform _rotationRoot;
        private Quaternion _targetRot;
        private bool _idle;
        private Color _actionColor;
        private float _time;
        private float _nextRot;
        private bool _rotating;
        private bool _doPowerUp;
        private Tower _tower;
        private LineRenderer _lineRenderer;
        private IEnumerator _rotator;
        private IEnumerator _powerUp;

        public void Init()
        {
            _lineRenderer = gameObject.GetComponent<LineRenderer>();
        }

        public void Load(RoomObject roomObject)
        {
            _tower = roomObject as Tower;
            _time = 0f;
            AdjustScale();
        }

        public void Delta(JSONObject data)
        {
            AdjustScale();

            if (_tower != null)
            {
                var action = _tower.Actions.FirstOrDefault(c => !c.Value.IsNull);
                if (action.Value == null)
                {
                    _idle = true;
                    if(_powerUp != null) {
                        StopCoroutine(_powerUp);
                    }
                    return; // Early
                }
                _idle = false;
                if (_rotator != null) StopCoroutine(_rotator);

                var endPos = PosUtility.Convert(action.Value, _tower.Room);
                _rotationRoot.rotation = Quaternion.LookRotation(endPos - _tower.Position);
                _actionColor = action.Key == "attack" ? Color.blue : action.Key == "heal" ? Color.green : Color.yellow;
                EffectsUtility.Beam(_tower, action.Value, new BeamConfig(_actionColor, 0.6f, 0.3f));

                _powerUp = PowerUp();
                if(_powerUp != null) {
                    StopCoroutine(_powerUp);
                }
                StartCoroutine(_powerUp);
            }
            // StartCoroutine(Beam.Draw(_tower, action.Value, _lineRenderer, new BeamConfig(color, 0.6f, 0.3f)));
        }

        public void Unload(RoomObject roomObject)
        {
        }

        private void AdjustScale()
        {
            if (_tower != null)
            {
                _energyDisplay.SetVisibility(_tower.TotalResources / _tower.TotalCapacity);
            }
        }

        private void Update()
        {
            if (_tower == null)
            {
                // A ruin tower should not rotate. 
                // TODO: perhaps we want it to point downwards towards the ground?
                return;
            }

            if(_idle) {
                _body.sharedMaterials[0].SetFloat("EmissionStrength", 0f);
                return;
            }

            if (!_idle || _rotating || !(Time.time > _nextRot || _doPowerUp))  {
            //     _body?.sharedMaterials[0].SetFloat("EmissionStrength", 0f);
            //     _body?.sharedMaterials[0].SetColor("EmissionColor", new Color(0f, 0f, 0f));
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
            _doPowerUp = true;
            var targetEmission = 150;
            var keepPowerTime = 2;            
            _body.sharedMaterials[0].SetFloat("EmissionStrength", 0f);
            _body?.sharedMaterials[0].SetColor("EmissionColor", _actionColor);

            while (_body?.sharedMaterials[0].GetFloat("EmissionStrength") < targetEmission)
            {    

                _body.sharedMaterials[0].SetFloat("EmissionStrength", _body.sharedMaterials[0].GetFloat("EmissionStrength") + 5f);
                yield return new WaitForSeconds(.1f);
            }
        }
    }
}