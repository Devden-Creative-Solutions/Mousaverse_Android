using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;
using UnityEngine.Networking;

public class BGMAudioController : MonoBehaviour
{
    public AudioSource recorder;
    public AudioSource[] operaRecorders;


    public RecordedAudioSessionDetails recordedAudioSessionDetails;

    string audioSessionDetailsPath = "audioSessionDetail/detail";

    string currentAudioUrl;
    string currentAudioScheduledForPlaying;

    FirebaseFirestore fs;
    ListenerRegistration listenerRegistration;


    // Start is called before the first frame update
    void Start()
    {
        fs = FirebaseFirestore.DefaultInstance;

        SceneChangeController.Instance.OnTeleportedTo += GetDocumentDetails;

        recorder.playOnAwake = false;

        InvokeRepeating("GetDocumentDetails", 0.5f, 2.5f);
        //GetDocumentDetails();
        listenerRegistration = fs.Document(audioSessionDetailsPath).Listen(snapShot =>
        {
            recordedAudioSessionDetails = snapShot.ConvertTo<RecordedAudioSessionDetails>();
            DisplayData();
        });
    }

    private void OnDestroy()
    {
        SceneChangeController.Instance.OnTeleportedTo -= GetDocumentDetails;
    }

    void GetDocumentDetails()
    {
        fs.Collection("audioSessionDetail").Document("detail").GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            recordedAudioSessionDetails = task.Result.ConvertTo<RecordedAudioSessionDetails>();
            DisplayData();
        });
    }

    void DisplayData()
    {
        //Debug.Log(recordedAudioSessionDetails.startTime);

        if (recordedAudioSessionDetails.playBGM == false)
        {
            recorder.Stop();
            recorder.clip = null;

            for (int i = 0; i < operaRecorders.Length; i++)
            {
                operaRecorders[i].Stop();
                operaRecorders[i].clip = null;
            }

            currentAudioUrl = "";
            currentAudioScheduledForPlaying = "";

            return;
        }

        if (!string.Equals(currentAudioUrl, recordedAudioSessionDetails.bgmUrl))
        {
            ChangeAudio(recordedAudioSessionDetails.bgmUrl);
            currentAudioUrl = recordedAudioSessionDetails.bgmUrl;
        }
        else if (recorder.clip)
        {
            if (recordedAudioSessionDetails.playBGM)
            {
                PlayAudio();
            }
        }
    }

    void ChangeAudio(string audioUrl)
    {
        StartCoroutine(GetAudioClip(audioUrl, x =>
        {
            recorder.clip = x;
            for (int i = 0; i < operaRecorders.Length; i++)
            {
                operaRecorders[i].clip = x;
            }
            if (recordedAudioSessionDetails.playBGM)
                PlayAudio();
        }));
    }

    void PlayAudio()
    {
        if (!string.Equals(currentAudioScheduledForPlaying, recordedAudioSessionDetails.bgmUrl))
        {
            if (recorder.clip)
            {
                recorder.Play();

                for (int i = 0; i < operaRecorders.Length; i++)
                {
                    operaRecorders[i].Play();
                }

                currentAudioScheduledForPlaying = recordedAudioSessionDetails.bgmUrl;
            }
        }
    }

    IEnumerator GetAudioClip(string audioUrl, Action<AudioClip> callback)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(audioUrl, AudioType.WAV))
        {
            yield return www.SendWebRequest();

            while (!www.isDone)
            {
                yield return null;
            }

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                AudioClip myClip = DownloadHandlerAudioClip.GetContent(www);
                callback?.Invoke(myClip);
            }
        }

        yield break;
    }

}
