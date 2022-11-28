using UnityEngine;

public class AvatarGroupBase : AvatarGroup
{
    public override void Ended()
    {
        GroupGameobject.SetActive(true);
    }

    public override void InProgress()
    {
        
    }

    public override void ResetEverything()
    {
        GroupGameobject.SetActive(true);
    }
}
