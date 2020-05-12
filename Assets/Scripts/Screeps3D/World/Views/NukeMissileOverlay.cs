using System;
using Screeps3D.Rooms;
using UnityEngine;

namespace Screeps3D.World.Views
{
    // This is not the "overlay" but the actual rendering of the missile and arc prefab ... hmmmm
    public class NukeMissileOverlay : WorldOverlay
    {
        public string Id { get; set; }

        public NukeMissileOverlay(string id)
        {
            this.Id = id;
        }

        public override string Type { get; set; } = "nukeMissile";
        public Room LaunchRoom { get; internal set; }
        public Room ImpactRoom { get; internal set; }
        public Vector3 ImpactPosition { get; internal set; }

        /// <summary>
        /// The ingame Tick when the nuke is supposed to land.
        /// </summary>
        public long LandingTime { get; internal set; }
        public long InitialLaunchTick { get; internal set; }
        public float Progress { get; internal set; }
        public DateTime EtaEarly { get; internal set; }
        public DateTime EtaLate { get; internal set; }
    }
}