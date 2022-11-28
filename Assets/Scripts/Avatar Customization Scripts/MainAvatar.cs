
using UnityEngine;

public abstract class MainAvatar : MonoBehaviour
{
    public int AvatarID;
    public GameObject AvatarGameobject;
    public string AvatarPrefabName;
    public int AvatarGroupID;

    public bool isSelected = false;
    public abstract void ResetEverything();
    public abstract void InProgress();
    public abstract void Ended();

}