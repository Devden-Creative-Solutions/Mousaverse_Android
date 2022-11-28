using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Gui;

public class WarningMessage : MonoBehaviour
{
    #region singleton


    private static WarningMessage _instance;

    public static WarningMessage Instance { get { return _instance; } }


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

        DontDestroyOnLoad(this.gameObject);
    }

    #endregion

    [SerializeField] LeanWindow warningWindow;
    private void Start()
    {
        warningWindow = GetComponent<LeanWindow>();    
    }

    public void ShowWarningMessage()
    {
        warningWindow.TurnOn();
    }

}
