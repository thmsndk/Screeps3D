using System;
using System.Collections.Generic;
using Common;
using Screeps3D.Rooms;
using Screeps_API;
using UnityEngine;
using System.Linq;
using Screeps3D.Player;
using System.Collections;
using System.Text.RegularExpressions;
using Screeps3D.Tools;
using Assets.Scripts.Screeps3D.Tools.ConstructionSite;

namespace Screeps3D
{
    public class WorldStatusUpdater : BaseSingleton<WorldStatusUpdater>
    {
        public delegate void OnWorldStatusChangedArgs(WorldStatus previous, WorldStatus current);

        public event OnWorldStatusChangedArgs OnWorldStatusChanged;

        public WorldStatus WorldStatus { get; set; } = WorldStatus.None;

        private void Start()
        {
            StartCoroutine(GetWorldStatus());
        }

        public IEnumerator GetWorldStatus()
        {
            while (true)
            {
                Debug.Log($"Getting world status");

                ScreepsAPI.Http.GetWorldStatus(GetWorldStatusCallback);
                
                // https://docs.screeps.com/auth-tokens.html#Rate-Limiting
                yield return new WaitForSecondsRealtime(10); // Official calls this endpoint every 6 seconds
            }
        }

        private void GetWorldStatusCallback(string jsonString)
        {
            var result = new JSONObject(jsonString);
            var ok = result["ok"];
            var status = result["status"];

            if (Enum.TryParse<WorldStatus>(status.str, true, out var worldStatus))
            {
                if (this.WorldStatus != worldStatus)
                {
                    OnWorldStatusChanged?.Invoke(this.WorldStatus, worldStatus);
                    this.WorldStatus = worldStatus; 
                }
            }
        }

        internal void SetWorldStatus(WorldStatus worldStatus)
        {
            var previous = this.WorldStatus;
            this.WorldStatus = worldStatus;

            OnWorldStatusChanged?.Invoke(previous, worldStatus);
        }
    }

    public enum WorldStatus
    {
        None,

        /// <summary>
        /// The world status when everything is fine
        /// </summary>
        Normal,

        /// <summary>
        /// The world status when you've lost your last spawn
        /// </summary>
        Lost,

        /// <summary>
        /// The world status when you have pressed Respawn, and are in spawn-placement mode.
        /// </summary>
        Empty
    }
}