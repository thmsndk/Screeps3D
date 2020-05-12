using System;
using System.Collections;
using Common;
using Screeps3D.RoomObjects;
using UnityEngine;

namespace Screeps3D.Effects
{
    public class NukeExplosionEffect : MonoBehaviour
    {
        public const string PATH = "Prefabs/Effects/WFX_Nuke";

        [SerializeField] private ParticleSystem nukeExplosionEffect = default;


        private const float _spawnDuration = 30;
        private float _time;
        private Vector3 _position;
        internal void Load(Vector3 position)
        {
            _time = 0f;

            _position = position;
            StartCoroutine(DisplayEffect());
        }

        private IEnumerator DisplayEffect()
        {
            gameObject.transform.SetPositionAndRotation(_position, gameObject.transform.rotation);
            
            nukeExplosionEffect.Play();

            while (_time < _spawnDuration)
            {
                _time += Time.unscaledDeltaTime;

                yield return null;
            }

            nukeExplosionEffect.Stop();

            PoolLoader.Return(PATH, gameObject);
        }
    }
}