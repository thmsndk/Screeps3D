using UnityEngine;

namespace Screeps3D.Rooms.Views
{
    public class PlainsView : MonoBehaviour, IRoomViewComponent
    {
        private const string BaseColor = "BaseColor";
        private Renderer _rend;
        private float _original;
        private float _current;
        private float _target;
        private float _targetRef;

        public void Init(Room room)
        {
            room.OnShowObjects += ManageHighlight;
        }

        private void ManageHighlight(bool showObjects)
        {
            if (showObjects)
            {
                Highlight();
            }
            else
            {
                Dim();
            }
        }

        public void Highlight()
        {
            if (!_rend)
            {
                _rend = GetComponent<Renderer>();
                _rend.transform.localPosition = _rend.transform.localPosition + (Vector3.up * 0.1f);
                // _original = _rend.materials[0].GetFloat("EmissionStrength");
                // var baseColor = _rend.material.GetColor(BaseColor);
                // _original = baseColor.r;
            }
            // _target = _original + .05f;
            enabled = true;
        }

        public void Dim()
        {
            _rend.transform.localPosition = _rend.transform.localPosition - (Vector3.up * 0.1f);
            _target = _original;
            enabled = true;
        }

        public void Update()
        {
            if (!_rend || Mathf.Abs(_current - _target) < 0f)
            {
                enabled = false;
                return;
            }
            // var baseVal = _rend.materials[0].GetFloat("EmissionStrength");
            // _current = Mathf.SmoothDamp(baseVal, _target, ref _targetRef, 1);
            // _rend.materials[0].SetFloat("EmissionStrength", _current);
        }
    }
}