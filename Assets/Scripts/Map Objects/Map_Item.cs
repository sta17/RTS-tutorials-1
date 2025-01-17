using UnityEditor;
using UnityEngine;
public enum Item_Map_Type
{
    Generic,
    Use_On_Pickup,
    Auto_Pickup
}

public class Map_Item : Map_Object, ISerializationCallbackReceiver
{
    [Header("Item Properties")]
    [SerializeField] public SO_Item item;
    [SerializeField] public int amount = 1;

    [Header("Selection and Interaction")]
    [SerializeField] public Item_Map_Type item_Map_Type;

    #region Unity

    void OnValidate()
    {
        FixItem();
    }

    public void OnBeforeSerialize()
    {
        FixItem();
    }

    public void FixItem()
    {
#if UNITY_EDITOR

        if (item != null)
        {
            if (item.name != null)
            {
                item = (SO_Item)Map_Object_Data;
                this.gameObject.name = item.name;
            }
            if (item.icon != null)
            {
                GetComponentInChildren<SpriteRenderer>().sprite = item.icon;
                EditorUtility.SetDirty(GetComponentInChildren<SpriteRenderer>());
            }
        }
#endif
    }

    #endregion

    #region Getters and Setters

    public Sprite getIcon()
    {
        return item.icon;
    }

    public Item_Map_Type getItemMapType()
    {
        return item_Map_Type;
    }

    public SO_Item getItem()
    {
        return item;
    }

    #endregion

    #region Misc

    public void ModelCleanUp()
    {
        Destroy(this.gameObject);
    }

    public bool HandlePickupType()
    {
        Debug.Log("Item Picked up");
        item.Use();
        return true;
    }

    public void OnAfterDeserialize()
    {
        
    }

    #endregion

}
