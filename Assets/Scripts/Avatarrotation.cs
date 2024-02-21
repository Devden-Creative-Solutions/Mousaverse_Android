using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Avatarrotation : MonoBehaviour
{
    [SerializeField] float speed;
    float starttouchposition;
    bool isRotating;
    [SerializeField] Transform Target;
    private void OnMouseDown()
    {
        isRotating = true;
        starttouchposition = Input.mousePosition.x;
        Debug.Log("onmousedown");
    }
    private void OnMouseUp()
    {
        isRotating = false;
        Debug.Log("Onmouseup");
    }
    private void Update()
    {
       //if(Input.GetMouseButtonDown(0))
       // {
           
       // }
       //else if(Input.GetMouseButtonUp(0))
       // {
       //     isRotating = false;
       // }

       if(isRotating)
        {
            float CurrentPosition = Input.mousePosition.x;
            float Mousemovement =CurrentPosition - starttouchposition;


            Target.transform.Rotate(Vector3.up,-Mousemovement*speed*Time.deltaTime);
            starttouchposition = CurrentPosition;
        }

    }
}
