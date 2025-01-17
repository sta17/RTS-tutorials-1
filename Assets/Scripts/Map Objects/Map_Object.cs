using UnityEngine;

public enum Map_Type
{
    Unit,
    Item,
    Doodad
}

public class Map_Object : MonoBehaviour
{
    [Header("Map")]
    [SerializeField] public Map_Type Map_type;
    [SerializeField] protected ScriptableObject Map_Object_Data;
    [SerializeField] private int mapObjectID;

    [Header("Selection and Interaction")]
    [SerializeField] private bool IsSelectable;
    [SerializeField] private float InteractRange;
    [SerializeField] private int selectionID;
    [SerializeField] private Canvas SelectionCircle;

    public Map_Type GetMap_Type()
    {
        return Map_type;
    }

    public bool GetIsSelectable()
    {
        return IsSelectable;
    }

    [System.Obsolete("Possible Removal")]
    public int getSelectionID()
    {
        return selectionID;
    }

    public float getInteractRange()
    {
        return InteractRange;
    }

    public void SetSelected(bool isSelected)
    {
        SelectionCircle.gameObject.SetActive(isSelected);
    }
}
