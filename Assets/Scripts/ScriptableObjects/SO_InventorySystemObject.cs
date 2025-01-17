using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public enum InterfaceType
{
    Inventory,
    Equipment,
    Chest
}

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class SO_InventorySystemObject : ScriptableObject
{
    public string savePath;
    public SO_Item_DatabaseObject database;
    public InterfaceType type;
    public Inventory Container;
    public InventorySlot[] GetSlots { get { return Container.Slots; } }
    //private System.Boolean IsNotCurrentlySaving = true;

    public bool AddItem(SO_Item _item, int _amount)
    {
        if (EmptySlotCount <= 0)
            return false;
        InventorySlot slot = FindItemOnInventory(_item);
        if (!database.ItemObjects[_item.Id].stackable || slot == null)
        {
            SetEmptySlot(_item, _amount);
            return true;
        }
        slot.AddAmount(_amount);
        return true;
    }
    
    public int EmptySlotCount
    {
        get
        {
            int counter = 0;
            for (int i = 0; i < GetSlots.Length; i++)
            {
                SO_Item tempItem = GetSlots[i].GetItem_Data();
                if (tempItem == null)
                {
                    counter++;
                }
            }
            return counter;
        }
    }
    
    public InventorySlot FindItemOnInventory(SO_Item _item)
    {
        for (int i = 0; i < GetSlots.Length; i++)
        {
            SO_Item tempItem = GetSlots[i].GetItem_Data();
            if( tempItem != null)
            {
                if (tempItem.Id == _item.Id)
                {
                    return GetSlots[i];
                }
            }
        }
        return null;
    }
    
    public InventorySlot SetEmptySlot(SO_Item _item, int _amount)
    {
        for (int i = 0; i < GetSlots.Length; i++)
        {
            SO_Item tempItem = GetSlots[i].GetItem_Data();
            if (tempItem == null)
            {
                GetSlots[i].UpdateSlot(_item, _amount);
                return GetSlots[i];
            }
        }
        //set up functionality for full inventory
        return null;
    }

    public void SwapItems(InventorySlot item1, InventorySlot item2)
    {
        if (item2.CanPlaceInSlot(item1.ItemObject) && item1.CanPlaceInSlot(item2.ItemObject))
        {
            InventorySlot temp = new InventorySlot(item2.GetItem_Data(), item2.amount);
            item2.UpdateSlot(item1.GetItem_Data(), item1.amount);
            item1.UpdateSlot(temp.GetItem_Data(), temp.amount);
        }
    }

    [ContextMenu("Save")]
    public void Save()
    {
        //if (IsNotCurrentlySaving)
        //{
        //IsNotCurrentlySaving = false;
        //string saveData = JsonUtility.ToJson(this, true);
        //BinaryFormatter bf = new BinaryFormatter();
        //FileStream file = File.Create(string.Concat(Application.persistentDataPath, savePath));
        //bf.Serialize(file, saveData);
        //file.Close();

        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Create, FileAccess.Write);
        formatter.Serialize(stream, Container);
        stream.Close();
        //IsNotCurrentlySaving = true;
        //}
    }
    
    [ContextMenu("Load")]
    public void Load()
    {
        if (File.Exists(string.Concat(Application.persistentDataPath, savePath)))
        {
            //BinaryFormatter bf = new BinaryFormatter();
            //FileStream file = File.Open(string.Concat(Application.persistentDataPath, savePath), FileMode.Open);
            //JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), this);
            //file.Close();

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Open, FileAccess.Read);
            Inventory newContainer = (Inventory)formatter.Deserialize(stream);
            for (int i = 0; i < GetSlots.Length; i++)
            {
                GetSlots[i].UpdateSlot(newContainer.Slots[i].GetItem_Data(), newContainer.Slots[i].amount);
            }
            stream.Close();
        }
    }
    
    [ContextMenu("Clear")]
    public void Clear()
    {
        Container.Clear();
    }

}
[System.Serializable]
public class Inventory
{
    public InventorySlot[] Slots = new InventorySlot[28];
    public void Clear()
    {
        for (int i = 0; i < Slots.Length; i++)
        {
            Slots[i].RemoveItem();
        }
    }
}

public delegate void SlotUpdated(InventorySlot _slot);

[System.Serializable]
public class InventorySlot
{
    public ItemType[] AllowedItems = new ItemType[0];
    [System.NonSerialized]
    public UI_InventoryInterface parent;
    [System.NonSerialized]
    public GameObject slotDisplay;
    [System.NonSerialized]
    public SlotUpdated OnAfterUpdate;
    [System.NonSerialized]
    public SlotUpdated OnBeforeUpdate;
    [System.NonSerialized]
    private SO_Item item;

    [SerializeField] private UI_TooltipPopup tooltipPopup;
    public int amount;

    public SO_Item ItemObject
    {
        get
        {
            if (item != null)
            {
                if (item.Id >= 0)
                {
                    return parent.inventory.database.ItemObjects[item.Id];
                }
            }
            
            return null;
        }
    }

    public InventorySlot()
    {
        UpdateSlot();
    }
    
    public InventorySlot(SO_Item _item, int _amount)
    {
        UpdateSlot(_item, _amount);
    }
    
    public void UpdateSlot(SO_Item _item, int _amount)
    {
        if (OnBeforeUpdate != null)
            OnBeforeUpdate.Invoke(this);
        item = _item;
        amount = _amount;
        if (OnAfterUpdate != null)
            OnAfterUpdate.Invoke(this);
    }

    public void UpdateSlot()
    {
        if (OnBeforeUpdate != null)
            OnBeforeUpdate.Invoke(this);
        amount = 0;
        item = null;
        if (OnAfterUpdate != null)
            OnAfterUpdate.Invoke(this);
    }
    
    public void RemoveItem()
    {
        UpdateSlot();
    }
    
    public void AddAmount(int value)
    {
        UpdateSlot(item, amount += value);
    }

    public void SetItem_Data(SO_Item newData)
    {
        item = newData;
    }

    public SO_Item GetItem_Data()
    {
        return item;
    }

    public bool CanPlaceInSlot(SO_Item _itemObject)
    {
        if (AllowedItems.Length <= 0 || _itemObject == null || _itemObject.Id < 0)
            return true;
        for (int i = 0; i < AllowedItems.Length; i++)
        {
            if (_itemObject.type == AllowedItems[i])
                return true;
        }
        return false;
    }

    public void SetToolTip(UI_TooltipPopup tooltipPopup)
    {
        this.tooltipPopup = tooltipPopup;
    }
    
    public void DisplayToolTip()
    {
        if (item != null)
        {
            if (tooltipPopup != null)
            {
                tooltipPopup.DisplayInfo(item);
            }
        }
    }

    public void HideToolTip()
    {
        if (tooltipPopup != null)
        {
            tooltipPopup.HideInfo();
        }
    }

}
