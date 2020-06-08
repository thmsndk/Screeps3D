using Common;
using Screeps3D;
using Screeps3D.RoomObjects;
using Screeps3D.Rooms;
using Screeps3D.Rooms.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Screeps3D.Tools.ConstructionSite
{
    /// <summary>
    /// Responsible for placement of a constructionsite
    /// </summary>
    public class PlaceConstructionSite : BaseSingleton<PlaceConstructionSite>
    {

        private RoomObject _roomObject;
        // TODO: render structure
        //  TODO: handle when another structure is selected, disponse old prefab. initialize new.
        // TODO: place structure and call HTTP endpoint.
        // TODO: cancel placement

        private void Start()
        {
            Debug.Log("Place Constructionsite startet");
            _roomObject = new Spawn() { Type = "spawn" }; // TODO: utilize type and the factory to get object.
        }

        private void Update()
        {
            if (!InputMonitor.OverUI /*&& !_showEditDialog*/)
            {
                if (GetCursorPositionInRoom(out var room, out var roomPosition))
                {
                    if (roomPosition.x != _roomObject.X || roomPosition.y != _roomObject.Y)
                    {
                        // TODO: could probably just set X,Y Room and call SetPosition
                        Debug.Log(roomPosition);
                        Debug.Log($"flag: {_roomObject.X}, {_roomObject.Y} => {_roomObject.Position} == {PosUtility.Convert(_roomObject.X, _roomObject.Y, room)}");
                        ////Debug.Log("placeflag delta");
                        _roomObject.Delta(new JSONObject($"{{\"x\":{roomPosition.x},\"y\":{roomPosition.y}}}"), room);

                        // Move roomobject roomobjects except creeps usually don't move.
                        if (_roomObject.View != null)
                        {
                            ////Debug.Log($"{_flag?.Room?.ShardName}/{_flag?.Room?.RoomName}");
                            _roomObject.View.transform.localPosition = _roomObject.Position;
                        }
                    }
                }

            }
        }

        private void OnDisable()
        {
            if (_roomObject.View != null)
            {
                _roomObject.HideObject(_roomObject.Room);
            }
        }

        public bool GetCursorPositionInRoom(out Room room, out Vector2Int position)
        {
            room = null;
            position = Vector2Int.zero;

            var rayTarget = Rayprobe();
            // move flage location, flag should be alphablended 
            if (rayTarget.HasValue)
            {
                var roomView = rayTarget.Value.collider.GetComponent<RoomView>();
                if (roomView == null)
                {
                    return false;
                }

                room = roomView.Room;

                ////Debug.Log(room.Position);
                ////Debug.Log(room.Position - rayTarget.Value.point);
                position = PosUtility.ConvertToXY(rayTarget.Value.point, room);

                return true;
            }

            return false;
        }

        private RaycastHit? Rayprobe()
        {
            RaycastHit hitInfo;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hit = Physics.Raycast(ray, out hitInfo, 1000f, 1 << 10 /* roomView */);
            if (!hit) return null; // Early

            return hitInfo;
        }
    }
}
