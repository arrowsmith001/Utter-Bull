using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

using Firebase;
using Firebase.Database;
using Firebase.Analytics;
using Firebase.Unity.Editor;
using Newtonsoft.Json;
using Firebase.Extensions;

public class GameJoiner : MonoBehaviour
{
    private MainScript main;
    private DatabaseReference databaseRef;
    private string playerName;

    public string gameCode;

    public string failureMessage = "";

    public GameJoiner()
    {

    }


    public void JoinGame(MainScript main, DatabaseReference databaseRef, string playerName, string gameCode)
    {
        this.main = main;
        this.databaseRef = databaseRef;
        this.playerName = playerName;
        this.gameCode = gameCode;

        try
        {
            GetRoomRegister();
        }
        catch{ UnknownError();}
    }

    void GetRoomRegister()
    {
        // Get roomsManifest etc (search: getting firebase rtd values)
        //databaseRef ...

        databaseRef.Child("RoomsRegister").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            try
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    failureMessage = "Failed to find room list.";
                    main.OnGameJoinComplete(failureMessage);
                }
                else if (task.IsCompleted)
                {
                    main.GameJoinUpdate("Searching rooms");
                    CheckRoomRegister(task.Result.Children);
                }
                else
                {
                    UnknownError();
                }
        }
        catch { UnknownError(); }
    });
    }


    void CheckRoomRegister(IEnumerable<DataSnapshot> roomsList)
    {
        try { 
        // This just transfers the room names from the DataSnapshot to a string list, this fixed a problem in the Java version
        List<string> roomNamesList = new List<string>();
        if (roomsList != null)
        {
            foreach (DataSnapshot room in roomsList)
            {
                //Debug.Log("ROOM NAME ADDED TO LIST: " + room.Key);
                roomNamesList.Add(room.Key);
            }
        }

        if (roomNamesList.Count == 0)
        {
            failureMessage = "Room not found.";
                main.OnGameJoinComplete(failureMessage);
            }
        else
        {
            bool roomFound = false;

            foreach (string roomName in roomNamesList)
            {
                if (string.Equals(roomName, gameCode)) roomFound = true;
            }

            if (!roomFound)
            {
                failureMessage = "Room not found.";
                    main.OnGameJoinComplete(failureMessage);
                }
            else
            {
                main.GameJoinUpdate("Room found!");

                GetRoomPhase();
            }
        }

        }
        catch { UnknownError(); }

    }

    void GetRoomPhase()
    {
        databaseRef.Child("Rooms/" + gameCode + "/phase").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            try
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    failureMessage = "Failed to determine game phase";
                    main.OnGameJoinComplete(failureMessage);
                }
                else if (task.IsCompleted)
                {
                    main.GameJoinUpdate("Checking room");

                    CheckRoomPhase(task.Result);
                }
                else
                {
                    UnknownError();
                }
        }
        catch { UnknownError(); }
    });
    }

    void CheckRoomPhase(DataSnapshot result)
    {
        try
        {
            string phaseName = JsonConvert.DeserializeObject<string>(result.GetRawJsonValue());

            if (phaseName == "Lobby")
            {
                GetPlayersList();
            }
            else
            {
                failureMessage = "Game is already in session";
                main.OnGameJoinComplete(failureMessage);
            }
        }
        catch{ UnknownError();
}
    }

    void GetPlayersList()
	{

		databaseRef.Child("Rooms/" + gameCode + "/players").GetValueAsync().ContinueWithOnMainThread(task =>
			{
                try
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        failureMessage = "Failed to find room list";
                        main.OnGameJoinComplete(failureMessage);
                    }
                    else if (task.IsCompleted)
                    {
                        main.GameJoinUpdate("Checking room");

                        CheckPlayersRegister(task.Result);
                    }
                    else
                    {
                        UnknownError();
                    }


                }
                catch { UnknownError(); }
            });
	}

	void CheckPlayersRegister(DataSnapshot playersSnapshot)
	{
        try
        {
            Dictionary<string, string> playersList = JsonConvert.DeserializeObject<Dictionary<string, string>>(playersSnapshot.GetRawJsonValue());
            bool dupeFound = false;

            foreach (string player in playersList.Values)
            {
                //Debug.Log(player);

                if (string.Equals(player, playerName))
                {
                    dupeFound = true;
                }
            }

            if (dupeFound)
            {
                this.failureMessage = "Sorry, there's already someone named " + playerName + " in that session.";
                main.OnGameJoinComplete(failureMessage);
            }
            else
            {
                JoinRoom();
            }

        }
        catch { UnknownError(); }
    }


	void JoinRoom()
    {
        try
        {
            Dictionary<string, System.Object> updates = new Dictionary<string, System.Object>();

            string key = databaseRef.Child("Rooms/" + gameCode + "/players").Push().Key;

            //updates.Add("Rooms/" + gameCode + "/players", playerName);
            updates.Add("Rooms/" + gameCode + "/players/" + key, playerName);
            updates.Add("Rooms/" + gameCode + "/scores/playerScores/" + playerName, 0);
            updates.Add("Rooms/" + gameCode + "/playerReadies/" + playerName, false);

            databaseRef.UpdateChildrenAsync(updates).ContinueWithOnMainThread(task =>
                    {
                        try
                        {
                            if (task.IsFaulted || task.IsCanceled)
                            {
                                failureMessage = "Failed to join room.";
                                main.OnGameJoinComplete(failureMessage);
                            }
                            else if (task.IsCompleted)
                            {
                                // DownloadRoom();

                                main.OnGameJoinComplete(null);
                            }
                            else
                            {
                                UnknownError();
                            }

                        }
                        catch { UnknownError(); }
                    });

        }
        catch { UnknownError(); }
    }


    void UnknownError()
    {
            failureMessage = "Unknown error, retry";
        main.OnGameJoinComplete(failureMessage);
    }

 //   void DownloadRoom()
	//{
 //       databaseRef.Child("Rooms/" + gameCode).GetValueAsync().ContinueWith(task =>
 //         {
 //             if (task.IsFaulted)
 //             {
 //                 failureMessage = "Failed to fetch room.";
 //                 main.OnGameJoinComplete(this, false);
 //             }
 //             else if (task.IsCompleted)
 //             {
 //                 room = JsonConvert.DeserializeObject<Room>(task.Result.GetRawJsonValue());

 //                 main.OnGameJoinComplete(this, true);
 //             }
 //         });


 //   }

}
