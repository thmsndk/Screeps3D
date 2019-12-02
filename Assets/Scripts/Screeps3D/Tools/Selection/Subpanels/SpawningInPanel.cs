using System;
using Common;
using Screeps3D.RoomObjects;
using TMPro;
using UnityEngine;
using WebSocketSharp;

namespace Screeps3D.Tools.Selection.Subpanels
{
    public class SpawningInPanel : LinePanel
    {
        [SerializeField] private TextMeshProUGUI _label;
        private ISpawningInObject _spawningInObject;
        private RoomObject _roomObject;

        public override string Name
        {
            get { return "SpawningIn"; }
        }

        public override Type ObjectType
        {
            get { return typeof(ISpawningInObject); }
        }

        public override void Load(RoomObject roomObject)
        {
            _roomObject = roomObject;
            roomObject.OnDelta += OnDelta;
            _spawningInObject = roomObject as ISpawningInObject;
            UpdateLabel();
        }

        private void UpdateLabel()
        {
            if (_spawningInObject.NextSpawnTime > 0)
            {
                _label.text = string.Format("{0:n0}", _spawningInObject.NextSpawnTime - _spawningInObject.Room.GameTime);
            }
            else
            {
                _label.text = string.Format("Unknown");
            }
        }

        private void OnDelta(JSONObject obj)
        {
            UpdateLabel();
        }

        public override void Unload()
        {
            if (_roomObject == null)
                return;
            _roomObject.OnDelta -= OnDelta;
            _roomObject = null;
        }
    }
}