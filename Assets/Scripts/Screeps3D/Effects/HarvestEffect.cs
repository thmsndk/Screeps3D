using System.Collections;
using Common;
using UnityEngine;
using Screeps3D.RoomObjects;

namespace Screeps3D.Effects
{
    public class HarvestEffect : MonoBehaviour
    {
        public const string PATH = "Prefabs/Effects/HarvestEffect";
        [SerializeField] private ParticleSystem _harvestParticles = default;
        private Vector3 _position;
        private Vector3 _target;
        private float _time;
        private const float _reserveDuration = 2;
        

        public void Load(Vector3 position)
        {
            _position = position;
            _time = 0f;
            StartCoroutine(Fire());
        }

        private IEnumerator Fire()
        {
            Quaternion tRotation = Quaternion.LookRotation(_position, Vector3.up);
            gameObject.transform.SetPositionAndRotation(_position, tRotation);

            _harvestParticles.Play();
            while (_time < _reserveDuration)
            {
                _time += Time.unscaledDeltaTime;
                yield return null;
            }
            _harvestParticles.Stop();
            PoolLoader.Return(PATH, gameObject);
        }
    }
}