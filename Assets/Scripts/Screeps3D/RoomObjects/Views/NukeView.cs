using Common;
using UnityEngine;
using Screeps_API;

namespace Screeps3D.RoomObjects.Views
{
    public class NukeView : MonoBehaviour
    {
        public const string Path = "Prefabs/RoomObjects/nuke";
        private Nuke _nuke;
        [SerializeField] private ParticleSystem _beacon = default;

        public void Init()
        {
        }


        public void Load(RoomObject roomObject)
        {
            _nuke = roomObject as Nuke;
        }

        public void Delta(JSONObject data)
        {
        }

        public void Unload(RoomObject roomObject)
        {
            _beacon.Stop();
            _nuke = null;
            _beacon = null;
        }

        private void Update()
        {
        }
    }
}