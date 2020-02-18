using System;
using System.Linq;
using System.Text;
using Common;
using Screeps3D.RoomObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Screeps3D.Tools.Selection.Subpanels
{
    public class ButtonsPanel : SelectionSubpanel
    {
        public new const float LineHeight = 30;
        [SerializeField] private GameObject _buttonPrefab;
        [SerializeField] private VerticalPanelGroup _buttonList;

        private IButtons _buttons;
        private RoomObject _roomObject;

        private string _path = "Prefabs/Selection/SubPanels/Button";

        public override string Name
        {
            get { return "Buttons"; }
        }

        public override Type ObjectType
        {
            get { return typeof(IButtons); }
        }

        public override void Load(RoomObject roomObject)
        {
            _roomObject = roomObject;
            //_roomObject.OnDelta += OnDelta;
            _buttons = roomObject as IButtons;
            //UpdateLabel();

            ClearButtons();

            //this.Rect.sizeDelta = Vector2.up; // reset height from previous  buttons
            var actions = _buttons.GetButtonActions();
                ////Height = actions.Count * LineHeight;
            foreach (var button in actions)
            {
                AddButtonToSelectionPanel(button, roomObject);
            }
        }

        public override void Unload()
        {
            ClearButtons();

            //_roomObject.OnDelta -= OnDelta;
            _roomObject = null;
            _buttons = null;
        }

        private void ClearButtons()
        {
            ////Debug.Log($"clearing buttons {_buttonList.transform.childCount}");

            foreach (Transform child in _buttonList.transform)
            {
                var element = child.GetComponent<VerticalPanelElement>();
                _buttonList.RemoveElement(element);

                var button = child.gameObject.GetComponent<Button>();

                if (button != null)
                {
                    button.onClick.RemoveAllListeners();
                    child.localPosition = Vector3.zero; // reset position
                    child.SetParent(null);
                    //child.parent = null;
                    PoolLoader.Return(_path, child.gameObject);
                }
            }
        }

        private void AddButtonToSelectionPanel(IRoomObjectPanelButton action, IRoomObject roomObject)
        {
            var go = PoolLoader.Load(_path);
            var text = go.GetComponentInChildren<TMP_Text>();
            text.text = action.Text;

            var button = go.GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                var type = action.GetType();
                var onClick = type.GetMethod("OnClick");
                onClick.Invoke(action, new[] { roomObject });
            });

            var element = go.GetComponent<VerticalPanelElement>();
            element.TargetPos = 0;
            go.transform.localScale = Vector3.one;

            _buttonList.AddElement(element);
            ////go.transform.SetParent(_buttonList.transform);

        }
    }
}