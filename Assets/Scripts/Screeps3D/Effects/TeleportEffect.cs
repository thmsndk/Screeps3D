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