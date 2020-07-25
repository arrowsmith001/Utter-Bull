using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Extensions;
using Newtonsoft.Json;
using UnityEngine;

public class MembershipChecker : MonoBehaviour
{
    public static string MEMBER_TRUE = "t";
    public static string MEMBER_UNKNOWN = "u";
    public static string MEMBER_FALSE = "f";

    private MainScript mainScript;
    private DatabaseReference databaseRef;
    private string deviceUniqueIdentifier;

    public bool isMember;

    public MembershipChecker(){}

    public void BeginChecking(MainScript mainScript, DatabaseReference databaseRef, string deviceUniqueIdentifier)
    {
        this.mainScript = mainScript;
        this.databaseRef = databaseRef;
        this.deviceUniqueIdentifier = deviceUniqueIdentifier;

        Debug.Log("MEMBERSHIP CHECKER START");
        databaseRef.Child("Members").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if(task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
            {
                SearchMembers(task);
            }
        });
    }

    private void SearchMembers(Task<DataSnapshot> task)
    {
        try
        {
            if (task == null || task.Result == null || task.Result.Value == null)
            {
                this.isMember = false;
                mainScript.OnMembershipConfirmed();
            }
            else
            {
                Dictionary<string, string> members
                    = JsonConvert.DeserializeObject<Dictionary<string, string>>(task.Result.GetRawJsonValue());

                this.isMember = members.ContainsValue(deviceUniqueIdentifier);

                mainScript.OnMembershipConfirmed();

            }
        }catch(Exception e) { Debug.Log("SEARCH MEMBERS: " + e.Message); }
    }
}
