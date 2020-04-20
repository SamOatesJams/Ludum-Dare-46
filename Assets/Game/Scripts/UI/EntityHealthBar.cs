using UnityEngine;
using UnityEngine.UI;

public class EntityHealthBar : MonoBehaviour
{
    public EntityController EntityTarget;
    public Slider Slider;

    void FixedUpdate()
    {
        Slider.value = (float)(EntityTarget.Health / EntityTarget.MaxHealth);
        Debug.Log($"{EntityTarget.Health} {EntityTarget.MaxHealth} {Slider.value}");
    }
}