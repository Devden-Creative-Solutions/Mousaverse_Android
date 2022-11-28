using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using Firebase.Database;
using Firebase;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class FirebaseManager : MonoBehaviour
{
    #region singleton


    private static FirebaseManager _instance;

    public static FirebaseManager Instance { get { return _instance; } }


    private void Awake()
    {
        Debug.unityLogger.logEnabled = true;

        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could Not resolve all Firebase Dependencies" + dependencyStatus);
            }
        });

        DontDestroyOnLoad(this.gameObject);
    }

    #endregion


    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;

    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text warningLoginText;
    public TMP_Text confirmLoginText;

    [Header("Register")]
    public TMP_InputField userNameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;
    public TMP_Text warningRegisterText;
    public TMP_Dropdown _regPlayerTypeDropDown;
    DatabaseReference DBreference;

    [Header("PlayerType")]
    public TMP_Dropdown playerTypeDropDown;

    [Header("Script References")]
    public RegionSelection regionSelection;

    [Header("Outfit Controllers")]
    [SerializeField] OutfitController _maleOutfitController;
    [SerializeField] OutfitController _femaleOutfitController;
    [SerializeField] ChangeGender changeGender;

    [Header("Forgot Password")]
    [SerializeField] TMP_InputField _forgotPswEmailInput;
    [SerializeField] TMP_Text _forgotPswWarningText;


    [SerializeField] TheBodyParts dataBaseBodyParts = new TheBodyParts();

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");

        auth = FirebaseAuth.DefaultInstance;
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
        auth.StateChanged += AuthStateChanged;
    }

    private void Start()
    {
        CheckNetworkAndEnableLoginScreen();
    }

    #region PublicMethods

    public void LoginButton()
    {
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }

    public void RegisterButton()
    {
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, userNameRegisterField.text));
    }

    public void JoinRandomRoom()
    {
        StartCoroutine(LoadYourAsyncScene());
    }

    public void SaveAvatarID(int avatarID)
    {
        StartCoroutine(UpdateAvatar(avatarID));
    }

    public void AvatarIDGet()
    {
        StartCoroutine(GetAvatarID());
    }

    public void SetPlayerType()
    {
        PlayerType playerType = (PlayerType)playerTypeDropDown.value;
        StartCoroutine(UpdatePlayerType(playerType));
    }

    public void GetThePlayerType()
    {
        StartCoroutine(GetPlayerType());
    }

    public void UpdateDisplayName(string displayName)
    {
        StartCoroutine(UpdateUserNameAuth(displayName));
    }

    public void UpdateThePlayerRegion(string _region)
    {
        StartCoroutine(UpdatePlayerRegion(_region));
    }

    public void GetThePlayerRegion()
    {
        StartCoroutine(GetPlayerRegion());
    }
    public void SignOut()
    {
        LogginOut();
        confirmLoginText.text = "Logged Out!!";
    }

    public void UpdatePlayerBodyParts(string head, string top, string footwear, string bottom, Gender gender)
    {
        StartCoroutine(SetPlayerBodyParts(head, top, footwear, bottom, gender));
    }

    public void GetThePlayerBodyParts()
    {
        StartCoroutine(GetPlayerBodyParts());
    }

    public void UpdateTheGender(Gender gender)
    {
        StartCoroutine(UpdateGender(gender));
    }

    public void ResettingPassword()
    {
        if (_forgotPswEmailInput.text != "")
        {
            ResetPassword(_forgotPswEmailInput.text);
        }
        else
        {
            print("There is nothing in the input field!!");
        }

    }

    #endregion

    #region CallBacks

    public void AfterLogin()
    {
        print("This is from the callback!!");


        AvatarIDGet();
        GetThePlayerType();
        //print("Below getplayer type");
        Invoke(nameof(GetUserName), .1f);
        GetThePlayerRegion();
        GetThePlayerBodyParts();

        PlayerInfoDontDestroy.Instance.LogInTheUser();
        PlayerInfoDontDestroy.Instance.SetFirebaseUser(User);
    }

    #endregion

    #region AuthorizationSystem
    IEnumerator Register(string emailReg, string passReg, string userName)
    {
        if (userName == "")
        {
            warningRegisterText.text = "Please enter Username";
        }
        else if (passwordRegisterField.text != passwordRegisterVerifyField.text)
        {
            warningRegisterText.text = "Password does not match!!";
        }
        else
        {
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(emailReg, passReg);
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                Debug.LogError(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Registration Failed";

                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Please enter Email";
                        break;

                    case AuthError.MissingPassword:
                        message = "Please enter password";
                        break;

                    case AuthError.WeakPassword:
                        message = "Weak Password";
                        break;

                    case AuthError.EmailAlreadyInUse:
                        message = "Email Already Exist!!";
                        break;
                }

                warningRegisterText.text = message;
            }

            else
            {
                User = RegisterTask.Result;

                if (User != null)
                {
                    UserProfile profile = new UserProfile { DisplayName = userName };
                    var DBTask = DBreference.Child("users").Child(User.UserId).Child("PlayerType").SetValueAsync(((PlayerType)_regPlayerTypeDropDown.value).ToString());
                    var ProfileTask = User.UpdateUserProfileAsync(profile);

                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted && DBTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        Debug.LogError(message: $"Failed to register Task with {ProfileTask.Exception}");
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                        warningRegisterText.text = "UserName Set Failed!";
                    }

                    else
                    {
                        UIManager.Instance.ShowUI("login");
                        warningRegisterText.text = "";
                        ClearRegisterFields();
                    }
                }
            }
        }
    }

    IEnumerator Login(string email, string password)
    {
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);


        if (LoginTask.Exception != null)
        {
            Debug.LogError(message: $"Failed to register task with {LoginTask.Exception}");

            FirebaseException firebaseExcep = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseExcep.ErrorCode;

            string message = "Login Failed!";

            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;

                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;

                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;

                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;

                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }
            warningLoginText.text = message;
        }
        else
        {
            User = LoginTask.Result;
            Debug.LogFormat("User signed in succesfully: {0}({1})", User.DisplayName, User.Email);
            warningLoginText.text = "";
            confirmLoginText.text = "Logged In";
            ClearLoginFields();

            //Callback
            AfterLogin();
        }


    }

    public void CheckIfEmailExist(string email, Action<bool> action)
    {
        auth.FetchProvidersForEmailAsync(email).ContinueWith((authTask) =>
        {
            if (authTask.IsCanceled)
            {
                Debug.Log("Provider fetch canceled.");
            }
            else if (authTask.IsFaulted)
            {
                Debug.Log("Provider fetch encountered an error.");
                Debug.Log(authTask.Exception.ToString());
            }
            else if (authTask.IsCompleted)
            {
                Debug.Log("Email Providers:");
                print("Result: " + authTask.Result);

                int providerCount = 0;
                foreach (string provider in authTask.Result)
                {
                    Debug.Log(provider);
                    providerCount++;
                }

                if (providerCount > 0)
                {
                    action?.Invoke(true);
                }
                else
                {
                    action?.Invoke(false);
                }
            }
        });
    }


    void LogginOut()
    {
        auth.SignOut();
        ClearLoginFields();
        UIManager.Instance.LoginScreen();
    }

    void ClearLoginFields()
    {
        emailLoginField.text = "";
        passwordLoginField.text = "";
    }

    void ClearRegisterFields()
    {
        emailRegisterField.text = "";
        passwordRegisterField.text = "";
        userNameRegisterField.text = "";
        passwordRegisterVerifyField.text = "";
    }

    IEnumerator LoadYourAsyncScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        yield break;
    }
    #endregion

    #region StoreData
    private IEnumerator UpdateUserNameAuth(string _userName)
    {
        UserProfile profile = new UserProfile { DisplayName = _userName };

        var ProfileTask = User.UpdateUserProfileAsync(profile);

        yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

        if (ProfileTask.Exception != null)
        {
            Debug.LogError(message: $"Failed to register task with {ProfileTask.Exception}");
        }
        else
        {

        }
    }


    private IEnumerator UpdateUserNameDatabase(string _userName)
    {
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("username").SetValueAsync(_userName);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogError(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            print("database username is now updated");
        }
    }

    public string GetUserName()
    {
        Firebase.Auth.FirebaseUser user = auth.CurrentUser;
        string name = "Anon";
        if (user != null)
        {
            name = user.DisplayName;
            print("Name == " + name);
        }

        return name;
    }

    private IEnumerator UpdateAvatar(int _avatarID)
    {
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("avatarID").SetValueAsync(_avatarID);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogError(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            print("Avatar has been set");
        }
    }

    private IEnumerator UpdatePlayerType(PlayerType playerType)
    {
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("PlayerType").SetValueAsync(playerType.ToString());

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogError(message: $"Failed to register task with {DBTask.Exception}");
        }

        else
        {
            print("Player Type has been set");
            PlayerInfoDontDestroy.Instance.currentPlayerType = playerType;
        }
    }

    private IEnumerator UpdatePlayerRegion(string region)
    {
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("Region").SetValueAsync(region);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogError(message: $"Failed to register task with {DBTask.Exception}");
        }

        else
        {
            print("The region has been set to: " + region);

        }
    }

    private IEnumerator GetAvatarID()
    {
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("avatarID").GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogError(message: $"Failed to register Task with {DBTask.Exception}");
        }

        else
        {
            DataSnapshot snapshot = DBTask.Result;
            int avatarID;
            if (snapshot.Value != null)
            {
                int.TryParse(snapshot.Value.ToString(), out avatarID);
                print("Avatar ID : " + avatarID);
                AvatarGroupManager.Instance.SetCurrentAvatar(avatarID);
                UIManager.Instance.AvatarCustomizationScreen();
            }
            else
            {
                print("Avatar ID : " + 0);
                AvatarGroupManager.Instance.SetCurrentAvatar(0);
                UIManager.Instance.AvatarCustomizationScreen();
            }


        }
    }

    private IEnumerator GetPlayerType()
    {
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("PlayerType").GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogError(message: $"Failed to register Task with {DBTask.Exception}");
            PlayerInfoDontDestroy.Instance.currentPlayerType = PlayerType.Guest;
            UIManager.Instance.SetPlayerType(0);
        }

        else
        {
            DataSnapshot snapshot = DBTask.Result;

            if (snapshot.Value != null)
            {
                string playerType = snapshot.Value.ToString();

                print("Avatar ID : " + playerType);

                var tempPlayerType = (PlayerType)Enum.Parse(typeof(PlayerType), playerType);

                PlayerInfoDontDestroy.Instance.currentPlayerType = tempPlayerType;
                UIManager.Instance.SetPlayerType((int)tempPlayerType);
            }
            else
            {
                PlayerInfoDontDestroy.Instance.currentPlayerType = PlayerType.Guest;
                UIManager.Instance.SetPlayerType((int)PlayerType.Guest);
            }

        }
    }

    private IEnumerator GetPlayerRegion()
    {
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("Region").GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogError(message: $"Failed to register Task with{DBTask.Exception}");
        }

        else
        {
            DataSnapshot snapshot = DBTask.Result;
            if (snapshot.Value != null)
            {
                string region = snapshot.Value.ToString();

                print("Selected Region: " + region);

                regionSelection.SetDropdown(region);
            }
        }
    }


    void CheckNetworkAndEnableLoginScreen()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            UIManager.Instance.LoginScreen();
        }
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != null)
        {
            if (auth.CurrentUser != User)
            {

                bool signedIn = User != auth.CurrentUser && auth.CurrentUser != null;
                if (!signedIn && User != null)
                {
                    Debug.Log("Signed out " + User.UserId);
                }


                User = auth.CurrentUser;
                if (signedIn)
                {
                    Debug.Log("Signed in " + User.UserId);

                    MainThreadDispatcher.Instance().Enqueue(AfterLogin);

                }

            }

        }
        else
        {
            print("The part is running!!");
            MainThreadDispatcher.Instance().Enqueue(UIManager.Instance.LoginScreen);

        }


    }



    void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }
    #endregion

    #region New Avatar Data Storing
    private IEnumerator SetPlayerBodyParts(string head, string top, string footwear, string bottom, Gender gender)
    {
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("BodyParts").Child("Head").SetValueAsync(head);
        var DBTask2 = DBreference.Child("users").Child(User.UserId).Child("BodyParts").Child("Top").SetValueAsync(top);
        var DBTask3 = DBreference.Child("users").Child(User.UserId).Child("BodyParts").Child("FootWear").SetValueAsync(footwear);
        var DBTask4 = DBreference.Child("users").Child(User.UserId).Child("BodyParts").Child("Bottom").SetValueAsync(bottom);
        var DBTask5 = DBreference.Child("users").Child(User.UserId).Child("BodyParts").Child("Gender").SetValueAsync(gender.ToString());


        yield return new WaitUntil(predicate: () => DBTask.IsCompleted && DBTask2.IsCompleted && DBTask3.IsCompleted && DBTask4.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogError(message: $"Failed to register Task with{DBTask.Exception}");
        }

        else
        {
            print("Updated body parts to database");
        }
    }

    private IEnumerator GetPlayerBodyParts()
    {
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("BodyParts").GetValueAsync();


        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogError(message: $"Failed to register Task with{DBTask.Exception}");
            changeGender.ToggleMale();
            _maleOutfitController.SetPlayerBodyFromDatabase("0", "0", "0", "0");
        }

        else
        {
            DataSnapshot snapshot = DBTask.Result;
            if (snapshot != null)
            {
                var snapshotValue = snapshot.GetRawJsonValue();

                if (snapshotValue != null)
                {
                    print("JSON: -- " + snapshotValue.ToString());
                    dataBaseBodyParts = JsonUtility.FromJson<TheBodyParts>(snapshotValue.ToString());

                    if (dataBaseBodyParts.Gender == Gender.Male.ToString())
                    {
                        changeGender.ToggleMale();
                        _maleOutfitController.SetPlayerBodyFromDatabase(dataBaseBodyParts.Head, dataBaseBodyParts.Bottom, dataBaseBodyParts.FootWear, dataBaseBodyParts.Top);
                        PlayerInfoDontDestroy.Instance._selectedGender = Gender.Male;
                    }

                    else if (dataBaseBodyParts.Gender == Gender.Female.ToString())
                    {
                        changeGender.ToggleFemale();
                        _femaleOutfitController.SetPlayerBodyFromDatabase(dataBaseBodyParts.Head, dataBaseBodyParts.Bottom, dataBaseBodyParts.FootWear, dataBaseBodyParts.Top);
                        PlayerInfoDontDestroy.Instance._selectedGender = Gender.Female;
                    }
                }
            }
            print("Body parts successfully fetched");
        }
    }

    private IEnumerator UpdateGender(Gender gender)
    {

        var DBTask = DBreference.Child("users").Child(User.UserId).Child("BodyParts").Child("Gender").SetValueAsync(gender.ToString());


        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogError(message: $"Failed to register Task with{DBTask.Exception}");
        }

        else
        {
            print("Updated Gender to database");
            PlayerInfoDontDestroy.Instance._selectedGender = gender;
        }
    }
    #endregion


    #region Forgot Password

    void ResetPassword(string emailAddress)
    {


        auth.SendPasswordResetEmailAsync(emailAddress).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SendPasswordResetEmailAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SendPasswordResetEmailAsync encountered an error: " + task.Exception);
                return;
            }

            Debug.Log("Password reset email sent successfully.");
            _forgotPswWarningText.text = "The Email has been sent to reset password!";
        });

    }
    #endregion
}

[Serializable]
public class TheBodyParts
{
    public string Head;
    public string Top;
    public string FootWear;
    public string Bottom;
    public string Gender;
}