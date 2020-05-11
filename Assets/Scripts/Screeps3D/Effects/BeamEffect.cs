using System.Collections;
using Common;
using UnityEngine;

namespace Screeps3D.Effects
{
    public class BeamEffect : MonoBehaviour
    {
        public const string PATH = "Prefabs/Effects/BeamEffect";
        
        private const float BeamDuration = 1;
        private const float HalfDuration = BeamDuration / 2;

        [SerializeField] private LineRenderer lineRenderer = default;
        private float _time;
        private Vector3 _startPos;
        private Vector3 _endPos;

        public void Load(Vector3 startPos, Vector3 endPos, Color color)
        {
            _time = 0f;
            _startPos = startPos;
            _endPos = endPos;
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;

            StartCoroutine(Fire());
        }

        private IEnumerator Fire()
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, _startPos);
            while (_time < HalfDuration)
            {
                var factor = _time / HalfDuration;
                var point = (_endPos - _startPos) * factor + _startPos;
                lineRenderer.SetPosition(1, point);
                _time += Time.unscaledDeltaTime;
                yield return null;
            }
            
            lineRenderer.SetPosition(1, _endPos);
            while (_time < BeamDuration)
            {
                var factor = (_time - HalfDuration) / HalfDuration;
                var point = (_endPos - _startPos) * factor + _startPos;
                lineRenderer.SetPosition(0, point);
                _time += Time.unscaledDeltaTime;
                yield return null;
            }
            
            lineRenderer.enabled = false;
            PoolLoader.Return(PATH, gameObject);
        }
    }
}