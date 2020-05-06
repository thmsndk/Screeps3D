using System;
using Common;
using Screeps3D.RoomObjects;
using UnityEngine;

namespace Screeps3D.Tools.Selection
{
    public abstract class SelectionSubpanel : VerticalPanelElement
    {
        public const float LineHeight = 20;
        public abstract string Name { get; }
        public abstract Type ObjectType { get; }
        public abstract void Load(RoomObject roomObject);
        public abstract void Unload();

        public virtual bool IsPanelAvailabelForObject(RoomObject roomObject){
            return ObjectType.IsInstanceOfType(roomObject);
        }
    }
}