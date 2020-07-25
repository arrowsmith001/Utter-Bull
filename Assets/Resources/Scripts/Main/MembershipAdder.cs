using System;
using System.Collections.Generic;
using Firebase.Database;
using UnityEngine;

public class MembershipAdder : MonoBehaviour
{
    private MainScript mainScript;
    private string deviceUniqueIdentifier;
    private DatabaseReference databaseRef;


    public bool success;

    public MembershipAdder() { }

    public void BeginAdding(MainScript mainScript, DatabaseReference databaseRef, string deviceUniqueIdentifier)
    {
        this.mainScript = mainScript;
        this.databaseRef = databaseRef;
        this.deviceUniqueIdentifier = deviceUniqueIdentifier;

        string key = databaseRef.Child("Members").Push().Key;

        databaseRef.Child("Members/" + key).SetValueAsync(this.deviceUniqueIdentifier).ContinueWith(task =>
        {
                this.success = task.IsCompleted && !task.IsFaulted && !task.IsCanceled;
            
                mainScript.OnMemberAdded();
           
        });
    }
}