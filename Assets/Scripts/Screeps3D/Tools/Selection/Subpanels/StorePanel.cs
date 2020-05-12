using System;
using System.Linq;
using System.Text;
using Screeps3D.RoomObjects;
using TMPro;
using UnityEngine;

namespace Screeps3D.Tools.Selection.Subpanels
{
    public class StorePanel : SelectionSubpanel
    {
        [SerializeField] private TextMeshProUGUI _label = default;

        private IStoreObject _selected;
        private RoomObject _roomObject;

        public override string Name
        {
            get { return "Store"; }
        }

        public override Type ObjectType
        {
            get { return typeof(IStoreObject); }
        }

        public override void Load(RoomObject roomObject)
        {
            _roomObject = roomObject;
            _roomObject.OnDelta += OnDelta;
            _selected = roomObject as IStoreObject;
            UpdateLabel();
        }

        private void UpdateLabel()
        {
            var resources = _selected.Store
                .Where(a => a.Value > 0)
                .OrderBy(a => GetResourceOrder(a.Key)).ToList();

            if (resources.Count == 0)
            {
                Hide();
                return;
            }
            if (!IsVisible)
            {
                Show();
            }

            var sb = new StringBuilder();
            foreach (var resource in resources)
                sb.AppendLine(string.Format("{0}: {1:n0}", char.ToUpper(resource.Key[0]) + resource.Key.Substring(1), resource.Value));

            _label.text = sb.ToString();

            Rect.sizeDelta = new Vector2(Rect.sizeDelta.x, resources.Count * 20);
            Height = resources.Count * LineHeight;
        }

        private void OnDelta(JSONObject obj)
        {
            var store = obj.HasField("store") ? obj["store"] : obj; // this supports both PRE and POST store update
            var hasChanged = store == null || store.keys.Any(k => Constants.ResourcesAll.Contains(k));

            if (hasChanged)
            {
                UpdateLabel();
            }
        }

        private static short GetResourceOrder(string resourceType)
        {
            switch (resourceType[0])
            {
                case 'e':
                    return 0;
                case 'p':
                    return 1;
                default:
                    return (short)resourceType.Length;
            }
        }

        public override void Unload()
        {
            _roomObject.OnDelta -= OnDelta;
            _roomObject = null;
            _selected = null;
        }
    }
}