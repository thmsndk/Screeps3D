using System.Collections.Generic;
using Screeps3D.Rooms;
using Screeps_API;
using UnityEngine;
using System;
using Assets.Scripts.Screeps3D.RoomObjects;

namespace Screeps3D.RoomObjects
{

    internal interface IObjectViewComponent
    {
        void Init();
        void Load(RoomObject roomObject);
        void Delta(JSONObject data);
        void Unload(RoomObject roomObject);
    }

    internal interface IMapViewComponent
    {
        void Show();
        void Hide();
        int roomPosX { get; set; }
        int roomPosY { get; set; }
        Transform transform { get; }
    }

    public interface IRoomObject
    {
        Room Room { get; }
    }

    internal interface IEnergyObject
    {
        float Energy { get; set; }
        float EnergyCapacity { get; set; }
    }

    internal interface IPowerObject
    {
        float Power { get; set; }
        float PowerCapacity { get; set; }
    }

    internal interface IResourceObject
    {
        float ResourceAmount { get; set; }
        float ResourceCapacity { get; set; }
        string ResourceType { get; set; }
    }

    internal interface IDepositObject
    {
        string DepositType { get; set; }
    }

    internal interface IRegenerationObject : IRoomObject
    {
        float NextRegenerationTime { get; set; }
    }

    internal interface ISpawningInObject : IRoomObject
    {
        float NextSpawnTime { get; set; }
    }

    internal interface INamedObject
    {
        string Name { get; set; }
    }

    internal interface IOwnedObject
    {
        string UserId { get; set; }
        ScreepsUser Owner { get; set; }
    }

    internal interface ICooldownObject
    {
        float Cooldown { get; set; }
    }

    internal interface ICooldownTime : IRoomObject
    {
        long CooldownTime { get; set; }
    }

    internal interface IEffect {
        string effect {get; set;}
        string power {get; set;}
        long endTime {get; set;}
        long duration {get; set;}
    }
    internal interface IEffectObject {
        List<EffectDto> Effects { get; set; }
    }

    internal interface IDepositCooldown : IRoomObject
    {
        int Harvested { get; set; }
        float CooldownTime { get; set; }
    }

    internal interface IHitpointsObject
    {
        float Hits { get; set; }
        float HitsMax { get; set; }
    }

    internal interface IDecay : IRoomObject
    {
        float NextDecayTime { get; set; }
    }

    internal interface IStoreObject : IRoomObject
    {
        Dictionary<string, float> Store { get; }
        Dictionary<string, float> Capacity { get; }
        float TotalCapacity { get; set; }
        float TotalResources { get; set; }
    }

    internal interface IProgress
    {
        float Progress { get; set; }
        float ProgressMax { get; set; }
    }

    internal interface IReserved: IRoomObject
    {
        float ReservationEnd { get; set; }
        ScreepsUser ReservedBy { get; set; }
    }

    internal interface IActionObject : IRoomObject
    {
        Dictionary<string, JSONObject> Actions { get; }
    }

    internal interface ICreepAction : IActionObject
    {
        Vector3? ActionTarget { get; set; }
    }

    internal interface IBump
    {
        Vector3 PrevPosition { get; }
        Vector3 BumpPosition { get; }
    }

    internal interface IPortalDestination
    {
        string DestinationShard { get; set; }

        // RoomPosition
        string DestinationRoom { get; set; }
        string DestinationPosition { get; set; }
    }

    internal interface ILevel
    {
        int Level { get; set; }
        int LevelMax { get; set; }
    }

    internal interface IButtons 
    {
        List<IRoomObjectPanelButton> GetButtonActions();
    }
    public interface IRoomObjectPanelButton
    {
        string Text { get; set; }
    }

    public interface IRoomObjectPanelButton<T> : IRoomObjectPanelButton where T: IRoomObject
    {
        void OnClick(T roomObject);
    }

    public interface ICreepBody
    {
        CreepBody Body { get; }
    }
}