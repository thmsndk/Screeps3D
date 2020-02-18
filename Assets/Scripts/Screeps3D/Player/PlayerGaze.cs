using System.Collections;
using System.Collections.Generic;
using Screeps3D.Rooms;
using Screeps3D.Rooms.Views;
using Screeps_API;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Screeps3D.Player
{
    public class PlayerGaze : MonoBehaviour
    {
        public bool allowFocus;

        public const int ViewDistance = 2;
        public const int SubscribeLimit = 2;
        public const float MapDistance = 200;
        
        private Dictionary<Room, bool> _loadedNeighbors = new Dictionary<Room, bool>();
        private Queue<Room> queue = new Queue<Room>();
        private List<Room> _mapRooms = new List<Room>();
        private double _nextMap;

        PlayerGaze()
        {
            allowFocus = true;
        }

        private void Update()
        {
            if (!ScreepsAPI.IsConnected)
                return;
            
            DisplayObjects();
            DisplayMap();
        }

        private void DisplayMap()
        {
            if (_nextMap > Time.time)
                return;
            _nextMap = Time.time + 1;

            DisableOutsideRange();
            EnableInRange();
        }

        private void DisableOutsideRange()
        {
            for (var i = 0; i < _mapRooms.Count; i++)
            {
                var room = _mapRooms[i];
                if (Vector3.Distance(transform.position, room.Position) <= MapDistance)
                    continue;

                room.ShowMap(false);
                _mapRooms.RemoveAt(i);
                i--;
            }
        }

        private void EnableInRange()
        {
            foreach (var col in Physics.OverlapSphere(transform.position, MapDistance, 1 << 10))
            {
                var roomView = col.GetComponent<RoomView>();
                if (!roomView || Vector3.Distance(transform.position, roomView.Room.Position) > MapDistance)
                    continue;
                if (_mapRooms.Contains(roomView.Room))
                    continue;
                
                roomView.Room.ShowMap(true);
                _mapRooms.Add(roomView.Room);
            } 
        }

        private void DisplayObjects()
        {

            var ray = Camera.main.ViewportPointToRay(new Vector3(.5f, .5f, 0));
            if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonUp(0))
            {
                // temporarily disable click to show
                // ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            }

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 200, 1 << 10 /* roomview */))
            {
                var roomView = hit.collider.GetComponent<RoomView>();
                if (roomView == null)
                {
                    return;
                }

                if (roomView.Room.ShowingObjects)
                {
                    // make sure the room we are viewing is last in the queue
                    var nextRoom = queue.Peek();
                    if (nextRoom == roomView.Room)
                    {
                        var room = queue.Dequeue();
                        queue.Enqueue(room);
                    }
                    
                    return;
                }


                ShowObjects(roomView.Room);
            }
            else
            {
                HideAll();
            }
        }

        private void HideAll()
        {
            while (queue.Count > 0)
            {
                var room = queue.Dequeue();
                room.ShowObjects(false);
            }
        }

        private void ShowObjects(Room room)
        {
            if (queue.Count >= SubscribeLimit)
            {
                var otherRoom = queue.Dequeue();
                otherRoom.ShowObjects(false);
            }
            queue.Enqueue(room);

            if (allowFocus)
                room.ShowObjects(true);
            LoadNeighbors(room);
        }
        
        public void LoadNeighbors(Room room)
        {
            if (_loadedNeighbors.ContainsKey(room))
                return;
            _loadedNeighbors[room] = true;
            
            for (var xDelta = -ViewDistance; xDelta <= ViewDistance; xDelta++)
            {
                for (var yDelta = -ViewDistance; yDelta <= ViewDistance; yDelta++)
                {
                    if (xDelta == 0 && yDelta == 0) continue;
                    var neighbor = RoomManager.Instance.GetNeighbor(room, xDelta, yDelta);
                    neighbor.Show(true);
                }
            }
        }
    }
}