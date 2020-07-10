using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    public List<TabButton> tabButtons;

    private TabButton selectedTab;

    public List<GameObject> tabs;

    public Color32 tabColor;
    public Color32 tabHoverColor;
    public Color32 tabSelectedColor;

    public void Subscribe(TabButton button)
    {
        if (tabButtons == null)
        {
            tabButtons = new List<TabButton>();
        }

        tabButtons.Add(button);
    }

    public void OnTabEnter(TabButton button)
    {
        if (selectedTab != button)
        {
            button.background.color = tabHoverColor;
        }
    }
    public void OnTabExit(TabButton button)
    {
        if (selectedTab != button)
        {
            button.background.color = tabColor;
        }
    }

    public void OnTabSelected(TabButton button)
    {
        if (selectedTab != null)
        {
            selectedTab.background.color = tabColor;
        }

        selectedTab = button;

        button.background.color = tabSelectedColor;

        int index = button.transform.GetSiblingIndex();
        for (int i = 0; i < tabs.Count; i++)
        {
            tabs[i].SetActive(i == index);
        }
    }

    public void ResetTabs()
    {
        foreach (var button in tabButtons)
        {
            if (selectedTab == button)
            {
                continue;
            }

            // reset
            button.background.color = tabColor;
        }
    }

}
