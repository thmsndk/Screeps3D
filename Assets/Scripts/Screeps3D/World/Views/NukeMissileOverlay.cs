using System;
using Assets.Scripts.Screeps_API;
using Assets.Scripts.Screeps3D;
using Assets.Scripts.Screeps3D.World.Overlay;
using Screeps3D.Rooms;
using UnityEngine;

namespace Screeps3D.World.Views
{
    // This is not the "overlay" but the actual rendering of the missile and arc prefab ... hmmmm
    public class NukeMissileOverlay : SingleNukeWorldOverlay
    {
        private NukeMonitor.NukeData nuke;

        public string Id { get; set; }

        public NukeMissileOverlay(string id)
        {
            this.Id = id;
        }

        public NukeMissileOverlay(NukeMonitor.NukeData nuke)
        {
            this.nuke = nuke;
        }

        public string Shard { get { return nuke.Shard; } }

        public Room LaunchRoom { get { return nuke.LaunchRoom; } }
        public string LaunchRoomName { get { return nuke.LaunchRoomName; } }
        public Room ImpactRoom { get { return nuke.ImpactRoom; } }
        public string ImpactRoomName { get { return nuke.ImpactRoomName; } }
        public Vector3 ImpactPosition { get { return nuke.ImpactPosition; } }

        /// <summary>
        /// The ingame Tick when the nuke is supposed to land.
        /// </summary>
        public long LandingTime { get { return nuke.LandingTime; } }
        public long InitialLaunchTick { get { return nuke.InitialLaunchTick; } }
        public float Progress { get { return nuke.Progress; } }
        public DateTime EtaEarly { get { return nuke.EtaEarly; } }
        public DateTime EtaLate { get { return nuke.EtaLate; } }

        public ShardInfoDto ShardInfo { get { return nuke.ShardInfo; } }
    }
}