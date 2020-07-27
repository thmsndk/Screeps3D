using Common;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    internal class DepositView : MonoBehaviour, IObjectViewComponent/*, IMapViewComponent*/
    {
        public const string Path = "Prefabs/RoomObjects/mineral";

        [SerializeField] private Renderer _deposit;
        [SerializeField] private Collider _collider;
        [SerializeField] private ScaleVisibility _vis;
        //[SerializeField] private Transform _rotationRoot;

        private Quaternion _rotTarget;
        private Vector3 _posTarget;
        private Vector3 _posRef;
        private Deposit _depositObject;

        public void Init()
        {
        }

        public void Load(RoomObject roomObject)
        {
            _depositObject = roomObject as Deposit;

            _rotTarget = transform.rotation;
            _posTarget = roomObject.Position;

            // Move deposit up "above" the terrain
            transform.localPosition = roomObject.Position + (Vector3.up * 0.3f);
        }

        public void Delta(JSONObject data)
        {
        }

        public void Unload(RoomObject roomObject)
        {
        }

        public int roomPosX { get; set; }
        public int roomPosY { get; set; }
        public void Show()
        {
           _vis.Show();
           _collider.enabled = false;
        }
        public void Hide()
        {
           _vis.Hide();
           _collider.enabled = true;
        }
    }
}