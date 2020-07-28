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
        private int _resolution = 100;
        private int _renderedArcLength = 35;
        private int _renderArcSpeed = 3;
        public float _velocity = 5f;
        public float _angle = 5f;
        private float _maxParabolaHeight = 1f;

        public void Load(Vector3 startPos, Vector3 targetPos, Color color)
        {
            _time = 0f;
            _sPos = startPos;
            _tPos = targetPos;

            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.positionCount = _renderedArcLength;
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
            lineRenderer.materials[0].SetColor("EmissionColor", color);
            lineRenderer.materials[0].SetFloat("EmissionStrength", 1.5f);

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
                arcArray[i] = CalculateArcBeamPoint((Vector3)_sPos, (Vector3)_tPos, t, maxDistance);
            }
            return arcArray;
        }
        private Vector3 CalculateArcBeamPoint(Vector3 sPos, Vector3 tPos, float t, float maxDistance = 0f)
        {
            return MathParabola.Parabola(sPos, tPos, _maxParabolaHeight, t);
        }

        private void setLineRendererPoints(int startIndex, int pointsToSetCount, Vector3[] allPoints) {
            for(int i = 0; i < pointsToSetCount; i++) {
                int index = startIndex + i;
                if(index >= allPoints.Length) {
                    return;
                }
                lineRenderer.SetPosition(i, allPoints[index]);
            }
        }

        private IEnumerator Fire()
        {
            lineRenderer.enabled = true;
            Vector3[] points = CalculateArcArray();

            // _radianAngle = Mathf.Deg2Rad * _angle;
            // var maxDistance = (_velocity * _velocity * Mathf.Sin(2 * _radianAngle)) / _gravity;
            // lineRenderer.SetPosition(0, _sPos);
            
            for (int i = 0; i < _resolution; i += _renderArcSpeed)
            {
                setLineRendererPoints(i, _renderedArcLength, points);
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