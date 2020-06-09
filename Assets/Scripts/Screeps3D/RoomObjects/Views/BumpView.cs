using UnityEngine;
using Screeps3D.Effects;

namespace Screeps3D.RoomObjects.Views
{
    public class BumpView : MonoBehaviour, IObjectViewComponent
    {

        [SerializeField] private Transform _bumpRoot = default;
        private IBump _creep;
        private Vector3 _bumpRef;
        private bool _bumping;
        private bool _sparkling;
        private bool _animating;

        public void Init()
        {
        }

        public void Load(RoomObject roomObject)
        {
            _creep = roomObject as IBump;
        }

        public void Delta(JSONObject data)
        {
            if (_creep.BumpPosition == default(Vector3))
                return;

            _bumping = true;
            _animating = true;
            _sparkling = false;
        }

        public void Unload(RoomObject roomObject)
        {
            _creep = null;
        }

        private void Update()
        {
            if (_creep == null || !_animating)
                return;

            var localBase = Vector3.zero;
            var targetLocalPos = localBase;
            var speed = .2f;
            if (_bumping)
            {
                // either half forward (creep is having -z as forward - reasons...)
                targetLocalPos = Vector3.back * .5f;
                speed = .1f;
            }
            // creep IS rotated towards source/action so we just need to go forward via Z, and do not care about X axis
            targetLocalPos.x = 0f;
            targetLocalPos.y = 0f;
            _bumpRoot.transform.localPosition =
                Vector3.SmoothDamp(_bumpRoot.transform.localPosition, targetLocalPos, ref _bumpRef, speed);

            var sqrMag = (_bumpRoot.transform.localPosition - targetLocalPos).sqrMagnitude;
            if(_bumping && sqrMag < .005f && !_sparkling) {
                EffectsUtility.Attack(_creep as RoomObject, _creep.BumpPosition); // For debug
                _sparkling = true;
            }

            if (sqrMag < .0001f)
            {
                if (_bumping) {
                    _bumping = false;
                    _sparkling = false;
                }
                else {
                    _animating = false;
                }
            }
        }
    }
}