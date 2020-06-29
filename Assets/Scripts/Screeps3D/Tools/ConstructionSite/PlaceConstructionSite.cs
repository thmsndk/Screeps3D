using Common;
using Screeps_API;
using Screeps3D;
using Screeps3D.RoomObjects;
using Screeps3D.Rooms;
using Screeps3D.Rooms.Views;
using System;
using System.Collections;
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
    public class PlaceConstructionSite : PlaceRoomObject<PlaceConstructionSite>
    {
        public Action OnConstructionSiteCreated;

        private void Start()
        {
            ChooseConstructionSite.Instance.OnConstructionSiteChange += ChangeRoomObjectType;
            OnPlaceRoomObject += PlaceConstructionSite_OnPlaceRoomObject;
        }

        private void PlaceConstructionSite_OnPlaceRoomObject()
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
                    NotifyText.Message($"<size=40><b>{error.str}</b></size>", Color.red);
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
}
