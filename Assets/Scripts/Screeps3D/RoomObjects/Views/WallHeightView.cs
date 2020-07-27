using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class WallHeightView: MonoBehaviour, IObjectViewComponent
    {

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

            if(_wall.Hits == 0 && _wall.HitsMax == 0) {
                height = 1;
            }
            var ls = transform.localScale;
            transform.localScale = new Vector3(ls.x, height, ls.z);
        }

        public void Unload(RoomObject roomObject)
        {
        }
    }
}