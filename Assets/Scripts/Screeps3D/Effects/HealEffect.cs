using System.Collections;
using Common;
using UnityEngine;
using Screeps3D.RoomObjects;

namespace Screeps3D.Effects
{
    public class HealEffect : MonoBehaviour
    {
        public const string PATH = "Prefabs/Effects/HealEffect";
        [SerializeField] private ParticleSystem _healParticles = default;
        
        private void attachToCreepOrigin(RoomObject origin) {
            gameObject.transform.parent = origin.View.transform;
            gameObject.transform.localPosition = Vector3.zero; // center it
        }

        public void Load(RoomObject origin)
        {
            attachToCreepOrigin(origin);
            _time = 0f;
            StartCoroutine(Fire());
        }
        private float _time;
        private const float _healDuration = 2;

        internal void Load(Vector3 position)
        {
            _time = 0f;
            StartCoroutine(Fire());
        }

        private IEnumerator Fire()
        {
            // at target pos
            // Quaternion tRotation = Quaternion.LookRotation(_target, Vector3.up);
            // gameObject.transform.SetPositionAndRotation(_target, tRotation);

            _healParticles.Play();
            while (_time < _healDuration)
            {
                _time += Time.unscaledDeltaTime;
                yield return null;
            }
            _healParticles.Stop();
            PoolLoader.Return(PATH, gameObject);
        }
    }
}