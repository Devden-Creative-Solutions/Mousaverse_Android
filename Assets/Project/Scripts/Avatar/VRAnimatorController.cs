using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VRAnimatorController : MonoBehaviour
{
    //public PhotonView photonView;
    //public float speedThreshold;
    //public float smoothing;

    //Animator animator;
    //Vector3 previousPos;
    //AvatarController avatarController;

    //// Start is called before the first frame update
    //void Start()
    //{
    //    animator = GetComponent<Animator>();

    //    //if (!photonView.IsMine)
    //        //animator.enabled = false;

    //    avatarController = GetComponent<AvatarController>();
    //    previousPos = avatarController.head.vrTarget.position;
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    if (!photonView.IsMine)
    //        return;

    //    Vector3 headsetSpeed = (avatarController.head.vrTarget.position - previousPos) / Time.deltaTime;
    //    headsetSpeed.y = 0;

    //    Vector3 headsetLocalSpeed = transform.InverseTransformDirection(headsetSpeed);
    //    previousPos = avatarController.head.vrTarget.position;


    //    float previousDirectionX = animator.GetFloat("DirectionX");
    //    float previousDirectionY = animator.GetFloat("DirectionY");


    //    animator.SetBool("isMoving", headsetLocalSpeed.magnitude > speedThreshold);
    //    animator.SetFloat("DirectionX", Mathf.Lerp(previousDirectionX,Mathf.Clamp(headsetLocalSpeed.x,-1,1),smoothing));
    //    animator.SetFloat("DirectionY", Mathf.Lerp(previousDirectionY, Mathf.Clamp(headsetLocalSpeed.z,-1,1),smoothing));
    //}
}
