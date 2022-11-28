using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class SpirintButtonSupporter : SprintControlWrapper
{

    PhotonView photonView;
    [SerializeField] Character character;

    private void Start()
    {
        photonView = transform.parent.GetComponent<PhotonView>();

        if (!photonView.IsMine)
            return;

        character = GetComponent<Character>();
        sprintingButton.pointerDown += DoSprint;
    }

    private void OnDestroy()
    {
        if (!photonView.IsMine)
            return;
        sprintingButton.pointerDown -= DoSprint;
    }

    public void DoSprint(bool val)
    {
        if (photonView.IsMine)
            character.IsSprinting = val;
    }
}
