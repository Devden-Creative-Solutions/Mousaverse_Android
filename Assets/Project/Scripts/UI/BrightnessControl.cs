using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrightnessControl : MonoBehaviour
{
    [SerializeField]
    Slider brightnessSlider;

    // Start is called before the first frame update
    void Start()
    {
        brightnessSlider.value = Screen.brightness;

        brightnessSlider.onValueChanged.AddListener(x =>
        {
            Screen.brightness = x;
        });
    }

    private void OnDestroy()
    {
        brightnessSlider.onValueChanged.RemoveAllListeners();
    }

}
