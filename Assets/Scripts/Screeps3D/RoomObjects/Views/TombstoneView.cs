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
        [SerializeField] private Renderer _body = default;
        //[SerializeField] private Transform _rotationRoot = default;

        private Quaternion _rotTarget;
        private Vector3 _posTarget;
        private Vector3 _posRef;
        private Tombstone _tombstone;
        private IEnumerator _blink;
        private float _emissionDecayFactor;
        private float _nextBlink;
        private bool _blinking;
        private float _minBadgeEmission = 2f;
        private float _maxAddedEmission = 8f;
        private float _currentAddedEmission = 6f;
        private long _lastTickUpdate;
        private long _loadTick;


        internal override void Load(RoomObject roomObject)
        {
            base.Load(roomObject);            
            _tombstone = roomObject as Tombstone;
            // Base setup
            _body.materials[1].SetColor("EmissionColor", new Color(0.7f, 0.7f, 0.7f, 1f));
            _body.materials[1].SetTexture("EmissionTexture", _tombstone?.Owner?.Badge);
            _body.materials[1].SetFloat("EmissionStrength", 2);

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
                return;
            }
            // if tick changed, degrade currentAddedEmission
            if(_lastTickUpdate != ScreepsAPI.Time) {
                long now = ScreepsAPI.Time;
                _lastTickUpdate = now;
                float decayFactor = (float)now / ((float)_tombstone.DeathTime + (float)_tombstone.NextDecayTime);
                _currentAddedEmission =  _maxAddedEmission * decayFactor;
            }
            float decayEmission = _minBadgeEmission + Mathf.Abs(Mathf.Sin(Time.time)) * _currentAddedEmission;
            _body.materials[1].SetFloat("EmissionStrength", decayEmission);
        }
    }
}