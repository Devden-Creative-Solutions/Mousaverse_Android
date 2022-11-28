//using Firebase;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseDependencies : GenericSingletonClass<FirebaseDependencies>
{
    //public DependencyStatus dependencyStatus;

    public Action OnFirebaseIntialized;

    // Start is called before the first frame update
    void Start()
    {
        //FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        //{
        //    dependencyStatus = task.Result;

        //    if (dependencyStatus == DependencyStatus.Available)
        //    {
        //        OnFirebaseIntialized?.Invoke();
        //    }
        //    else
        //    {
        //        Debug.Log("Firebase not intialized properly");
        //    }
        //});
    }


}
