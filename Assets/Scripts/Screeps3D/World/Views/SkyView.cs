using Assets.Scripts.Common.SettingsManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Screeps3D.World.Views
{
    public class SkyView : MonoBehaviour
    {
        [SerializeField] public Volume _volume;
        SkySettings _skySettings;
        bool _sunRise;
        bool _sunSet;
        bool _night;
        bool _day;
        float _dayExposition = 1f;
        float _nightExposition = -6f;
        float _expositionChange = 0.001f;

        // day-night lux in range of .04 to 0.01

        [Setting("Gameplay/Day & Night", "Day Brightness", "")]
        static float _dayLux = 0.04f;
        [Setting("Gameplay/Day & Night", "Night Brightness", "")]
        static float _nightLux = 0.01f;

        float _luxChange = 0.0001f;

        // day-night length anywhere between 1 and 1 billion
        [Setting("Gameplay/Day & Night", "Night Length", "")]
        static float _nightLength = 10f;
        [Setting("Gameplay/Day & Night", "Day Length", "")]
        static float _dayLength = 5f;

        float _currentNightProgress = 0f;
        float _currentDayProgress = 0f;
        float _nightProgress = 0.005f;
        float _skyRotation = 0.005f;

        [Setting("Gameplay/Fog", "Fog Attenuation Distance", "")]
        static float _fogAttenuationDistance = 5f;

        [Setting("Gameplay/Fog", "Maximum Height", "")]
        static float _fogMaximumHeight = 15f;

        [Setting("Gameplay/Fog", "Max Fog Distance", "")]
        static float _fogMaxDistance = 0f;

        private Fog _fog;

        void Start()
        {            
            Volume volume = GetComponent<Volume>();
            
            if (volume.profile.TryGet<HDRISky>(out HDRISky tempSkySett))
            {
                _skySettings = tempSkySett;
            }

            if (volume.profile.TryGet<Fog>(out var fog))
            {
                _fog = fog;
                UpdateFogSettings();
            }

            _sunRise = false;
            _day = true;
            _sunSet = false;
            _night = false;
        }

        private void UpdateFogSettings()
        {
            _fog.maximumHeight.value = _fogMaximumHeight;
            _fog.maxFogDistance.value = _fogMaxDistance;
            _fog.meanFreePath.value = _fogAttenuationDistance;
        }

        private void rotateSky() {
            _skySettings.rotation.value += _skyRotation;
            if(_skySettings.rotation.value == 360) {
                _skySettings.rotation.value = 0;
            }
        }

        private void expositionSkySet() {
            if(_night) {
                _nightLength -= _expositionChange;
                if(_nightLength > 0) {
                    return;
                }
                _sunRise = true;
                _night = false;
            }
            
            if(_sunRise) {
                _skySettings.exposure.value += _expositionChange;
                if(_skySettings.exposure.value <= _dayExposition) {
                    return;
                }
                _sunRise = false;
                _sunSet = true;
            }

            if(_sunSet) {
                _skySettings.exposure.value -= _expositionChange;
                if(_skySettings.exposure.value >= _nightExposition) {
                    return;
                }
                _sunSet = false;
                _night = true;
                _nightLength = 2f;
            }
        }

        private void changeLux(float val) {
            if(_skySettings.desiredLuxValue.value < 0.03) {
                val = 0.1f * val;
            }
            if(_skySettings.desiredLuxValue.value < 0.001) {
                val = 0.1f * val;
            }
            _skySettings.desiredLuxValue.value = Mathf.Max(_nightLux, _skySettings.desiredLuxValue.value + val);
        }
        private void luxSkySet() {
            if(_night) {
                _currentNightProgress += _nightProgress;
                if(_currentNightProgress < _nightLength) {
                    return;
                }
                _sunRise = true;
                _night = false;
                _currentNightProgress = 0;
            }
            
            if(_sunRise) {
                if(_skySettings.desiredLuxValue.value <= _dayLux) {
                    changeLux(_luxChange);
                    return;
                }
                _sunRise = false;
                _day = true;
            }
            
            if(_day) {
                _currentDayProgress += _nightProgress;
                if(_currentDayProgress < _dayLength) {
                    return;
                }
                _day = false;
                _sunSet = true;
                _currentDayProgress = 0;
            }

            if(_sunSet) {
                if(_skySettings.desiredLuxValue.value > _nightLux) {
                    changeLux(_luxChange * -1f);
                    return;
                }
                _sunSet = false;
                _night = true;
            }
        }

        void Update()
        {
            if ((long)Time.time % 2 != 0) {
                return;
            }
            rotateSky();
            // return;
            luxSkySet();

            UpdateFogSettings();
        }
    }
}
