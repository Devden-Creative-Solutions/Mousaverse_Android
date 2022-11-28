using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Voice;
using Photon.Voice.PUN;

public class MutePlayers : MonoBehaviour
{
    [SerializeField] TMP_Text playerName;
    [SerializeField] GameObject playerGO;

    public void MuteThePlayer()
    {
        PhotonVoiceView photonVoiceView =  playerGO.GetComponent<PhotonVoiceView>();
        photonVoiceView.SpeakerInUse.gameObject.GetComponent<AudioSource>().mute = true;
    }

    public void UnMuteThePlayer()
    {
        PhotonVoiceView photonVoiceView = playerGO.GetComponent<PhotonVoiceView>();
        photonVoiceView.SpeakerInUse.gameObject.GetComponent<AudioSource>().mute = false;
    }

    public void SetThePlayerGameObjectAndName(string playerName, GameObject playerGO)
    {
        this.playerName.text = playerName;
        this.playerGO = playerGO;

    }
}
