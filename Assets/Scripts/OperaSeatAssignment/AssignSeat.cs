using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Firebase.Auth;
using Photon.Realtime;
using Photon.Voice.PUN;

public class AssignSeat : MonoBehaviourPunCallbacks
{
    [SerializeField] PhotonView photonView;
    public PlayerType currentPlayerType;
    [SerializeField] OperaSeat AssignedSeat;
    [SerializeField] SingerSeat AssignedSingerSeat;
    [SerializeField] OperaHouseID currentAssignedOperaHouse;
    [SerializeField] NetworkedAnimator networkedAnimator;
    public GameObject _mainCamera;

    public FirebaseAuth auth;
    public FirebaseUser User;

    public string PlayerUserID;
    public string localUserID;

    private void Start()
    {
        OperaPlacement.gettingOutOfOpera += GetOutOfOperaHouse;
        if (photonView.IsMine)
        {
            currentPlayerType = PlayerInfoDontDestroy.Instance.currentPlayerType;
            SetUserID();
            StartCoroutine(CheckAndSetPlayerID());
        }

    }

    IEnumerator CheckAndSetPlayerID()
    {
        yield return new WaitUntil(() => User.UserId != null);
        print("User ID has been set!!");
        photonView.gameObject.GetComponent<RunRPCFunctions>().SetAssignSeatUserIDRPC(User.UserId);
    }



    void SetUserID()
    {
        auth = FirebaseAuth.DefaultInstance;
        User = auth.CurrentUser;
        //PlayerUserID = User.UserId;
        localUserID = PhotonNetwork.LocalPlayer.UserId;

    }


    public void GetSeatInOpera(OperaHouseID operaHouseID)
    {
        if (photonView.IsMine)
        {
            OperaPlacement.Instance.PlacePlayerInOpera(this.gameObject, operaHouseID);
            print("Getting Seat in Opera!!");
        }
    }

    public void SetAssignedOperaHouse(OperaHouseID operaHouseID)
    {
        currentAssignedOperaHouse = operaHouseID;
    }

    public OperaHouseID GetAssignedOperaHouseID()
    {
        return currentAssignedOperaHouse;
    }

    public void SetAssignedSeat(OperaSeat operaSeat)
    {
        AssignedSeat = operaSeat;
    }

    public void SetAssignedSingerSeat(SingerSeat singerSeat)
    {
        AssignedSingerSeat = singerSeat;
    }

    public void GetSingerSeat(OperaHouseID operaHouseID)
    {
        if (photonView.IsMine)
        {
            OperaPlacement.Instance.PlaceSingerInOpera(this.gameObject, operaHouseID);
        }
    }

    [PunRPC]
    void DisableOperaSingerForAllPlayers()
    {

    }

    public void StartSittingAnimation()
    {
        if (photonView.IsMine)
        {
            //var anim = this.GetComponent<Animator>();

            //anim.enabled = true;
            //anim.SetBool("isWalking", false);
            //anim.SetFloat("HorizontalSpeed", 0);

            Invoke(nameof(DelayedSit), .3f);
        }
    }

    void DelayedSit()
    {
        if (photonView.IsMine)
        {
            var anim = this.GetComponent<Animator>();

            anim.SetBool("isWalking", false);
            anim.SetFloat("HorizontalSpeed", 0);
            //anim.SetTrigger("canSit");
            anim.enabled = true;
            networkedAnimator.SendPlayAnimationEvent(photonView.ViewID, "canSit", "Trigger");
            //this.transform.rotation = Quaternion.identity;
        }
    }

    public void MutePlayer()
    {
        if (photonView.IsMine)
        {
            OperaPlacement.Instance.MuteGuest();
        }
    }

    public void UnMutePlayer()
    {
        if (photonView.IsMine)
        {
            //photonView.gameObject.transform.GetChild(0).GetComponent<LipSync>().playerAudioSource.mute = false;
            OperaPlacement.Instance.UnMuteGuest();
        }
    }

    public void ClearAssignedSeats()
    {
        AssignedSingerSeat = null;
        currentAssignedOperaHouse = OperaHouseID.Nothing;
    }

    public void GetOutOfOperaHouse()
    {
        if (photonView.IsMine)
        {

            switch (currentAssignedOperaHouse)
            {
                case OperaHouseID.OperaHouse1:
                    SceneChangeController.Instance.TeleportOut(0, false);
                    break;

                case OperaHouseID.OperaHouse2:
                    SceneChangeController.Instance.TeleportOut(3, false);
                    break;

            }


            if (currentPlayerType == PlayerType.Guest)
            {
                AssignedSeat.ClearSeat();
            }
            else if (currentPlayerType == PlayerType.Singer)
            {
                AssignedSingerSeat.ClearSeat();
            }
        }
    }

    private void OnDestroy()
    {
        OperaPlacement.gettingOutOfOpera -= GetOutOfOperaHouse;
    }
}
