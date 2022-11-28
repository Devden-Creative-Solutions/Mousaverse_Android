using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubCategoryBtn : MonoBehaviour
{

    // Start is called before the first frame update

    private void Awake()
    {
        //OutfitController.OnEnableObject += EnableObject;
    }

    public void Init()
    { 
        OutfitController.OnEnableObject += EnableObject;
    }

    public void Destroy()
    {
        print("Game object destroyed Unsubscribing");
        OutfitController.OnEnableObject -= EnableObject;
    }

    void EnableObject(bool val)
    {

        if (gameObject)
            gameObject.SetActive(val);
    }
}
