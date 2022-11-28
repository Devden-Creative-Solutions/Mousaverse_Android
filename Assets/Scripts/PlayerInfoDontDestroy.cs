using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;

public class PlayerInfoDontDestroy : MonoBehaviour
{
    #region singleton


    private static PlayerInfoDontDestroy _instance;

    public static PlayerInfoDontDestroy Instance { get { return _instance; } }


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

    public string SelectedAvatarName = "Network Player";
    public PlayerType currentPlayerType = PlayerType.Guest;
    public string userName = "Anonymous";
    public float applicatonVolume = .5f;
    private bool hasLoggedIn = false;
    private int theSelectedAvatar = 0;
    public Gender _selectedGender = Gender.Male;
    [SerializeField]
    private string SelectedRegionCode = "asia";

    [SerializeField] FirebaseUser firebaseUser;
    [SerializeField] string firebaseUserID;

    [SerializeField] TheBodyParts selectedBodyParts;

    #region RegionSelection
    public void SetSelectedRegionCode(string code)
    {
        SelectedRegionCode = code;
    }

    public string GetSelectedRegionCode()
    {
        return SelectedRegionCode;
    }
    #endregion

    #region SetFirebaseUser
    public void SetFirebaseUser(FirebaseUser _firebaseUser)
    {
        firebaseUser = _firebaseUser;
        firebaseUserID = _firebaseUser.UserId;
    }
    public FirebaseUser GetFireBaseUser()
    {
        return firebaseUser;
    }
    #endregion

    #region UserLogin
    public bool hasUserLoggedIn()
    {
        return hasLoggedIn;
    }

    public void LogInTheUser()
    {
        hasLoggedIn = true;
    }
    #endregion

    #region SelectedAvatar
    public void SetSelectedAvatar(int avatarID)
    {
        theSelectedAvatar = avatarID;
    }
    public int GetSelectedAvatar()
    {
        return theSelectedAvatar;
    }
    #endregion

    #region Select Body Parts
    public void SetSelectedBodyParts(TheBodyParts theBodyParts)
    {
        this.selectedBodyParts = theBodyParts;
    }

    public TheBodyParts GetSelectedBodyParts()
    {
        return this.selectedBodyParts;
    }
    #endregion

}
