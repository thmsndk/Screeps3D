using Assets.Scripts.Common.SettingsManagement;
using Common;
using Screeps_API;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Screeps3D.Player
{

    public class PlayerResourcesView : MonoBehaviour
    {

        [Setting("Gameplay/UI", "Show Resources", "Should resources be shown or not")]
        private static bool showResources = true;

        [SerializeField] private HorizontalLayoutGroup SecondaryLine = default;
        private RectTransform layoutGroupRect = default;

        [SerializeField] private RectTransform creditsContainer = default;
        [SerializeField] private TMP_Text credits = default;

        [SerializeField] private RectTransform cpuUnlocksContainer = default;
        [SerializeField] private TMP_Text cpuUnlocks = default;

        [SerializeField] private RectTransform pixelsContainer = default;
        [SerializeField] private TMP_Text pixels = default;

        [SerializeField] private RectTransform keysContainer = default;
        [SerializeField] private TMP_Text keys = default;

        private void Start()
        {
            ScreepsAPI.Resources.OnResources += OnResources;
            layoutGroupRect = SecondaryLine.GetComponent<RectTransform>();
        }

        private void OnDisable()
        {
            //ScreepsAPI.Resources.OnResources -= OnResources;
        }

        private void Update()
        {
            
        }

        private void OnResources()
        {
            if (!showResources)
            {
                creditsContainer?.gameObject?.SetActive(showResources);
                cpuUnlocksContainer?.gameObject?.SetActive(showResources);
                pixelsContainer?.gameObject?.SetActive(showResources);
                keysContainer?.gameObject?.SetActive(showResources);

                return;
            }

            if (ScreepsAPI.IsConnected && ScreepsAPI.Me != null && ScreepsAPI.Resources != null)
            {
                credits.text = ScreepsAPI.Resources.Credits.ToString("N0");
                cpuUnlocks.text = ScreepsAPI.Resources.CPUUnlocks.ToString();
                pixels.text = ScreepsAPI.Resources.Pixels.ToString("N0");
                keys.text = ScreepsAPI.Resources.AccessKeys.ToString();

                creditsContainer.gameObject.SetActive(ScreepsAPI.Resources.Credits > 0);
                creditsContainer.sizeDelta = new Vector2(credits.rectTransform.sizeDelta.x, creditsContainer.sizeDelta.y);

                cpuUnlocksContainer.gameObject.SetActive(ScreepsAPI.Resources.CPUUnlocks > 0);
                cpuUnlocksContainer.sizeDelta = new Vector2(cpuUnlocks.rectTransform.sizeDelta.x, cpuUnlocksContainer.sizeDelta.y);

                pixelsContainer.gameObject.SetActive(ScreepsAPI.Resources.Pixels > 0);
                pixelsContainer.sizeDelta = new Vector2(pixels.rectTransform.sizeDelta.x, pixelsContainer.sizeDelta.y);

                keysContainer.gameObject.SetActive(ScreepsAPI.Resources.AccessKeys > 0);
                keysContainer.sizeDelta = new Vector2(keys.rectTransform.sizeDelta.x, keysContainer.sizeDelta.y);

                float layoutsizeX = SecondaryLine.padding.right + SecondaryLine.padding.left;
                layoutsizeX += GetWidthIfActive(cpuUnlocksContainer);
                layoutsizeX += GetWidthIfActive(pixelsContainer);
                layoutsizeX += GetWidthIfActive(keysContainer);

                layoutGroupRect.sizeDelta = new Vector2(layoutsizeX, layoutGroupRect.sizeDelta.y);
            }
        }

        private float GetWidthIfActive(RectTransform container)
        {
            if (container.gameObject.activeSelf)
            {
                return container.sizeDelta.x;
            }

            return 0;
        }
    }
}