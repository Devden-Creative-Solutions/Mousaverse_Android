using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableMainMenuAvatar : MonoBehaviour
{
    [SerializeField] GameObject mainMenuAvatars;

    private void OnEnable()
    {
        mainMenuAvatars.SetActive(true);
    }

    private void OnDisable()
    {
        mainMenuAvatars.SetActive(false);
    }
}
