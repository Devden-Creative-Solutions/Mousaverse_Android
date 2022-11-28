using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VRMap
{
    //public Transform vrTarget;
    //public Transform rigTarget;

    //public Vector3 trackingPosOffset;
    //public Vector3 trackingRotOffset;

    //public Vector3 handTrackingPosOffset;
    //public Vector3 handTrackingRotOffset;

    //public void Map()
    //{
    //    var isHandTrackingEnabled = OVRPlugin.GetHandTrackingEnabled();

    //    var posOffset = isHandTrackingEnabled ? handTrackingPosOffset : trackingPosOffset;
    //    var rotOffset = isHandTrackingEnabled ? handTrackingRotOffset : trackingRotOffset;

    //    rigTarget.position = vrTarget.TransformPoint(posOffset);
    //    rigTarget.rotation = vrTarget.rotation * Quaternion.Euler(rotOffset);
    //}
}

public class AvatarController : MonoBehaviour
{
    //public Renderer[] meshesToDisableInBody;

    //public VRMap head;
    //public VRMap leftHand;
    //public VRMap rightHand;

    //public Transform headConstraint;
    //public Vector3 headBodyOffset;

    //public float headSmoothingTime;

    //public PhotonView photonView;

    //private void Start()
    //{
    //    if (photonView.IsMine)
    //    {
    //        for (int i = 0; i < meshesToDisableInBody.Length; i++)
    //        {
    //            meshesToDisableInBody[i].enabled = false;
    //        }

    //        OVRManager vRManager = FindObjectOfType<OVRManager>();

    //        head.vrTarget = vRManager.transform.Find("TrackingSpace/CenterEyeAnchor");
    //        leftHand.vrTarget = vRManager.transform.Find("TrackingSpace/LeftHandAnchor");
    //        rightHand.vrTarget = vRManager.transform.Find("TrackingSpace/RightHandAnchor");

    //        headBodyOffset = transform.position - headConstraint.position;
    //        //headBodyOffset = new Vector3(0, -1.88f, 0);
    //    }
    //}

    //// Update is called once per frame
    //void FixedUpdate()
    //{
    //    if (photonView.IsMine)
    //    {
    //        transform.position = headConstraint.position + headBodyOffset;
    //        transform.forward = Vector3.Lerp(transform.forward, Vector3.ProjectOnPlane(headConstraint.up, Vector3.up).normalized, headSmoothingTime);

    //        head.Map();
    //        leftHand.Map();
    //        rightHand.Map();
    //    }
    //}
}
