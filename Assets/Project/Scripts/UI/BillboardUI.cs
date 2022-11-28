using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    [SerializeField]
    PhotonView photonView;

    Transform cam;

    private void Start()
    {
        cam = Camera.main.transform;
    }

    private void Update()
    {
        if (photonView.IsMine)
            return;

        if (cam)
        {
            Vector3 targetPostition = new Vector3(cam.position.x,
                                       this.transform.position.y,
                                       cam.position.z);
            transform.LookAt(targetPostition);
            transform.Rotate(Vector3.up * 180);
        }
    }
}
