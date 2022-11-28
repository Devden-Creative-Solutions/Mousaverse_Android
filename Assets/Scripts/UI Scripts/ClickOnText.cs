using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ClickOnText : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] TMP_Text textToUnderline;
     public UnityEvent uEvent;

    public void OnPointerClick(PointerEventData eventData)
    {
        uEvent?.Invoke();
    }

}
