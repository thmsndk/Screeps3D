using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class BumpView : MonoBehaviour, IObjectViewComponent
    {

        [SerializeField] private Transform _bumpRoot;
        private IBump _creep;
        private Vector3 _bumpTarget;
        private Vector3 _bumpRef;
        private bool _bumping;
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

            _bumpTarget = (_creep.BumpPosition - _creep.PrevPosition) * .2f;
            _bumping = true;
            _animating = true;
        }

        public void Unload(RoomObject roomObject)
        {
            _creep = null;
        }

        private void Update()
        {
            if (_creep == null || !_animating)
                return;

            var target = Vector3.zero;
            var speed = .2f;
            if (_bumping)
            {
                target = _bumpTarget;
                speed = .1f;
            }

            _bumpRoot.transform.localPosition =
                Vector3.SmoothDamp(_bumpRoot.transform.localPosition, target, ref _bumpRef, speed);

            var sqrMag = (_bumpRoot.transform.localPosition - target).sqrMagnitude;
            if (sqrMag < .0001f)
            {
                if (_bumping)
                    _bumping = false;
                else
                    _animating = false;
            }
        }
    }
}