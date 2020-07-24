using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class ExtractorView : MonoBehaviour, IObjectViewComponent
    {
        [SerializeField] private Renderer rend = default;

        private Extractor _extractor;
        // TODO: we also need the mineral on the location to get regen time if we want to do something specific in regards to that

        public void Init()
        {
        }

        public void Load(RoomObject roomObject)
        {
            _extractor = roomObject as Extractor;
            rend.transform.Translate(Vector3.up * 0.8f);
        }

        public void Delta(JSONObject data)
        {
        }

        public void Unload(RoomObject roomObject)
        {
            _extractor = null;
        }

        private void Update()
        {
            if (_extractor == null)
                return;
         
            if (_extractor.Cooldown > 0)
            {
                var speed = 10f;
                rend.transform.Rotate(Vector3.up * speed * Time.deltaTime);
            }
        }
    }
}