using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using Lean.Gui;

public class ForgotPasswordManager : MonoBehaviour
{
    private readonly string createUserLink = "http://54.200.254.206/mousaverse/create_user";
    private readonly string verifyOTPLink = "http://54.200.254.206/mousaverse/verify_otp";
    private readonly string resendOTPLink = "http://54.200.254.206/mousaverse/resend_otp";

    [SerializeField] TMP_InputField _emailInput;
    [SerializeField] TMP_InputField _OTPInput;
    [SerializeField] TMP_InputField _passInput;
    [SerializeField] TMP_InputField _cnfPassInput;

    [SerializeField] string _secretCode;
    [SerializeField] RequestData requestData;
    [SerializeField] RequestData requestDataActualResend;

    [SerializeField] TMP_Text _warningText;

    [SerializeField] LeanToggle _otpScreenToggle;
    [SerializeField] LeanToggle _pswChangeScreenToggle;
 

    public void SendOTP()
    {
        if(_emailInput.text == "")
        {
            print("Nothing is there in the input thing!!");
        }
        else
        {
            print("Sending OTP...");
            FirebaseManager.Instance.CheckIfEmailExist(_emailInput.text, StartingCoroutineForSendingOTP);
            
        }
    }

    public void VerifyTheOTP()
    {
        if(_OTPInput.text == "")
        {
            print("Nothing is there in the OTP input thing!!");
        }
        else
        {
            if(_OTPInput.text != "" && _secretCode != "")
            {
                StartCoroutine(VerifyOTP());
            }
            else
            {
                print("Secret Code is not there!!");
            }
        }
    }

    void AfterVerifyingOTP()
    {
        _pswChangeScreenToggle.TurnOn();
    }

    void StartingCoroutineForSendingOTP(bool isEmailExists)
    {
        if (isEmailExists)
        {
            print("Yes!! The Email Exists!!");
            MainThreadDispatcher.Instance().Enqueue(SendingTheOTP);
           
        }
        else
        {
            print("No!! The Email Doesnt Exists!!");
        }
     
    }

    void SendingTheOTP()
    {
        StartCoroutine(SendingOTP(_emailInput.text));
    }

    IEnumerator SendingOTP(string email)
    {
        print("Starting Sending OTP Coroutine!!");
        WWWForm form = new WWWForm();
        form.AddField("email", email);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(createUserLink,form))
        {

            yield return webRequest.SendWebRequest();

            if(webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Error: " + webRequest.error);
                StartCoroutine(ResendingOTP(_emailInput.text));
            }
            else
            {
                print("Succesfully send OTP");
                string jsonText = webRequest.downloadHandler.text;
                requestData = JsonUtility.FromJson<RequestData>(jsonText);
                _secretCode = requestData.reset_key;
            }
        }
    }

    IEnumerator ResendingOTP(string email)
    {
        print("Resending OTP.....");
        WWWForm form = new WWWForm();
        form.AddField("email", email);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(resendOTPLink, form))
        {

            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Error: " + webRequest.error);
            }
            else
            {
                print("Succesfully Resent OTP");
                string jsonText =  webRequest.downloadHandler.text;
                requestData = JsonUtility.FromJson<RequestData>(jsonText);
                _secretCode = requestData.reset_key;
            }
        }
    }

    IEnumerator VerifyOTP()
    {
        print("Verifying OTP!!");

        WWWForm form = new WWWForm();
        form.AddField("otp", _OTPInput.text);
        form.AddField("reset_key", _secretCode);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(verifyOTPLink, form))
        {
            yield return webRequest.SendWebRequest();

            if(webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Error: " + webRequest.error);
                Debug.Log("The OTP is not correct!!");
            }

            else
            {
                print("Successfully Verified OTP!");
                print(webRequest.downloadHandler.text);
                AfterVerifyingOTP();
            }
        }
    }

    

}

[System.Serializable]
public class RequestData
{
    public string reset_key;
    public string msg;
}
