using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using Firebase.Database;
using Photon.Pun;

public class SetOperaHouseVariableByRoom : MonoBehaviour
{
    FirebaseUser user;
    FirebaseAuth auth;
    DatabaseReference DBreference;
    [SerializeField] GetRoomList roomList;
    List<string> tempRoomListName = new List<string>();

    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
       
    }

    public void UpdateOperaHouseVariables()
    {
        StartCoroutine(UpdateOperaHouseDatabase());

    }

    public void DeleteOperaHouseVariables()
    {
        StartCoroutine(GetListOfRoomsInFirebase(roomList._roomList.Count));
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

    private IEnumerator DeleteOperaHouseDatabase(string roomName)
    {
        var DBTask = DBreference.Child("OperaHouses_ByRooms").Child(PlayerInfoDontDestroy.Instance.GetSelectedRegionCode()).Child(roomName).RemoveValueAsync();
        yield return new WaitUntil(() => DBTask.IsCompleted);

        if(DBTask.Exception != null)
        {
            Debug.LogError(message: $"Failed to register task with {DBTask.Exception}");
        }

        else
        {
            print("Database Opera house has been deleted");
        }
    }

    private IEnumerator GetListOfRoomsInFirebase(int numberOfRoom)
    {
        print("Getting List of rooms from firebase");
        var DBTask = DBreference.Child("OperaHouses_ByRooms").Child(PlayerInfoDontDestroy.Instance.GetSelectedRegionCode()).GetValueAsync();
        yield return new WaitUntil(() => DBTask.IsCompleted);

        if(DBTask.Exception != null)
        {
            Debug.LogError(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;
            if(snapshot != null)
            {
                tempRoomListName.Clear();
                foreach (var x in snapshot.Children)
                {
                    print(x.Key.ToString());
                    tempRoomListName.Add(x.Key);
                }

                if(numberOfRoom == 0)
                {
                    CheckCurrentRoomAndDeleteOthers();
                }
                else if(numberOfRoom > 0)
                {
                    CompareRoomsAndThenDelete();
                }
                
            }

        }
    }

    private void CheckCurrentRoomAndDeleteOthers()
    {
        tempRoomListName.Remove(PhotonNetwork.CurrentRoom.Name);

        foreach(var x in tempRoomListName)
        {
            StartCoroutine(DeleteOperaHouseDatabase(x));
        }
    }

    private void CompareRoomsAndThenDelete()
    {
        foreach(var x in roomList._roomList)
        {
            tempRoomListName.Remove(x);
        }

        foreach (var x in tempRoomListName)
        {
            StartCoroutine(DeleteOperaHouseDatabase(x));
        }
    }


    
}


