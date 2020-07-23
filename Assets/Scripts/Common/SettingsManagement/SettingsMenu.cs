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
        [SerializeField] public TabGroup TabGroup;
        [SerializeField] public TabButton ButtonPrefab;
        [SerializeField] public GameObject PagePrefab;
        [SerializeField] public GameObject PageSectionPrefab;
        private void Awake()
        {
            var provider = new SettingsProvider();
            provider.SearchForSettingsAttribute();

            ButtonPrefab.tabGroup = TabGroup;

            foreach (var category in provider.m_Settings.OrderBy(c => c.Key))
            {
                var categoryName = category.Key.Substring(0, category.Key.IndexOf("/"));

                var buttonName = categoryName + "TabButton";

                var button = TabGroup.tabsContainer.GetComponentsInChildren<TabButton>().SingleOrDefault(b => b.name == buttonName);
                if (button == null)
                {
                    button = Instantiate(ButtonPrefab, TabGroup.transform);
                    button.name = buttonName;
                    button.label.text = categoryName;
                    TabGroup.Subscribe(button);
                }


                var pageName = categoryName + "Tab";
                var page = TabGroup.pagesContainer.GetComponentsInChildren<TabPage>(true).SingleOrDefault(b => b.name == pageName);

                if (page == null)
                {
                    var prefab = Resources.Load<TabPage>("Prefabs/Options/" + "Page");
                    page = Instantiate(prefab, TabGroup.pagesContainer.transform);
                    page.name = pageName;
                    page.gameObject.SetActive(false);
                    TabGroup.tabs.Add(page);
                }

                // TODO: Sections
                var sectionName = category.Key.Substring(category.Key.IndexOf("/") + 1);
                var section = page.Content.GetComponentsInChildren<TabPageSection>().SingleOrDefault(b => b.name == sectionName);

                if (section == null)
                {
                    var prefab = Resources.Load<TabPageSection>("Prefabs/Options/" + "PageSection");
                    section = Instantiate(prefab, page.Content.transform);
                    section.name = sectionName;
                    section.Title.text = sectionName;
                }

                // Loop settings on that tabgroup and add them to page
                foreach (var setting in category.Value.OrderBy(s => s.content.text))
                {

                    switch (Type.GetTypeCode(setting.ValueType))
                    {
                        case TypeCode.Boolean:
                            LoadLabelToggle(section.Content, setting);
                            break;
                        case TypeCode.Int32:
                        case TypeCode.Single:
                        case TypeCode.String:
                        default:
                            LoadLabelInput(section.Content, setting);
                            break;
                    }
                }
            }
        }

        private static void LoadLabelInput(GameObject parent, SettingsProvider.SettingEntry setting)
        {
            var prefab = Resources.Load("Prefabs/Options/" + "LabelInput") as GameObject;
            var labelInput = Instantiate(prefab, parent.transform);
            labelInput.name = setting.content.text;

            var label = labelInput.GetComponentInChildren<TMP_Text>();
            label.text = setting.content.text;

            var input = labelInput.GetComponentInChildren<TMP_InputField>();
            input.text = setting.GetValue()?.ToString();
            input.onValueChanged.AddListener(value => setting.SetValue(value));
            if (setting.Secret)
            {
                input.inputType = TMP_InputField.InputType.Password;
            }
        }

        private static void LoadLabelToggle(GameObject parent, SettingsProvider.SettingEntry setting)
        {
            var prefab = Resources.Load("Prefabs/Options/" + "LabelToggle") as GameObject;
            var labelInput = Instantiate(prefab, parent.transform);
            labelInput.name = setting.content.text;

            var label = labelInput.GetComponentInChildren<TMP_Text>();
            label.text = setting.content.text;
            var input = labelInput.GetComponentInChildren<Toggle>();
            input.isOn = (bool)setting.GetValue();
            input.onValueChanged.AddListener(value => setting.SetValue(input.isOn));
        }
    }
}
