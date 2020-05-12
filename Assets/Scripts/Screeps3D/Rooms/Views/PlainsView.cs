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
                var baseColor = _rend.material.GetColor(BaseColor);
                _original = baseColor.r;
            }
            _target = _original + .1f;
            enabled = true;
        }

        public void Dim()
        {
            _target = _original;
            enabled = true;
        }

        public void Update()
        {
            if (!_rend || Mathf.Abs(_current - _target) < .001f)
            {
                enabled = false;
                return;
            }
            var baseColor = _rend.material.GetColor(BaseColor);

            _current = Mathf.SmoothDamp(baseColor.r, _target, ref _targetRef, 1);
            _rend.material.SetColor(BaseColor, new Color(_current, _current, _current));
        }
    }
}