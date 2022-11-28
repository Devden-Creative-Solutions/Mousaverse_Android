using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnSceneChange : MonoBehaviour
{
    int avatarID = 0;
    void Start()
    {
        if (PlayerInfoDontDestroy.Instance.hasUserLoggedIn())
        {
            UIManager.Instance.AvatarCustomizationScreen();
            UIManager.Instance.SetPlayerType((int)PlayerInfoDontDestroy.Instance.currentPlayerType);
            avatarID = PlayerInfoDontDestroy.Instance.GetSelectedAvatar();
            Invoke(nameof(DelayedAvatarSet), .1f);
            FirebaseManager.Instance.User = PlayerInfoDontDestroy.Instance.GetFireBaseUser();
            
        }
    }

    void DelayedAvatarSet()
    {
        AvatarGroupManager.Instance.SetCurrentAvatar(avatarID);
    }
}
