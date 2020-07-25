using Common;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class ObserverView : MonoBehaviour, IObjectViewComponent
    {
        public const string Path = "Prefabs/RoomObjects/observer";
        [SerializeField] private Renderer _eye = default;
        [SerializeField] private Renderer _spire = default;
        private Observer _observer;
       
        public void Init()
        {
        }

        public void Load(RoomObject roomObject)
        {
            _observer = roomObject as Observer;
            Color c = _observer.Owner.UserId.Equals(Screeps_API.ScreepsAPI.Me.UserId) ?  new Color(0.33f, 1.000f, 0.33f, 0.0f) : new Color(1.000f, 0.33f, 0.33f, 0.0f);
            _eye.materials[0].SetColor("EmissionColor", c);
            _eye.materials[1].SetColor("EmissionColor", c);

            _spire.materials[1].SetColor("EmissionColor", c);
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