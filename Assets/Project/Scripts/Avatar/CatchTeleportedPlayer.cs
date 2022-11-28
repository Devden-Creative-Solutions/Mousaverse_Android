using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchTeleportedPlayer : MonoBehaviour
{
    public static GameObject playerObj;

    private void OnEnable()
    {
        if (!playerObj)
            return;

        if (!playerObj.transform.parent.GetComponent<PhotonView>().IsMine)
            return;

        playerObj.transform.position = transform.position;
        playerObj.transform.rotation = transform.rotation;

        playerObj.GetComponent<Character>().enabled = true;
        playerObj = null;
    }
}
