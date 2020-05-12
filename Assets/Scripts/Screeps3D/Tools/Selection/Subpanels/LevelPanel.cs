using System;
using Common;
using Screeps3D.RoomObjects;
using TMPro;
using UnityEngine;

namespace Screeps3D.Tools.Selection.Subpanels
{
    public class LevelPanel : LinePanel
    {
        [SerializeField] private TextMeshProUGUI _label = default;
        [SerializeField] private ScaleAxes _meter = default;
        
        private ILevel _level;
        private RoomObject _roomObject;

        public override string Name
        {
            get { return "Level"; }
        }

        public override Type ObjectType
        {
            get { return typeof(ILevel); }
        }

        public override void Load(RoomObject roomObject)
        {
            _level = roomObject as ILevel;
            //if (_level.LevelMax == 0)
            //{
            //    gameObject.SetActive(false);
            //} else
            //{
                gameObject.SetActive(true);
                _roomObject = roomObject;
                _roomObject.OnDelta += OnDelta;
                UpdateDisplay();
            //}
        }

        private void UpdateDisplay()
        {
            if (_level.Level > 0)
            {
                _label.text = string.Format("{0:n0} / {1:n0}", _level.Level, _level.LevelMax);
            }
            else
            {
                _label.text = "Neutral";
            }

            _meter.SetVisibility(_level.Level / _level.LevelMax);
        }

        private void OnDelta(JSONObject obj)
        {
            UpdateDisplay();
        }

        public override void Unload()
        {
            _level = null;
            if (_roomObject != null)
                _roomObject.OnDelta -= OnDelta;
            _roomObject = null;
        }
    }
}