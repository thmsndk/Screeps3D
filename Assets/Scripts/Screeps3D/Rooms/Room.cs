using System;
using System.Collections.Generic;
using Common;
using Screeps3D.RoomObjects;
using Screeps3D.Rooms.Views;
using UnityEngine;

namespace Screeps3D.Rooms
{
    public class Room
    {
        public readonly string Name;
        public readonly string RoomName;
        public readonly string ShardName;
        public readonly string XDir;
        public readonly string YDir;
        public readonly int ShardNumber;
        public readonly int XCoord;
        public readonly int YCoord;
        public readonly Vector3 Position;

        public Dictionary<string, RoomObject> Objects { get; private set; }
        
        public RoomView View { get; private set; }
        public RoomUnpacker RoomUnpacker { get; set; }
        public RoomObjectStream ObjectStream { get; private set; }
        public RoomMapStream MapStream { get; set; }
        
        public bool InitializedView { get; private set; }
        public bool ShowingObjects { get; private set; }
        public bool ShowingMap { get; private set; }
        public bool Shown { get; private set; }
        public long GameTime { get; set; }
        
        public event Action<bool> OnShowObjects;
        public event Action<bool> OnShowMap;
        public event Action<bool> OnShow;
        public event Action OnView;

        public Room(string roomName, string shardName, string xDir, string yDir, int shardNumber, int xCoord, int yCoord)
        {
            Name = roomName + shardName;
            RoomName = roomName;
            ShardName = shardName;
            XDir = xDir;
            YDir = yDir;
            ShardNumber = shardNumber;
            XCoord = xCoord;
            YCoord = yCoord;
            Position = FindPosition(xDir, xCoord, yDir, yCoord, shardNumber);
            Objects = new Dictionary<string, RoomObject>();
        }

        public static Vector3 FindPosition(string xDir, int xCoord, string yDir, int yCoord, int shardNumber)
        {
            return new Vector3(xDir == "E" ? xCoord * 50 : (-xCoord - 1) * 50, shardNumber * Constants.ShardHeight,
                yDir == "N" ? yCoord * 50 : (-yCoord - 1) * 50);
        }

        public void ShowObjects(bool show)
        {
            if (show == ShowingObjects)
                return;
            ShowingObjects = show;
            
            if (OnShowObjects != null)
                OnShowObjects(show);
        }

        public void ShowMap(bool show)
        {
            if (show == ShowingMap)
                return;
            ShowingMap = show;
            
            if (OnShowMap != null)
                OnShowMap(show);
        }

        public void Show(bool show)
        {
            if (show == Shown)
                return;
            Shown = show;
            
            if (show && !InitializedView)
            {
                InitializeView();
                InitializedView = true;
            }
            
            if (OnShow != null)
                OnShow(show);
        }

        private void InitializeView()
        {
            ObjectStream = new RoomObjectStream();
            ObjectStream.Init(this);
            MapStream = new RoomMapStream();
            MapStream.Init(this);
            RoomUnpacker = new RoomUnpacker();
            RoomUnpacker.Init(this);
            Scheduler.Instance.Add(AssignView);
        }

        private void AssignView()
        {
            View = RoomViewFactory.GetInstance(this);
            OnView?.Invoke();
        }
    }
}