using Common;
using UnityEngine;
using UnityEngine.UI;

namespace Screeps3D.Menus.Options
{
    public class MultiSelect : MonoBehaviour
    {
        public static bool IsOn { get; private set; }

        [SerializeField] private Toggle _multiToggle = default;
        [SerializeField] private Toggle _clickDragToggle = default;

        private readonly string _prefsKey = "MultiSelect";

        private void Start()
        {
            IsOn = PlayerPrefs.GetInt(_prefsKey, 1) == 1;
            _multiToggle.isOn = IsOn;
            _clickDragToggle.isOn = !IsOn;
            _multiToggle.onValueChanged.AddListener(OnToggle);
            UpdateRig(IsOn);
        }

        private void OnToggle(bool isOn)
        {
            PlayerPrefs.SetInt(_prefsKey, isOn ? 1 : 0);
            IsOn = isOn;
            UpdateRig(IsOn);
        }

        private void UpdateRig(bool isOn)
        {
            CameraRig.ClickToPan = !isOn;
        }
    }
}