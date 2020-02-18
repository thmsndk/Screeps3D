using System;
using UnityEngine;
using UnityEngine.UI;

namespace Screeps3D.Tools.Selection
{
    public class FlagColorToggle : MonoBehaviour
    {
        [SerializeField] private ToggleGroup _colors;
        [SerializeField] private Toggle _colorPrefab;
        // need the group to fill with colors
        // need to determine wich one is toggled on.
        // _selectionToggle.onValueChanged.AddListener(isOn => ToggleInput(isOn, ToolType.Selection));
        public Constants.FlagColor SelectedColor { get; private set; }
        public Action<Constants.FlagColor> OnColorChange;

        private bool initialized;

        private void Start()
        {
            foreach (Transform child in _colors.transform)
            {
                var toggle = child.GetComponent<Toggle>();
                toggle?.onValueChanged.RemoveAllListeners();

                Destroy(child.gameObject);
            }

            foreach (Constants.FlagColor color in Enum.GetValues(typeof(Constants.FlagColor)))
            {
                var toggle = Instantiate(_colorPrefab, _colors.transform);
                toggle.group = _colors;
                toggle.onValueChanged.AddListener(isOn => ToggleInput(toggle, isOn, color));

                var colors = toggle.colors;
                var flagColor = Constants.FlagColors[(int)color];
                colors.normalColor = flagColor;
                colors.selectedColor = flagColor;
                toggle.colors = colors;

                toggle.name = ((int)color).ToString();
                toggle.isOn = color == SelectedColor;
                this.ScaleToggleButton(toggle, toggle.isOn);
            }

            initialized = true;
        }

        private void ToggleInput(Toggle toggle, bool isOn, Constants.FlagColor flagColor)
        {
            this.ScaleToggleButton(toggle, isOn);

            if (isOn)
            {
                SelectedColor = flagColor;
            }

            OnColorChange?.Invoke(flagColor);
        }

        private void ScaleToggleButton(Toggle toggle, bool isOn)
        {
            if (!isOn)
            {
                toggle.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
                return;
            }

            toggle.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        private void Update()
        {
            
        }

        public void SetColor(Constants.FlagColor flagColor)
        {
            SelectedColor = flagColor;

            foreach (Transform child in _colors.transform)
            {
                var toggle = child.GetComponent<Toggle>();
                if (toggle.name == ((int)flagColor).ToString())
                {
                    toggle.isOn = true;
                    break;
                }
            }
        }
    }
}