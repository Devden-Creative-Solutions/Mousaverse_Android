using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DisplayNameLookAt : MonoBehaviour
{
    public static Transform cameraTransform;
    [SerializeField] PhotonView photonView;

    void Start()
    {
        photonView = transform.parent.parent.GetComponent<PhotonView>();
        if (photonView.IsMine)
        {
            cameraTransform = Camera.main.transform;
        }
    }
    
    void Update()
    {
        var lookPos = transform.position - cameraTransform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 10);
    }


}
