using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class TargetHealthBarScript : MonoBehaviour
{
    private Slider slider;
    public TMP_Text text;

    public CanvasGroup UI;

    internal void SetName(string name)
    {
        text.text = name;
    }
    internal void SetHealth(int newValue)
    {
        slider.value = Math.Clamp(newValue, slider.minValue, slider.maxValue);
    }

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    internal void Hide()
    {
        UI.alpha = 0;
    }

    internal void Show()
    {
        UI.alpha = 1;
    }
}
