using Lean.Gui;
using Lean.Transition;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeController : GenericSingletonClass<SceneChangeController>
{

    public Action OnTeleportedTo;

    PhotonView playerPhotonView;
    Character playerTransform;

    [SerializeField] CanvasGroup MiniMapCanvasGroup;

    public VoiceChatController voiceChatController;

    public LeanToggle screenFader;

    public int playerTeleportScene;

    public Transform[] teleportPoints;

    public GameObject[] environments;

    public Transform operaHouse1Focus;
    public Transform operaHouse2Focus;

    private GameObject currentEnvironment;

    private void Start()
    {
        currentEnvironment = environments[0];
    }

    public void TurnOnScreenFade() => screenFader.TurnOn();
    public void TurnOffScreenFade() => screenFader.TurnOff();

    public void PingPongScreenFade()
    {
        screenFader.TurnOn();
        transform.EventTransition(() =>
        {
            screenFader.TurnOff();
        }, 0.9f);
    }

    public void AddPlayerPun(PhotonView photonView)
    {
        playerPhotonView = photonView;
        playerTransform = photonView.transform.GetChild(0).GetComponent<Character>();
    }

    public void TeleportTo(int _playerTeleportScene, bool _operaEntrance)
    {
        if (!playerPhotonView.IsMine)
            return;

        playerTeleportScene = _playerTeleportScene;

        TurnOnScreenFade();

        transform.EventTransition(() =>
        {
            if (playerPhotonView.IsMine)
            {
                currentEnvironment.SetActive(false);
                environments[playerTeleportScene].SetActive(true);
                currentEnvironment = environments[playerTeleportScene];

                MiniMapCanvasGroup.alpha = 0;
                playerTransform.gameObject.SetActive(false);
                //playerTransform.transform.position = teleportPoints[playerTeleportScene].position;
                //playerTransform.transform.rotation = teleportPoints[playerTeleportScene].rotation;
                playerTransform.GetComponent<Character>().enabled = false;
                playerTransform.GetComponent<CharacterAnimator>().canWalk = false;



                if (playerTransform.GetComponent<AssignSeat>().currentPlayerType == PlayerType.Guest)
                {
                    playerTransform.GetComponent<AssignSeat>().GetSeatInOpera((OperaHouseID)playerTeleportScene);
                    //playerTransform.GetComponent<AssignSeat>().MutePlayer();                                         // If you want to mute the Guests when inside opera house

                    OperaPlacement.Instance.EnableOrDisableStageViewUI((OperaHouseID)playerTeleportScene, true);

                    //Automatically switching camera to stage view which is not needed by client
                    //transform.EventTransition(() =>
                    //{
                    //   OperaPlacement.Instance.SetVirtualCameraInOperaHouse((OperaHouseID)playerTeleportScene);

                    //},2f);


                    //playerTransform.GetComponent<Animator>().enabled = false;


                    transform.EventTransition(() =>
                    {

                        //playerTransform.GetComponent<CharacterAnimator>().canWalk = true;
                    }, .4f);
                    transform.JoinTransition();
                    transform.EventTransition(() =>
                    {
                        //playerTransform.GetComponent<Character>().MoveVector = new Vector3(-.1f, 0f, -1f);
                        //playerTransform.GetComponent<Character>().enabled = true;

                        var tempCam = playerTransform.GetComponent<AssignSeat>()._mainCamera;
                        CameraController camController = tempCam.GetComponent<CameraController>();
                        switch ((OperaHouseID)playerTeleportScene)
                        {

                            case OperaHouseID.OperaHouse1:

                                camController.useInputForRotation = false;

                                Vector3 v3_Dir = operaHouse1Focus.position - tempCam.transform.position;
                                v3_Dir.y = 0;
                                var rotation = Quaternion.LookRotation(v3_Dir);

                                var lookAngle = FromQ2(rotation);
                                PlayerInput.tiltAngle = 0;
                                PlayerInput.lookAngle = lookAngle.y;

                                camController.useInputForRotation = true;
                                voiceChatController.ChangeAudioGroup(21);

                                print("Rotate the camera for Primary Opera");
                                break;

                            case OperaHouseID.OperaHouse2:
                                camController.useInputForRotation = false;

                                Vector3 v3_Dir2 = operaHouse2Focus.position - tempCam.transform.position;
                                v3_Dir2.y = 0;
                                var rotation2 = Quaternion.LookRotation(v3_Dir2);

                                var lookAngle2 = FromQ2(rotation2);

                                PlayerInput.tiltAngle = 0;
                                PlayerInput.lookAngle = lookAngle2.y;
                                //Debug.Log(Vector3.Angle(tempCam.transform.position, operaHouse2Focus.position));
                                camController.useInputForRotation = true;
                                voiceChatController.ChangeAudioGroup(22);
                                print("Rotate the camera for Primary Opera");
                                break;
                        }

                    }, .4f);

                }
                else
                {
                    //Place the singer here
                    playerTransform.GetComponent<AssignSeat>().GetSingerSeat((OperaHouseID)playerTeleportScene);
                    transform.EventTransition(() =>
                    {

                        playerTransform.GetComponent<CharacterAnimator>().canWalk = true;
                    }, .4f);
                    transform.JoinTransition();
                    transform.EventTransition(() =>
                    {
                        playerTransform.GetComponent<Character>().MoveVector = new Vector3(-.1f, 0f, -1f);
                        playerTransform.GetComponent<Character>().enabled = true;

                        CameraController camController = playerTransform.GetComponent<AssignSeat>()._mainCamera.GetComponent<CameraController>();
                        switch ((OperaHouseID)playerTeleportScene)
                        {

                            case OperaHouseID.OperaHouse1:

                                camController.useInputForRotation = false;
                                PlayerInput.tiltAngle = 0;
                                PlayerInput.lookAngle = -180;
                                camController.useInputForRotation = true;
                                voiceChatController.ChangeAudioGroup(21);


                                print("Rotate the camera for Primary Opera");
                                break;

                            case OperaHouseID.OperaHouse2:
                                camController.useInputForRotation = false;
                                PlayerInput.tiltAngle = 0;
                                PlayerInput.lookAngle = -180;
                                camController.useInputForRotation = true;
                                print("Rotate the camera for Primary Opera");
                                voiceChatController.ChangeAudioGroup(22);
                                break;
                        }

                    }, .4f);


                }
                playerTransform.GetComponent<AssignSeat>().SetAssignedOperaHouse((OperaHouseID)playerTeleportScene);



                if (_operaEntrance)
                    OnTeleportedTo?.Invoke();

                Invoke("DelayedSwitchScene", 0.1f);
            }
        }, 0.5f);

        transform.EventTransition(() => playerTransform.GetComponent<AssignSeat>()._mainCamera.GetComponent<AudioListener>().enabled = true, .8f);
    }

    public static Vector3 FromQ2(Quaternion q1)
    {
        float sqw = q1.w * q1.w;
        float sqx = q1.x * q1.x;
        float sqy = q1.y * q1.y;
        float sqz = q1.z * q1.z;
        float unit = sqx + sqy + sqz + sqw; // if normalised is one, otherwise is correction factor
        float test = q1.x * q1.w - q1.y * q1.z;
        Vector3 v;

        if (test > 0.4995f * unit)
        { // singularity at north pole
            v.y = 2f * Mathf.Atan2(q1.y, q1.x);
            v.x = Mathf.PI / 2;
            v.z = 0;
            return NormalizeAngles(v * Mathf.Rad2Deg);
        }
        if (test < -0.4995f * unit)
        { // singularity at south pole
            v.y = -2f * Mathf.Atan2(q1.y, q1.x);
            v.x = -Mathf.PI / 2;
            v.z = 0;
            return NormalizeAngles(v * Mathf.Rad2Deg);
        }
        Quaternion q = new Quaternion(q1.w, q1.z, q1.x, q1.y);
        v.y = (float)Math.Atan2(2f * q.x * q.w + 2f * q.y * q.z, 1 - 2f * (q.z * q.z + q.w * q.w));     // Yaw
        v.x = (float)Math.Asin(2f * (q.x * q.z - q.w * q.y));                             // Pitch
        v.z = (float)Math.Atan2(2f * q.x * q.y + 2f * q.z * q.w, 1 - 2f * (q.y * q.y + q.z * q.z));      // Roll
        return NormalizeAngles(v * Mathf.Rad2Deg);
    }

    static Vector3 NormalizeAngles(Vector3 angles)
    {
        angles.x = NormalizeAngle(angles.x);
        angles.y = NormalizeAngle(angles.y);
        angles.z = NormalizeAngle(angles.z);
        return angles;
    }

    static float NormalizeAngle(float angle)
    {
        while (angle > 360)
            angle -= 360;
        while (angle < 0)
            angle += 360;
        return angle;
    }

    public void TeleportOut(int _playerTeleportScene, bool _operaEntrance)
    {
        if (!playerPhotonView.IsMine)
            return;

        playerTeleportScene = _playerTeleportScene;

        TurnOnScreenFade();

        transform.EventTransition(() =>
        {
            if (playerPhotonView.IsMine)
            {
                currentEnvironment.SetActive(false);
                environments[playerTeleportScene].SetActive(true);
                currentEnvironment = environments[playerTeleportScene];

                playerTransform.GetComponent<Animator>().enabled = true;
                playerTransform.gameObject.SetActive(false);
                playerTransform.transform.position = teleportPoints[playerTeleportScene].position;
                playerTransform.transform.rotation = teleportPoints[playerTeleportScene].rotation;
                playerTransform.GetComponent<Character>().enabled = true;
                playerTransform.GetComponent<CharacterAnimator>().canWalk = true;
                //playerTransform.GetComponent<CharacterAnimator>().enabled = true;

                MiniMapCanvasGroup.alpha = 1;
                var assignSeat = playerTransform.GetComponent<AssignSeat>();

                if (assignSeat.currentPlayerType == PlayerType.Guest)
                {
                    assignSeat.UnMutePlayer();
                    OperaPlacement.Instance.DisableAllVirtualCamera();
                    OperaPlacement.Instance.EnableOrDisableStageViewUI((OperaHouseID)playerTeleportScene, false);

                }
                else if (assignSeat.currentPlayerType == PlayerType.Singer)
                {
                    OperaPlacement.Instance.EnableOperaSingerAfterPlayerSingerExit(assignSeat.GetAssignedOperaHouseID());
                    playerTransform.GetComponent<AssignSeat>().ClearAssignedSeats();
                }

                voiceChatController.ChangeAudioGroup(20);

                if (_operaEntrance)
                    OnTeleportedTo?.Invoke();

                Invoke("DelayedSwitchScene", 0.1f);
            }
        }, 0.5f);
    }

    void DelayedSwitchScene()
    {
        if (playerPhotonView.IsMine)
        {
            //playerTransform.GetComponent<CharacterAnimator>().enabled = true;
            playerTransform.gameObject.SetActive(true);


        }

        transform.EventTransition(() =>
        {
            TurnOffScreenFade();

        }, 0.8f);
    }

    //public void TeleportTo(int _playerTeleportScene)
    //{
    //    if (!playerPhotonView.IsMine)
    //        return;

    //    playerTeleportScene = _playerTeleportScene;

    //    StartCoroutine(LoadYourAsyncScene());
    //}

    //IEnumerator LoadYourAsyncScene()
    //{
    //    AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(playerTeleportScene);

    //    // Wait until the asynchronous scene fully loads
    //    while (!asyncLoad.isDone)
    //    {
    //        yield return null;
    //    }

    //    playerPhotonView.RPC("update_player_inGame", RpcTarget.All);
    //}

    //[PunRPC]
    //public void update_player_inGame()
    //{

    //    if (!playerPhotonView.IsMine)// this ensures only objects with this id in other client's machine will be deleted
    //    {
    //        playerTransform.gameObject.SetActive(false);
    //        playerCam.SetActive(false);
    //    }
    //}
}

public enum InputMode
{
    KEYBOARD,
    TOUCH
}
