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
    // This seems more like "room info" in regards to status of the room
    public class WorldStatusUpdater : BaseSingleton<WorldStatusUpdater>
    {
        [SerializeField] private ToolChooser _toolChooser = default;
        [SerializeField] private ChooseConstructionSite _chooseConstruction = default;
        [SerializeField] private GameObject _lostSpawnPopup = default;

        private WorldStatus _worldStatus = WorldStatus.None;
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
                switch (worldStatus)
                {
                    case WorldStatus.None:
                        break;
                    case WorldStatus.Normal:
                        if (this._worldStatus == WorldStatus.Empty)
                        {
                            _toolChooser?.Show(ToolType.Flag);
                            _toolChooser?.Show(ToolType.Construction);
                            _toolChooser?.Hide(ToolType.Spawn);
                        }
                        break;
                    case WorldStatus.Lost:
                        if (this._worldStatus != WorldStatus.Lost)
                        {
                            _lostSpawnPopup?.SetActive(true);
                        }

                        break;
                    case WorldStatus.Empty:
                        _toolChooser?.Hide(ToolType.Flag);
                        _toolChooser?.Hide(ToolType.Construction);
                        _toolChooser?.Show(ToolType.Spawn);
                        break;
                    default:
                        break;
                }
                Debug.Log($"[WorldStatus] {this._worldStatus} => {worldStatus}");
                this._worldStatus = worldStatus;
            }
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