//using Firebase.Firestore;
//using Photon.Voice.PUN;
//using Photon.Voice.Unity;

using Firebase.Firestore;
using Firebase.Extensions;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;

public class AudioController : MonoBehaviour
{
    #region singleton


    private static AudioController _instance;

    public static AudioController Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    #endregion


    public AudioSource recorder;
    public AudioSource[] operaRecorders;
    public string istDate;
    public string istTime;
    public AudioClip[] audioClips;

    public RecordedAudioSessionDetails recordedAudioSessionDetails;

    string audioSessionDetailsPath = "audioSessionDetail/detail";

    string currentAudioUrl;
    string currentAudioScheduledForPlaying;

    FirebaseFirestore fs;
    ListenerRegistration listenerRegistration;

    

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
        if (!string.Equals(currentAudioUrl, recordedAudioSessionDetails.audioUrl))
        {
            ChangeAudio(recordedAudioSessionDetails.audioUrl);
            currentAudioUrl = recordedAudioSessionDetails.audioUrl;
        }
        else if (recorder.clip)
        {
            if (recordedAudioSessionDetails.startSession)
            {
                CalculateRemainingTimeAndPlay();
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
            if (recordedAudioSessionDetails.startSession)
                CalculateRemainingTimeAndPlay();
        }));
    }

    void CalculateRemainingTimeAndPlay()
    {
        if (string.IsNullOrEmpty(recordedAudioSessionDetails.startTime))
            return;

        StartCoroutine(GetCurrentTime("https://worldtimeapi.org/api/timezone/Asia/Kolkata", x =>
        {
            var temp = DateTime.Parse(x.datetime.Split('.')[0]).ToLongTimeString();

            DateTime currentTime = DateTime.Parse(temp);
            DateTime scheduledTime = DateTime.Parse(recordedAudioSessionDetails.startTime);

            TimeSpan diff = scheduledTime - currentTime;

            var absTimeDiff = Mathf.Abs((float)diff.TotalSeconds);
            //Debug.Log(absTimeDiff);
            if (!string.Equals(currentAudioScheduledForPlaying, recordedAudioSessionDetails.audioUrl))
            {
                if (diff.Ticks <= 0)
                {
                    PlayAudio(absTimeDiff);
                    currentAudioScheduledForPlaying = recordedAudioSessionDetails.audioUrl;
                }
                else
                {
                    if (absTimeDiff < 2.5f)
                    {

                        StartCoroutine(ScheduleTimeForAudio(absTimeDiff, () =>
                        {
                            PlayAudio(absTimeDiff);
                        }));


                        currentAudioScheduledForPlaying = recordedAudioSessionDetails.audioUrl;
                    }
                }
            }
        }));

    }


    void PlayAudio(float absTimeDiff)
    {
        if (recorder.clip && recorder.clip.length > absTimeDiff)
        {
            recorder.time = absTimeDiff;
            recorder.Play();

            for (int i = 0; i < operaRecorders.Length; i++)
            {
                operaRecorders[i].time = absTimeDiff;
                operaRecorders[i].Play();
            }
        }
    }


    IEnumerator ScheduleTimeForAudio(float scheduledTime, Action callback)
    {
        yield return new WaitForSeconds(scheduledTime);
        callback?.Invoke();
    }

    IEnumerator GetCurrentTime(string uri, Action<CurrentTimeDetails> callback)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    //Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    var currentTime = JsonUtility.FromJson<CurrentTimeDetails>(webRequest.downloadHandler.text);
                    callback?.Invoke(currentTime);
                    break;
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

[FirestoreData]
public struct RecordedAudioSessionDetails
{
    [FirestoreProperty]
    public string audioUrl { get; set; }

    [FirestoreProperty]
    public string bgmUrl { get; set; }

    [FirestoreProperty]
    public bool playBGM { get; set; }

    [FirestoreProperty]
    public bool startSession { get; set; }

    [FirestoreProperty]
    public string startTime { get; set; }
}

[System.Serializable]
public class CurrentTimeDetails
{
    public string abbreviation;
    public string client_ip;
    public string datetime;
    public int day_of_week;
    public int day_of_year;
    public bool dst;
    public object dst_from;
    public int dst_offset;
    public object dst_until;
    public int raw_offset;
    public string timezone;
    public int unixtime;
    public DateTime utc_datetime;
    public string utc_offset;
    public int week_number;
}

