using System.Collections;
using Common;
using UnityEngine;
using Screeps3D.RoomObjects;

namespace Screeps3D.Effects
{
    public class AttackEffect : MonoBehaviour
    {
        public const string PATH = "Prefabs/Effects/AttackEffect";
        [SerializeField] private Transform _rotationRoot = default;
        [SerializeField] private ParticleSystem _attackParticles = default;
        
        public void Load(RoomObject origin, Vector3 target)
        {
            Debug.LogError("public");
            _position = origin.View.transform.position;
            _target = target;
            _time = 0f;
            StartCoroutine(Fire());
        }
        private float _time;
        private const float _attackDuration = 2;
        private Vector3 _position;
        private Vector3 _target;

        internal void Load(Vector3 position, Vector3 target)
        {
            Debug.LogError("Internal");
            _position = position;
            _target = target;
            _time = 0f;
            StartCoroutine(Fire());
        }

        private IEnumerator Fire()
        {
            // Quaternion tRotation = Quaternion.LookRotation(relativePos, Vector3.up);
            // gameObject.transform.SetPositionAndRotation(_position, tRotation);

            // at target pos
            Quaternion tRotation = Quaternion.LookRotation(_target, Vector3.up);
            gameObject.transform.SetPositionAndRotation(_target, tRotation);

            _attackParticles.Play();
            while (_time < _attackDuration)
            {
                _time += Time.unscaledDeltaTime;
                yield return null;
            }
            _attackParticles.Stop();
            PoolLoader.Return(PATH, gameObject);
        }
    }
}