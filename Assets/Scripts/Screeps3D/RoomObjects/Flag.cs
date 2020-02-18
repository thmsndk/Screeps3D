using System.Collections.Generic;
using Assets.Scripts.Screeps3D.Tools.Selection;
using Screeps3D.Rooms;
using Screeps3D.Tools.Selection;
using UnityEngine;

namespace Screeps3D.RoomObjects
{
    public class Flag: RoomObject, INamedObject, IButtons
    {
        public string Name { get; set; }

        public int SecondaryColor { get; set; }
        public int PrimaryColor { get; set; }

        public bool PauseDeltaUpdates { get; set; }

        public Flag(string name)
        {
            Id = name;
            Name = name;
            Type = "flag";
        }

        public void FlagDelta(string[] dataArray, Room room)
        {
            if (PauseDeltaUpdates)
            {
                return;
            }

            PrimaryColor = int.Parse(dataArray[1]);
            SecondaryColor = int.Parse(dataArray[2]);
            X = int.Parse(dataArray[3]);
            Y = int.Parse(dataArray[4]);
            Room = room;
            RoomName = room.RoomName;
        }

        public List<IRoomObjectPanelButton> GetButtonActions()
        {
            return new List<IRoomObjectPanelButton>
            {
                new SelectionRoomObjectButton<Flag>("Change position", (flag) => PlaceFlag.Instance.MoveFlag(flag)),
                new SelectionRoomObjectButton<Flag>("Change color", (flag) => EditFlag.Instance.ShowEditDialog(flag)),
                new SelectionRoomObjectButton<Flag>("Remove flag", (flag) => EditFlag.Instance.DeleteFlag(flag)),
                //new SelectionRoomObjectButton<Flag>("view memory", (flag) => Debug.Log($"{flag.Name} remove flag clicked"))
            };
        }
    }
}