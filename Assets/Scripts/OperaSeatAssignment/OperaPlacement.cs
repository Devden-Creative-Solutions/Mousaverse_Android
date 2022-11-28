using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperaPlacement : MonoBehaviour
{
    #region singleton


    private static OperaPlacement _instance;

    public static OperaPlacement Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    #endregion


    [SerializeField] List<OperaSeat> _operaSeats1;
    [SerializeField] List<OperaSeat> _operaSeats2;
    [SerializeField] List<OperaSeat> _tempSeat;

    public List<SingerSeat> singerOperaSeats;

    [SerializeField] GameObject _operaSinger1;
    [SerializeField] GameObject _operaSinger2;

    [SerializeField] PhotonView photonView;
    public delegate void GettingOutofOpera();
    public static GettingOutofOpera gettingOutOfOpera;

    [SerializeField] string currentSingerPrimary;
    [SerializeField] string currentSingerSecondary;
    [SerializeField] GameObject currentSingerGO;
    [SerializeField] Recorder voiceRecorder;

    [SerializeField] GameObject _primaryOperaVCAM;
    [SerializeField] GameObject _secondaryOperaVCAM;
    [SerializeField] GameObject _stageViewTogglePrimary;
    [SerializeField] GameObject _stageViewToggleSecondary;

    [SerializeField] List<Player> connectedPlayerList = new List<Player>();
    [SerializeField] List<GameObject> photonPlayerGOList = new List<GameObject>();

    [SerializeField] GameObject mutePlayerPrefab;
    [SerializeField] Transform contentMutePlayers;


    public void PlacePlayerInOpera(GameObject player, OperaHouseID operaHouseID = OperaHouseID.OperaHouse1)
    {
        switch (operaHouseID)
        {
            case OperaHouseID.OperaHouse1:
                _tempSeat = _operaSeats1;
                break;

            case OperaHouseID.OperaHouse2:
                _tempSeat = _operaSeats2;
                break;
        }

        if (isFullHouse(_tempSeat))
        {
            print("There are not Empty Seats!!"); // Handle Full seats here
        }
        else
        {
            int randomSeat;
            randomSeat = GetSeat(_tempSeat);
            if (randomSeat != -1)
            {
                player.transform.position = _tempSeat[randomSeat].transform.position;
                player.transform.localRotation = _tempSeat[randomSeat].transform.localRotation;

                var assignedSeat = player.GetComponent<AssignSeat>();

                assignedSeat.SetAssignedSeat(_tempSeat[randomSeat]);
                assignedSeat.StartSittingAnimation();
                _tempSeat[randomSeat].TakeSeat();
            }
        }
    }



    public void TryingToGetPlayers()
    {
        photonPlayerGOList.Clear();
        ClearContent();
        print("Trying to get players:___ -__ _-__-_--__--______");
        var photonViews = UnityEngine.Object.FindObjectsOfType<PhotonView>();
        foreach (var view in photonViews)
        {
            if(view.transform.childCount > 0)
            {
                AssignSeat assignSeat;
                if (view.transform.GetChild(0).TryGetComponent<AssignSeat>(out assignSeat))
                {
                    print("The name of the object is : - " + view.gameObject.name);
                    photonPlayerGOList.Add(view.gameObject);
                    PopulateMutePlayerList(view.Owner.NickName.ToString(), view.gameObject);
                }
            }

        }
    }

    void ClearContent()
    {
        foreach(Transform x in contentMutePlayers.transform)
        {
            Destroy(x.gameObject);
        }
    }

    void PopulateMutePlayerList(string playerName, GameObject playerGO)
    {
        var GO = Instantiate(mutePlayerPrefab, contentMutePlayers);
        GO.GetComponent<MutePlayers>().SetThePlayerGameObjectAndName(playerName, playerGO);
    }

    public void SetPrimaryStageView()
    {
        SetVirtualCameraInOperaHouse(OperaHouseID.OperaHouse1);
    }

    public void SetSecondaryStageView()
    {
        SetVirtualCameraInOperaHouse(OperaHouseID.OperaHouse2);
    }

    public void SetVirtualCameraInOperaHouse(OperaHouseID operaHouseID)
    {
        switch (operaHouseID)
        {
            case OperaHouseID.OperaHouse1:
                _primaryOperaVCAM.SetActive(true);
                break;

            case OperaHouseID.OperaHouse2:
                _secondaryOperaVCAM.SetActive(true);
                break;
        }
    }

    public void DisableAllVirtualCamera()
    {
        _primaryOperaVCAM.SetActive(false);
        _secondaryOperaVCAM.SetActive(false);
    }

    public void EnableOrDisableStageViewUI(OperaHouseID operaHouseID,bool value)
    {

        switch (operaHouseID)
        {
            case OperaHouseID.OperaHouse1:
                _stageViewTogglePrimary.SetActive(value);
                break;

            case OperaHouseID.OperaHouse2:
                _stageViewToggleSecondary.SetActive(value);
                break;
        }
        
    }



    public void ClearTheFirebaseSingerSeat()
    {
        singerOperaSeats[0].ClearFirebaseSingerSeat();
    }

    public void PlaceSingerInOpera(GameObject player, OperaHouseID operaHouseID = OperaHouseID.OperaHouse1)
    {

        player.transform.position = singerOperaSeats[(int)operaHouseID - 1].transform.position;
        player.transform.localRotation = singerOperaSeats[(int)operaHouseID - 1].transform.localRotation;
        singerOperaSeats[(int)operaHouseID - 1].TakeSeat();
        singerOperaSeats[(int)operaHouseID - 1].SetSinger(player.transform.parent.GetComponent<PhotonView>());
        player.GetComponent<AssignSeat>().SetAssignedSingerSeat(singerOperaSeats[(int)operaHouseID - 1]);

       
        photonView.RPC(nameof(DisableOperaSingerForAllPlayers), RpcTarget.AllBuffered, operaHouseID);
        //photonView.RPC("SetSinger", RpcTarget.AllBuffered, player.GetComponent<AssignSeat>().PlayerUserID);


            string userID = PhotonNetwork.LocalPlayer.UserId;
            photonView.RPC("SetSinger", RpcTarget.AllBuffered,userID,operaHouseID);

    }


    [PunRPC]
    void SetSinger(string playerID, OperaHouseID operaHouseID)
    {
        switch (operaHouseID)
        {
            case OperaHouseID.OperaHouse1:
                currentSingerPrimary = playerID;
                break;

            case OperaHouseID.OperaHouse2:
                currentSingerSecondary = playerID;
                break;
        }

    }


    public void OnSingerDisconnect(string playerID)
    {
        photonView.RPC(nameof(SingerDisconnectHandling), RpcTarget.AllBuffered, playerID);
    }

    [PunRPC]
    void SingerDisconnectHandling(string playerID)
    {
        print("The player with ID: " + playerID + " has disconnected");
        SingerSeat localSingerSeat;

        if(playerID == currentSingerPrimary)
        {
            localSingerSeat = singerOperaSeats[0];
        }
        else if(playerID == currentSingerSecondary)
        {
            localSingerSeat = singerOperaSeats[1];
        }

        var thePlayers = PhotonNetwork.PlayerList;

        if (singerOperaSeats[0].GetSeatStatus())
        {
            int count = 0;
            foreach (var x in thePlayers)
            {
                if(x.UserId == currentSingerPrimary )
                {
                    count++;
                }
            }

            if (count == 0 && playerID == currentSingerPrimary)
            {
                singerOperaSeats[0].ClearSeat();
            }
        }

        if (singerOperaSeats[1].GetSeatStatus())
        {
            int count = 0;
            foreach (var x in thePlayers)
            {
                if (x.UserId == currentSingerSecondary )
                {
                    count++;
                }
            }

            if (count == 0 && playerID == currentSingerSecondary)
            {
                singerOperaSeats[1].ClearSeat();
            }
        }

    }


    public void EnableOperaSingerAfterPlayerSingerExit(OperaHouseID operaHouseID)
    {
        photonView.RPC(nameof(EnableOperaSingerForAllPlayers), RpcTarget.AllBuffered, operaHouseID);
    }

    [PunRPC]
    void DisableOperaSingerForAllPlayers(OperaHouseID operaHouseID)
    {
        switch (operaHouseID)
        {
            case OperaHouseID.OperaHouse1:
                _operaSinger1.SetActive(false);
                break;

            case OperaHouseID.OperaHouse2:
                _operaSinger2.SetActive(false);
                break;
        }
    }

    [PunRPC]
    void EnableOperaSingerForAllPlayers(OperaHouseID operaHouseID)
    {
        switch (operaHouseID)
        {
            case OperaHouseID.OperaHouse1:
                _operaSinger1.SetActive(true);
                break;

            case OperaHouseID.OperaHouse2:
                _operaSinger2.SetActive(true);
                break;
        }
    }

    public void MuteGuest()
    {
        //photonView.RPC(nameof(Mute), RpcTarget.AllBuffered, playerID);
        voiceRecorder.TransmitEnabled = false;
    }

    public void UnMuteGuest()
    {
        voiceRecorder.TransmitEnabled = true;
    }

    [PunRPC]
    void Mute(string PlayerID)
    {
        foreach(var x in connectedPlayerList)
        {
            print("user ID: --- " + (string)x.UserId);
        }
        var allPlayersInNetwork = PhotonNetwork.PlayerList;

        foreach (Player x in allPlayersInNetwork)
        {

            if (x.UserId == PlayerID)
            {

                GameObject go = (GameObject)x.TagObject;
                print("Gameobject: " + go.name);
                go.transform.GetChild(0).gameObject.GetComponent<LipSync>().playerAudioSource.mute = true;
                print("The user Gameobject is:--" + go.name);
            }
        }
        //photonView.gameObject.transform.GetChild(0).GetComponent<LipSync>().playerAudioSource.mute = true;
        //Player player = PhotonNetwork.LocalPlayer;

    }

    public void AddPlayerToConnectList(Player player)
    {
        photonView.RPC(nameof(AddPlayerToList), RpcTarget.AllBuffered, player);
    }

    [PunRPC]
    void AddPlayerToList(Player player)
    {
        connectedPlayerList.Add(player);
    }

    public void AddPhotonPlayerWithGameobject(GameObject playerGO)
    {
        photonPlayerGOList.Add(playerGO);
    }



    public void PlayerGetOut()
    {
        gettingOutOfOpera.Invoke();
    }

    int GetSeat(List<OperaSeat> operaSeats)
    {
        int randomSeat = Random.Range(0, operaSeats.Count);
        if (operaSeats[randomSeat].GetSeatStatus())
        {
            GetSeat(operaSeats);
            return -1;
        }
        else
        {
            return randomSeat;
        }
    }

    bool isFullHouse(List<OperaSeat> operaSeats)
    {
        int i = 0;
        foreach (var x in operaSeats)
        {
            if (x.GetSeatStatus())
            {
                i++;
            }
        }

        if (i == operaSeats.Count)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}

public enum OperaHouseID
{
    Nothing,
    OperaHouse1,
    OperaHouse2
}

public enum PlayerType
{
    Guest,
    Singer
}
