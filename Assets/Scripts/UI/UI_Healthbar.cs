using UnityEngine;
using UnityEngine.UI;

public class UI_Healthbar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;

    public void HideBar()
    {
        this.gameObject.SetActive(false); //set the object to active
    }

    public void ShowBar()
    {
        this.gameObject.SetActive(true); //set the object to active
    }

    public void SetMaxHealthSlider(float health)
    {
        slider.maxValue = health;
        slider.value = health;

        fill.color = gradient.Evaluate(1f);
    }

    public void SetMaxHealth()
    {

        fill.fillAmount = 1;
        fill.color = gradient.Evaluate(1f);
    }

    public void SetHealthSlider(float health)
    {
        slider.value = health;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    public void SetHealth(float Maxhealth, float currentHealth)
    {
        var value = currentHealth / Maxhealth;
        //Debug.Log("UI HEALTHBAR FILL: " + value);
        fill.fillAmount = value;
        fill.color = gradient.Evaluate(value);
    }
}
