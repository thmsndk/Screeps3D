using System;
using System.Collections;
using Common;
using Screeps3D.RoomObjects;
using UnityEngine;

namespace Screeps3D.Effects
{
    public class TeleportEffect : MonoBehaviour
    {
        public const string PATH = "Prefabs/Effects/TeleportEffect";

        [SerializeField] private ParticleSystem particleSystem;

        public void Load(RoomObject origin/*, Vector3 endPos, Color color*/)
        {
            origin.OnShow += Origin_OnShow; // can't really unregister the effect with this design

            // Add TeleportEffect as child
            if (origin.View != null)
            {
                particleSystem.Play();
                gameObject.transform.parent = origin.View.transform; // attach to creep
                gameObject.transform.localPosition = Vector3.zero; // center it
            }
        }

        private const float _spawnDuration = 3;
        private float _time;
        private Vector3 _position;
        internal void Load(Vector3 position)
        {
            _time = 0f;

            _position = position;
            StartCoroutine(DisplaySpawnEffect());
        }

        private IEnumerator DisplaySpawnEffect()
        {
            gameObject.transform.SetPositionAndRotation(_position, gameObject.transform.rotation);
            // pretty sure this causes the effect to be underground currently, and a missmatch between the TP effect and TP Spawn
            //gameObject.transform.Rotate(Vector3.right, 180); // make the animation go the other way to simulate "spawning" 
            // TODO: should be a courutine so the spawning effect is only rendered for a specific amount of time

            particleSystem.Play();

            while (_time < _spawnDuration)
            {
                _time += Time.unscaledDeltaTime;

                yield return null;
            }

            particleSystem.Stop();

            PoolLoader.Return(PATH, gameObject);
        }

        private void Origin_OnShow(bool show)
        {
            if (!show)
            {
                gameObject.transform.parent = null; // Detatch effect from creep ... also causes the effect to not really dissapear....
                particleSystem.Stop();
                PoolLoader.Return(PATH, gameObject);
            }
        }
    }
}