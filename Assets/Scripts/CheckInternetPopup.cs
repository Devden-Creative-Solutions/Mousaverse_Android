using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Lean.Gui;

public class CheckInternetPopup : MonoBehaviour
{
    [SerializeField] LeanToggle _popupLean;
    float timer;
    float timeToReach=3;

    void Start()
    {
        
    }

    private void Update()
    {
        if(timer < timeToReach)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0;
            print("Timer reached its goal");
            StartCheckingForInternet();
        }
    }

    //void StartCheckingForInternet()
    //{
    //    StartCoroutine(CheckInternetConnection(isConnected =>
    //    {
    //        if (isConnected)
    //        {
    //            Debug.Log("Internet Available!");
    //            _popupLean.TurnOff();
    //        }
    //        else
    //        {
    //            Debug.Log("Internet Not Available");
    //            _popupLean.TurnOn();
    //        }
    //    }));
    //}

    //IEnumerator CheckInternetConnection(Action<bool> action)
    //{
    //    UnityWebRequest request = new UnityWebRequest("http://google.com");
    //    yield return request.SendWebRequest();
    //    if (request.error != null)
    //    {
    //        Debug.Log("Error");
    //        action(false);
    //    }
    //    else
    //    {
    //        Debug.Log("Success");
    //        action(true);
    //    }
    //}

    void StartCheckingForInternet()
    {
        CheckInternetConnection(isConnected =>
        {
            if (isConnected)
            {
                Debug.Log("Internet Available");
                _popupLean.TurnOff();
            }
            else
            {
                Debug.Log("Internet Not Available");
                _popupLean.TurnOn();
            }
        });
    }

    void CheckInternetConnection(Action<bool> action)
    {
        if(Application.internetReachability == NetworkReachability.NotReachable)
        {
            action(false);
        }
        else
        {
            action(true);
        }
    }


}
