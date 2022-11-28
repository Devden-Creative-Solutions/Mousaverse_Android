using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DevLogin : MonoBehaviour
{
    [SerializeField] TMP_InputField EmailInput;
    [SerializeField] TMP_InputField PasswordInput;
    [SerializeField] Button loginButton;

    /// <summary>
    /// Only for Dev Testing Delete this is production
    /// </summary>
    public void DoTheDevLogin()
    {
        EmailInput.text = "gfx460@gmail.com";
        PasswordInput.text = "123456";

        loginButton.onClick.Invoke();
    }

}
