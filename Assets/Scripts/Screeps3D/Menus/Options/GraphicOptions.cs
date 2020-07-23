using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Assets.Scripts.Screeps3D.Menus.Options
{
    public class GraphicOptions : MonoBehaviour
    {
        ////public AudioMixer audioMixer;
        public TMP_Dropdown resolutionDropdown;
        public TMP_Dropdown qualityDropdown;
        ////public TMP_Dropdown textureDropdown;
        ////public TMP_Dropdown aaDropdown;
        ////public Slider volumeSlider;

        float currentVolume;
        Resolution[] resolutions;

        // Start is called before the first frame update
        void Start()
        {
            resolutionDropdown.ClearOptions();
            List<string> options = new List<string>();
            resolutions = Screen.resolutions.OrderByDescending(r => r.width).ThenByDescending(r => r.height).ToArray();
            int currentResolutionIndex = 0;

            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                options.Add(option);

                if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                    currentResolutionIndex = i;
            }

            resolutionDropdown.AddOptions(options);
            resolutionDropdown.RefreshShownValue();

            qualityDropdown.ClearOptions();

            var qualityOptions = new List<TMP_Dropdown.OptionData>();
            for (int i = 0; i < QualitySettings.names.Length; i++)
            {
                qualityOptions.Add(new TMP_Dropdown.OptionData(QualitySettings.names[i]));
            }

            qualityDropdown.AddOptions(qualityOptions);
            qualityDropdown.value = QualitySettings.GetQualityLevel();


            resolutionDropdown.onValueChanged.AddListener(SetResolution);
            qualityDropdown.onValueChanged.AddListener(SetQuality);

            LoadSettings(currentResolutionIndex);
        }

        ////public void SetVolume(float volume)
        ////{
        ////    audioMixer.SetFloat("Volume", volume);
        ////    currentVolume = volume;
        ////}

        public void SetFullscreen(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
        }

        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            PlayerPrefs.SetInt("ResolutionPreference", resolutionIndex);
        }

        public void SetTextureQuality(int textureIndex)
        {
            QualitySettings.masterTextureLimit = textureIndex;
            qualityDropdown.value = 6;
        }

        public void SetAntiAliasing(int aaIndex)
        {
            QualitySettings.antiAliasing = aaIndex;
            qualityDropdown.value = 6;
        }

        public void SetQuality(int qualityIndex)
        {

            if (qualityIndex != 6) // if the user is not using any of the presets
                QualitySettings.SetQualityLevel(qualityIndex);

            ////switch (qualityIndex)
            ////{
            ////    case 0: // quality level - very low
            ////        textureDropdown.value = 3;
            ////        aaDropdown.value = 0;
            ////        break;
            ////    case 1: // quality level - low
            ////        textureDropdown.value = 2;
            ////        aaDropdown.value = 0;
            ////        break;
            ////    case 2: // quality level - medium
            ////        textureDropdown.value = 1;
            ////        aaDropdown.value = 0;
            ////        break;
            ////    case 3: // quality level - high
            ////        textureDropdown.value = 0;
            ////        aaDropdown.value = 0;
            ////        break;
            ////    case 4: // quality level - very high
            ////        textureDropdown.value = 0;
            ////        aaDropdown.value = 1;
            ////        break;
            ////    case 5: // quality level - ultra
            ////        textureDropdown.value = 0;
            ////        aaDropdown.value = 2;
            ////        break;
            ////}

            //qualityDropdown.value = qualityIndex;
            PlayerPrefs.SetInt("QualitySettingPreference", qualityIndex);
        }

        public void SaveSettings()
        {
            PlayerPrefs.SetInt("QualitySettingPreference", qualityDropdown.value);
            PlayerPrefs.SetInt("ResolutionPreference", resolutionDropdown.value);
            //PlayerPrefs.SetInt("TextureQualityPreference", textureDropdown.value);
            //PlayerPrefs.SetInt("AntiAliasingPreference", aaDropdown.value);
            PlayerPrefs.SetInt("FullscreenPreference", Convert.ToInt32(Screen.fullScreen));
            PlayerPrefs.SetFloat("VolumePreference", currentVolume);
        }

        public void LoadSettings(int currentResolutionIndex)
        {
            if (PlayerPrefs.HasKey("QualitySettingPreference"))
                qualityDropdown.value = PlayerPrefs.GetInt("QualitySettingPreference");
            else
                qualityDropdown.value = QualitySettings.GetQualityLevel();

            if (PlayerPrefs.HasKey("ResolutionPreference"))
                resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionPreference");
            else
                resolutionDropdown.value = currentResolutionIndex;

            ////if (PlayerPrefs.HasKey("TextureQualityPreference"))
            ////    textureDropdown.value = PlayerPrefs.GetInt("TextureQualityPreference");
            ////else
            ////    textureDropdown.value = 0;

            ////if (PlayerPrefs.HasKey("AntiAliasingPreference"))
            ////    aaDropdown.value = PlayerPrefs.GetInt("AntiAliasingPreference");
            ////else
            ////    aaDropdown.value = 0;

            if (PlayerPrefs.HasKey("FullscreenPreference"))
                Screen.fullScreen = Convert.ToBoolean(PlayerPrefs.GetInt("FullscreenPreference"));
            ////else
            ////    Screen.fullScreen = true;

            ////if (PlayerPrefs.HasKey("VolumePreference"))
            ////    volumeSlider.value = PlayerPrefs.GetFloat("VolumePreference");
            ////else
            ////    volumeSlider.value = PlayerPrefs.GetFloat("VolumePreference");
        }
    }
}
