using System.Collections;
using Common;
using UnityEngine;
using Screeps3D.RoomObjects;

namespace Screeps3D.Effects
{
    public class ReserveEffect : MonoBehaviour
    {
        public const string PATH = "Prefabs/Effects/ReserveEffect";
        [SerializeField] private ParticleSystem _reserveParticles = default;
        [SerializeField] private Renderer _effectEmission = default;
        private Vector3 _target;
        private float _time;
        private const float _reserveDuration = 2;
        
        public void Load(Vector3 position)
        {
            _target = position;
            _time = 0f;
            StartCoroutine(Fire());
        }

        private IEnumerator Fire()
        {
            Quaternion tRotation = Quaternion.LookRotation(_target, Vector3.up);
            gameObject.transform.SetPositionAndRotation(_target, tRotation);

            _reserveParticles.Play();
            while (_time < _reserveDuration)
            {
                _time += Time.unscaledDeltaTime;
                yield return null;
            }
            _reserveParticles.Stop();
            PoolLoader.Return(PATH, gameObject);
        }
    }
}