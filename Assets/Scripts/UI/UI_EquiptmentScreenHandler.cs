using UnityEngine;

public class UI_EquiptmentScreenHandler : MonoBehaviour
{
    public GameObject Inventory;
    public GameObject Equiptment;

    public void TurnOn()
    {
        Equiptment.SetActive(true);
        Inventory.SetActive(true);
    }

    public void TurnOff()
    {
        Equiptment.SetActive(false);
        Inventory.SetActive(false);
    }
}
