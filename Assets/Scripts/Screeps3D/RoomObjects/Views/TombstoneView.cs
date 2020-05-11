using System.Collections;
using System.Linq;
using Common;
using Screeps3D.Effects;
using Screeps_API;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    internal class TombstoneView : ObjectView
    {
        [SerializeField] private Renderer _body;
        //[SerializeField] private Transform _rotationRoot;

        private Quaternion _rotTarget;
        private Vector3 _posTarget;
        private Vector3 _posRef;
        private Tombstone _tombstone;
        private IEnumerator _blink;
        private float _emissionDecayFactor;
        private float _nextBlink;
        private bool _blinking;
        private long _maxEmission = 8;
        private long _minEmission = 2;
        private float _currentEmission;
        private long _lastTickUpdate;
        private long _loadTick;


        internal override void Load(RoomObject roomObject)
        {
            base.Load(roomObject);            
            _tombstone = roomObject as Tombstone;
            long now = ScreepsAPI.Time;

            long deathNowDiff = now - _tombstone.DeathTime;

            _emissionDecayFactor = (float) (_maxEmission - _minEmission) / (deathNowDiff + (long)_tombstone.NextDecayTime - now);
            _currentEmission = _maxEmission - deathNowDiff * _emissionDecayFactor;
            _lastTickUpdate = now;

            // Base setup
            _body.materials[1].SetColor("EmissionColor", new Color(0.7f, 0.7f, 0.7f, 1f));
            _body.materials[1].SetTexture("EmissionTexture", _tombstone?.Owner?.Badge);
            _body.materials[1].SetFloat("EmissionStrength", _currentEmission);

            _rotTarget = transform.rotation;
            _posTarget = roomObject.Position;

            // should be ObjectViewComponent so Load is run everytime?
            if (!string.IsNullOrEmpty(_tombstone?.Saying))
            {
                EffectsUtility.Speech(_tombstone, _tombstone.Saying);
            }
        }

        internal override void Delta(JSONObject data)
        {
            base.Delta(data);


            //var posDelta = _posTarget - RoomObject.Position;

            //if (posDelta.sqrMagnitude > .01)
            //{
            //    _posTarget = RoomObject.Position;
            //} 
        }

        private void Update()
        {
            if (_tombstone == null) {
                if(_blinking != null) {
                    StopCoroutine(_blink);
                }
                return;
            }
            // if tick changed
            if(_lastTickUpdate != ScreepsAPI.Time) {
                _currentEmission = _body.materials[1].GetFloat("EmissionStrength") - _emissionDecayFactor;
                _lastTickUpdate = ScreepsAPI.Time;
            }

            if(!_blinking ) {
                _blink = Blink();
                StartCoroutine(_blink);
            } 
        }

        private IEnumerator Blink()
        {
            _blinking = true;
            while (_body.materials[1].GetFloat("EmissionStrength") < _currentEmission)
            {
                _body.materials[1].SetFloat("EmissionStrength", _body.materials[1].GetFloat("EmissionStrength") + 0.1f);
                yield return null;
            }
            while (_body.materials[1].GetFloat("EmissionStrength") > _minEmission)
            {
                _body.materials[1].SetFloat("EmissionStrength", _body.materials[1].GetFloat("EmissionStrength") - 0.1f);
                yield return null;
            }
            _nextBlink = Time.time + Random.value + 1;
            _blinking = false;
        }        
    }
}