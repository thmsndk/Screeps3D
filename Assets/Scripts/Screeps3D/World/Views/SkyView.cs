using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Screeps3D.World.Views
{
    public class SkyView : MonoBehaviour
    {
        [SerializeField] public Volume _volume;
        SkySettings _skySettings;
        bool sunRise;
        bool sunSet;
        float dayEmission = -2f;
        bool night;
        float nightEmission = -10f;
        float nightLength = 40f;
        float progress = 0.01f;
        void Start()
        {            
            Volume volume = GetComponent<Volume>();
            // SkySettings tempSkySett;    
    
            if (volume.profile.TryGet<HDRISky>(out HDRISky tempSkySett))
            {
                _skySettings = tempSkySett;
            }
            sunRise = true;
            sunSet = false;
            night = false;
        }
        void Update()
        {
            _skySettings.rotation.value += 0.005f;
            if(_skySettings.rotation.value == 360) {
                _skySettings.rotation.value = 0;
            }

            Debug.LogError("_skySettings.exposure.value:" + _skySettings.exposure.value);
            Debug.LogError("night:" + night);
            Debug.LogError("sunRise:" + sunRise);
            Debug.LogError("sunSet:" + sunSet);
            if(night) {
                nightLength -= progress;
                if(nightLength > 0) {
                    return;
                }
                sunRise = true;
                night = false;
            }
            
            if(sunRise) {
                _skySettings.exposure.value += progress;
                if(_skySettings.exposure.value <= dayEmission) {
                    return;
                }
                sunRise = false;
                sunSet = true;
            }

            if(sunSet) {
                _skySettings.exposure.value -= progress;
                if(_skySettings.exposure.value >= nightEmission) {
                    return;
                }
                sunSet = false;
                night = true;
                nightLength = 2f;
            }
        }
    }
}
