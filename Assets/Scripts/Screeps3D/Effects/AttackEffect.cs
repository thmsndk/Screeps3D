using System.Collections;
using Common;
using UnityEngine;
using Screeps3D.RoomObjects;

namespace Screeps3D.Effects
{
    public class AttackEffect : MonoBehaviour
    {
        public const string PATH = "Prefabs/Effects/AttackEffect";
        [SerializeField] private ParticleSystem _attackParticles = default;
        private float _time;
        private const float _attackDuration = 2;
        private Vector3 _target;
        
        public void Load(Vector3 target)
        {
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