using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

using Photon.Pun;

public class NetworkPlayer : MonoBehaviour
{
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;
    PhotonView photonView;

    Transform headRig;
    Transform leftHandRig;
    Transform rightHandRig;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        //OVRManager vRManager = FindObjectOfType<OVRManager>();

        //headRig = vRManager.transform.Find("TrackingSpace/CenterEyeAnchor");
        //leftHandRig = vRManager.transform.Find("TrackingSpace/LeftHandAnchor");
        //rightHandRig = vRManager.transform.Find("TrackingSpace/RightHandAnchor");
    }

    // Update is called once per frame
    void Update()
    {
        //if (photonView.IsMine)
        //{
        //    head.gameObject.SetActive(false);
        //    leftHand.gameObject.SetActive(false);
        //    rightHand.gameObject.SetActive(false);

        //    MapPosition(head, headRig);
        //    MapPosition(leftHand, leftHandRig);
        //    MapPosition(rightHand, rightHandRig);
        //}
    }

    void MapPosition(Transform target, Transform node)
    {
        target.position = node.position;
        target.rotation = node.rotation;
    }
}
