using System.Collections;
using Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Screeps3D.Menus.Main
{
    public class MainMenu : VerticalPanelElement
    {
        [SerializeField] private TextMeshProUGUI _description = default;
        [SerializeField] private Toggle _toggle = default;

        private void Start()
        {
            _toggle.onValueChanged.AddListener(OnToggle);
            Hide();
        }

        private void OnToggle(bool isOn)
        {
            Show(isOn);
        }

        public string Description
        {
            get { return _description.text; }
            set { _description.text = value; }
        }
    }
}