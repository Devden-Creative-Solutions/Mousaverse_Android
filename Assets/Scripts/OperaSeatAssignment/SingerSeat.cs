using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Firebase.Auth;
using Firebase.Database;

public class SingerSeat : MonoBehaviour
{
    [SerializeField]
    private bool isTaken = false;

    [SerializeField]
    PhotonView singerPhotonView;

    FirebaseUser user;
    FirebaseAuth auth;
    DatabaseReference DBreference;

    [SerializeField] OperaHouseID currentOperaHouseID;

    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;

        DBreference = FirebaseDatabase.DefaultInstance.RootReference;


        StartCoroutine(WaitForCurrentRoom());
    }

    IEnumerator WaitForCurrentRoom()
    {
        yield return new WaitUntil(() => PhotonNetwork.CurrentRoom != null);
        DelayedSubscribing();
    }
    
    void DelayedSubscribing()
    {
        switch (currentOperaHouseID)
        {
            case OperaHouseID.OperaHouse1:
                DBreference.Child("OperaHouses_ByRooms").Child(PlayerInfoDontDestroy.Instance.GetSelectedRegionCode()).Child(PhotonNetwork.CurrentRoom.Name).Child("primary").ValueChanged += GetPrimaryOperaSeatStatus;
                StartCoroutine(GetPrimaryOperaSingerSeat());
                break;

            case OperaHouseID.OperaHouse2:
                DBreference.Child("OperaHouses_ByRooms").Child(PlayerInfoDontDestroy.Instance.GetSelectedRegionCode()).Child(PhotonNetwork.CurrentRoom.Name).Child("secondary").ValueChanged += GetSecondarySeatStatus;
                StartCoroutine(GetSecondaryOperaSingerSeat());
                break;
        }
    }

    public void ClearFirebaseSingerSeat()
    {
        StartCoroutine(UpdateOperaHouseDatabase());
    }

    public void SetSinger(PhotonView photonView)
    {
        singerPhotonView = photonView;
    }

    public PhotonView GetSinger()
    {
        return singerPhotonView;
    }


    private IEnumerator SetPrimaryOperaSingerSeat(bool value)
    {
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
        var DBTask = DBreference.Child("OperaHouses_ByRooms").Child(PlayerInfoDontDestroy.Instance.GetSelectedRegionCode()).Child(PhotonNetwork.CurrentRoom.Name).Child("primary").SetValueAsync(value);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogError(message: $"Failed to register Task with {DBTask.Exception}");
        }

        else
        {
            print("Primary Opera House Seat Taken");

        }
    }

    private IEnumerator SetSecondaryOperaSingerSeat(bool value)
    {
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
        var DBTask = DBreference.Child("OperaHouses_ByRooms").Child(PlayerInfoDontDestroy.Instance.GetSelectedRegionCode()).Child(PhotonNetwork.CurrentRoom.Name).Child("secondary").SetValueAsync(value);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogError(message: $"Failed to register Task with {DBTask.Exception}");
        }

        else
        {
            print("Secondary Opera House Seat Taken");

        }
    }

    private IEnumerator UpdateOperaHouseDatabase()
    {
        var DBTask = DBreference.Child("OperaHouses_ByRooms").Child(PlayerInfoDontDestroy.Instance.GetSelectedRegionCode()).Child(PhotonNetwork.CurrentRoom.Name).Child("primary").SetValueAsync(false);
        var DBTask2 = DBreference.Child("OperaHouses_ByRooms").Child(PlayerInfoDontDestroy.Instance.GetSelectedRegionCode()).Child(PhotonNetwork.CurrentRoom.Name).Child("secondary").SetValueAsync(false);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted && DBTask2.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogError(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            print("database OperaHouse is now updated");
        }
    }

    void GetPrimaryOperaSeatStatus(object sender,ValueChangedEventArgs args)
    {
        StartCoroutine(GetPrimaryOperaSingerSeat());
    }

    private IEnumerator GetPrimaryOperaSingerSeat()
    {
        var DBTask = DBreference.Child("OperaHouses_ByRooms").Child(PlayerInfoDontDestroy.Instance.GetSelectedRegionCode()).Child(PhotonNetwork.CurrentRoom.Name).Child("primary").GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogError(message: $"Failed to register Task with {DBTask.Exception}");
        }

        else
        {
            DataSnapshot snapshot = DBTask.Result;
            bool isSeatTaken;
            if (snapshot.Value != null)
            {
                bool.TryParse(snapshot.Value.ToString(), out isSeatTaken);
                isTaken = isSeatTaken;

            }
            else
            {
              
            }


        }
    }

    void GetSecondarySeatStatus(object sender,ValueChangedEventArgs valueChangedEventArgs)
    {
        StartCoroutine(GetSecondaryOperaSingerSeat());
    }

    private IEnumerator GetSecondaryOperaSingerSeat()
    {
        var DBTask = DBreference.Child("OperaHouses_ByRooms").Child(PlayerInfoDontDestroy.Instance.GetSelectedRegionCode()).Child(PhotonNetwork.CurrentRoom.Name).Child("secondary").GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogError(message: $"Failed to register Task with {DBTask.Exception}");
        }

        else
        {
            DataSnapshot snapshot = DBTask.Result;
            bool isSeatTaken;
            if (snapshot.Value != null)
            {
                bool.TryParse(snapshot.Value.ToString(), out isSeatTaken);

                isTaken = isSeatTaken;
            }
            else
            {

            }


        }
    }

    public void TakeSeat()
    {
        switch (currentOperaHouseID)
        {
            case OperaHouseID.OperaHouse1:
                StartCoroutine(SetPrimaryOperaSingerSeat(true));
                break;

            case OperaHouseID.OperaHouse2:
                StartCoroutine(SetSecondaryOperaSingerSeat(true));
                break;
        }
    }


    public void ClearSeat()
    {
        switch (currentOperaHouseID)
        {
            case OperaHouseID.OperaHouse1:
                StartCoroutine(SetPrimaryOperaSingerSeat(false));
                break;

            case OperaHouseID.OperaHouse2:
                StartCoroutine(SetSecondaryOperaSingerSeat(false));
                break;
        }
    }



    /// <summary>
    /// If return true then the seat is taken and if false then seat is empty
    /// </summary>
    /// <returns>bool</returns>
    public bool GetSeatStatus()
    {
        if (isTaken)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnDestroy()
    {
        switch (currentOperaHouseID)
        {
            case OperaHouseID.OperaHouse1:
                DBreference.Child("OperaHouses_ByRooms").Child(PlayerInfoDontDestroy.Instance.GetSelectedRegionCode()).Child(PhotonNetwork.CurrentRoom.Name).Child("primary").ValueChanged -= GetPrimaryOperaSeatStatus;
                break;

            case OperaHouseID.OperaHouse2:
                DBreference.Child("OperaHouses_ByRooms").Child(PlayerInfoDontDestroy.Instance.GetSelectedRegionCode()).Child(PhotonNetwork.CurrentRoom.Name).Child("secondary").ValueChanged -= GetSecondarySeatStatus;
                break;
        }
    }
}
