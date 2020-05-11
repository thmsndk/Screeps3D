using TMPro;
using UnityEngine;

namespace Unity_Console
{
    public class ConsoleLine : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text = default;
        private float _target;
        private float _targetRef;

        private float Alpha
        {
            get { return _text.color.a; }
            set
            {
                var color = _text.color;
                color.a = value;
                _text.color = color;
            }
        }

        public string Text
        {
            get { return _text.text; }
            set
            {
                _text.text = value;
                _text.parseCtrlCharacters = true;
            }
        }

        public Color Color
        {
            get { return _text.color; }
            set
            {
                _text.color = value;
                Alpha = 0;
            }
        }

        public void Hide()
        {
            enabled = true;
            Alpha = 0;
            _target = 0;
        }

        public void Show()
        {
            enabled = true;
            _target = 1;
        }

        public float Height
        {
            get { return _text.preferredHeight; }
        }

        public void Update()
        {
            if (Mathf.Abs(Alpha - _target) < .01)
            {
                enabled = false;
                return;
            }

            Alpha = Mathf.SmoothDamp(Alpha, _target, ref _targetRef, .2f);
        }
    }
}