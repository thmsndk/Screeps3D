using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Screeps3D.RoomObjects;
using Screeps_API;
using UnityEngine;

namespace Screeps3D.RoomObjects
{
    public static class UnpackUtility
    {

        internal static void Id(RoomObject roomObject, JSONObject data)
        {
            var idObj = data["_id"];
            if (idObj != null)
                roomObject.Id = idObj.str;
        }

        internal static void Regeneration(IRegenerationObject roomObject, JSONObject data)
        {
            var regenObj = data["nextRegenerationTime"];
            if (regenObj != null)
                roomObject.NextRegenerationTime = regenObj.n;
        }

        internal static void Type(RoomObject roomObject, JSONObject data)
        {
            var typeObj = data["type"];
            if (typeObj != null)
                roomObject.Type = typeObj.str;
        }

        internal static void Position(RoomObject roomObject, JSONObject data)
        {
            var xObj = data["x"];
            if (xObj != null)
                roomObject.X = (int)xObj.n;

            var yObj = data["y"];
            if (yObj != null)
                roomObject.Y = (int)yObj.n;

            var roomNameObj = data["room"];
            if (roomNameObj != null)
                roomObject.RoomName = roomNameObj.str;
        }

        internal static void Energy(IEnergyObject energyObj, JSONObject data)
        {
            var energyCapData = data["energyCapacity"];
            if (energyCapData)
            {
                energyObj.EnergyCapacity = energyCapData.n;
            }

            var energyData = data["energy"];
            if (energyData != null)
            {
                energyObj.Energy = energyData.n > energyObj.EnergyCapacity ? energyObj.EnergyCapacity : energyData.n;
            }
        }

        internal static void Name(INamedObject obj, JSONObject data)
        {
            var nameData = data["name"];
            if (nameData != null)
            {
                obj.Name = nameData.str;
            }
        }

        internal static void HitPoints(IHitpointsObject obj, JSONObject data)
        {
            var hitsData = data["hits"];
            if (hitsData != null)
            {
                obj.Hits = hitsData.n;
            }

            var hitsMaxData = data["hitsMax"];
            if (hitsMaxData != null)
            {
                obj.HitsMax = hitsMaxData.n;
            }
        }

        internal static void Owner(IOwnedObject obj, JSONObject data)
        {
            var userData = data["user"];
            if (userData != null)
            {
                obj.UserId = userData.str;
                obj.Owner = ScreepsAPI.UserManager.GetUser(userData.str);
            }
        }

        internal static void Cooldown(ICooldownObject obj, JSONObject data)
        {
            var coolDownData = data["cooldown"];
            if (coolDownData != null)
            {
                obj.Cooldown = coolDownData.n;
                return;
            }
        }

        internal static void Cooldown(ICooldownTime obj, JSONObject data)
        {
            var coolDownData = data["cooldownTime"];
            if (coolDownData != null)
            {
                obj.CooldownTime = (long)coolDownData.n;
                return;
            }
        }

        internal static void Decay(IDecay obj, JSONObject data)
        {
            var decayData = data["nextDecayTime"];

            if (decayData == null)
            {
                decayData = data["decayTime"];
            }

            if (decayData == null)
            {
                decayData = data["ticksToDecay"]; // Portals: The amount of game ticks when the portal disappears, or undefined when the portal is stable.
            }

            if (decayData != null)
            {
                obj.NextDecayTime = decayData.n;
            }
        }

        internal static void Effects(IEffectObject obj, JSONObject data)
        {
            // Invader Core effects example:
            // "effects":[
            //     {"effect":1001,"power":1001,"endTime":2,641389E+07,"duration":5000},
            //     {"effect":1002,"power":1002,"endTime":2,649596E+07,"duration":82074}
            // ],

            var effectsData = data["effects"];

            if (effectsData != null)
            {
                obj.Effects.Clear();
                foreach (var effect in effectsData.list)
                {
                    var effectType = (int)effect["effect"].n;
                    var powerType = (int)effect["power"].n;
                    var endTime = (int)effect["endTime"].n;
                    var duration = (int)effect["duration"].n;

                    obj.Effects.Add(new EffectDto(effectType, powerType, endTime, duration));
                }

                ////var sb = new StringBuilder();
                ////sb.AppendLine($"{obj.Effects.Count} effects parsed");
                ////foreach (var effect in obj.Effects)
                ////{
                ////    sb.AppendLine($"    Effect: {effect.Effect}");
                ////    sb.AppendLine($"    Power: {effect.Power}");
                ////    sb.AppendLine($"    EndTime: {effect.EndTime}");
                ////    sb.AppendLine($"    Duration: {effect.Duration}");
                ////}

                ////Debug.Log(sb.ToString());
            }
        }

        internal static void Progress(IProgress progressObj, JSONObject data)
        {
            var progressData = data["progress"];
            if (progressData != null)
            {
                progressObj.Progress = progressData.n;
            }

            var progressTotalData = data["progressTotal"];
            if (progressTotalData != null)
            {
                progressObj.ProgressMax = progressTotalData.n;
            }
        }

        internal static void Level(ILevel obj, JSONObject data)
        {
            var levelData = data["level"];
            if (levelData != null)
            {
                obj.Level = (int)levelData.n;
            }
        }

        internal static void Store(IStoreObject obj, JSONObject data)
        {

            try
            {
                if (data != null && data.IsObject && data.keys.Count == 0)
                {
                    // bail out early of "empty" updates
                    return;
                }

                // TODO: Convert existing energy data structure to new data structure?

                // ---- PRE STORE UPDATE
                // TODO: convert energy

                if (data.HasField("energyCapacity"))
                {
                    obj.TotalCapacity = data["energyCapacity"].n;
                }

                // ----- POST STORE UPDATE 

                var store = data.HasField("store") ? data["store"] : data; // this supports both PRE and POST store update

                if (store == null)
                {
                    obj.Store.Clear();
                }
                else if (!store.IsNull)
                {
                    foreach (var resourceType in store.keys)
                    {
                        if (!Constants.ResourcesAll.Contains(resourceType)) continue; // Early

                        if (obj.Store.ContainsKey(resourceType))
                        {
                            obj.Store[resourceType] = store[resourceType].n;
                        }
                        else
                        {
                            obj.Store.Add(resourceType, store[resourceType].n);
                        }
                    }
                }

                obj.TotalResources = obj.Store.Sum(a => a.Value);

                if (data.HasField("storeCapacityResource"))
                {
                    // TODO: store capacity resource is actually an array just like store with a capacity for each store, should probably add a TotalResourceCapacity
                    var storeCapacityResource = data["storeCapacityResource"];
                    if (storeCapacityResource != null && !storeCapacityResource.IsNull)
                    {
                        obj.TotalCapacity = 0;

                        foreach (var resourceType in storeCapacityResource.keys)
                        {
                            if (!Constants.ResourcesAll.Contains(resourceType)) continue; // Early

                            obj.TotalCapacity += storeCapacityResource[resourceType].n;

                            if (obj.Capacity.ContainsKey(resourceType))
                            {
                                obj.Capacity[resourceType] = storeCapacityResource[resourceType].n;
                            }
                            else
                            {
                                obj.Capacity.Add(resourceType, storeCapacityResource[resourceType].n);
                            }
                        }
                    }
                }

                if (data.HasField("storeCapacity")) // Labs seems to have this and not storeCapacityResource when they do not contain a mineral type?, atleast according to this https://github.com/screeps/storage/blob/b045531aca745f0942293bd32e0bdb5813bc12e2/lib/db.js#L123-L131
                {
                    var storeCapacity = data["storeCapacity"].n;

                    if (storeCapacity > 0)
                    {

                        obj.TotalCapacity = storeCapacity;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
                throw;
            }
        }

        internal static void ActionLog(IActionObject actionObject, JSONObject data)
        {
            var actionLog = data["actionLog"];
            if (actionLog != null)
            {
                foreach (var key in actionLog.keys)
                {
                    var actionData = actionLog[key];
                    if (actionData.IsNull)
                    {
                        actionObject.Actions.Remove(key);
                    }
                    else
                    {
                        actionObject.Actions[key] = actionData;
                    }
                }
            }
        }
    }
}