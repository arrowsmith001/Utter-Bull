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
using Firebase.Extensions;

public class GameCreator : MonoBehaviour
{
    public int ATTEMPTS_ALLOWED = 100;

    private MainScript main;
    private DatabaseReference databaseRef;
    private string playerName;
    private GameSettings gameSettings;

    public Room room;
    public string failureMessage = "";

    public GameCreator()
	{

	}

    public GameCreator(string failureMessage)
	{
        this.failureMessage = failureMessage;
	}

    public void CreateGame(MainScript main, DatabaseReference databaseRef, string playerName, GameSettings gameSettings)
    {
        this.main = main;
        this.databaseRef = databaseRef;
        this.playerName = playerName;
        this.gameSettings = gameSettings;

        //if(main.debugModeOn)
        //{
        //    Debug.Log("GameCreator: DEBUG MODE");
        //    FabricateRoom();
        //}
        //else
        //{
        GetRoomRegister();
        //}

    }

    void GetRoomRegister()
	{
        // Get roomsManifest etc (search: getting firebase rtd values)
        //databaseRef ...

        databaseRef.Child("RoomsRegister").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                failureMessage = "Failed to find room list";
                main.OnGameCreationComplete(failureMessage);
            }
            else if (task.IsCompleted)
            {
                main.GameCreationUpdate("Generating game code");
                GenerateValidCode(task.Result.Children);
            }
        });
    }

    void GenerateValidCode(IEnumerable<DataSnapshot> roomsList)
    {
        try
        {

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

            // Game Code generation objects
            string gameCode = "";
            string abc = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string num = "0123456789";
            System.Random rand = new System.Random();

            // While loop break conditions
            bool codeGot = false;
            int attemptsLeft = ATTEMPTS_ALLOWED; // ATTEMPTS_ALLOWED is a field set to 100

            // WHILE LOOP BEGINS
            while (!codeGot && attemptsLeft > 0)
            {
                // Generates a 5 digit code
                gameCode = "";
                gameCode = gameCode + abc[(int)(rand.NextDouble() * 26)];
                gameCode = gameCode + abc[(int)(rand.NextDouble() * 26)];
                gameCode = gameCode + abc[(int)(rand.NextDouble() * 26)];
                gameCode = gameCode + num[(int)(rand.NextDouble() * 10)];
                gameCode = gameCode + num[(int)(rand.NextDouble() * 10)];

                // DEBUG PURPOSES ONLY - Forces code to be equal to the only existing code on database
                if (main.debugModeOn) gameCode = "ABC99";

                //Debug.Log("THIS CODE: " + gameCode);

                // If no rooms, then there's no possibility of conflict so we just continue...
                if (roomNamesList.Count == 0)
                {
                    codeGot = true;
                }
                else // If there are rooms registered, we must look for a conflict...
                {
                    bool conflictFound = false;

                    foreach (string roomName in roomNamesList)
                    {
                        //Debug.Log("ROOM BEING TESTED FOR MATCH: "+roomName);

                        // If there exists any room with the code generated, conflict found set to true
                        if (string.Equals(roomName, gameCode)) conflictFound = true;

                        //Debug.Log("CONFLICTFOUND?: " + conflictFound.ToString()); // This will be true every time
                    }

                    // If no conflict was found, we've got our code
                    if (!conflictFound) codeGot = true;
                }

                attemptsLeft--;

                //Debug.Log("ATTEMPTS LEFT: "+attemptsLeft.ToString()+",CODEGOT?: "+codeGot.ToString());
            }
            // end of while loop

            if (codeGot)
            {
                main.GameCreationUpdate("Creating room " + gameCode);
                CreateRoom(gameCode);
            }
            else
            {
                failureMessage = "Failed to generate unique game code. Try again later.";
                main.OnGameCreationComplete(failureMessage);
            }

        }
        catch(Exception e)
        {
            Debug.Log("GAME CREATION ERROR: "+e.Message + ", " + e.Source + ", " + e.StackTrace);
        }

	}///

    void CreateRoom(string gameCode)
	{
        string key = databaseRef.Child("Rooms/" + gameCode + "/players").Push().Key;
        Debug.Log("b4 room");
        room = new Room(gameCode, playerName, gameSettings, key);
        Debug.Log("after4 room");

        AddToRoomRegister(gameCode);
    }


    void AddToRoomRegister(string gameCode)
    {
        databaseRef.Child("RoomsRegister/" + gameCode).SetValueAsync(DateTime.UtcNow.ToString()).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                failureMessage = "Failed to add room to register.";
                main.OnGameCreationComplete(failureMessage);
            }
            else if (task.IsCompleted)
            {
                main.GameCreationUpdate("Setting up room");
                AddRoom(gameCode);
            }
        });
    }


    void AddRoom(string gameCode)
    {

        string roomJson = JsonConvert.SerializeObject(room);

        databaseRef.Child("Rooms/" + gameCode).SetRawJsonValueAsync(roomJson).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                failureMessage = "Failed to add room.";
                main.OnGameCreationComplete(failureMessage);
            }
            else if (task.IsCompleted)
            {
                main.OnGameCreationComplete(null); // Successful completion
            }
        });
    }

    void FabricateRoom()
    {
        //room = new Room("ABC99", main.playerName, new GameSettings(3, true, false), "-M6AAA");
        //room.phase = "RevealsIntro";

        //string p1 = main.playerName

        //Dictionary<string, bool?> d = new new Dictionary<string, bool?>();
        //d.Add()

        //room.playerReadies = new Dictionary<string, bool?>

    }

}

