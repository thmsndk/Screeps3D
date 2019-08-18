using System;
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
            // TODO: convert to room links

            if (!string.IsNullOrEmpty(_destinationObject.DestinationShard))
            {
                // inter shard
                _label.text = string.Format("{0} / {1}", _destinationObject.DestinationShard, _destinationObject.DestinationRoom);
            }
            else
            {
                // inter room
                _label.text = string.Format("{1} ({2})", _destinationObject.DestinationShard, _destinationObject.DestinationRoom, _destinationObject.DestinationPosition);
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