using UnityEngine;
using System.Collections;
using Screeps3D.Player;
using Screeps3D.Rooms;
using System.Collections.Generic;
using Screeps3D;
using Screeps3D.RoomObjects;
using Screeps_API;
using System.Linq;
using System;
using Common;
using UnityEngine.UI;

namespace Assets.Scripts.Screeps3D.Menus
{
    public class RoomInfoWidget : MonoBehaviour
    {
        private const string RoomInfoPlayerWidgetPrefab = "Prefabs/RoomInfoPlayerWidget";

        private Room playerPositionRoom;

        // Use this for initialization
        void Start()
        {
            // Start with a clean slate.
            foreach (Transform child in this.transform)
            {
                Destroy(child.gameObject);
            }

            PoolLoader.Preload(RoomInfoPlayerWidgetPrefab, 2);
        }

        private void OnEnable()
        {
            PlayerPosition.Instance.OnRoomChange += OnRoomChange;

            // register for ticks/delta or room updates
            playerPositionRoom = PlayerPosition.Instance.Room;
            if (playerPositionRoom != null)
            {
                playerPositionRoom.ObjectStream.OnData += OnRoomData; 
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnRoomChange()
        {
            if (playerPositionRoom != null && playerPositionRoom.ObjectStream != null)
            {
                playerPositionRoom.ObjectStream.OnData -= OnRoomData;
            }

            Debug.Log("We swapped room!: " + PlayerPosition.Instance.Room?.Name);
            playerPositionRoom = PlayerPosition.Instance.Room;

            if (playerPositionRoom != null && playerPositionRoom.ObjectStream != null)
            {
                playerPositionRoom.ObjectStream.OnData += OnRoomData;
            }

        }

        private void OnRoomData(JSONObject obj)
        {
            // We don't have players in room, we might want that, we might also want room objects seperated by players? we can do this later
            //playerPositionRoom.Objects
            var data = new Dictionary<ScreepsUser, Dictionary<string, int>>();
            
            foreach (var roomObject in playerPositionRoom.Objects)
            {
                switch (roomObject.Value.Type)
                {
                    case Constants.TypeCreep:
                        var creep = roomObject.Value as Creep;
                        if (!data.TryGetValue(creep.Owner, out var playerData))
                        {
                            playerData = new Dictionary<string, int>();
                            playerData.Add("move", 0);
                            playerData.Add("work", 0);
                            playerData.Add("attack", 0);
                            playerData.Add("ranged_attack", 0);
                            playerData.Add("heal", 0);
                            playerData.Add("tough", 0);
                            playerData.Add("claim", 0);
                            playerData.Add("carry", 0);

                            data.Add(creep.Owner, playerData);
                        }

                        foreach (var part in creep.Body.Parts)
                        {
                            playerData[part.Type]++;
                        }

                        break;

                    default:
                        break;
                }

                foreach (var item in data)
                {
                    var gameObject = transform.Find(item.Key.UserId)?.gameObject;
                    if (gameObject == null)
                    {
                        Debug.Log("Spawning room info for " + item.Key.Username);
                        gameObject = PoolLoader.Load(RoomInfoPlayerWidgetPrefab); //Instantiate(RoomInfoPlayerWidgetPrefab, this.transform).gameObject;
                        gameObject.transform.SetParent(this.transform);
                        gameObject.name = item.Key.UserId;
                    }

                    var widget = gameObject.GetComponent<RoomInfoPlayerWidget>();

                    // crude initial way
                    widget.BadgeAndLabel.SetOwner(item.Key);
                    widget.StatsLabel.text = item.Value.Where(x => x.Value > 0).Select(x => "<color="+ConvertBodyPartToHexColor(x.Key)+">" + x.Value + "" + x.Key.Substring(0, 1).ToUpper() + "</color>").Aggregate((s1, s2) => s1 + " " + s2);
                    gameObject.gameObject.SetActive(true);
                }
            }

            // loop children and disable other plays
            foreach (Transform child in this.transform)
            {
                var hasData = data.Any(x => x.Key.UserId == child.name);
                if (!hasData)
                {
                    child.gameObject.SetActive(false);
                    PoolLoader.Return(RoomInfoPlayerWidgetPrefab, child.gameObject);
                }
            }

            // There is a strange issue where the first time, they are horizontally aligned
            var vlg = GetComponent<VerticalLayoutGroup>();
            vlg.SetLayoutVertical();
            vlg.SetLayoutHorizontal();
        }

        private string ConvertBodyPartToHexColor(string partType)
        {
            switch (partType)
            {
                case "move":
                    return Constants.CreepBodyPartColors.MoveHex;
                case "work":
                    return Constants.CreepBodyPartColors.WorkHex;
                case "attack":
                    return Constants.CreepBodyPartColors.AttackHex;
                case "ranged_attack":
                    return Constants.CreepBodyPartColors.RangedAttackHex;
                case "heal":
                    return Constants.CreepBodyPartColors.HealHex;
                case "tough":
                    return Constants.CreepBodyPartColors.ToughHex;
                case "claim":
                    return Constants.CreepBodyPartColors.ClaimHex;
                case "carry":
                    return Constants.CreepBodyPartColors.CarryHex;
            }
            return "white";
        }

        private void OnDisable()
        {
            if (playerPositionRoom != null)
            {
                playerPositionRoom.ObjectStream.OnData += OnRoomData;
            }
        }
    }
}