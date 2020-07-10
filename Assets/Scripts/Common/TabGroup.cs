using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    public List<TabButton> tabButtons;

    private TabButton selectedTab;

    public List<GameObject> tabs;

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

    }
    public void OnTabExit(TabButton button)
    {

    }

    public void OnTabSelected(TabButton button)
    {
        selectedTab = button;

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
            //button.background.sprite = null
        }
    }

}
