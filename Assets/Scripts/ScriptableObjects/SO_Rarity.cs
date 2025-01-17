using UnityEngine;

[CreateAssetMenu(fileName = "New Rarity", menuName = "Inventory System/Items/Rarity")]
public class SO_Rarity : ScriptableObject
{
    [SerializeField] private new string name;
    [SerializeField] private Color textColour;

    public string Name { get { return name; } }
    public Color TextColour { get { return textColour; } }
}