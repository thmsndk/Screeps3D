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


namespace Assets.Scripts.Screeps3D.Tools.ConstructionSite
{
    /// <summary>
    /// Responsible for placement of initial spawn
    /// </summary>
    public class PlaceFirstSpawn : PlaceRoomObject<PlaceFirstSpawn>
    {
        public Action OnFirstSpawnPlaced;

        private void Start()
        {
            ChangeRoomObjectType(Constants.TypeSpawn);
            OnPlaceRoomObject += PlaceFirstSpawn_OnPlaceRoomObject;
        }

        private void PlaceFirstSpawn_OnPlaceRoomObject()
        {
            Action<string> onSuccess = (jsonString) =>
            {
                var result = new JSONObject(jsonString);

                var ok = result["ok"];

                if (ok != null && ok.n == 1)
                {
                    HideRoomObject();
                    OnFirstSpawnPlaced?.Invoke();
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

            // ask for name and then spawn.
            PlayerInput.Get("Name your spawn", spawnName =>
            {
                if (spawnName == null)
                {
                    // cancel clicked, should probably have a cancel callback instead.
                    return;
                }

                if (string.IsNullOrWhiteSpace(spawnName))
                {
                    NotifyText.Message($"Invalid spawnname", Color.red);
                    return;
                }

                ScreepsAPI.Http.PlaceSpawn(
                _roomObject.Room.ShardName,
                _roomObject.Room.RoomName,
                _roomObject.X,
                _roomObject.Y,
                spawnName,
                onSuccess: onSuccess);
            });
        }
    }
}
