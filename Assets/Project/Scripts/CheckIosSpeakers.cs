using System.Collections;
using System.Collections.Generic;
using Photon.Voice.Unity;
using UnityEngine;

public class CheckIosSpeakers : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        iPhoneSpeaker.CheckiOSPrepare();
    }

}
