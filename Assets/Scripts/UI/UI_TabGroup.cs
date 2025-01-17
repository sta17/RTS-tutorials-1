using System.Collections.Generic;
using UnityEngine;

public class UI_TabGroup : MonoBehaviour
{
    public List<UI_TabButtonScript> tabButtons;
    public Sprite tabIdle;
    public Sprite tabHover;
    public Sprite tabSelected;
    public UI_TabButtonScript selectedTab;
    public List<GameObject> objectsToSwap;

    public UI_TabButtonScript startButton;

    public GameObject TabButtons;
    public GameObject PageArea;

    public List<GameObject> objectsToStart;

    public void Start()
    {
        foreach (GameObject item in objectsToStart)
        {
            item.SetActive(true);
            item.SetActive(false);
        }
    }

    public void TurnOn()
    {
        PageArea.SetActive(true);
        TabButtons.SetActive(true);
        OnTabSelected(startButton);
    }

    public void Subscribe(UI_TabButtonScript button)
    {
        if (tabButtons == null)
        {
            tabButtons = new List<UI_TabButtonScript>();
        }

        tabButtons.Add(button);
    }

    public void OnTabEnter(UI_TabButtonScript button)
    {
        if (selectedTab == null || selectedTab != button)
        {
            button.background.sprite = tabHover;
        }
    }

    public void OnTabExit(UI_TabButtonScript button)
    {
        ResetTabs();
    }

    public void OnTabSelected(UI_TabButtonScript button)
    {
        if (selectedTab != null)
        {
            selectedTab.DeSelect();
        }

        selectedTab = button;

        selectedTab.Select();

        ResetTabs();

        selectedTab.background.sprite = tabSelected;

        int index = button.transform.GetSiblingIndex();

        for (int i = 0; i < objectsToSwap.Count; i++)
        {
            if (i == index)
            {
                objectsToSwap[i].SetActive(true);
            }
            else
            {
                objectsToSwap[i].SetActive(false);
            }
        }

    }

    public void ResetTabs()
    {
        foreach (UI_TabButtonScript button in tabButtons)
        {
            if (button != null && button != selectedTab)
            {
                button.background.sprite = tabIdle;
            }
        }
    }

}
