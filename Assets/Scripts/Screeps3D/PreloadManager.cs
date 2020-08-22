using Common;
using Screeps3D.Rooms.Views;
using UnityEngine;

namespace Screeps3D
{
    public class PreloadManager : MonoBehaviour
    {
        private void Start()
        {
            // Preloaded mapdots does not seem to do anything :thinking:
            //PoolLoader.Preload(MapDotView.Path, 200);
        }
    }
}