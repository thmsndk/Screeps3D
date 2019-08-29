using System;
using Assets.Scripts.Screeps_API.ConsoleClientAbuse;
using Common;
using Screeps3D.RoomObjects;
using TMPro;
using UnityEngine;

namespace Screeps3D.Tools.Selection.Subpanels
{
    public class DestinationPanel : LinePanel
    {
        [SerializeField] private TextMeshProUGUI _label;
        private IPortalDestination _destinationObject;
        private RoomObject _roomObject;

        public override string Name
        {
            get { return "Destination"; }
        }

        public override Type ObjectType
        {
            get { return typeof(IPortalDestination); }
        }

        public override void Load(RoomObject roomObject)
        {
            _roomObject = roomObject;
            _destinationObject = roomObject as IPortalDestination;
            UpdateLabel();
        }

        private void UpdateLabel()
        {
            if (!string.IsNullOrEmpty(_destinationObject.DestinationShard))
            {
                // inter shard
                var linkText = string.Format("{0} / {1}", _destinationObject.DestinationShard, _destinationObject.DestinationRoom);
                _label.text = RoomLink.FormatTMPLink(_destinationObject.DestinationShard, _destinationObject.DestinationRoom, linkText);
            }
            else
            {
                // inter room
                var linkText = string.Format("{1} ({2})", _destinationObject.DestinationShard, _destinationObject.DestinationRoom, _destinationObject.DestinationPosition);
                _label.text = RoomLink.FormatTMPLink(_destinationObject.DestinationShard, _destinationObject.DestinationRoom, linkText);
            }
            
        }

        public override void Unload()
        {
            if (_roomObject == null)
                return;
            _roomObject = null;
        }
    }
}