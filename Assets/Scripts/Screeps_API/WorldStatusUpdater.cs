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

namespace Screeps3D
{
    // This seems more like "room info" in regards to status of the room
    public class WorldStatusUpdater : BaseSingleton<WorldStatusUpdater>
    {
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
            // TODO: parse an react accordingly
            var result = new JSONObject(jsonString);
            Debug.Log(jsonString);
        }
    }
}