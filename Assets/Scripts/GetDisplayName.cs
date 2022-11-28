using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Firebase.Auth;


public class GetDisplayName : MonoBehaviour
{
    [SerializeField]
    TMP_Text displayName;
    [SerializeField]
    PhotonView photonView;
    FirebaseUser user;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        ChangeDisplayName();
    }


    private void Start()
    {

        //photonView = GetComponent<PhotonView>();
        //photonView.RPC("ChangeDisplayName",RpcTarget.All);
        //ChangeDisplayName();
    }

    void ChangeDisplayName()
    {
        if (photonView.IsMine)
        {
            var auth = FirebaseAuth.DefaultInstance;
            user = auth.CurrentUser;
            photonView.Owner.NickName = user.DisplayName;
        }
        displayName.transform.parent.gameObject.SetActive(false);
        Invoke("DelayedSetName", 1f);
    }

    void DelayedSetName()
    {
        displayName.text = photonView.Owner.NickName;
        displayName.transform.parent.gameObject.SetActive(true);
    }


}
