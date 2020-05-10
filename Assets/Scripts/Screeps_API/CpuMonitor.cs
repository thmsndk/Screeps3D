using System;
using System.Collections.Generic;
using UnityEngine;

namespace Screeps_API
{
    public class CpuMonitor : MonoBehaviour
    {

        public Action<int> OnCpu;
        
        public int CPU { get; private set; }
        public int Memory { get; private set; }
        
        private Queue<JSONObject> queue = new Queue<JSONObject>();

        private void Start()
        {
            ScreepsAPI.OnConnectionStatusChange += SubscribeCpu;
        }

        private void SubscribeCpu(bool connected)
        {
            if (connected)
            {
                ScreepsAPI.Socket.Subscribe(string.Format("user:{0}/cpu", ScreepsAPI.Me.UserId), RecieveData);
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
            UnpackCpu(queue.Dequeue());

        }

        private void UnpackCpu(JSONObject data)
        {
            var cpuData = data["cpu"];
            if (cpuData != null)
            {
                CPU = (int) cpuData.n;
            }
            
            var memoryData = data["memory"];
            if (memoryData != null)
            {
                Memory = (int) memoryData.n;
            }

            if (OnCpu != null)
                OnCpu(CPU);
        }
    }
}