using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] Slider volumeSlider;

    private void Start()
    {
        AudioListener.volume = PlayerInfoDontDestroy.Instance.applicatonVolume;
        volumeSlider.value = PlayerInfoDontDestroy.Instance.applicatonVolume;
    }

    public void ChangeVolume()
    {
        AudioListener.volume = volumeSlider.value;
        PlayerInfoDontDestroy.Instance.applicatonVolume = volumeSlider.value;
    }

    public void ExitApplication()
    {
        Application.Quit();
    }
}
