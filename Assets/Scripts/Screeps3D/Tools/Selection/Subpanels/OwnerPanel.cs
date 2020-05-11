using System;
using Screeps3D.RoomObjects;
using Screeps_API;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Screeps3D.Tools.Selection.Subpanels
{
    public class OwnerPanel : LinePanel
    {
        [SerializeField] private TextMeshProUGUI _label = default;
        [SerializeField] private Image _badge = default;

        private IOwnedObject _selected;

        public override string Name
        {
            get { return "Owner"; }
        }

        public override Type ObjectType
        {
            get { return typeof(IOwnedObject); }
        }

        public override void Load(RoomObject roomObject)
        {
            _selected = roomObject as IOwnedObject;
            if (_selected != null && _selected.Owner != null)
            {
                gameObject.SetActive(true);
                _label.text = string.Format("{0}", _selected.Owner.Username);
                _badge.sprite = Sprite.Create(_selected.Owner.Badge,
                    new Rect(0.0f, 0.0f, BadgeManager.BADGE_SIZE, BadgeManager.BADGE_SIZE), new Vector2(.5f, .5f));
            } 
            else
            {
                gameObject.SetActive(false);
            }
        }

        public override void Unload()
        {
            _selected = null;
        }
    }
}