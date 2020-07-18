using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace Common
{
    public class NotifyText : BaseSingleton<NotifyText>
    {
        [SerializeField] private TextMeshProUGUI _label = default;
        private float _expire;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="color"></param>
        /// <param name="duration"> duration in seconds untill expiration</param>
        public static void Message(string msg, Color color = default(Color), float duration = 5)
        {
            if (color == default(Color))
                color = Color.white;
            Instance.Msg(msg, color, duration);
        }
        
        private void Msg(string msg, Color color, float duration)
        {
            _label.text = msg;
            _label.color = color;
            _expire = Time.time + duration;
        }

        private void Update()
        {
            if (_expire == 0)
                return;
            var visibility = _expire + 1 - Time.time;
            SetAlpha(visibility);
        }

        private void SetAlpha(float visibility)
        {
            visibility = Mathf.Clamp(visibility, 0, 1);

            var color = _label.color;
            color.a = visibility;
            _label.color = color;
        }
    }
}