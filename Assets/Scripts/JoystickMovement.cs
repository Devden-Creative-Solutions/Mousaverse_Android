using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Gui;
using Photon.Pun;

public class JoystickMovement : MonoBehaviour
{
    [SerializeField] LeanJoystick leanJoystick;
    [SerializeField] LeanJoystick leanJoystickCamera;
    [SerializeField] Character character;
    [SerializeField] CharacterController characterController;
    [SerializeField] GameObject FPSVirtualCam;
    [SerializeField] GameObject _mainCamera;
    PhotonView photonView;
    Vector3 moveDirection = Vector3.zero;

    [SerializeField] float _horizontalCamMove;
    [SerializeField] float _verticalCamMove;
    private void Start()
    {
        FPSVirtualCam.SetActive(false);
        photonView = transform.parent.GetComponent<PhotonView>();
        _horizontalCamMove = 1;
        _verticalCamMove = 1;
        character = GetComponent<Character>();
        characterController = GetComponent<CharacterController>();
        leanJoystick = UIElements.Instance.leanJoystick;
        leanJoystickCamera = UIElements.Instance.leanJoystickCamera;
        CameraSensitivity.changingVerticalSensitivity += SetVerticalSensitivity;
        CameraSensitivity.changingHorizontalSensitivity += SetHorizontalSensitivity;
        UIElements.Instance.enableTheVirtualFPSCamera += SetCamera;
    }

    private void Update()
    {
        SetMovementVector();
        SetCameraVector();
    }
    void SetMovementVector()
    {

        if (leanJoystick.ScaledValue.x > .5)
        {
            PlayerInput._horizontalAxisJS = 1;
        }

        else if(leanJoystick.ScaledValue.x < -.5)
        {
            PlayerInput._horizontalAxisJS = -1;
        }
        else
        {
            PlayerInput._horizontalAxisJS = 0;
        }


        if (leanJoystick.ScaledValue.y > .5)
        {
            PlayerInput._verticalAxisJS = 1;
        }

        else if (leanJoystick.ScaledValue.y < -.5)
        {
            PlayerInput._verticalAxisJS = -1;
        }
        else
        {
            PlayerInput._verticalAxisJS = 0;
        }
    }

    void SetCameraVector()
    {

        if (leanJoystickCamera.ScaledValue.x > .5)
        {
            PlayerInput._horizontalAxisCam = _horizontalCamMove;
        }

        else if (leanJoystickCamera.ScaledValue.x < -.5)
        {
            PlayerInput._horizontalAxisCam = -(_horizontalCamMove);
        }
        else
        {
            PlayerInput._horizontalAxisCam = 0;
        }


        if (leanJoystickCamera.ScaledValue.y > .5)
        {
            PlayerInput._verticalAxisCam = _verticalCamMove;
        }

        else if (leanJoystickCamera.ScaledValue.y < -.5)
        {
            PlayerInput._verticalAxisCam = -(_verticalCamMove);
        }
        else
        {
            PlayerInput._verticalAxisCam = 0;
        }
    }

    //Control Camera Sensitivity
    void SetHorizontalSensitivity(float sensi)
    {
        _horizontalCamMove = sensi;
    }

    void SetVerticalSensitivity(float sensi)
    {
        _verticalCamMove = sensi;
    }


    //First Person Camera Section

    void SetCamera(bool canEnable)
    {
        if (photonView.IsMine)
        {
            FPSVirtualCam.SetActive(canEnable);
            _mainCamera.SetActive(!canEnable);
        }
    }
}
