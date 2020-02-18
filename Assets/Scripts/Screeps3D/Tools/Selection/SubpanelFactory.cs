using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Screeps3D.RoomObjects;
using UnityEngine;

namespace Screeps3D.Tools.Selection
{
    public class SubpanelFactory : BaseSingleton<SubpanelFactory>
    {
        [SerializeField] private Transform _inactiveParent;

        private readonly List<Func<RoomObject, SelectionSubpanel>> _factories = new List<Func<RoomObject, SelectionSubpanel>>();
        private const string Path = "Prefabs/Selection/Subpanels/";

        private readonly string[] _prefabNames =
        {
            "Type", "Owner", "Name", "Pos", "Hits", "Energy", "Age", "Fatigue", "Decay", "Level", "Progress", "Construction", 
            "Capacity", "Store", "Cooldown", "Resource", "Spawning", "SpawningIn", "Regeneration",
            "Power", "Destination",
            "Buttons"
        };
        
        private void Start()
        {
            for (var i = 0; i < _prefabNames.Length; i++)
            {
                var panelName = _prefabNames[i];
                var path = string.Format("{0}{1}", Path, panelName);
                var panel = PrefabLoader.Look(path);
                if (!panel)
                    continue;
                
                var component = panel.GetComponent<SelectionSubpanel>();
                if (!component) 
                    continue;
                
                _factories.Add(roomObject =>
                {
                    if (!component.ObjectType.IsInstanceOfType(roomObject))
                    {
                        ////if (component.ObjectType.IsGenericType)
                        ////{
                        ////    // This is to support IButtons<T> where T: IRoomObject
                        ////    Type[] typeParameters = component.ObjectType.GetGenericArguments();
                        ////    if (!typeParameters.Any(t => t.IsInstanceOfType(roomObject)))
                        ////    {
                        ////        return null;
                        ////    }
                        ////}
                        ////else
                        ////{
                            return null;
                        ////}
                    }
                    return PoolLoader.Load(path).GetComponent<SelectionSubpanel>();
                });
            }
        }

        internal void AddSubpanels(RoomObject selected, List<SelectionSubpanel> subpanels)
        {
            foreach (var factory in _factories)
            {
                var subpanel = factory(selected);
                if (!subpanel) continue;
                subpanels.Add(subpanel);
                subpanel.Load(selected);
            }
        }

        public void ReturnSubpanels(List<SelectionSubpanel> subpanels)
        {
            foreach (var subpanel in subpanels)
            {
                var path = string.Format("{0}{1}", Path, subpanel.Name);
                PoolLoader.Return(path, subpanel.gameObject);
                subpanel.transform.SetParent(_inactiveParent, false);
                subpanel.transform.localScale = Vector3.one;
            }
            
            subpanels.Clear();
        }
    }
}