using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class HumanPlayer : Faction
{
    #region Variables, Start, Awake and Constructors

    [Header("Controllers")]
    [SerializeField] protected GeneralInputAction menuActions;

    //[Header("Party")]
    //[SerializeField] private List<PartyMemberWrapper> PartyMembers = new List<PartyMemberWrapper>();

    [Header("Inventory")]
    public SO_InventorySystemObject inventory;
    public SO_InventorySystemObject equipment;

    [Header("UI")]
    public UI_Manager UI;

    [Header("Attributes")]
    public Attribute[] attributes;

    [Header("Selection")]
    private RaycastHit hit;
    private RaycastHit LeftClickStarthit;
    [SerializeField] private List<Map_Object> selectedList = new();
    [SerializeField] private bool isDragging = false;
    private Vector3 mousePositon;

    [Header("Misc")]
    [SerializeField] private Camera cam;

    private void Awake()
    {
        menuActions = new GeneralInputAction();
        playerType = PlayerTypes.Human;
        cam = Camera.main;
    }

    private void Start()
    {
        for (int i = 0; i < attributes.Length; i++)
        {
            attributes[i].SetParent(this);
        }
        for (int i = 0; i < equipment.GetSlots.Length; i++)
        {
            equipment.GetSlots[i].OnBeforeUpdate += OnBeforeSlotUpdate;
            equipment.GetSlots[i].OnAfterUpdate += OnAfterSlotUpdate;
        }
        UI.setMenuAction(menuActions);
    }

    #endregion

    #region Inventory

    public void OnBeforeSlotUpdate(InventorySlot _slot)
    {
        if (_slot.ItemObject == null)
            return;
        switch (_slot.parent.inventory.type)
        {
            case InterfaceType.Inventory:
                break;
            case InterfaceType.Equipment:
                print(string.Concat("Removed ", _slot.ItemObject, " on ", _slot.parent.inventory.type, ", Allowed Items: ", string.Join(", ", _slot.AllowedItems)));

                for (int i = 0; i < _slot.GetItem_Data().buffs.Length; i++)
                {
                    for (int j = 0; j < attributes.Length; j++)
                    {
                        if (attributes[j].type == _slot.GetItem_Data().buffs[i].attribute)
                            attributes[j].value.RemoveModifier(_slot.GetItem_Data().buffs[i]);
                    }
                }

                break;
            case InterfaceType.Chest:
                break;
            default:
                break;
        }
    }

    public void OnAfterSlotUpdate(InventorySlot _slot)
    {
        if (_slot.ItemObject == null)
            return;
        switch (_slot.parent.inventory.type)
        {
            case InterfaceType.Inventory:
                break;
            case InterfaceType.Equipment:
                print(string.Concat("Placed ", _slot.ItemObject, " on ", _slot.parent.inventory.type, ", Allowed Items: ", string.Join(", ", _slot.AllowedItems)));

                for (int i = 0; i < _slot.GetItem_Data().buffs.Length; i++)
                {
                    for (int j = 0; j < attributes.Length; j++)
                    {
                        if (attributes[j].type == _slot.GetItem_Data().buffs[i].attribute)
                            attributes[j].value.AddModifier(_slot.GetItem_Data().buffs[i]);
                    }
                }

                break;
            case InterfaceType.Chest:
                break;
            default:
                break;
        }
    }

    #endregion

    #region General

    public void OnCollisionEnter(Collision other)
    {
         Collider otherCol = other.collider;
        CheckCollider(otherCol);
    }

    public void OnTriggerEnter(Collider other)
    {
        CheckCollider(other);
    }

    private void CheckCollider(Collider other)
    {
        var map_object = other.GetComponent<Map_Object>();
        if (map_object)
        {
            if (map_object.Map_type == Map_Type.Item)
            {
                var groundItem = other.GetComponent<Map_Item>();
                if (groundItem)
                {
                    if (groundItem.item_Map_Type == Item_Map_Type.Auto_Pickup)
                    {
                        SO_Item _item = groundItem.item;
                        if (inventory.AddItem(_item, groundItem.amount))
                        {
                            other.gameObject.SetActive(false);
                            Destroy(other.gameObject);
                        }
                    }
                }
            }
        }
    }

    private void Update()
    {
        //Detect if mouse is down
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            LeftMouseButtonDown();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            LeftMouseButtonUP(Mouse.current.position.ReadValue());
        }
        else if (Mouse.current.rightButton.wasReleasedThisFrame)//&& selectedUnits.Count > 0)
        {
            RightMouseButtonDown();
        }
    }

    private void LeftMouseButtonDown()
    {
        mousePositon = Mouse.current.position.ReadValue();
        //Create a ray from the camera to our space
        var camRay = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

        //Shoot that ray and get the hit data
        if (!Helpers.PointerIsOverUI() && Physics.Raycast(camRay, out hit))
        {
            if (hit.transform.CompareTag("UI"))
            {
                return;
            }
            else
            {
                LeftClickStarthit = hit;
                isDragging = true;
            }
        }
    }

    private void LeftMouseButtonUP(Vector2 LeftMouseStart)
    {
        if (Vector3.Distance(mousePositon, LeftMouseStart) == 0)
        {
            if (LeftClickStarthit.transform.CompareTag("Unit"))
            {
                Map_Unit unit = LeftClickStarthit.transform.GetComponent<Map_Unit>();
                SelectUnit(unit);
                UI.Add_Map_Unit(unit);
            }
            else if (LeftClickStarthit.transform.CompareTag("Item"))
            {
                DeselectAll();
                Map_Item item = LeftClickStarthit.transform.GetComponent<Map_Item>();
                SelectUnit(item);
                UI.Add_Map_Item(item);
            }
            else
            {
                DeselectAll();
                UI.ClearDisplay();
            }
        } 
        else if (isDragging)
        {
            foreach (var selectableObject in FindObjectsOfType<Map_Unit>())
            {
                if (IsWithinSelectionBounds(selectableObject.transform))
                {
                    if (selectableObject.transform.CompareTag("Unit"))
                    {
                        SelectUnit(selectableObject.gameObject.GetComponent<Map_Unit>(), true);
                    }
                }
            }
        }

        isDragging = false;
    }

    private void RightMouseButtonDown()
    {
        var camRay = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        //Shoot that ray and get the hit data
        if (Physics.Raycast(camRay, out hit))
        {
            if (hit.transform.CompareTag("Ground"))
            {
                foreach (var selectableObj in selectedList)
                {
                    if (selectableObj.Map_type == Map_Type.Unit)
                    {
                        var Map_unit = (Map_Unit)selectableObj;
                        if (Map_unit.faction.playerID == playerID)
                        {
                            Map_unit.MoveUnit(hit.point);
                        }
                    }
                }
            }
            else if (hit.transform.CompareTag("Unit"))
            {
                if (CanAttack(hit.transform))
                {
                    foreach (var selectableObj in selectedList)
                    {
                        var Map_unit = (Map_Unit)selectableObj;
                        Map_unit.SetNewTarget(hit.transform);
                    }
                }
            }
            else if (hit.transform.CompareTag("Item"))
            {
                Collider other = hit.collider;

                //foreach (var selectableObj in selectedList)
                //{
                    //selectableObj.SetNewTarget(hit.transform);
                    //check if item is in range,
                    //if in range, do add item, see OnTriggerEnter
                //}

                var map_object = other.GetComponent<Map_Object>();
                if (map_object)
                {
                    if (map_object.Map_type == Map_Type.Item)
                    {
                        var groundItem = other.GetComponent<Map_Item>();
                        if (groundItem)
                        {
                            SO_Item _item = groundItem.item;
                            if (groundItem.item_Map_Type == Item_Map_Type.Use_On_Pickup)
                            {
                                if (_item.Use())
                                {
                                    other.gameObject.SetActive(false);
                                    Destroy(other.gameObject);
                                }
                            } else
                            {
                                if (inventory.AddItem(_item, groundItem.amount))
                                {
                                    other.gameObject.SetActive(false);
                                    Destroy(other.gameObject);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private bool CanAttack(Transform transform)
    {
        var unit = hit.transform.GetComponent<Map_Unit>();

        if (unit.playerID == playerID)
        {
            return false;
        }
        else
        {
            return true;
        }

    }

    private void LateUpdate()
    {
        if (menuActions.QuickSaveandLoad.Save.WasPerformedThisFrame())
        {
            //inventory.Save();
            //equipment.Save();
        }
        else if (menuActions.QuickSaveandLoad.Load.WasPerformedThisFrame())
        {
            inventory.Load();
            equipment.Load();
        }
        else if (menuActions.Menu.OpenMenu.WasPerformedThisFrame())
        {
            UI.DisplayHideMenuWindow();
        }
        else if (menuActions.Menu.Encyclopedia.WasPerformedThisFrame())
        {
            UI.DisplayHideEncyclopediaWindow();
        }
        else if (menuActions.Menu.Options.WasPerformedThisFrame())
        {
            UI.DisplayHideOptionsWindow();
        }
        else if (menuActions.Menu.Save.WasPerformedThisFrame())
        {
            //inventory.Save();
            //equipment.Save();
        }
        else if (menuActions.Menu.Map.WasPerformedThisFrame())
        {
            UI.DisplayHideMapWindow();
        }
        else if (menuActions.Menu.Party.WasPerformedThisFrame())
        {
            UI.DisplayHidePartyWindow();
        }
        else if (menuActions.Menu.Inventory.WasPerformedThisFrame())// && t >= 0)
        {
            UI.DisplayHideInventoryWindow();
        }
    }

    private void OnApplicationQuit()
    {
        inventory.Clear();
        equipment.Clear();
    }

    private void OnEnable()
    {
        menuActions.Menu.Enable();
        menuActions.Menu.Inventory.Enable();
    }

    private void OnDisable()
    {
        menuActions.Disable();
        menuActions.Menu.Inventory.Disable();
    }

    #endregion

    #region Attributes

    public void AttributeModified(Attribute attribute)
    {
        Debug.Log(string.Concat(attribute.type, " was updated! Value is now ", attribute.value.ModifiedValue));
    }

    #endregion

    #region Selection

    private void OnGUI()
    {
        if (isDragging)
        {
            var rect = ScreenHelper.GetScreenRect(mousePositon, Mouse.current.position.ReadValue());
            ScreenHelper.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.1f));
            ScreenHelper.DrawScreenRectBorder(rect, 1, Color.blue);
        }

    }

    private void AddUnitToSelectionList(Map_Object unit)
    {
        selectedList.Add(unit);
        unit.SetSelected(true);
        UI.AddSelection((Map_Unit)unit);
    }

    [System.Obsolete("Possible Removal")]
    private void RemoveUnitFromSelectionList(Map_Object unit)
    {
        selectedList = selectedList.Where(UnitController => UnitController.getSelectionID() != unit.getSelectionID()).ToList();
        unit.SetSelected(false);
    }

    private void DeselectAll()
    {
        if (selectedList.Count > 0)
        {
            UI.ClearSelection();
            foreach (Map_Object mapobject in selectedList)
            {
                if (mapobject != null)
                {
                    mapobject.SetSelected(false);
                }
            }
            selectedList = new List<Map_Object>();
        }
    }

    private bool IsPlayerUnit(Map_Object obj)
    {
        if (obj.GetMap_Type() == Map_Type.Unit)
        {
            var unit = (Map_Unit)obj;
            if (unit.playerID == playerID)
            {
                return true;
            }
            return false;
        }
        else
        {
            return false;
        }
    }

    public void SelectUnit(Map_Object obj, bool isMultiSelect = false, bool updateUiOnly = false)
    {
        if (obj.GetIsSelectable())
        {
            bool isShiftclick = menuActions.Selection.MultiSelect.WasPerformedThisFrame();
            if ((isMultiSelect | isShiftclick) & obj.Map_type == Map_Type.Unit)
            {
                if (IsPlayerUnit(obj))
                {
                    //RemoveUnitFromSelectionList(obj); // need to check if already selected here somehow
                    AddUnitToSelectionList(obj);
                }
            }
            else
            {
                DeselectAll();
                AddUnitToSelectionList(obj);
            }
        }
    }

    public List<Map_Object> GetSelectionList()
    {
        return selectedList;
    }

    private bool IsWithinSelectionBounds(Transform transform)
    {
        if (!isDragging)
        {
            return false;
        }
        var viewportBounds = ScreenHelper.GetViewportBounds(cam, mousePositon, Mouse.current.position.ReadValue());
        return viewportBounds.Contains(cam.WorldToViewportPoint(transform.position));
    }

    public void OnBeforeUpdate_UI_Slot(UI_Slot _slot)
    {
        if (_slot.GetMap_Unit() == null)
            return;
    }

    public void OnAfterUpdate_UI_Slot(UI_Slot _slot)
    {
        if (_slot.GetMap_Unit() == null)
            return;
    }

    #endregion

}

[System.Serializable]
public class Attribute
{
    [System.NonSerialized]
    public HumanPlayer parent;
    public Attributes type;
    public ModifiableInt value;

    public void SetParent(HumanPlayer _parent)
    {
        parent = _parent;
        value = new ModifiableInt(AttributeModified);
    }
    public void AttributeModified()
    {
        parent.AttributeModified(this);
    }
}