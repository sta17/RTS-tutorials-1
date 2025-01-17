using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public abstract class UI_InventoryInterface : MonoBehaviour
{
    [SerializeField] private UI_TooltipPopup tooltipPopup;
    public SO_InventorySystemObject inventory;
    public Dictionary<GameObject, InventorySlot> slotsOnInterface = new();
    public bool IsInitialised = false;

     public void Awake()
    {
        Setup();
    }

    public void Setup()
    {
        for (int i = 0; i < inventory.GetSlots.Length; i++)
        {
            inventory.GetSlots[i].parent = this;
            inventory.GetSlots[i].OnAfterUpdate += OnSlotUpdate;

        }
        CreateSlots();
        AddEvent(gameObject, EventTriggerType.PointerEnter, delegate { OnEnterInterface(gameObject); });
        AddEvent(gameObject, EventTriggerType.PointerExit, delegate { OnExitInterface(gameObject); });
        IsInitialised = true;
    }

    private void OnSlotUpdate(InventorySlot _slot)
    {
        if (_slot.GetItem_Data() != null)
        {
            _slot.slotDisplay.transform.GetChild(0).GetComponentInChildren<Image>().sprite = _slot.ItemObject.icon;
            _slot.slotDisplay.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);
            _slot.slotDisplay.GetComponentInChildren<TextMeshProUGUI>().text = _slot.amount == 1 ? "" : _slot.amount.ToString("n0");
            _slot.slotDisplay.GetComponent<UI_ToolTipInteract>().SetDisplayItem(_slot.ItemObject);
            _slot.slotDisplay.GetComponent<UI_ToolTipInteract>().tooltipPopup = tooltipPopup;
        }
        else
        {
            _slot.slotDisplay.transform.GetChild(0).GetComponentInChildren<Image>().sprite = null;
            _slot.slotDisplay.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0);
            _slot.slotDisplay.GetComponentInChildren<TextMeshProUGUI>().text = "";
            _slot.slotDisplay.GetComponent<UI_ToolTipInteract>().SetDisplayItem(null);
        }
    }

    //// Update is called once per frame
    //void Update()
    //{
    //    slotsOnInterface.UpdateSlotDisplay();
    //}
    public abstract void CreateSlots();

    protected void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        var eventTrigger = new EventTrigger.Entry
        {
            eventID = type
        };
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);
    }

    public void OnEnter(GameObject obj)
    {
        InventoryMouseData.slotHoveredOver = obj;

        //Debug.Log("OnEnter");

        //if (MouseData.slotHoveredOver != null)
        //{
        //    Debug.Log("MouseData.slotHoveredOver != null" + MouseData.slotHoveredOver != null);
        //}
        //if (MouseData.interfaceMouseIsOver != null)
        //{
        //    Debug.Log("MouseData.interfaceMouseIsOver != null" + MouseData.interfaceMouseIsOver != null);
        //}
        if (InventoryMouseData.interfaceMouseIsOver != null && InventoryMouseData.slotHoveredOver != null)
        {
            // gets the inventory slot from the Dictionary of the User Interface.
            InventorySlot mouseHoverSlotData = InventoryMouseData.interfaceMouseIsOver.slotsOnInterface[InventoryMouseData.slotHoveredOver];
            //Set Tooltip Canvas
            mouseHoverSlotData.SetToolTip(tooltipPopup);
            //display
            mouseHoverSlotData.DisplayToolTip();
        }
    }
    public void OnExit(GameObject obj)
    {
        InventoryMouseData.slotHoveredOver = null;
    }
    public void OnEnterInterface(GameObject obj)
    {
        //gets the current interface its over and sets that.
        InventoryMouseData.interfaceMouseIsOver = obj.GetComponent<UI_InventoryInterface>();

        //Debug.Log("OnEnterInterface");

        //Debug.Log("OnEnter");

        //if (MouseData.interfaceMouseIsOver != null && MouseData.slotHoveredOver != null)
        //{
        //    InventorySlot mouseHoverSlotData = MouseData.interfaceMouseIsOver.slotsOnInterface[MouseData.slotHoveredOver];
        //    mouseHoverSlotData.SetToolTip(tooltipPopup);
        //    mouseHoverSlotData.DisplayToolTip();
        //}
    }
    public void OnExitInterface(GameObject obj)
    {
        //Debug.Log("OnExit");

        //if (MouseData.interfaceMouseIsOver != null && MouseData.slotHoveredOver != null)
        //{
        //    InventorySlot mouseHoverSlotData = obj.GetComponent<UserInterface>().slotsOnInterface[MouseData.slotHoveredOver];
        //    mouseHoverSlotData.HideToolTip();
        //}

        //set interface exit to null
        InventoryMouseData.interfaceMouseIsOver = null;
    }
    public void OnDragStart(GameObject obj)
    {
        //saves the item being dragged info
        InventoryMouseData.tempItemBeingDragged = CreateTempItem(obj);
    }
    public GameObject CreateTempItem(GameObject obj)
    {
        GameObject tempItem = null;
        if (slotsOnInterface[obj].GetItem_Data() != null)
        {
            if (slotsOnInterface[obj].GetItem_Data().Id >= 0)
            {
            tempItem = new GameObject();
            var rt = tempItem.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(50, 50);
            tempItem.transform.SetParent(transform.parent);
            var img = tempItem.AddComponent<Image>();
            img.sprite = slotsOnInterface[obj].ItemObject.icon;
            img.raycastTarget = false;
            }
        }
        
        return tempItem;
    }
    public void OnDragEnd(GameObject obj)
    {
        Destroy(InventoryMouseData.tempItemBeingDragged);
        if (InventoryMouseData.interfaceMouseIsOver == null)
        {
            slotsOnInterface[obj].RemoveItem();
            return;
        }
        if (InventoryMouseData.slotHoveredOver)
        {
            InventorySlot mouseHoverSlotData = InventoryMouseData.interfaceMouseIsOver.slotsOnInterface[InventoryMouseData.slotHoveredOver];
            inventory.SwapItems(slotsOnInterface[obj], mouseHoverSlotData);
        }
    }
    public void OnDrag(GameObject obj)
    {
        if (InventoryMouseData.tempItemBeingDragged != null)
            InventoryMouseData.tempItemBeingDragged.GetComponent<RectTransform>().position = Mouse.current.position.ReadValue();
    }
}

public static class InventoryMouseData
{
    public static UI_InventoryInterface interfaceMouseIsOver;
    public static GameObject tempItemBeingDragged;
    public static GameObject slotHoveredOver;
}

public static class ExtensionMethods
{
    public static void UpdateSlotDisplay(this Dictionary<GameObject, InventorySlot> _slotsOnInterface)
    {
        foreach (KeyValuePair<GameObject, InventorySlot> _slot in _slotsOnInterface)
        {
            if (_slot.Value.GetItem_Data().Id >= 0)
            {
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = _slot.Value.ItemObject.icon;
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);
                _slot.Key.GetComponentInChildren<TextMeshProUGUI>().text = _slot.Value.amount == 1 ? "" : _slot.Value.amount.ToString("n0");
            }
            else
            {
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = null;
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0);
                _slot.Key.GetComponentInChildren<TextMeshProUGUI>().text = "";
            }
        }
    }
}