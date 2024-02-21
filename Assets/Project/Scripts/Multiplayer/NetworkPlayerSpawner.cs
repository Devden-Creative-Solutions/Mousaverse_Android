using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using static CharacterAnimator;

public class NetworkPlayerSpawner : MonoBehaviourPunCallbacks
{
    private GameObject spawnedPlayerPrefab;

    public MiniMapController miniMapController;

    public Sprite playerMinimapIndicator;

    [SerializeField] GetRoomList getRoomList;

    CharacterAnimator characterAnim;

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        string NetworkPlayerName = "";
        if (PlayerInfoDontDestroy.Instance != null)
        {
            switch (PlayerInfoDontDestroy.Instance._selectedGender)
            {
                case Gender.Male:
                    NetworkPlayerName = "MaleAvatar";

                    break;

                case Gender.Female:
                    NetworkPlayerName = "FemaleAvatar";

                    break;
            }
            //NetworkPlayerName = PlayerInfoDontDestroy.Instance.SelectedAvatarName;
        }
        else
        {
            NetworkPlayerName = "Network Player";
        }


        SpawnPlayer(NetworkPlayerName);
    }

    void SpawnPlayer(string NetworkPlayerName)
    {
        spawnedPlayerPrefab = PhotonNetwork.Instantiate("NetworkAvatars\\" + NetworkPlayerName,
        new Vector3(transform.position.x + UnityEngine.Random.Range(0, 5), transform.position.y, transform.position.z), transform.rotation);
        characterAnim = spawnedPlayerPrefab.GetComponentInChildren<CharacterAnimator>();

           


        Player networkPlayer = PhotonNetwork.LocalPlayer;
        networkPlayer.TagObject = spawnedPlayerPrefab;

        var player = spawnedPlayerPrefab.transform.GetChild(0);
        miniMapController.target = player;

        var minimapComp = player.gameObject.AddComponent<MiniMapComponent>();
        minimapComp.enabled = false;
        minimapComp.rotateWithObject = true;
        minimapComp.icon = playerMinimapIndicator;

        miniMapController.enabled = true;
        minimapComp.enabled = true;

    }


    public void playemote(int I)
    {
      switch (I)
        {
            case 0: characterAnim.SelectAndPlayGreetings(GreetingStyle.Salute);
                break;
            case 1: characterAnim.SelectAndPlayGreetings(GreetingStyle.Hi);
                break;
            case 2: characterAnim.SelectAndPlayGreetings(GreetingStyle.ShakeHands);
                break;
            case 3: characterAnim.SelectAndPlayGreetings(GreetingStyle.BreakDance);
                break;
            default:
                characterAnim.SelectAndPlayGreetings(GreetingStyle.Hi);
                break;
        }
    }




    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        if (spawnedPlayerPrefab)
            PhotonNetwork.Destroy(spawnedPlayerPrefab);
    }




}
