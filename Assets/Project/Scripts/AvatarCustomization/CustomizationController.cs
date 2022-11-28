using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizationControllerWrapper : MonoBehaviour
{
    public CustomizationController customizationController { get { return FindObjectOfType<CustomizationController>(); } }
}

public class CustomizationController : MonoBehaviour
{
    public AvatarPart[] avatarParts;

    int currentAvatarPartIndex;

    void SetCurrentAvatarPart(int index)
    {
        currentAvatarPartIndex = index;
    }

    public void SwitchAvatarSubPart(int index)
    {
        avatarParts[currentAvatarPartIndex].avatarSubParts[index].SetActive(true);

        for (int i = 0; i < avatarParts[currentAvatarPartIndex].avatarSubParts.Length; i++)
        {
            if (i != index)
            {
                avatarParts[currentAvatarPartIndex].avatarSubParts[i].SetActive(false);
            }
        }
    }
}

[System.Serializable]
public class AvatarPart
{
    public string part;
    public GameObject[] avatarSubParts;
}
