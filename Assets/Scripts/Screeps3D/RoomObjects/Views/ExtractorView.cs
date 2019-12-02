using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class ExtractorView : MonoBehaviour, IObjectViewComponent
    {
        [SerializeField] private Renderer rend;
        [SerializeField] private Animator anim;

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

            if (null != anim)
            {
                // play Bounce but start at a quarter of the way though
                //anim.Play("Bounce", 0, 0.25f);
                // Sadly I can't get the animation to work, only a Default Take seems to get imported, and when an animation controller is attached, the model dissappears
                //anim.Play("cooldown");
                // new Vector2(5 * speed * Time.deltaTime, 0)
                if (_extractor.Cooldown > 0)
                {
                    var speed = 10f;
                    rend.transform.Rotate(Vector3.up * speed * Time.deltaTime);
                }
            }
        }
    }
}