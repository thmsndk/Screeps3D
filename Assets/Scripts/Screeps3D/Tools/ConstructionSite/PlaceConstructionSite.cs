using Common;
using Screeps_API;
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


/*
 TODO: Indicate where you can place the structure / csite, some places are valid, others are not depending on type

*/
namespace Assets.Scripts.Screeps3D.Tools.ConstructionSite
{
    /// <summary>
    /// Responsible for placement of a constructionsite
    /// </summary>
    public class PlaceConstructionSite : BaseSingleton<PlaceConstructionSite>
    {
        private ObjectFactory _factory = new ObjectFactory();
        private RoomObject _roomObject;

        public Action OnConstructionSiteCreated;

        private void Start()
        {
            ChooseConstructionSite.Instance.OnConstructionSiteChange += ConstructionSiteChanged;
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
                        //Debug.Log($"{_roomObject.Type}: {_roomObject.X}, {_roomObject.Y} => {_roomObject.Position} == {PosUtility.Convert(roomPosition.x, roomPosition.y, room)} {roomPosition.x},{roomPosition.y}");
                        ////Debug.Log("placeflag delta");
                        _roomObject.Delta(new JSONObject($"{{\"x\":{roomPosition.x},\"y\":{roomPosition.y}}}"), room);

                        // Move roomobject roomobjects except creeps usually don't move.
                        if (_roomObject.View != null)
                        {
                            ////Debug.Log($"{_flag?.Room?.ShardName}/{_flag?.Room?.RoomName}");
                            _roomObject.View.transform.localPosition = _roomObject.Position;
                        }

                        // TODO: validate if the specific constructionsite can be placed
                        // extractor can only be placed on minerals
                        // only roads and extractors(if mineral) can be placed on normal walls
                        // you can't place the same csite ontop of the same csite.
                    }
                }

            }

            if (/*!_showEditDialog && */Input.GetMouseButtonUp(0) && !InputMonitor.OverUI)
            {
                Action<string> onSuccess = (jsonString) =>
                {
                    var result = new JSONObject(jsonString);

                    var ok = result["ok"];

                    if (ok != null && ok.n == 1)
                    {
                        OnConstructionSiteCreated?.Invoke();
                    }
                    else
                    {
                        var error = result["error"];
                        // CreateConstructionsite failed {"error":"RCL not enough"}
                        // CreateConstructionsite failed {"error":"invalid location"}
                        // error
                        NotifyText.Message($"{error.str}", Color.red);
                        //Debug.LogError($"CreateConstructionsite failed {result.ToString()}");
                    }
                };

                switch (_roomObject.Type)
                {
                    case Constants.TypeSpawn:
                        // ask for name and then spawn.
                        PlayerInput.Get("Name your spawn", spawnName =>
                        {
                            if (string.IsNullOrWhiteSpace(spawnName))
                            {
                                NotifyText.Message($"Invalid spawnname", Color.red);
                                return;
                            }

                            ScreepsAPI.Http.CreateConstructionsite(
                            _roomObject.Room.ShardName,
                            _roomObject.Room.RoomName,
                            _roomObject.X,
                            _roomObject.Y,
                            _roomObject.Type,
                            spawnName,
                            onSuccess: onSuccess);
                        });
                        break;
                    default:
                        ScreepsAPI.Http.CreateConstructionsite(
                            _roomObject.Room.ShardName,
                            _roomObject.Room.RoomName,
                            _roomObject.X,
                            _roomObject.Y,
                            _roomObject.Type,
                            onSuccess: onSuccess);
                        break;
                }
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
