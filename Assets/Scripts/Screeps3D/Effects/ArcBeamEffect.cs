using System.Collections;
using Common;
using UnityEngine;

namespace Screeps3D.Effects
{
    public class ArcBeamEffect : MonoBehaviour
    {
        public const string PATH = "Prefabs/Effects/ArcBeamEffect";
        
        private const float BeamDuration = 1;
        private const float HalfDuration = BeamDuration / 2;

        [SerializeField] private LineRenderer lineRenderer = default;
        private float _time;
        private Vector3 _sPos;
        private Vector3 _tPos;
        private float _gravity = Mathf.Abs(Physics.gravity.y);
        private float _radianAngle;
        private int _resolution = 250;
        public float _velocity = 2f;
        public float _angle = 5f;
        private float _maxParabolaHeight = 1f;

        public void Load(Vector3 startPos, Vector3 targetPos, Color color)
        {
            _time = 0f;
            _sPos = startPos;
            _tPos = targetPos;

            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.positionCount = _resolution;
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;

            StartCoroutine(Fire());
            // if(lineRenderer.enabled)
            // lineRenderer.SetPositions(CalculateArcArray());
        }

        private Vector3[] CalculateArcArray()
        {
            var arcArray = new Vector3[_resolution + 1];
            _radianAngle = Mathf.Deg2Rad * _angle;
            var maxDistance = (_velocity * _velocity * Mathf.Sin(2 * _radianAngle)) / _gravity;

            for (int i = 0; i <= _resolution; i++)
            {
                var t = (float)i / (float)_resolution;
                arcArray[i] = CalculateArcPoint((Vector3)_sPos, (Vector3)_tPos, t, maxDistance);
            }
            return arcArray;
        }
        private Vector3 CalculateArcPoint(Vector3 sPos, Vector3 tPos, float t, float maxDistance = 0f)
        {
            return MathParabola.Parabola(sPos, tPos, _maxParabolaHeight, t);
        }


        private IEnumerator Fire()
        {
            lineRenderer.enabled = true;
            Vector3[] points = CalculateArcArray();

            // _radianAngle = Mathf.Deg2Rad * _angle;
            // var maxDistance = (_velocity * _velocity * Mathf.Sin(2 * _radianAngle)) / _gravity;
            // lineRenderer.SetPosition(0, _sPos);
            
            for (int i = 0; i < _resolution-1; i++)
            {
                // Vector3 point = CalculateArcPoint((Vector3)_sPos, (Vector3)_tPos, t, maxDistance);
                Debug.LogError("points pair " + points[i] + " " + points[i+1]);
                lineRenderer.SetPosition(0, points[i]);
                lineRenderer.SetPosition(1, points[i+1]);
                yield return null;
            }

            // while (_time < HalfDuration)
            // {
            //     var factor = _time / HalfDuration;
            //     var point = CalculateArcPoint(_sPos, _tPos, 1, maxDistance);
            //     lineRenderer.SetPosition(1, point);
            //     _time += Time.unscaledDeltaTime;
            //     yield return null;
            // }
            
            // lineRenderer.SetPosition(1, _tPos);
            // while (_time < BeamDuration)
            // {
            //     var factor = (_time - HalfDuration) / HalfDuration;
            //     var point = CalculateArcPoint(_sPos, _tPos, 1, maxDistance);
            //     lineRenderer.SetPosition(0, point);
            //     _time += Time.unscaledDeltaTime;
            //     yield return null;
            // }
            
            lineRenderer.enabled = false;
            PoolLoader.Return(PATH, gameObject);
        }
    }
}