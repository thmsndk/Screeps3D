using System;
using System.Text.RegularExpressions;
using Screeps3D.RoomObjects;
using Screeps3D.Rooms;
using UnityEngine;

namespace Screeps3D
{
    public static class PosUtility
    {
        public static Vector3 Convert(int x, int y, Room room)
        {
            return room.Position + new Vector3(x, 0, 49 - y);
        }

        public static Vector3 Convert(JSONObject posData, Room room)
        {
            var x = 0;
            var y = 0;
            if (posData["x"])
            {
                x = (int)posData["x"].n;
            }
            if (posData["y"])
            {
                y = (int)posData["y"].n;
            }
            return Convert(x, y, room);
        }

        public static Vector3 Relative(int xDelta, int yDelta, Vector3 originPos)
        {
            var delta = new Vector3(xDelta, 0, -yDelta);
            return originPos + delta;
        }

        public static Vector3 Relative(int xDelta, int yDelta, RoomObject roomObject)
        {
            var delta = new Vector3(xDelta, 0, -yDelta);
            return roomObject.Position + delta;
        }

        internal static Vector2Int ConvertToXY(Vector3 point, Room room)
        {
            var basePosition = point - room.Position;
            var x = Mathf.FloorToInt(Math.Abs(basePosition.x + 0.5f));
            var y = 49 - Mathf.FloorToInt(Math.Abs(basePosition.z + 0.5f));
            return new Vector2Int(x, y);
        }

        internal static  (int, int) XYFromRoom(string room)
        {
            var match = Regex.Match(room, @"^(?<dx>[WE])(?<x>\d+)(?<dy>[NS])(?<y>\d+)$");

            var dx = match.Groups["dx"].Value;
            var x = int.Parse(match.Groups["x"].Value);

            var dy = match.Groups["dy"].Value;
            var y = int.Parse(match.Groups["y"].Value);
            if (dx == "W") x = -x - 1;
            if (dy == "N") y = -y - 1;
            return (x, y);
        }
    }
}