using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UserNameDisplay : MonoBehaviour
{
    [SerializeField]
    PhotonView photonView;

    [SerializeField]
    TextMeshProUGUI playerName;

    // Start is called before the first frame update
    void Start()
    {
        //if (!photonView.IsMine)
        //    return;

        //PhotonNetwork.NickName = AuthManager.myUserName;

        //if (string.IsNullOrEmpty(PhotonNetwork.NickName))
        //    playerName.text = AuthManager.myUserName;
        //else
        //    playerName.text = photonView.Owner.NickName;
    }
}
