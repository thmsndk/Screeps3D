using Screeps_API;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ServerInfo : MonoBehaviour
{
    [SerializeField] private TMP_Text ServerHostname = default;
    [SerializeField] private TMP_Text CurrentTick = default;
    [SerializeField] private TMP_Text TickRate = default;

    private DateTime lastTick;

    // Start is called before the first frame update
    void Start()
    {
        lastTick = DateTime.Now;
        ScreepsAPI.OnTick += ScreepsAPI_OnTick; // Tick may drift because the ScreepsAPI.Time seems to not be updated with the last recieved tick from websocket.
        this.ServerHostname.SetText(ScreepsAPI.Cache.Address.HostName.ToLowerInvariant());
    }

    private void ScreepsAPI_OnTick(long time)
    {
//        Debug.Log($"Tick: {time}");
        var timeSpan = DateTime.Now - lastTick;
        var ticksPerSecond = Math.Round(1f / timeSpan.TotalSeconds, 3);
        var timeElapsed = Math.Round(timeSpan.TotalSeconds, 3);
        this.TickRate.SetText($"{ticksPerSecond} ticks / s ({timeElapsed} s)");
        this.CurrentTick.SetText($"Tick: {time}");
        lastTick = DateTime.Now;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
