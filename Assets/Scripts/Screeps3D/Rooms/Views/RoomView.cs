using Common;
using UnityEngine;

namespace Screeps3D.Rooms.Views
{
    public class RoomView : MonoBehaviour
    {
        [SerializeField] private ScaleVisibility _vis = default;
        [SerializeField] private GameObject _ProhibitedRoomProjector = default;

        public Room Room { get; private set; }
        private IRoomViewComponent[] _viewComponents;

        public void Init(Room room)
        {
            _vis.Show();
            Room = room;

            //SpawnProhibited(false);

            _viewComponents = GetComponentsInChildren<IRoomViewComponent>();
            foreach (var component in _viewComponents)
            {
                component.Init(room);
            }
        }
        public void SpawnProhibited(bool prohibited)
        {
            _ProhibitedRoomProjector.SetActive(prohibited);
        }
    }
}