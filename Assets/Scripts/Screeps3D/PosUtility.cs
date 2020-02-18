using System;
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
                x = (int) posData["x"].n;
            }
            if (posData["y"])
            {
                y = (int) posData["y"].n;
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
            var basePosition = room.Position - point;
            var x = Mathf.FloorToInt(Math.Abs(basePosition.x));
            var y = 49 - Mathf.FloorToInt(Math.Abs(basePosition.z));
            return new Vector2Int(x, y);
        }
    }
}