using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Common.SettingsManagement
{
    public class SettingsMenu : MonoBehaviour
    {
        [Setting("Misc/TestCategory", "TestSetting", "TestTooltip")]
        public static string testSetting;

        [SerializeField] public TabGroup TabGroup;
        [SerializeField] public TabButton ButtonPrefab;
        private void Awake()
        {
            var provider = new SettingsProvider();
            provider.SearchForSettingsAttribute();

            ButtonPrefab.tabGroup = TabGroup;

            foreach (var category in provider.m_Settings)
            {
                var button = Instantiate(ButtonPrefab,TabGroup.transform);
                button.label.text = category.Key;
                TabGroup.Subscribe(button);
            }
            // TODO: for each setting we should add a page
        }
    }
}
