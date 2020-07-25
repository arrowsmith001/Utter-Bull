using System;
using System.Collections.Generic;
using Firebase.Database;
using Firebase.Extensions;
using UnityEditor;
using UnityEngine;

public class RoomExiter : MonoBehaviour
{
    private MainScript main;
    private Room room;
    private Dictionary<string, Player> playersLookup;
    private DatabaseReference databaseRef;
    private DatabaseReference roomRef;
    private string playerName;

    

    public string failureMessage = "";

    public RoomExiter()
    {

    }

    public void BeginExit(
        MainScript mainScript, Room room, Dictionary<string, Player> playersLookup,
        DatabaseReference roomRef, DatabaseReference databaseRef, string playerName)
    {
        this.main = mainScript;
        this.room = room;
        this.playersLookup = playersLookup;
        this.roomRef = roomRef;
        this.databaseRef = databaseRef;
        this.playerName = playerName;

        // Stop listening
        if (main.rl.IsListening) main.rl.End();

        if(toLobby)
        {
            SetRoomToLobby();
        }
        else
        {
            LeaveRoomType();
        }

        

    }

    private void SetRoomToLobby()
    {
        roomRef.Child("phase").SetValueAsync("Lobby").ContinueWithOnMainThread(task =>
        {
            LeaveRoomType();
        });
    }

    private void LeaveRoomType()
    {
        if (room.players.Count == 1)
        {
            RemoveRoomFromRegister();
        }
        else
        {
            RemoveFromPlayers();
        }
    }

    private void RemoveRoomFromRegister()
    {
        databaseRef.Child("RoomsRegister/" + room.roomCode).SetValueAsync(null).ContinueWithOnMainThread(task =>
          {
              DestroyRoom();
          });
    }

    private void DestroyRoom()
    {
        roomRef.SetValueAsync(null).ContinueWithOnMainThread(task =>
        {
            main.OnExitRoomComplete();
        });
    }

    string key;
    bool isHost;
    internal bool toLobby = false;

    private void RemoveFromPlayers()
    {
        Dictionary<string, object> updates = new Dictionary<string, object>();

        key = playersLookup[playerName].key;
        isHost = playersLookup[playerName].isHosting;

        // If is host
        if(isHost)
        {
            string newHost = "";

            // List other players
            List<string> otherPlayers = new List<string>();
            foreach(string pName in room.players.Values)
            {
                if (pName != playerName) otherPlayers.Add(pName);
            }

            // Pick random player
            System.Random rand = new System.Random();
            int randomIndex = (int) rand.NextDouble() * otherPlayers.Count;

            newHost = otherPlayers[randomIndex];

            updates.Add("hostName", newHost);
        }


        updates.Add("players/" + key, null);
        updates.Add("playerReadies/" + playerName, null);
        updates.Add("scores/playerScores" + playerName, null);

        roomRef.UpdateChildrenAsync(updates).ContinueWithOnMainThread((task) =>
        {
            main.OnExitRoomComplete();
        });
    }
}