using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_StatsPanel : MonoBehaviour
{
    [Header("Shared")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI object_name;
    [Header("Hero")]
    [SerializeField] private GameObject heroPanel;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI level;
    [Header("Unit")]
    [SerializeField] private GameObject unitPanel;
    [SerializeField] private TextMeshProUGUI hp_value;
    [SerializeField] private TextMeshProUGUI aether_value;
    [SerializeField] private TextMeshProUGUI damage;
    [SerializeField] private TextMeshProUGUI range;
    [SerializeField] private TextMeshProUGUI atk_speed;
    [SerializeField] private Map_Unit unit;
    [Header("Item")]
    [SerializeField] private GameObject itemPanel;
    [SerializeField] private TextMeshProUGUI amount;
    [SerializeField] private TextMeshProUGUI type;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Map_Item item;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (unit != null)
        {
            hp_value.text = unit.getCurrentHealth().ToString() + " of " + unit.getCurrentMaxHealth().ToString();
        }
    }

    public void Add_Hero(Map_Hero hero)
    {
        TurnOffPanels();
        unit = hero;

        hp_value.text = unit.unit.name;
        icon.sprite = unit.unit.Icon;
        icon.enabled = true;

        hp_value.text = unit.getCurrentHealth().ToString() + " of " + unit.getCurrentMaxHealth().ToString();
        aether_value.text = unit.getCurrentAether().ToString() + " of " + unit.getCurrentMaxAether().ToString();

        SO_Person so_hero = (SO_Person)unit.unit;

        level.text = hero.CurrentHeroLevel.ToString();
        title.text = so_hero.DisplayTitle;


        damage.text = unit.unit.attackDamage.ToString();
        range.text = unit.unit.attackRange.ToString();
        atk_speed.text = unit.unit.attackSpeed.ToString();

        heroPanel.SetActive(true);
        unitPanel.SetActive(true);
    }

    public void Add_Unit(Map_Unit unit)
    {
        TurnOffPanels();
        this.unit = unit;

        hp_value.text = unit.unit.name;
        icon.sprite = unit.unit.Icon;
        icon.enabled = true;

        hp_value.text = unit.getCurrentHealth().ToString() + " of " + unit.getCurrentMaxHealth().ToString();
        aether_value.text = unit.getCurrentAether().ToString() + " of " + unit.getCurrentMaxAether().ToString();

        damage.text = unit.unit.attackDamage.ToString();
        range.text = unit.unit.attackRange.ToString();
        atk_speed.text = unit.unit.attackSpeed.ToString();

        unitPanel.SetActive(true);
    }

    public void Add_Item(Map_Item item)
    {
        TurnOffPanels();

        hp_value.text = item.item.name;
        icon.sprite = item.getIcon();
        icon.enabled = true;
        type.text = item.item.type.ToString();
        description.text = item.item.GetTooltipInfoText();
        amount.text = item.amount.ToString();

        //amount,type, description,

        itemPanel.SetActive(true);
    }

    public void TurnOffPanels()
    {
        heroPanel.SetActive(false);
        unitPanel.SetActive(false);
        itemPanel.SetActive(false);
    }

    public void ClearDisplay()
    {
        TurnOffPanels();
        icon.sprite = null;
        icon.enabled = false;

        object_name.text = "";
        amount.text = "";
    }

}
