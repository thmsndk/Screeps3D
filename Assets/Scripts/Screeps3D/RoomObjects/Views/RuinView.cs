using Common;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    internal class RuinView : MonoBehaviour, IObjectViewComponent
    {
        [SerializeField] private Animation _ruinAnimation = default;
        [SerializeField] private Transform _ruinRoot = default;
        [SerializeField] private ParticleSystem _psSmoke;
        [SerializeField] private ScaleVisibility _vis = default;
        private Ruin _ruinObject;


        public void Init()
        {
        }

        public void Load(RoomObject roomObject)
        {
            _ruinObject = roomObject as Ruin;

            var euler = transform.eulerAngles;
            euler.y = Random.Range(0.0f, 360.0f);
            if (_ruinObject.StructureType == Constants.TypeInvaderCore)
            {
                // forgive me father because i sinned...
                // it's easier than rotating whole animation in blender...
                euler.x = -90f;
            }
            _ruinRoot.eulerAngles = euler;

            if (_ruinAnimation != null)
            {
                _ruinAnimation.Play();
            }

            if (_psSmoke != null)
            {
                _psSmoke.Play();
            }
        }

        public void Delta(JSONObject data)
        {
        }

        public void Unload(RoomObject roomObject)
        {
            if (_psSmoke != null)
            {
                _psSmoke.Stop();
            }
        }
    }
}