using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using Lean.Gui;

public class SprintControlWrapper : MonoBehaviour
{
    public SprintingButton sprintingButton { get { return FindObjectOfType<SprintingButton>(); } }
}

public class SprintingButton : MonoBehaviour, IPointerDownHandler
{
    public LeanToggle sprintButtonLean;

    public Action<bool> pointerDown;

    bool invokeBool = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        invokeBool = !invokeBool;
        pointerDown?.Invoke(invokeBool);
        if (invokeBool)
        {
            sprintButtonLean.TurnOn();
        }
        else
        {
            sprintButtonLean.TurnOff();
        }
        
    }
}
