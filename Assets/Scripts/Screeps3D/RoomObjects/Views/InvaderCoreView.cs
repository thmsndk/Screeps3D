using System.Collections;
using System.Linq;
using Common;
using Screeps3D.Effects;
using Screeps_API;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{    
    public class InvaderCoreView : MonoBehaviour, IObjectViewComponent
    {
        [SerializeField] private Renderer _decayDisplay;
        [SerializeField] private Renderer _core;
        [SerializeField] private Renderer _walls;
        [SerializeField] private Renderer _top;
        [SerializeField] private Renderer _topL1;
        [SerializeField] private Renderer _topL2;
        [SerializeField] private Renderer _topL3;
        [SerializeField] private Renderer _topL4;
        [SerializeField] private Renderer _topL5;
        [SerializeField] private Transform _rotationRoot;

        private Quaternion _targetRot;
        private InvaderCore _invadeCore;
        private Color _actionColor = Color.red;
        private LineRenderer _lineRenderer;
        private IEnumerator _pulse;
        private bool _pulsing;
        private bool _idle;
        private bool _invunerable;
        private float _invProgress;

        private long _lastTickUpdate = 0;

        public void Init()
        {
            _lineRenderer = gameObject.GetComponent<LineRenderer>();
            _invProgress = 0;
        }

        private void pulseEmission() {

            float tSin = Mathf.Sin(Time.time);

            // Texture
            _core.materials[0].SetColor("EmissionColor", _actionColor);
            _core.materials[0].SetFloat("EmissionStrength", .1f + Mathf.Abs(tSin) * .4f);

            // decay on top
            float decayEmission = .1f + Mathf.Abs(tSin) * .4f;
            _decayDisplay.materials[0].SetFloat("EmissionStrength", decayEmission);
            iterateOverLevelsAndSetEmission(decayEmission);
        }

        private void iterateOverLevelsAndSetEmission(float emissionStr) {
            Renderer[] levels = {_topL1, _topL2, _topL3, _topL4, _topL5};
            for(var i = 0; i < levels.Length; i++) {
                levels[i].materials[0].SetFloat("EmissionStrength", emissionStr);
            }
        }

        private void scaleDecayBall(float factor) {
            _decayDisplay.transform.localScale = Vector3.one * factor;
        }

        private void dimNotUsedLevels() {
            int level = 0;
            if(_invadeCore?.Level != null ) {
                level = _invadeCore.Level;
            }
            
            _topL5.enabled = level > 4;
            _topL4.enabled = level > 3;
            _topL3.enabled = level > 2;
            _topL2.enabled = level > 1;
            _topL1.enabled = level > 0;
        }
        public void Load(RoomObject roomObject)
        {
            _invadeCore = roomObject as InvaderCore;
            _actionColor = Color.red;
            _pulsing = false;
            dimNotUsedLevels();
        }

        public void Delta(JSONObject data)
        {
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
                }
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
            long now = ScreepsAPI.Time;
            if(_lastTickUpdate < now) {
                _invunerable = false;
                _lastTickUpdate = now;
                foreach(var e in _invadeCore.Effects) {
                    if(e.Effect.ToString() == "EFFECT_COLLAPSE_TIMER") {
                        long leftToTick = e.EndTime - now;
                        float progressLeft = Mathf.Round((float)leftToTick / (float)e.Duration * 100f) / 100f;
                        scaleDecayBall(progressLeft);
                    }

                    if(e.Effect.ToString() == "EFFECT_INVULNERABILITY") {
                        float leftToTick = Mathf.Max(0, e.EndTime - now);
                        _invunerable = leftToTick > 0;
                        _invProgress = Mathf.Round((float)leftToTick / (float)e.Duration * 100f) / 100f;
                    }
                }
            }
            if(_invProgress > 0) {
                var speed = 100f * _invProgress;
                _walls.transform.Rotate(Vector3.forward * speed * Time.deltaTime);
            }
            pulseEmission();
            return;
        }
    
        public void Unload(RoomObject roomObject)
        {
        }
    }
}