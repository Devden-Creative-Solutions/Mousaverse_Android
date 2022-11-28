using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Gui;

public class UIElements : MonoBehaviour
{
    #region singleton


    private static UIElements _instance;

    public static UIElements Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    #endregion


    public LeanJoystick leanJoystick;
    public LeanJoystick leanJoystickCamera;
    public bool canUseJoystick = false;

    public delegate void EnableTheVirtualFPSCamera(bool canEnable);
    public EnableTheVirtualFPSCamera enableTheVirtualFPSCamera;

    public void UseJoystick()
    {
        canUseJoystick = true;
    }

    public void DontUseJoystick()
    {
        canUseJoystick = false;
    }

    public void UseFPSCamera()
    {
        enableTheVirtualFPSCamera?.Invoke(true);
    }

    public void DontUserFPSCamera()
    {
        enableTheVirtualFPSCamera?.Invoke(false);
    }
}
