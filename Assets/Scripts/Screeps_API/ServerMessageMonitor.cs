using Common;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Screeps_API
{
    public class ServerMessageMonitor : MonoBehaviour
    {

        public Action<int> OnCpu;
        
        public int CPU { get; private set; }
        public int Memory { get; private set; }
        
        private Queue<JSONObject> queue = new Queue<JSONObject>();

        private void Start()
        {
            ScreepsAPI.OnConnectionStatusChange += SubsribeServerMessage;
        }

        private void SubsribeServerMessage(bool connected)
        {
            if (connected)
            {
                ScreepsAPI.Socket.Subscribe("server-message", RecieveData);
            }
        }

        private void RecieveData(JSONObject data)
        {
            queue.Enqueue(data);
        }

        private void Update()
        {
            if (queue.Count == 0)
                return;
            UnpackServerMessage(queue.Dequeue());

        }

        private void UnpackServerMessage(JSONObject data)
        {
            NotifyText.Message($"<size=40><b>{data.str}</b></size>", Color.white, 10);
        }
    }
}