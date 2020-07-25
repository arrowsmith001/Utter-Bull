using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

using Firebase;
using Firebase.Database;
using Firebase.Analytics;
using Firebase.Unity.Editor;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Linq;
using Firebase.Extensions;

public class RoomChanger : MonoBehaviour
{
    public bool isActive;

    public DatabaseReference roomRef;

    public MainScript main;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void Activate(DatabaseReference roomRef)
	{
        this.roomRef = roomRef;
        isActive = true;

        Debug.Log("RoomChanger ACTIVE: Roomref non-null: " + (roomRef != null));
	}

    public void Deactivate()
	{
        this.roomRef = null;
        isActive = false;
	}

    public void ChangeHost(string newHostName, Fragment fragment)
    {
        roomRef.Child("hostName").SetValueAsync(newHostName).ContinueWithOnMainThread(task =>
        {
            fragment.RoomChangeResult("ChangeHost", task, task.IsCompleted && !task.IsFaulted, newHostName);
        });
    }

    public void ChangeTimer(int mins, Fragment fragment)
    {
        roomRef.Child("settings").Child("rtm").SetValueAsync(mins).ContinueWithOnMainThread(task =>
        {

            fragment.RoomChangeResult("ChangeTimer", task, task.IsCompleted && !task.IsFaulted, mins);
        });
    }

    public void ChangeRdiReady(bool? b, Fragment fragment)
    {
        roomRef.Child("rdiReady").SetValueAsync(b).ContinueWithOnMainThread(task =>
        {

            fragment.RoomChangeResult("ChangeRdiReady", task, task.IsCompleted && !task.IsFaulted, b);
        });
    }

    public void SetAllTrueEnabled(bool b, Fragment fragment)
    {
        // bool newToggleState = !main.rl.room.settings.ate;

        roomRef.Child("settings").Child("ate").SetValueAsync(b).ContinueWithOnMainThread(task =>
        {

            fragment.RoomChangeResult("ToggleAllTrueEnabled", task, task.IsCompleted && !task.IsFaulted, b);
        });
    }

    public void SetLewdEnabled(bool b, Fragment fragment)
    {
        //bool newToggleState = !main.rl.room.settings.le;

        roomRef.Child("settings").Child("le").SetValueAsync(b).ContinueWithOnMainThread(task =>
        {

            fragment.RoomChangeResult("ToggleLewdEnabled", task, task.IsCompleted && !task.IsFaulted, b);
        });
    }

    public void ChangeScores(Dictionary<string, int> newScores, bool updated, Fragment fragment)
    {
        Dictionary<string, System.Object> updates = new Dictionary<string, System.Object>();

        updates.Add("scores/playerScores", newScores);
        updates.Add("scores/updated", updated);

        roomRef.UpdateChildrenAsync(updates).ContinueWithOnMainThread(task =>
        {
            fragment.RoomChangeResult("ChangeScores", task, task.IsCompleted && !task.IsFaulted, newScores);
        });

    }

    public void ChangePhase(string newPhase, Fragment fragment) // Simultaneously un-readies everyone
    {
        int j = 0;

        Dictionary<string, System.Object> updates = new Dictionary<string, System.Object>();

        Dictionary<string, bool> newReadies = new Dictionary<string, bool>();
    
        foreach (string key in main.rl.room.playerReadies.Keys)
		{
            newReadies.Add(key, false);
        }

        updates.Add("phase", newPhase);
        updates.Add("playerReadies", newReadies);

        roomRef.UpdateChildrenAsync(updates).ContinueWithOnMainThread(task =>
        {
            j++;
            Debug.Log(j);
            // SET PHASE PREF HERE
            if (task.IsCompleted && !task.IsFaulted) main.SavePrefString("statePref",newPhase);

            j++;
            Debug.Log(j);
            fragment.RoomChangeResult("ChangePhase_"+newPhase, task, task.IsCompleted && !task.IsFaulted, newPhase);
        });
    }

    public void SetReadyStatus(bool isReady, Fragment fragment)
    {
        roomRef.Child("playerReadies").Child(main.playerName).SetValueAsync(isReady).ContinueWithOnMainThread(task =>
        {
            fragment.RoomChangeResult("ChangeReadyStatus", task, task.IsCompleted && !task.IsFaulted, isReady);
        });
    }

    public void ChangeRevealsPage(int[] index, Fragment fragment)
    {
        Dictionary<string, System.Object> updates = new Dictionary<string, System.Object>();

        updates.Add("revealsPageIndex", index);

        roomRef.UpdateChildrenAsync(updates).ContinueWithOnMainThread(task =>
        {
            if (fragment != null) fragment.RoomChangeResult("ChangeRevealsPage", task, task.IsCompleted && !task.IsFaulted, index);
        });
    }

    public void ChangeTextEntry(string text, string playerName, Fragment fragment)
    {
        Debug.Log(playerName + " TEXT CHANGED TO " + text);

        roomRef.Child("playerTexts").Child(playerName).SetValueAsync(text).ContinueWithOnMainThread(task =>
        {
            fragment.RoomChangeResult("ChangeTextEntry", task, task.IsCompleted && !task.IsFaulted, text);
        });
    }

    public void AddVote(string vote, int time, Fragment fragment)
    {
        Debug.Log(main.playerName + " VOTE ADDED: " + vote + " in " + time);

        string voteKey = roomRef.Child("playerVotes/" + main.playerName + "/").Push().Key;
        string voteDataJson = JsonConvert.SerializeObject(new VoteData(vote, time));

        roomRef.Child("playerVotes/" + main.playerName + "/" + voteKey).SetRawJsonValueAsync(voteDataJson).ContinueWithOnMainThread(task =>
        {
            Dictionary<string, System.Object> updates = new Dictionary<string, System.Object>();
            updates.Add("playerReadies/" + main.playerName, true);

            roomRef.UpdateChildrenAsync(updates).ContinueWithOnMainThread(task2 =>
            {
                fragment.RoomChangeResult("AddVote_" + vote, task2, task2.IsCompleted && !task2.IsFaulted, voteKey);

            });
        });
    }

    public void ChangeTruth(bool truth, string playerName, Fragment fragment)
    {
        roomRef.Child("playerTruths").Child(playerName).SetValueAsync(truth).ContinueWithOnMainThread(task =>
        {
            fragment.RoomChangeResult("ChangeTruth", task, task.IsCompleted && !task.IsFaulted, truth);
        });
    }

    public void AddRolesAndOrder(Dictionary<string,bool?> playerTruths,Dictionary<string, string> playerTargets, Fragment fragment)
    {
        Dictionary<string, System.Object> updates = new Dictionary<string, System.Object>();

        updates.Add("playerTruths", playerTruths);
        updates.Add("playerTargets", playerTargets);

        roomRef.UpdateChildrenAsync(updates).ContinueWithOnMainThread(task =>
        {

            fragment.RoomChangeResult("AddPlayerRoles", task, task.IsCompleted && !task.IsFaulted, updates);
        });

    }


    internal void SetNewTurn(int? nextTurn, Fragment fragment)
    {

        Debug.Log("SETNEWTURN");
        foreach (Player p in main.data.playersLeftToPlay)
        {
            Debug.Log("playersLeftToPlay: " + p.playerName);
        }
        foreach (Player p in main.data.playersWhovePlayed)
        {
            Debug.Log("playersWhovePlayed: " + p.playerName);
        }

        if (nextTurn == null)
        {
            fragment.RoomChangeResult("SetNewTurn", null, true, null);
        }
        else
        {
            roomRef.Child("order/turnNum").SetValueAsync(nextTurn).ContinueWithOnMainThread(task =>
            {
                fragment.RoomChangeResult("SetNewTurn", task, task.IsCompleted && !task.IsFaulted, nextTurn);
            });

            // TODO: Set and return new player
        }

    }

    public void ResetVars(Fragment fragment)
    {

        Dictionary<string, System.Object> updates = new Dictionary<string, System.Object>();

        updates.Add("playerTruths", null);
        updates.Add("playerTargets", null);
        updates.Add("playerTexts", null);
        updates.Add("playerTimes", null);
        updates.Add("playerVotes", null);
        updates.Add("revealsPageIndex", null);
        updates.Add("scores/updated", false);


        roomRef.UpdateChildrenAsync(updates).ContinueWithOnMainThread(task =>
        {

            fragment.RoomChangeResult("ResetVars", task, task.IsCompleted && !task.IsFaulted, updates);
        });

        main.SavePrefString("TurnStartTime", "");
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    internal void SetTurnTimeStart(DateTime dt, Fragment fragment)
    {
        Dictionary<string, System.Object> updates = new Dictionary<string, System.Object>();

        updates.Add("turnTimeStart", dt.ToString());

        roomRef.UpdateChildrenAsync(updates).ContinueWithOnMainThread(task =>
        {
            if (fragment != null) fragment.RoomChangeResult("SetTurnTimeStart", task, task.IsCompleted && !task.IsFaulted, dt);
        });
    }

    internal void AddPlayerOrder(List<string> players, Fragment fragment)
    {
        Dictionary<string, System.Object> updates = new Dictionary<string, System.Object>();

        List<string> shuffledList = players.OrderBy(x => UnityEngine.Random.value).ToList();

        updates.Add("order/playerOrder", shuffledList);
        updates.Add("order/turnNum", 0);

        roomRef.UpdateChildrenAsync(updates).ContinueWithOnMainThread(task =>
        {
            if (fragment != null) fragment.RoomChangeResult("AddPlayerOrder", task, task.IsCompleted && !task.IsFaulted, updates);
        });
    }
}
