using System;
using Common;
using Screeps3D.RoomObjects;
using TMPro;
using UnityEngine;

namespace Screeps3D.Tools.Selection.Subpanels
{
    public class ProgressPanel : LinePanel
    {
        [SerializeField] private TextMeshProUGUI _label = default;
        [SerializeField] private ScaleAxes _meter = default;
        
        private IProgress _progressor;
        private RoomObject _roomObject;

        public override string Name
        {
            get { return "Progress"; }
        }

        public override Type ObjectType
        {
            get { return typeof(IProgress); }
        }

        public override void Load(RoomObject roomObject)
        {
            _progressor = roomObject as IProgress;
            if (_progressor.ProgressMax == 0)
            {
                gameObject.SetActive(false);
            } else
            {
                gameObject.SetActive(true);
                _roomObject = roomObject;
                _roomObject.OnDelta += OnDelta;
                UpdateDisplay();
            }
        }

        private void UpdateDisplay()
        {
            _label.text = string.Format("{0:n0} / {1:n0}", _progressor.Progress, _progressor.ProgressMax);
            _meter.SetVisibility(_progressor.Progress / _progressor.ProgressMax);
        }

        private void OnDelta(JSONObject obj)
        {
            UpdateDisplay();
        }

        public override void Unload()
        {
            _progressor = null;
            if (_roomObject != null)
                _roomObject.OnDelta -= OnDelta;
            _roomObject = null;
        }
    }
}