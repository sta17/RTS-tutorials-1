using UnityEngine;

[CreateAssetMenu(menuName = "Unit/Person")]
public class SO_Person : SO_Unit
{

    [field: Header("Level")]
    [field: SerializeField] public int heroExperience { get; private set; } = 0;
    [field: SerializeField] public int heroLevel { get; private set; } = 1;

    [field: Header("Title")]
    [field: SerializeField] public string DisplayTitle { get; private set; }


    [field: Header("Start Person Stats")]
    [field: SerializeField] public int StrengthStart { get; private set; }
    [field: SerializeField] public int FinesseStart { get; private set; }
    [field: SerializeField] public int KnowledgeStart { get; private set; }

    [field: Header("Stat gain")]
    [field: SerializeField] public float healthGainPerLevel { get; private set; }
    [field: SerializeField] public float attackSpeedGainPerLevel { get; private set; }
    [field: SerializeField] public float attackDamageGainPerLevel { get; private set; }
    [field: SerializeField] public float defenseGainPerLevel { get; private set; }

    void OnValidate()
    {
        IsCharacter = true;
    }

    public new void OnBeforeSerialize()
    {
        IsCharacter = true;
    }

}
