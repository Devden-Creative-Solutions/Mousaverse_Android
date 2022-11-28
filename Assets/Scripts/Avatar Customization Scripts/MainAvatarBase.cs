using UnityEngine;

public class MainAvatarBase : MainAvatar
{
    public override void Ended()
    {
        AvatarGameobject.SetActive(false);
    }

    public override void InProgress()
    {
        
    }

    public override void ResetEverything()
    {
        AvatarGameobject.SetActive(true);
    }
}
