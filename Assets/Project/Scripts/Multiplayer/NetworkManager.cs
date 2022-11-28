using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Lean.Gui;
using UnityEngine.SceneManagement;
using Photon.Voice.PUN;
using TMPro;
using Lean.Transition;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] LeanToggle joystickToggle;
    [SerializeField] ServerSettings serverSettings;
    [SerializeField] SetOperaHouseVariableByRoom setOperaHouseVariableByRoom;

    [SerializeField] GetRoomList getRoomList;
    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();

    public TextMeshProUGUI muteStatusText;
    bool isMuted;
    int roomCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        ConnectToServer();
    }

    
    void JoinTheLobby()
    {
        TypedLobby typedLobby = new TypedLobby(PlayerInfoDontDestroy.Instance.GetSelectedRegionCode(), LobbyType.Default);
        PhotonNetwork.JoinLobby(typedLobby);
    }

    void ConnectToServer()
    {

        serverSettings.AppSettings.FixedRegion = PlayerInfoDontDestroy.Instance.GetSelectedRegionCode();
        serverSettings.EnableSupportLogger = true;
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Trying to connect");

    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        JoinTheLobby();

    }

    void JoinTheRoom()
    {
        Debug.Log("Connected to server");


        RoomOptions roomOptions = new RoomOptions();

        roomOptions.MaxPlayers = 20;
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.PublishUserId = true;
        //PhotonNetwork.JoinOrCreateRoom("Room 1", roomOptions, TypedLobby.Default);
        
        PhotonNetwork.JoinRandomOrCreateRoom(null, 20, MatchmakingMode.FillRoom, TypedLobby.Default, null, null, roomOptions);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a room");
        base.OnJoinedRoom();
        
        print("The name of room is: " + PhotonNetwork.CurrentRoom.Name);

        joystickToggle.TurnOn();

        print("Count of players: " + PhotonNetwork.CountOfPlayers);
        if (PhotonNetwork.CountOfPlayers == 1)
        {
            OperaPlacement.Instance.ClearTheFirebaseSingerSeat();
           // setOperaHouseVariableByRoom.UpdateOperaHouseVariables();
            
        }
        this.transform.EventTransition(()=>OperaPlacement.Instance.TryingToGetPlayers(),1f);
        OperaPlacement.Instance.AddPlayerToConnectList(PhotonNetwork.LocalPlayer);
        setOperaHouseVariableByRoom.DeleteOperaHouseVariables();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("Player entered the room " + newPlayer.NickName);
        this.transform.EventTransition(() => OperaPlacement.Instance.TryingToGetPlayers(), 1f);
        base.OnPlayerEnteredRoom(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        string userID = otherPlayer.UserId;
        OperaPlacement.Instance.OnSingerDisconnect(userID);
        print("The user: " + otherPlayer.UserId + " has disconnected");
        this.transform.EventTransition(() => OperaPlacement.Instance.TryingToGetPlayers(), 1f);

    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel(0);
        base.OnLeftRoom();
    }

    public void LeaveTheRoom()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
    }

    public void MuteOtherPlayers()
    {
        isMuted = !isMuted;

        muteStatusText.text = "Muted : " + isMuted.ToString();


        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            ToggleMutePlayer(PhotonNetwork.PlayerList[i], isMuted);
        }
    }

    public bool ToggleMutePlayer(Player player, bool mute)
    {
        if (player != null)
        {

            int actorNr = player.ActorNumber;
            for (int viewId = actorNr * PhotonNetwork.MAX_VIEW_IDS + 1; viewId < (actorNr + 1) * PhotonNetwork.MAX_VIEW_IDS; viewId++)
            {
                PhotonView photonView = PhotonView.Find(viewId);
                if (photonView /*&& (photonView.OwnerActorNr == actorNr || photonView.ControllerActorNr == actorNr)*/)
                {
                    PhotonVoiceView photonVoiceView = photonView.GetComponent<PhotonVoiceView>();
                    if (photonVoiceView && photonVoiceView.IsSpeaker)
                    {
                        AudioSource audioSource = photonVoiceView.SpeakerInUse.GetComponent<AudioSource>();
                        audioSource.mute = mute;
                    }
                }
            }
        }
        return false;
    }


    //------------------Get Room List----------------
    private void UpdateCahcedRoomList(List<RoomInfo> roomList)
    {
        print("Getting the room list-----");
        print("The count of room list is--- " + roomList.Count);
        roomCount = roomList.Count;
        for (int i = 0; i < roomList.Count; i++)
        {
            RoomInfo info = roomList[i];
            if (info.RemovedFromList)
            {
                cachedRoomList.Remove(info.Name);
            }
            else
            {
                cachedRoomList[info.Name] = info;
                print("Room List Names: " + info);
            }
        }

        getRoomList._roomList.Clear();
        foreach (var x in cachedRoomList)
        {
            getRoomList._roomList.Add(x.Value.Name);
        }

        
    }

    public override void OnJoinedLobby()
    {
        cachedRoomList.Clear();
        JoinTheRoom();
        print("Joined a lobby");

    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UpdateCahcedRoomList(roomList);
    }

    public override void OnLeftLobby()
    {
        cachedRoomList.Clear();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        cachedRoomList.Clear();
    }
    //----------------------------------------------------------
}
