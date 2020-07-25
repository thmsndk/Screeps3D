using Common;
using UnityEngine;
using Screeps_API;

namespace Screeps3D.RoomObjects.Views
{
    public class NukeView : MonoBehaviour
    {
        public const string Path = "Prefabs/RoomObjects/nuke";
        private Nuke _nuke;

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
        }

        private void Update()
        {
        }
    }
}