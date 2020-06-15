using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class WallHeightView: MonoBehaviour, IObjectViewComponent
    {
        private static readonly Dictionary<int, float> Scales = new Dictionary<int, float>
        {
            {1, 0.3f},
            {1000000, 0.5f},
            {10000000, 1f},
            {100000000, 1.5f},
            {300000000, 2f}
        };

        private IHitpointsObject _wall;
        public void Init()
        {
        }

        public void Load(RoomObject roomObject)
        {
            _wall = roomObject as IHitpointsObject;
            SetScale();
        }

        public void Delta(JSONObject data)
        {
            if (data.HasField("hits"))
               SetScale();
            
        }

        private void SetScale()
        {
            float height = 0;
            if(_wall.Hits != null && _wall.Hits > 0) {
                height = Mathf.Floor(Mathf.Log(Mathf.Ceil(_wall.Hits / (10 * 1000)))) * 0.2f + 0.3f;
            }
            var ls = transform.localScale;
            Debug.LogError("_wall.Hits: " + _wall.Hits + " -> " + height);
            transform.localScale = new Vector3(ls.x, height, ls.z);
        }

        public void Unload(RoomObject roomObject)
        {
        }
    }
}