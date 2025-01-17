using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_Selection_List_Generated : UI_Selection_List
{
    [Header("Slot Creation")]
    public GameObject SlotPrefab;
    public int X_START;
    public int Y_START;
    public int X_SPACE_BETWEEN_ITEM;
    public int NUMBER_OF_COLUMN;
    public int Y_SPACE_BETWEEN_ITEMS;
    public Transform compTransform;

    public GameObject[] Fixedslots;

    public override void CreateSlots()
    {
        //compTransform = transform;
        // base.CreateSlots();
        slotsOnInterface = new Dictionary<GameObject, UI_Slot>();
        for (int i = 0; i < Slots.Length; i++)
        {
            //var obj = Instantiate(SlotPrefab, Vector3.zero, Quaternion.identity, compTransform);
            //obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
            var obj = Fixedslots[i];

            AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj); });
            AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExit(obj); });
            AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDragStart(obj); });
            AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragEnd(obj); });
            AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });
            Slots[i].slotDisplay = obj;
            slotsOnInterface.Add(obj, Slots[i]);
        }
    }

    public Vector3 GetPosition(int i)
    {
        return new Vector3(X_START + (X_SPACE_BETWEEN_ITEM * (i % NUMBER_OF_COLUMN)), Y_START + (-Y_SPACE_BETWEEN_ITEMS * (i / NUMBER_OF_COLUMN)), 0f);
    }
}

public abstract class UI_Selection_List : MonoBehaviour
    {
    [Header("Lists")]
    [SerializeField] public Dictionary<GameObject, UI_Slot> slotsOnInterface = new();
    [SerializeField] public UI_Slot[] Slots = new UI_Slot[28];
    [Header("Misc")]
    [SerializeField] public HumanPlayer player;
    [SerializeField] private UI_TooltipPopup tooltipPopup;

    public void Awake()
    {
        Setup();
    }

    public void Setup()
    {
        Slots = new UI_Slot[28];

        for (int i = 0; i < Slots.Length; i++)
        {
            Slots[i] = new UI_Slot();
            Slots[i].OnAfterUpdate += OnSlotUpdate;

        }

        for (int i = 0; i < player.GetSelectionList().Count; i++)
        {
            Slots[i] = new UI_Slot();
            Slots[i].OnAfterUpdate += OnSlotUpdate;

        }
        CreateSlots();
        AddEvent(gameObject, EventTriggerType.PointerEnter, delegate { OnEnterInterface(gameObject); });
        AddEvent(gameObject, EventTriggerType.PointerExit, delegate { OnExitInterface(gameObject); });
    }

    private void OnSlotUpdate(UI_Slot _slot)
    {
        if (_slot.GetMap_Unit() != null)
        {
            if (_slot.GetMap_Unit().unit.Id >= 0)
            {
                _slot.slotDisplay.transform.GetChild(0).GetComponentInChildren<Image>().sprite = _slot.GetMap_Unit().unit.Icon;
                _slot.slotDisplay.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);
                _slot.slotDisplay.GetComponent<UI_ToolTipInteract>().SetDisplayItem(_slot.GetMap_Unit().unit);
                _slot.slotDisplay.GetComponent<UI_ToolTipInteract>().tooltipPopup = tooltipPopup;
            }
        }
        else
        {
            _slot.slotDisplay.transform.GetChild(0).GetComponentInChildren<Image>().sprite = null;
            _slot.slotDisplay.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0);
            _slot.slotDisplay.GetComponentInChildren<TextMeshProUGUI>().text = "";
            _slot.slotDisplay.GetComponent<UI_ToolTipInteract>().SetDisplayItem(null);
        }
    }

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

    #region Mouse Handling

    public void OnEnter(GameObject obj)
    {
        SelectionMouseData.slotHoveredOver = obj;

        if (SelectionMouseData.interfaceMouseIsOver != null && SelectionMouseData.slotHoveredOver != null)
        {
            // gets the inventory slot from the Dictionary of the User Interface.
            UI_Slot mouseHoverSlotData = SelectionMouseData.interfaceMouseIsOver.slotsOnInterface[SelectionMouseData.slotHoveredOver];
            //Set Tooltip Canvas
            mouseHoverSlotData.SetToolTip(tooltipPopup);
            //display
            mouseHoverSlotData.DisplayToolTip();
        }
    }
    public void OnExit(GameObject obj)
    {
        SelectionMouseData.slotHoveredOver = null;
    }
    public void OnEnterInterface(GameObject obj)
    {
        //gets the current interface its over and sets that.
        SelectionMouseData.interfaceMouseIsOver = this;
    }
    public void OnExitInterface(GameObject obj)
    {
        //set interface exit to null
        SelectionMouseData.interfaceMouseIsOver = null;
    }

    public void OnDragStart(GameObject obj)
    {
        //saves the item being dragged info
        SelectionMouseData.tempItemBeingDragged = CreateTempItem(obj);
    }

    public GameObject CreateTempItem(GameObject obj)
    {
        GameObject tempItem = null;
        if (slotsOnInterface[obj].GetMap_Unit() != null)
        {
            if (slotsOnInterface[obj].GetMap_Unit().unit.Id >= 0)
            {
                tempItem = new GameObject();
                var rt = tempItem.AddComponent<RectTransform>();
                rt.sizeDelta = new Vector2(50, 50);
                tempItem.transform.SetParent(transform.parent);
                var img = tempItem.AddComponent<Image>();
                img.sprite = slotsOnInterface[obj].GetMap_Unit().unit.Icon;
                img.raycastTarget = false;
            }
        }

        return tempItem;
    }

    public void OnDragEnd(GameObject obj)
    {
        Destroy(SelectionMouseData.tempItemBeingDragged);
        if (SelectionMouseData.interfaceMouseIsOver == null)
        {
            slotsOnInterface[obj].RemoveUnit();
            return;
        }
        if (SelectionMouseData.slotHoveredOver)
        {
            UI_Slot mouseHoverSlotData = SelectionMouseData.interfaceMouseIsOver.slotsOnInterface[SelectionMouseData.slotHoveredOver];
            SwapItems(slotsOnInterface[obj], mouseHoverSlotData);
        }
    }
    
    public void OnDrag(GameObject obj)
    {
        if (SelectionMouseData.tempItemBeingDragged != null)
            SelectionMouseData.tempItemBeingDragged.GetComponent<RectTransform>().position = Mouse.current.position.ReadValue();
    }

    #endregion

    public void SwapItems(UI_Slot item1, UI_Slot item2)
    {
        UI_Slot temp = new(item2.GetMap_Unit());
        item2.UpdateSlot(item1.GetMap_Unit());
        item1.UpdateSlot(temp.GetMap_Unit());
    }

    public bool AddUnit(Map_Unit _item)
    {
        if (EmptySlotCount <= 0)
            return false;
        SetEmptySlot(_item);
        return true;
    }
    public int EmptySlotCount
    {
        get
        {
            int counter = 0;
            for (int i = 0; i < Slots.Length; i++)
            {
                UI_Slot slot = Slots[i];
                Map_Unit unit = slot.GetMap_Unit();
                if (unit == null)
                {
                    counter++;
                }
                else
                {
                    SO_Unit tempItem = unit.unit;
                    if (tempItem == null)
                    {
                        counter++;
                    }
                }
                
            }
            return counter;
        }
    }

    public UI_Slot SetEmptySlot(Map_Unit _item)
    {
        for (int i = 0; i < Slots.Length; i++)
        {
            Map_Unit slot_unit = Slots[i].GetMap_Unit();
            //SO_Unit tempItem = Slots[i].GetMap_Unit().unit;
            if (slot_unit == null)
            {
                Slots[i].UpdateSlot(_item);
                return Slots[i];
            }
        }
        //set up functionality for full inventory
        return null;
    }

    public void ClearSelectionList()
    {
        for (int i = 0; i < player.GetSelectionList().Count; i++)
        {
            Slots[i].RemoveUnit();

        }
    }

}

public static class SelectionMouseData
{
    public static UI_Selection_List interfaceMouseIsOver;
    public static GameObject tempItemBeingDragged;
    public static GameObject slotHoveredOver;
}

public delegate void UI_SlotUpdated(UI_Slot _slot);

public class UI_Slot
{

    [System.NonSerialized]
    public GameObject slotDisplay;
    [System.NonSerialized]
    public UI_SlotUpdated OnAfterUpdate;
    [System.NonSerialized]
    public UI_SlotUpdated OnBeforeUpdate;
    [System.NonSerialized]
    private Map_Unit unit;

    [SerializeField] private UI_TooltipPopup tooltipPopup;

    public UI_Slot()
    {
        Map_Unit _unit = null;
        UpdateSlot(_unit);
    }
    public UI_Slot(Map_Unit _unit)
    {
        UpdateSlot(_unit);
    }
    
    public void UpdateSlot(Map_Unit _unit)
    {
        if (OnBeforeUpdate != null)
            OnBeforeUpdate.Invoke(this);
        unit = _unit;
        if (OnAfterUpdate != null)
            OnAfterUpdate.Invoke(this);
    }
    
    public void RemoveUnit()
    {
        Map_Unit _unit = null;
        UpdateSlot(_unit);
    }

    public void SetMap_Unit(Map_Unit newData)
    {
        unit = newData;
    }

    public Map_Unit GetMap_Unit()
    {
        return unit;
    }

    public void SetToolTip(UI_TooltipPopup tooltipPopup)
    {
        this.tooltipPopup = tooltipPopup;
    }
    
    public void DisplayToolTip()
    {
        if (unit != null)
        {
            if (tooltipPopup != null)
            {
                tooltipPopup.DisplayInfo(unit.unit);
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