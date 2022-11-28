using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;

public class VoiceChatController : VoiceComponent
{

    private List<byte> groupsToAdd = new List<byte>();
    private List<byte> groupsToRemove = new List<byte>();

    [SerializeField] // TODO: make it readonly
    private byte[] subscribedGroups;

    [SerializeField] private PhotonVoiceView photonVoiceView;
    [SerializeField] private PhotonView photonView;

    public byte TargetInterestGroup
    {
        get
        {
            if (this.photonView != null)
            {
                return (byte)this.photonView.OwnerActorNr;
            }
            return 0;
        }
    }

    byte previousAudioGroup;

    void Start()
    {
        if (photonView.IsMine)
        {
            SceneChangeController.Instance.voiceChatController = this;
            ChangeAudioGroup(20);
        }

        this.IsLocalCheck();
    }

    public void ChangeAudioGroup(byte val)
    {
        //PhotonVoiceNetwork.Instance.Client.GlobalInterestGroup = val;

        //PhotonVoiceNetwork.Instance.Client.OpChangeGroups(new byte[0], new byte[] { val });


        if (!this.groupsToRemove.Contains(previousAudioGroup))
        {
            this.groupsToRemove.Add(previousAudioGroup);
        }


        if (!this.groupsToAdd.Contains(val))
        {
            this.groupsToAdd.Add(val);
            previousAudioGroup = val;
        }

        if (this.photonVoiceView.RecorderInUse != null)
        {
            if (this.photonVoiceView.RecorderInUse.InterestGroup != previousAudioGroup)
            {
                if (this.Logger.IsInfoEnabled)
                {
                    this.Logger.LogInfo("Setting RecorderInUse's InterestGroup to {0}", previousAudioGroup);
                }
                this.photonVoiceView.RecorderInUse.InterestGroup = previousAudioGroup;
            }
        }
    }

    //public void EnterOperaHouse1(Collider other)
    //{
    //    if (this.IsLocalCheck())
    //    {
    //        ProximityVoiceTrigger trigger = other.GetComponent<ProximityVoiceTrigger>();
    //        if (trigger != null)
    //        {
    //            byte group = trigger.TargetInterestGroup;
    //            if (this.Logger.IsDebugEnabled)
    //            {
    //                this.Logger.LogDebug("OnTriggerEnter {0}", group);
    //            }
    //            if (group == this.TargetInterestGroup)
    //            {
    //                return;
    //            }
    //            if (group == 0)
    //            {
    //                return;
    //            }
    //            if (!this.groupsToAdd.Contains(group))
    //            {
    //                this.groupsToAdd.Add(group);
    //            }
    //        }
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (this.IsLocalCheck())
    //    {
    //        ProximityVoiceTrigger trigger = other.GetComponent<ProximityVoiceTrigger>();
    //        if (trigger != null)
    //        {
    //            byte group = trigger.TargetInterestGroup;
    //            if (this.Logger.IsDebugEnabled)
    //            {
    //                this.Logger.LogDebug("OnTriggerExit {0}", group);
    //            }
    //            if (group == this.TargetInterestGroup)
    //            {
    //                return;
    //            }
    //            if (group == 0)
    //            {
    //                return;
    //            }
    //            if (this.groupsToAdd.Contains(group))
    //            {
    //                this.groupsToAdd.Remove(group);
    //            }
    //            if (!this.groupsToRemove.Contains(group))
    //            {
    //                this.groupsToRemove.Add(group);
    //            }
    //        }
    //    }
    //}

    private void Update()
    {
        if (!PhotonVoiceNetwork.Instance.Client.InRoom)
        {
            this.subscribedGroups = null;
        }
        else if (this.IsLocalCheck())
        {
            if (this.groupsToAdd.Count > 0 || this.groupsToRemove.Count > 0)
            {
                byte[] toAdd = null;
                byte[] toRemove = null;
                if (this.groupsToAdd.Count > 0)
                {
                    toAdd = this.groupsToAdd.ToArray();
                }
                if (this.groupsToRemove.Count > 0)
                {
                    toRemove = this.groupsToRemove.ToArray();
                }
                if (this.Logger.IsInfoEnabled)
                {
                    this.Logger.LogInfo("client of actor number {0} trying to change groups, to_be_removed#:{1} to_be_added#={2}", this.TargetInterestGroup, this.groupsToRemove.Count, this.groupsToAdd.Count);
                }
                if (PhotonVoiceNetwork.Instance.Client.OpChangeGroups(toRemove, toAdd))
                {
                    if (this.subscribedGroups != null)
                    {
                        List<byte> list = new List<byte>();
                        for (int i = 0; i < this.subscribedGroups.Length; i++)
                        {
                            list.Add(this.subscribedGroups[i]);
                        }
                        for (int i = 0; i < this.groupsToRemove.Count; i++)
                        {
                            if (list.Contains(this.groupsToRemove[i]))
                            {
                                list.Remove(this.groupsToRemove[i]);
                            }
                        }
                        for (int i = 0; i < this.groupsToAdd.Count; i++)
                        {
                            if (!list.Contains(this.groupsToAdd[i]))
                            {
                                list.Add(this.groupsToAdd[i]);
                            }
                        }
                        this.subscribedGroups = list.ToArray();
                    }
                    else
                    {
                        this.subscribedGroups = toAdd;
                    }
                    this.groupsToAdd.Clear();
                    this.groupsToRemove.Clear();
                }
                else if (this.Logger.IsErrorEnabled)
                {
                    this.Logger.LogError("Error changing groups");
                }
            }
        }
    }

    private bool IsLocalCheck()
    {
        if (this.photonVoiceView.IsPhotonViewReady)
        {
            if (this.photonView.IsMine)
            {
                return true;
            }
            if (this.enabled)
            {
                if (this.Logger.IsInfoEnabled)
                {
                    this.Logger.LogInfo("Disabling ProximityVoiceTrigger as does not belong to local player, actor number {0}", this.TargetInterestGroup);
                }
                this.enabled = false;
            }
        }
        return false;
    }
}
