using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class BumpView : MonoBehaviour, IObjectViewComponent
    {

        [SerializeField] private Transform _bumpRoot = default;
        private IBump _creep;
        private Vector3 _localTargetPos;
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

            _localTargetPos= (_creep.BumpPosition - (_creep as RoomObject).Position);
            // _bumpTarget = (_creep.BumpPosition - (_creep as RoomObjects).Position) * .2f;
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

            var localBase = Vector3.zero;
            var targetLocalPos = localBase;
            var speed = .2f;
            if (_bumping)
            {
                targetLocalPos = _localTargetPos;
                speed = .1f;
            }
            // creep IS rotated towards source/action so we just need to go forward via Z, and do not care about X axis
            targetLocalPos.x = 0f;
            targetLocalPos.y = 0f;

            _bumpRoot.transform.localPosition =
                Vector3.SmoothDamp(_bumpRoot.transform.localPosition, targetLocalPos, ref _bumpRef, speed);

            var sqrMag = (_bumpRoot.transform.localPosition - targetLocalPos).sqrMagnitude;
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