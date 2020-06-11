using Common;
using Screeps_API;
using Screeps3D;
using Screeps3D.RoomObjects;
using Screeps3D.Rooms;
using Screeps3D.Rooms.Views;
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
        private ObjectFactory _factory = new ObjectFactory();
        private RoomObject _roomObject;
        // TODO: render structure
        //  TODO: handle when another structure is selected, disponse old prefab. initialize new.
        // TODO: place structure and call HTTP endpoint.
        // TODO: cancel placement

        private void Start()
        {
            ChooseConstructionSite.Instance.OnConstructionSiteChange += ConstructionSiteChanged;
            Debug.Log("Place Constructionsite startet");
            //_roomObject = new Spawn() { Type = "spawn" }; // TODO: utilize type and the factory to get object.
        }

        private void ConstructionSiteChanged(string type)
        {
            if (this._roomObject != null)
            {
                this._roomObject.HideObject(this._roomObject.Room);
            }

            this._roomObject = _factory.Get(type);
            this._roomObject.Type = type;

        }

        private void Update()
        {
            if (_roomObject == null)
            {
                return;
            }

            if (!InputMonitor.OverUI /*&& !_showEditDialog*/)
            {
                if (GetCursorPositionInRoom(out var room, out var roomPosition))
                {
                    if (roomPosition.x != _roomObject.X || roomPosition.y != _roomObject.Y)
                    {
                        //Debug.Log(roomPosition);
                        Debug.Log($"{_roomObject.Type}: {_roomObject.X}, {_roomObject.Y} => {_roomObject.Position} == {PosUtility.Convert(roomPosition.x, roomPosition.y, room)}");
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

            if (/*!_showEditDialog && */Input.GetMouseButtonUp(0) && !InputMonitor.OverUI)
            {
                // TODO: Place constructionsite
                //ScreepsAPI.Http.CreateFlag

                //_showEditDialog = true;
                //ToggleEditFlagPopup(true);
            }
        }

        private void OnDisable()
        {
            if (_roomObject?.View != null)
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
