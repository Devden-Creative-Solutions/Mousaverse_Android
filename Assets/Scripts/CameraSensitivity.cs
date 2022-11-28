using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CameraSensitivity : MonoBehaviour
{
    public delegate void ChangingVerticalSensitivity(float val);
    public static ChangingVerticalSensitivity changingVerticalSensitivity;

    public delegate void ChangingHorizontalSensitivity(float val);
    public static ChangingHorizontalSensitivity changingHorizontalSensitivity;

    [SerializeField] Slider verticalSlider;
    [SerializeField] Slider horizontalSlider;

    public void ChangeVerticalCamSensitivity()
    {
        changingVerticalSensitivity?.Invoke(verticalSlider.value);
    }

    public void ChangeHorizontalCamSensitivity()
    {
        changingHorizontalSensitivity?.Invoke(horizontalSlider.value);
    }
}
