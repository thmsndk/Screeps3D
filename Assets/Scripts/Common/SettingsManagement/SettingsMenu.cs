using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Common.SettingsManagement
{
    public class SettingsMenu : MonoBehaviour
    {
        [Setting("Misc/TestCategory", "TestSetting", "TestTooltip")]
        public static string testSetting = "200";

        [SerializeField] public TabGroup TabGroup;
        [SerializeField] public TabButton ButtonPrefab;
        [SerializeField] public GameObject PagePrefab;
        private void Awake()
        {
            var provider = new SettingsProvider();
            provider.SearchForSettingsAttribute();

            ButtonPrefab.tabGroup = TabGroup;

            foreach (var category in provider.m_Settings.OrderBy(c => c.Key))
            {
                var categoryName = category.Key.Substring(0, category.Key.IndexOf("/"));

                var buttonName = categoryName + "TabButton";
                // TODO: find or create tab button
                var button = TabGroup.tabsContainer.GetComponentsInChildren<TabButton>().SingleOrDefault(b => b.name == buttonName);
                if (button == null)
                {
                    button = Instantiate(ButtonPrefab,TabGroup.transform);
                    button.name = buttonName;
                    button.label.text = categoryName;
                    TabGroup.Subscribe(button);
                }


                var pageName = categoryName + "Tab";
                var page = TabGroup.pagesContainer.transform.Find(pageName)?.gameObject;

                if (page == null)
                {
                    var prefab = Resources.Load("Prefabs/Options/" + "Page") as GameObject;
                    page = Instantiate(prefab, TabGroup.pagesContainer.transform);
                    page.name = pageName;
                    page.SetActive(false);
                    TabGroup.tabs.Add(page);
                }

                // TODO: Sections

                // Loop settings on that tabgroup and add them to page
                foreach (var setting in category.Value.OrderBy(s => s.content.text))
                {
                    var prefab = Resources.Load("Prefabs/Options/" + "LabelInput") as GameObject;
                    var labelInput = Instantiate(prefab, page.transform);
                    labelInput.name = setting.content.text;
                    
                    var label = labelInput.GetComponentInChildren<TMP_Text>();
                    label.text = setting.content.text;
                    var input = labelInput.GetComponentInChildren<TMP_InputField>();
                    input.text = setting.GetValue()?.ToString(); // TODO: stuff with type
                    input.onValueChanged.AddListener(value => setting.SetValue(value));
                    
                    // it is properly initialized after this though, so how do we get the "proper" value?
                }
            }
            // TODO: for each setting we should add a page
        }
    }
}
