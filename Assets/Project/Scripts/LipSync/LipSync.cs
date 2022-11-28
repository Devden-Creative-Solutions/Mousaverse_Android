using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LipSync : MonoBehaviour
{
    public Transform testObj;
    public SkinnedMeshRenderer mouthBlend;
    public SkinnedMeshRenderer teethBlend;
    public float mouthBlendModifier;
    public float teethBlendModifier;
    public TypeOfVoice currentTypeOfVoice;

    public AudioSource playerAudioSource;

    AudioClip[] sounds; // set the array size and the sounds in the Inspector    
    private float[] freqData;
    private int nSamples = 256;
    private float fMax = 24000;
    private AudioSource theAudio; // AudioSource attached to this object

    float BandVol(float fLow, float fHigh)
    {
        fLow = Mathf.Clamp(fLow, frqLowClamp, fMax); // limit low...
        fHigh = Mathf.Clamp(fHigh, frqHighClamp, fMax); // and high frequencies
                                                        // get spectrum: freqData[n] = vol of frequency n * fMax / nSamples
        theAudio.GetSpectrumData(freqData, 0, FFTWindow.BlackmanHarris);
        int n1 = (int)Mathf.Floor(fLow * nSamples / fMax);
        int n2 = (int)Mathf.Floor(fHigh * nSamples / fMax);
        float sum = 0;
        // average the volumes of frequencies fLow to fHigh
        for (int i = n1; i < n2; i++)
        {
            sum += freqData[i];
        }
        return sum / (n2 - n1 + 1);
    }

    public GameObject mouth;
    public float volume = 40;
    public float frqLow = 200;
    public float frqHigh = 800;

    public float frqLowClamp;
    public float frqHighClamp;

    float y0;

    void Start()
    {
        if(currentTypeOfVoice == TypeOfVoice.OperaSinger)
        {
            theAudio = GetComponent<AudioSource>(); // get AudioSource component
        }
        else if(currentTypeOfVoice == TypeOfVoice.PlayerSinger)
        {
            theAudio = playerAudioSource;
        }

        //y0 = mouth.transform.position.y;
        freqData = new float[nSamples];
        theAudio.Play();
    }

    void Update()
    {
        mouthBlend.SetBlendShapeWeight(37, Mathf.Lerp(mouthBlend.GetBlendShapeWeight(37), (BandVol(frqLow, frqHigh) * volume) * mouthBlendModifier, Time.deltaTime * 10));
        teethBlend.SetBlendShapeWeight(37, Mathf.Lerp(teethBlend.GetBlendShapeWeight(37), (BandVol(frqLow, frqHigh) * volume) * teethBlendModifier, Time.deltaTime * 10));
        //mouthBlend.SetBlendShapeWeight(0, Mathf.Lerp(mouthBlend.GetBlendShapeWeight(37), (BandVol(frqLow, frqHigh) * volume) * mouthBlendModifier, Time.deltaTime * 10));
        //testObj.transform.localScale = Vector3.one * (1 + BandVol(frqLow, frqHigh) * volume);
        //mouth.transform.position.y = y0 + BandVol(frqLow, frqHigh) * volume;
    }

    // A function to play sound N:
    void PlaySoundN(int N)
    {

        theAudio.clip = sounds[N];
        theAudio.Play();
    }
}

public enum TypeOfVoice
{
    OperaSinger,
    PlayerSinger
}
