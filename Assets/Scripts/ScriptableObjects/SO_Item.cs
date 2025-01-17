using System.Text;
using UnityEngine;

public enum ItemType
{
    Default,
    Food,
    Helmet, Head,

    Boots, Feet,

    Chest,

    Shoulder,
    Wrist,
    Hands,
    Waist,

    Legs,
    Neck,
    Back,
    Finger,

    Trinket,

    Weapon,
    Shield,
    OneHand,
    TwoHand,
    MainHand,
    OffHand
}

public enum Attributes
{
    Agility,
    Intellect,
    Stamina,
    Strength,
    Dexterity,
    Constitution,
    Intelligence,
    Wisdom,
    Charisma,
    Speed,
    Mind,
    Perception,
    Finesse,
    Knowledge
}
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory System/Items/item")]
public class SO_Item : ScriptableObject, UI_Info
{
    [field: SerializeField] public Sprite icon { get; private set; }
    [field: SerializeField] public bool stackable { get; private set; }
    [field: SerializeField] public ItemType type { get; private set; }
    [SerializeField] private string Name;
    [TextArea(15, 20)]
    [SerializeField] private string description;
    public int Id;

    //[SerializeField] private ItemType type;

    [SerializeField] private SO_Rarity rarity;
    [SerializeField] private bool isUseable;
    [SerializeField] private string useText;

    public ItemBuff[] buffs;

    public void Item_Data(SO_Item item)
    {
        Name = item.name;
        Id = item.Id;
        buffs = new ItemBuff[item.buffs.Length];
        rarity = item.rarity;
        for (int i = 0; i < buffs.Length; i++)
        {
            buffs[i] = new ItemBuff(item.buffs[i].min, item.buffs[i].max)
            {
                attribute = item.buffs[i].attribute
            };
        }
    }

    public string GetColouredName
    {
        get
        {
            var rarityColour = Color.black;
            if (rarity != null)
            {
                rarityColour = rarity.TextColour;
            }
            string hexColour = ColorUtility.ToHtmlStringRGB(rarityColour);
            return $"<color=#{hexColour}>{Name}</color>";
        }
    }

    public string GetTooltipInfoText()
    {
        StringBuilder builder = new();

        var rarityName = "Common";
        if (rarity != null)
        {
            rarityName = rarity.Name;
        }

        builder.Append(rarityName).AppendLine();
        builder.Append("Description").AppendLine();
        builder.Append(description).AppendLine();
        if (isUseable)
        {
            builder.Append("<color=green>Use: ").Append(useText).Append("</color>").AppendLine();

        }
        return builder.ToString();
    }

    public bool Use()
    {
        Debug.Log("Item Used");
        return true;
    }

    string UI_Info.GetColouredName()
    {
        var rarityColour = Color.black;
        if (rarity != null)
        {
            rarityColour = rarity.TextColour;
        }
        string hexColour = ColorUtility.ToHtmlStringRGB(rarityColour);
        return $"<color=#{hexColour}>{Name}</color>";
    }
}

[System.Serializable]
public class ItemBuff : IModifier
{
    public Attributes attribute;
    public int value;
    public int min;
    public int max;
    public ItemBuff(int _min, int _max)
    {
        min = _min;
        max = _max;
        GenerateValue();
    }

    public void AddValue(ref int baseValue)
    {
        baseValue += value;
    }

    public void GenerateValue()
    {
        value = UnityEngine.Random.Range(min, max);
    }
}