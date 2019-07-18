using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class CarryView : MonoBehaviour, IObjectViewComponent
    {
        [SerializeField] private GameObject _carry;
        private IStoreObject _creep;

        public void Init()
        {
        }

        public void Load(RoomObject roomObject)
        {
            _creep = roomObject as IStoreObject;

            if (_creep != null)
            {
                _carry.SetActive(_creep.StoreCapacity > 0);
            }
        }

        public void Delta(JSONObject data)
        {
        }

        public void Unload(RoomObject roomObject)
        {
        }
    }
}