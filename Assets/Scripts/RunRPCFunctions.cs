using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RunRPCFunctions : MonoBehaviour
{
    [SerializeField] AssignSeat assignSeat;
    [SerializeField] PhotonView photonView;
    [SerializeField] OutfitControllerOld outfitController;
    public TheBodyParts rpcBodyParts = new TheBodyParts();

    public void SetAssignSeatUserIDRPC(string userID)
    {
        photonView.RPC("SetAssignSeatPlayerID",RpcTarget.AllBuffered ,userID);
    }

    private void Start()
    {
        
        if (photonView.IsMine)
        {
            TheBodyParts tempbp = PlayerInfoDontDestroy.Instance.GetSelectedBodyParts();

            SetRPCBodyParts(tempbp.Head, tempbp.Top, tempbp.FootWear, tempbp.Bottom, tempbp.Gender);
            StartCoroutine(DelayedCallForChangingOutfit(tempbp.Head, tempbp.Top, tempbp.FootWear, tempbp.Bottom, tempbp.Gender));
        }
    }

    [PunRPC]
    void SetAssignSeatPlayerID(string userID)
    {
        assignSeat.PlayerUserID = userID;
    }
    IEnumerator DelayedCallForChangingOutfit(string head, string top, string footwear, string bottom, string gender)
    {
        yield return new WaitForSeconds(.3f);
        photonView.RPC("SettingTheRPCBodyParts", RpcTarget.AllBuffered, head, top, footwear, bottom, gender);
    }

    public void SetRPCBodyParts(string head, string top,string footwear,string bottom, string gender)
    {
       // photonView.RPC("SettingTheRPCBodyParts", RpcTarget.AllBuffered, head, top, footwear, bottom, gender);
    }

    [PunRPC]
    void SettingTheRPCBodyParts(string head, string top, string footwear,string bottom, string gender)
    {
        outfitController.StartOutfitChange();
        outfitController.SetPlayerBodyFromDatabase(head, bottom, footwear, top);
    }
}
