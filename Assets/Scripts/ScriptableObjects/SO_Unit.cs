using System.Text;
using UnityEngine;

[CreateAssetMenu(menuName = "Unit/BaseUnit")]
public class SO_Unit : ScriptableObject, UI_Info
{
    [field: Header("Unit Stats")]
    [field: SerializeField] public float attackDamage { get; private  set; }
    [field: SerializeField] public float attackRange { get; private set; }
    [field: SerializeField] public float attackSpeed { get; private set; }
    [field: SerializeField] public float unitMaxHealth { get; private set; }
    [field: SerializeField] public float aetherMaxHealth { get; private set; }


    [field: Header("Is Character")]
    [field: SerializeField] public bool IsCharacter { get; set; }

    [field: Header("Icon and Name")]

    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public string DisplayName { get; private set; }

    [TextArea(15, 20)]
    [field: SerializeField] public string description;

    [field: Header("Player and Faction")]
    [field: SerializeField] public bool usePlayerColour { get; private set; }
    public int Id = -1;

    void OnValidate()
    {
        IsCharacter = false;
    }

    public void OnBeforeSerialize()
    {
        IsCharacter = false;
    }

    public string GetTooltipInfoText()
    {
        StringBuilder builder = new();

        builder.Append("Unit Description").AppendLine();
        builder.Append(description).AppendLine();
        return builder.ToString();
    }

    public string GetColouredName()
    {
        var rarityColour = Color.black;

        string hexColour = ColorUtility.ToHtmlStringRGB(rarityColour);
        return $"<color=#{hexColour}>{DisplayName}</color>";
    }
}
