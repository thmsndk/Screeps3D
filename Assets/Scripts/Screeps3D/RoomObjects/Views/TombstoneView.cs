using System.Collections;
using System.Linq;
using Common;
using Screeps3D.Effects;
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
        private float _nextBlink;
        private bool _blinking;

        internal override void Load(RoomObject roomObject)
        {
            base.Load(roomObject);
            _tombstone = roomObject as Tombstone;
            _body.materials[1].SetColor("EmissionColor", new Color(0.7f, 0.7f, 0.7f, 1f));
            _body.materials[1].SetTexture("EmissionTexture", _tombstone?.Owner?.Badge);
            _body.materials[1].SetFloat("EmissionStrength", 5f);

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

            if(_blinking || Time.time < _nextBlink ) {
                return;
            }
            
            _blink = Blink();
            StartCoroutine(_blink);
            //transform.localPosition = Vector3.SmoothDamp(transform.localPosition, _posTarget, ref _posRef, .5f);
            //_rotationRoot.transform.rotation = Quaternion.Slerp(_rotationRoot.transform.rotation, _tombstone.Rotation, Time.deltaTime * 5);
        }

        private IEnumerator Blink()
        {
            float min = 2f;
            float max = 8f;
            _blinking = true;
            while (_body.materials[1].GetFloat("EmissionStrength") < max)
            {
                _body.materials[1].SetFloat("EmissionStrength", _body.materials[1].GetFloat("EmissionStrength") + 0.1f);
                yield return null;
            }
            while (_body.materials[1].GetFloat("EmissionStrength") > min)
            {
                _body.materials[1].SetFloat("EmissionStrength", _body.materials[1].GetFloat("EmissionStrength") - 0.1f);
                yield return null;
            }
            _nextBlink = Time.time + Random.value + 1;
            _blinking = false;
        }
        
    }
}