using System;
using System.Collections;
using Common;
using Screeps3D.RoomObjects;
using UnityEngine;

namespace Screeps3D.Effects
{
    /// <summary>
    /// https://www.youtube.com/watch?v=uR2jcU3x3kU
    /// </summary>
    public class ElectricExplosionEffect : MonoBehaviour
    {
        public const string PATH = "Prefabs/Effects/ElectricExplosionEffect";

        [SerializeField] private ParticleSystem electricExplosion = default;

        public void Load(RoomObject origin)
        {
            // Add Effect as child
            //if (origin.View != null)
            //{
            //electricExplosion.Stop();
            //gameObject.transform.parent = origin.View.transform; // attach to creep
            //gameObject.transform.localPosition = Vector3.zero; // center it
            _time = 0f;
            _position = origin.Position;
            StartCoroutine(DisplayElectricalExplosion());
            //}
        }

        private const float _explosionDuration = 2;
        private float _time;
        private Vector3 _position;
        internal void Load(Vector3 position)
        {
            _time = 0f;

            _position = position;
            StartCoroutine(DisplayElectricalExplosion());
        }

        private IEnumerator DisplayElectricalExplosion()
        {
            gameObject.transform.SetPositionAndRotation(_position, gameObject.transform.rotation);
            electricExplosion.Play();

            while (_time < _explosionDuration)
            {
                _time += Time.unscaledDeltaTime;

                yield return null;
            }

            electricExplosion.Stop();
            //gameObject.transform.parent = null;

            PoolLoader.Return(PATH, gameObject);
        }
    }
}