using System;
using Screeps3D.RoomObjects;
using TMPro;
using UnityEngine;

namespace Screeps3D.Tools.Selection.Subpanels
{
    public class NamePanel : LinePanel
    {
        [SerializeField] private TextMeshProUGUI _label = default;

        private INamedObject _selected;

        public override string Name
        {
            get { return "Name"; }
        }

        public override Type ObjectType
        {
            get { return typeof(INamedObject); }
        }

        public override void Load(RoomObject roomObject)
        {
            _selected = roomObject as INamedObject;
            _label.text = string.Format("{0}", _selected.Name);
        }

        public override void Unload()
        {
            _selected = null;
        }
    }
}