using UnityEngine;
using UnityEngine.UI;

public class EntityHealthBar : MonoBehaviour
{
    public EntityController EntityTarget;
    public Slider Slider;

    void FixedUpdate()
    {
        Slider.value = (float)(EntityTarget.Health / EntityTarget.MaxHealth);

        if (EntityTarget.Health <= 0.0f)
        {
            Slider.gameObject.SetActive(false);
        }
    }
}