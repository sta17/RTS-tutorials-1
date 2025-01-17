using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ToolTipInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] public UI_TooltipPopup tooltipPopup;
    [SerializeField] private UI_Info item;

    public void Awake()
    {
        item = null;
    }

    public void SetDisplayItem(UI_Info newItem)
    {
        item = newItem;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null && tooltipPopup != null)
        {
            tooltipPopup.DisplayInfo(item);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipPopup != null)
        {
            tooltipPopup.HideInfo();
        }
    }
}