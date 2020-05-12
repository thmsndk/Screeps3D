using System;
using Screeps3D.RoomObjects;
using TMPro;
using UnityEngine;

namespace Screeps3D.Tools.Selection.Subpanels
{
    public class ConstructionPanel : LinePanel
    {

        [SerializeField] private TextMeshProUGUI _label = default;
        
        public override string Name
        {
            get { return "Construction"; }
        }

        public override Type ObjectType
        {
            get { return typeof(ConstructionSite); }
        }

        public override void Load(RoomObject roomObject)
        {
            var site = roomObject as ConstructionSite;
            _label.text = site.StructureType;
        }

        public override void Unload()
        {
        }
    }
}