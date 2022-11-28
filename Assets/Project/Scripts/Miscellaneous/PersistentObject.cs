using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PersistentObject : MonoBehaviour
{
    public PhotonView photonView;
    public GameObject arFace;

    private void Start()
    {
        if (!photonView.IsMine)
        {
            DisableOtherUserThings();
            return;
        }

        //if (PlayerInfoDontDestroy.Instance.currentPlayerType == PlayerType.Singer)
        //    arFace.SetActive(true);

        arFace.SetActive(false);

        var sceneChangeController = SceneChangeController.Instance;

        sceneChangeController.AddPlayerPun(photonView);
        sceneChangeController.TurnOffScreenFade();
    }

    void DisableOtherUserThings()
    {
        var player = transform.GetChild(0);
        player.GetComponent<CharacterController>().enabled = false;
        player.GetComponent<Character>().enabled = false;
        player.GetComponent<CharacterAnimator>().enabled = false;
        player.GetComponent<JoystickMovement>().enabled = false;
        player.GetComponent<NavMeshAgent>().enabled = false;

    }
}
