
using UnityEngine;

public abstract class AvatarGroup : MonoBehaviour
{
    public int GroupID;
    public GameObject GroupGameobject;
    public int FirstChildID;
    public int childCount;
    public abstract void ResetEverything();
    public abstract void InProgress();
    public abstract void Ended();

}
