using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UI_TabButtonScript : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public UI_TabGroup tabgroup;

    public Image background;

    public UnityEvent onTabSelected;
    public UnityEvent onTabDeSelected;

    // Start is called before the first frame update
    void Start()
    {
        background = GetComponent<Image>();
        tabgroup.Subscribe(this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        tabgroup.OnTabSelected(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tabgroup.OnTabEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tabgroup.OnTabExit(this);
    }

    public void Select()
    {
        if (onTabSelected != null)
        {
            onTabSelected.Invoke();
        }
    }

    public void DeSelect()
    {
        if (onTabSelected != null)
        {
            onTabDeSelected.Invoke();
        }
    }
}
