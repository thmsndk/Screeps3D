using System.Collections;
using Common;
using Screeps_API;
using Screeps3D.RoomObjects;
using TMPro;
using UnityEngine;

namespace Screeps3D.Effects
{
    public class SpeechEffect : MonoBehaviour
    {
        public const string PATH = "Prefabs/Effects/SpeechEffect";
        
        private const float SpeechDuration = 1;

        [SerializeField] private TextMeshPro _label = default;
        private float _time;
        private Vector3 _endPos;
        private Vector3 _startPos;

        public void Load(RoomObject roomObject, string message, bool isPublic = true)
        {
            if (roomObject.View == null) return;
            
            _startPos = roomObject.View.transform.position + new Vector3(0, 2, 0);
            gameObject.transform.position = _startPos;
            _endPos = roomObject.View.transform.position + new Vector3(0, 4, 0);
            _label.enabled = false;
            _time = 0f;
            _label.text = message;

            var creep = roomObject as Creep;
            if (creep != null)
            {
                var privateAlpha = creep.Owner.UserId != ScreepsAPI.Me.UserId ? 0.0f : 0.5f;// should probably not render it at all if not your say
                _label.alpha = isPublic ? 1f : privateAlpha; // hide private say for now, need to show it for players owning the creep
            }

            StartCoroutine(Display());
        }

        private IEnumerator Display()
        {
            //TODO: Make this look nicer and enable it.
            //yield break; // Disabled until looks better.

            _label.enabled = true;
            
            while (_time < SpeechDuration)
            {
                var factor = _time / SpeechDuration;
                var point = (_endPos - _startPos) * factor + _startPos;
                _label.transform.position = point;
                _time += Time.unscaledDeltaTime;

                _label.transform.rotation = Camera.main.transform.rotation;

                yield return null;
            }
            
            _label.enabled = false;
            PoolLoader.Return(PATH, gameObject);
        }
    }
}