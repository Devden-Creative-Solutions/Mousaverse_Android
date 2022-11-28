using Lean.Gui;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : GenericSingletonClass<UIManager>
{
    public LeanToggle[] screens;
    [SerializeField] GameObject loginCanvas;
    [SerializeField] GameObject avatarCanvas;
    [SerializeField] TMP_Text _yourAvatarName;
    [SerializeField] TMP_Dropdown selectedPlayerType;

    [SerializeField] LeanToggle _MainMenuToggle;

    [SerializeField] GameObject _mainMenuAvatars;

    Dictionary<string, LeanToggle> screensDict;

    private void Start()
    {
        Debug.unityLogger.logEnabled = true;

        screensDict = new Dictionary<string, LeanToggle>();

        for (int i = 0; i < screens.Length; i++)
        {
            screensDict.Add(screens[i].gameObject.name, screens[i]);
        }
    }

    public void SetPlayerType(int index)
    {
        selectedPlayerType.value = index;
    }
    public void ShowUI(string screenName)
    {
        screensDict[screenName].TurnOn();
    }

    public void AvatarCustomizationScreen()
    {
        print("Changing Screen to Avatar Customization Screen");
        loginCanvas.SetActive(false);
        avatarCanvas.SetActive(true);
        _MainMenuToggle.TurnOn();
    }

    public void LoginScreen()
    {
        loginCanvas.SetActive(true);
        avatarCanvas.SetActive(false);
        _MainMenuToggle.TurnOff();
    }

    public void SetYourAvatarName(string avatarName)
    {
        _yourAvatarName.text = "Your Avatar: " + avatarName;
    }

    public void UpdateMainMenuAvatar(int AvatarID = -1)
    {
        foreach(Transform x in _mainMenuAvatars.transform)
        {
            if(x == _mainMenuAvatars.transform.GetChild(AvatarID) && AvatarID!=-1)
            {
                x.gameObject.SetActive(true);
            }

            else if(AvatarID== -1)
            {
                _mainMenuAvatars.transform.GetChild(_mainMenuAvatars.transform.childCount - 1).gameObject.SetActive(true);
            }
            else
            {
                x.gameObject.SetActive(false);
            }
        }
    }

}
