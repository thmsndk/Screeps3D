using System.Collections;
using System.Linq;
using Common;
using Screeps3D.Effects;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class TowerView : MonoBehaviour, IObjectViewComponent
    {
        [SerializeField] private ScaleAxes _energyDisplay = default;
        [SerializeField] private Renderer _base = default;
        [SerializeField] private Renderer _stand = default;
        [SerializeField] private Renderer _body = default;
        [SerializeField] private Transform _rotationRoot = default;
        [SerializeField] private LineRenderer _lineRenderer = default;
        private Quaternion _targetRot;
        private bool _idle;
        private float _nextRot;
        private bool _rotating;
        private Tower _tower;
        private Color _actionColor;

        private IEnumerator _powerUp;
        private IEnumerator _rotator;

        // New shit
        [SerializeField] private Transform _barrelEnd = default;
        private float _maxParabolaHeight = 0.95f;
        private Vector3? _sPos;
        private Vector3? _tPos;
        private float radianAngle = 5f;
        private float _angle = 1f;
        private int _resolution = 1000;
        private int _vertexCount = 12;
        
        private float _gravity = Mathf.Abs(Physics.gravity.y);
        private float _radianAngle = 30f;
        private float _velocity = 5f;


        public void Init()
        {
            _lineRenderer = gameObject.GetComponent<LineRenderer>();
        }

        private void setEmission(Color color, float strength) {

            _base.material.SetFloat("EmissionStrength", strength);
            _base.material.SetColor("EmissionColor", color);

            _body.material.SetFloat("EmissionStrength", strength);
            _body.material.SetColor("EmissionColor", color);

            _stand.material.SetFloat("EmissionStrength", strength);
            _stand.material.SetColor("EmissionColor", color);            
        }

        public void Load(RoomObject roomObject)
        {
            _tower = roomObject as Tower;
            AdjustScale();
        }


        public void Delta(JSONObject data)
        {
            AdjustScale();

            if (_tower != null)
            {
                // DEBUG                
                // _idle = false;
                // var targetPos = _barrelEnd.position + new Vector3(0, -0.95f, 15);
                // EffectsUtility.ArcBeam(_barrelEnd, targetPos, new BeamConfig(Color.green, 0f, 0f));
                // return;
                // end DEBUG

                var action = _tower.Actions.FirstOrDefault(c => !c.Value.IsNull);
                if (action.Value == null)
                {
                    _idle = true;
                    _stand.materials[1].SetColor("EmissionColor", Color.white);
                    return; // Early
                }
                _idle = false;
                if (_rotator != null) StopCoroutine(_rotator);

                var targetPos = PosUtility.Convert(action.Value, _tower.Room);
                _rotationRoot.rotation = Quaternion.LookRotation(targetPos - _tower.Position);
                _actionColor = action.Key == "attack" ? Color.blue : action.Key == "heal" ? Color.green : Color.yellow;

                EffectsUtility.ArcBeam(_barrelEnd, targetPos, new BeamConfig(_actionColor, 0f, 0f));
                _stand.materials[1].SetColor("EmissionColor", _actionColor);
                // EffectsUtility.Beam(_tower, action.Value, new BeamConfig(_actionColor, 0.6f, 0.3f));

                
                // _powerUp = PowerUp();
                // StartCoroutine(_powerUp);
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
            var targetPos = _barrelEnd.position + new Vector3(15, _barrelEnd.position.y * -1 , 15);

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
            var targetEmission = 1f;
            setEmission(_actionColor, 0f);
            // powerUp - brightness up
            while (_base.material.GetFloat("EmissionStrength") < targetEmission)
            {    
                setEmission(_actionColor, _base.material.GetFloat("EmissionStrength") + .025f);
                yield return null;
            }
            // powerUp - wind down
            while (_base.material.GetFloat("EmissionStrength") > 0)
            {    
                setEmission(_actionColor, _base.material.GetFloat("EmissionStrength") - .05f);
                yield return null;
            }
        }
    }
}