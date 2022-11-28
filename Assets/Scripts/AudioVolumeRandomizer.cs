using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVolumeRandomizer : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] float _maxVolume;
    [SerializeField] float _minVolume;

    float timer = 0;
    float goal = 5;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
    }

    private void Update()
    {
        if(timer < goal)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0;
            goal = Random.Range(1, 7);
            RandomizeTheAudio();
            print("Randomizing the Audio!!");
        }
    }

    void RandomizeTheAudio()
    {
        float x =  Random.Range(_minVolume, _maxVolume);
        audioSource.volume = x;
    }
}
