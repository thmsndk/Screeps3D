using System.Collections.Generic;
using System.Linq;
using Screeps_API;

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
                roomObject.X = (int) xObj.n;

            var yObj = data["y"];
            if (yObj != null)
                roomObject.Y = (int) yObj.n;

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

        internal static void Progress(IProgress progressObj, JSONObject data)
        {
            var progressData = data["progress"];
            if (progressData != null)
            {
                progressObj.Progress = progressData.n;
            }
        }

        internal static void Store(IStoreObject obj, JSONObject data)
        {
            foreach (var resourceType in data.keys)
            {
                if (!Constants.ResourcesAll.Contains(resourceType)) continue; // Early
                
                if (obj.Store.ContainsKey(resourceType))
                    obj.Store[resourceType] = data[resourceType].n;
                else
                    obj.Store.Add(resourceType, data[resourceType].n);
            }
            obj.TotalResources = obj.Store.Sum(a => a.Value);
            if (data.HasField("energyCapacity"))
                obj.StoreCapacity = data["energyCapacity"].n;
        }
        
        internal static void ActionLog(IActionObject actionObject, JSONObject data)
        {
            var actionLog = data["actionLog"];
            if (actionLog != null)
            {
                foreach (var key in actionLog.keys)
                {
                    var actionData = actionLog[key];
                    actionObject.Actions[key] = actionData;
                }
            }
        }
    }
}