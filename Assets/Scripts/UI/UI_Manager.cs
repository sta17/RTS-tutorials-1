using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    [Header("Controllers")]
    [SerializeField] private GeneralInputAction menuActions;
    [SerializeField] private bool UIActive = false;

    [Header("Screens")]
    public GameObject TabButtons;
    public GameObject PageArea;
    public GameObject Party;

    public UI_TabGroup UnitDetailsHandler;

    public GameObject Encyclopedia;

    public GameObject Options;
    public GameObject Save;
    public GameObject Map;

    public GameObject Menu;

    [Header("Misc")]
    [SerializeField] private UI_TooltipPopup tooltipPopup;
    public GameObject HorizontalBackground;
    public GameObject VerticalBackground;
    public UI_StatsPanel UI_StatsPanel;
    public UI_Selection_List_Generated selectionList;

    public void setMenuAction(GeneralInputAction menuActions)
    {
        this.menuActions = menuActions;
    }

    public bool isActive()
    {
        return UIActive;
    }

    #region Selection

    public void AddSelection(Map_Unit Unit)
    {
        selectionList.AddUnit(Unit);
    }

    public void ClearSelection()
    {
        selectionList.ClearSelectionList();
    }

    #endregion

    #region Stat Panel

    public void Add_Map_Unit(Map_Unit Unit)
    {
        if (Unit.unit.IsCharacter)
        {
            UI_StatsPanel.ClearDisplay();
            UI_StatsPanel.Add_Hero((Map_Hero)Unit);
        }
        else
        {
            UI_StatsPanel.ClearDisplay();
            UI_StatsPanel.Add_Unit(Unit);
        }
    }

    public void Add_Map_Item(Map_Item mapObject)
    {
        UI_StatsPanel.ClearDisplay();
        UI_StatsPanel.Add_Item((Map_Item)mapObject);
    }

    public void ClearDisplay()
    {
        UI_StatsPanel.ClearDisplay();
    }

    #endregion

    #region Hide Show Functions

    public void DisplayHideMenuWindow()
    {
        if (Menu.activeSelf)
        {
            UIOff();
        }
        else
        {
            UIOff();
            VerticalBackground.SetActive(true);
            Menu.SetActive(true);
            UIActive = true;
        }
    }

    private void UIOff()
    {
        HorizontalBackground.SetActive(false);
        VerticalBackground.SetActive(false);

        Menu.SetActive(false);

        TabButtons.SetActive(false);
        PageArea.SetActive(false);
        Party.SetActive(false);
        Encyclopedia.SetActive(false);
        Options.SetActive(false);
        Save.SetActive(false);
        Map.SetActive(false);
        UIActive = false;
        tooltipPopup.HideInfo();
    }

    public void DisplayHideInventoryWindow()
    {
        if (TabButtons.activeSelf || PageArea.activeSelf)
        {
            UIOff();
        }
        else
        {
            VerticalBackground.SetActive(false);
            HorizontalBackground.SetActive(true);
            UnitDetailsHandler.TurnOn();
            UIActive = true;
        }
    }

    public void DisplayHideEncyclopediaWindow()
    {
        if (Encyclopedia.activeSelf)
        {
            UIOff();
        }
        else
        {
            HorizontalBackground.SetActive(false);
            Encyclopedia.SetActive(true);
            UIActive = true;
        }
    }

    public void DisplayHideOptionsWindow()
    {
        if (Options.activeSelf)
        {
            UIOff();
        }
        else
        {
            HorizontalBackground.SetActive(false);
            Options.SetActive(true);
            UIActive = true;
        }
    }

    public void DisplayHideMapWindow()
    {
        if (Map.activeSelf)
        {
            UIOff();
        }
        else
        {
            HorizontalBackground.SetActive(false);
            Map.SetActive(true);
            UIActive = true;
        }
    }

    public void DisplayHidePartyWindow()
    {
        if (Party.activeSelf)
        {
            UIOff();
        }
        else
        {
            HorizontalBackground.SetActive(false);
            Party.SetActive(true);
            UIActive = true;
        }
    }

    #endregion

    #region Buttons Pressed

    public void EncyclopediaButtonPressed()
    {
        Menu.SetActive(false);
        DisplayHideEncyclopediaWindow();
    }

    public void OptionsButtonPressed()
    {
        Menu.SetActive(false);
        DisplayHideOptionsWindow();
    }

    public void SaveButtonPressed()
    {
        Menu.SetActive(false);
        Save.SetActive(true);
    }

    public void PartyButtonPressed()
    {
        Menu.SetActive(false);
        DisplayHidePartyWindow();
    }

    public void InventoryButtonPressed()
    {
        Menu.SetActive(false);
        DisplayHideInventoryWindow();
    }

    public void MapButtonPressed()
    {
        Menu.SetActive(false);
        DisplayHideMapWindow();
    }

    #endregion
}
