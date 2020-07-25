using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Extensions;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class GameChecker : MonoBehaviour
{
    private MainScript mainScript;
    private DatabaseReference databaseRef;

    private string hostName;

    public string resumeText;
    public string roomCodePref;
    public string playersText;
    public Sprite phaseImage;


    public GameChecker()
    {

    }

    public void BeginChecking(MainScript mainScript, DatabaseReference databaseRef, string roomCodePref)
    {
        this.mainScript = mainScript;
        this.databaseRef = databaseRef;
        this.roomCodePref = roomCodePref;

        this.resumeText = "RESUME GAME " + RichColour(roomCodePref, "yellow") + "?";

        CheckRegister();
    }

    void CheckRegister()
    {
        Debug.Log("CheckRegister: CALLED");

        databaseRef.Child("RoomsRegister/" + roomCodePref).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            CheckRegisterResult(task);
        });
    }

    private void CheckRegisterResult(Task<DataSnapshot> task)
    {
        Debug.Log("CheckRegisterResult: CALLED");

        if (!task.IsFaulted && task.IsCompleted)
        {
            if (task != null && task.Result != null && task.Result.Value != null)
            {
                // Room exists
                GetRoom();
            }
            else
            {
                mainScript.OnGameExistenceConfirmed(false);
            }
        }
    }


    void GetRoom()
    {
        Debug.Log("GetRoom: CALLED");

        databaseRef.Child("Rooms/" + roomCodePref).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            try
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    mainScript.OnGameExistenceConfirmed(false);
                }
                else if (task.IsCompleted)
                {
                    GetRoomResult(task.Result);
                }
                else
                {
                    mainScript.OnGameExistenceConfirmed(false);
                }
            }
            catch
            {
                mainScript.OnGameExistenceConfirmed(false);
            }
        });
    }

    public Room room;

    private void GetRoomResult(DataSnapshot result)
    {
        Debug.Log("GetRoomResult: CALLED");

        try
        {
            // DESERIALISE ROOM
            this.room = JsonConvert.DeserializeObject<Room>(result.GetRawJsonValue());
            this.hostName = room.hostName;
            Dictionary<string,string> players = room.players;

            // CREATE PLAYERS LIST
            this.playersText = "Host: " + RichColour(this.hostName, "yellow") + "\n"
                + "Playing: ";

            foreach (string player in players.Values)
            {
                if (player != this.hostName)
                {
                    this.playersText = this.playersText + player + ", ";
                }
            }
            this.playersText = this.playersText.Substring(0, this.playersText.Length - 2);

            // GET IMAGE FOR PHASE
            string p = room.phase;
            GetImageForPhase(p);
            

            Success();

        }
        catch
        {
            mainScript.OnGameExistenceConfirmed(false);
        }
    }

    private void GetImageForPhase(string p)
    {
        switch (p)
        {
            default:
                this.phaseImage = null;
                break;

        }
    }

    void GetPhase()
    {
        Debug.Log("GetPhase: CALLED");

        databaseRef.Child("Rooms/" + roomCodePref + "/phase").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            try
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    mainScript.OnGameExistenceConfirmed(false);
                }
                else if (task.IsCompleted)
                {
                    GetPhaseResult(task.Result);
                }
                else
                {
                    mainScript.OnGameExistenceConfirmed(false);
                }
            }
            catch
            {
                mainScript.OnGameExistenceConfirmed(false);
            }
        });
    }

    void GetPhaseResult(DataSnapshot result)
    {
        Debug.Log("GetPhaseResult: CALLED");

        try
        {
            string p = JsonConvert.DeserializeObject<string>(result.GetRawJsonValue());

           

        }
        catch
        {
            mainScript.OnGameExistenceConfirmed(false);
        }
    }

    private void Success()
    {
        Debug.Log("Success: CALLED");

        // SUCCESS EXIT POINT
        mainScript.OnGameExistenceConfirmed(true);
    }

    string RichColour(string text, string colour)
    {
        return "<color=" + colour + ">" + text + "</color>";
    }
}