using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkPlayerSpawner : MonoBehaviourPunCallbacks
{
    private GameObject spawnedPlayerPrefab;

    public MiniMapController miniMapController;

    public Sprite playerMinimapIndicator;

    [SerializeField] GetRoomList getRoomList;



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
        new Vector3(transform.position.x + Random.Range(0, 5), transform.position.y, transform.position.z), transform.rotation);



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

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        if (spawnedPlayerPrefab)
            PhotonNetwork.Destroy(spawnedPlayerPrefab);
    }




}
