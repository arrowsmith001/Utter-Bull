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
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using System.Linq;
using UnityEngine.Events;
using System.ComponentModel.Design.Serialization;

public class RoomListener : MonoBehaviour
{
    public MainScript main;

    public bool IsListening = false;
    public DatabaseReference roomRef;

    // PUBLIC OUTPUT FIELDS
    public Room room;
    public string roomCode;

    // LIFECYCLE
    void Start()
    {

    }

    public void ConfigureListeners(string roomCode, DatabaseReference databaseRef)
    {
        main.SavePrefString("roomCode", roomCode);

        this.roomCode = roomCode;

        if(this.room == null) this.room = new Room(roomCode);

        this.roomRef = databaseRef.Child("Rooms/" + roomCode);
        main.rc.Activate(this.roomRef);

        roomRef.Child("players").ValueChanged += UpdatePlayers;
        roomRef.Child("playerReadies").ValueChanged +=UpdateReadies;
        roomRef.Child("scores").ValueChanged += UpdateScores;
        roomRef.Child("playerTexts").ValueChanged += UpdateTexts;
        roomRef.Child("playerTargets").ValueChanged += UpdateTargets;
        roomRef.Child("playerTruths").ValueChanged += UpdateTruths;
        roomRef.Child("playerVotes").ValueChanged += UpdateVotes;
        roomRef.Child("order").ValueChanged += UpdateOrder;
        roomRef.Child("hostName").ValueChanged += UpdateHostName; 
        roomRef.Child("roomCode").ValueChanged += UpdateRoomCode; 
        roomRef.Child("phase").ValueChanged += UpdatePhase;
        roomRef.Child("settings").ValueChanged += UpdateSettings;
        roomRef.Child("rdiReady").ValueChanged += UpdateRDI;
        roomRef.Child("revealsPageIndex").ValueChanged += UpdateRevealsPageIndex;

        Debug.Log("ConfigureListeners: ISLISTENING = true, TO ROOM: "+roomCode);
        IsListening = true;
    }

    public void End()
    {
            IsListening = false;

            main.SavePrefString("roomCode", "");

        roomRef.Child("players").ValueChanged -= UpdatePlayers;
        roomRef.Child("playerReadies").ValueChanged -= UpdateReadies;
        roomRef.Child("scores").ValueChanged -= UpdateScores;
        roomRef.Child("playerTexts").ValueChanged -= UpdateTexts;
        roomRef.Child("playerTargets").ValueChanged -= UpdateTargets;
        roomRef.Child("playerVotes").ValueChanged -= UpdateVotes;
        roomRef.Child("order").ValueChanged -= UpdateOrder;
        roomRef.Child("playerTruths").ValueChanged -= UpdateTruths;
        roomRef.Child("hostName").ValueChanged -= UpdateHostName;
        roomRef.Child("roomCode").ValueChanged -= UpdateRoomCode;
        roomRef.Child("phase").ValueChanged -= UpdatePhase;
        roomRef.Child("settings").ValueChanged -= UpdateSettings;
        roomRef.Child("rdiReady").ValueChanged -= UpdateRDI;
        roomRef.Child("revealsPageIndex").ValueChanged -= UpdateRevealsPageIndex;

        main.rc.Deactivate();

        this.room = null;
        this.roomRef = null;

        Debug.Log("RL ENDED SUCCESSFULLY");
    }

    private void UpdateOrder(object sender, ValueChangedEventArgs args)
    {
        try
        {
            if (args.DatabaseError != null && IsListening)
            {
                Debug.Log("DatabaseError: " + args.DatabaseError.Message);
            }
            else
            {
                Debug.Log("UpdateOrder: " + args.Snapshot.GetRawJsonValue());
                Order order;

                order = JsonConvert.DeserializeObject<Order>(args.Snapshot.GetRawJsonValue());
                room.order = order;

            }
        }
        catch (Exception e) { Debug.Log("ERROR: " + e.Message + ", " + e.Source + ", " + e.StackTrace); }

        OnAnyUpdate(RoomUpdate.ORDER);
    }

    // PHASE
    void UpdatePlayers(object sender, ValueChangedEventArgs args)
    {
        try { 
        if  (args.DatabaseError != null && IsListening)
        {
            Debug.Log("DatabaseError: " + args.DatabaseError.Message);
        }
        else
        {
            Debug.Log("UpdatePlayers: " + args.Snapshot.GetRawJsonValue());

            // Possibility of null? TODO

            Dictionary<string, string> playerSnap = JsonConvert.DeserializeObject<Dictionary<string, string>>(args.Snapshot.GetRawJsonValue());

            if (this.room == null) this.room = new Room(roomCode);

            // Update room
            room.players = playerSnap;
        }

        }
        catch (Exception e) { Debug.Log("UpdatePlayersERROR: " + e.Message + ", " + e.Source + ", " + e.StackTrace); }

        OnAnyUpdate(RoomUpdate.PLAYERS);
    }
    void UpdatePhase(object sender, ValueChangedEventArgs args)
    {
        try { 
        if  (args.DatabaseError != null && IsListening)
        {
            Debug.Log("DatabaseError: " + args.DatabaseError.Message);
        }
        else
        {

            Debug.Log("UpdatePhase: " + args.Snapshot.GetRawJsonValue());
            string phaseSnap;

            phaseSnap = (string) args.Snapshot.Value;

            room.phase = phaseSnap;

        }

        }
        catch (Exception e) { Debug.Log("ERROR: " + e.Message + ", " + e.Source + ", " + e.StackTrace); }

        OnAnyUpdate(RoomUpdate.PHASE);


    }
    void UpdateReadies(object sender, ValueChangedEventArgs args)
    {
        try { 
        if  (args.DatabaseError != null && IsListening)
        {
            Debug.Log("DatabaseError: " + args.DatabaseError.Message);
        }
        else
        {
            Debug.Log("UpdateReadies: " + args.Snapshot.GetRawJsonValue());
            Dictionary<string, bool> readySnap;

            readySnap = JsonConvert.DeserializeObject<Dictionary<string, bool>>(args.Snapshot.GetRawJsonValue());
            Debug.Log("READYSNAP DESERIALIZED");

            // Update room
            room.playerReadies = readySnap;
       

            }

        }
        catch (Exception e) { Debug.Log("UPDATE READIES ERROR: 1-2" + e.Message + ", " + e.Source + ", " + e.StackTrace); }

        OnAnyUpdate(RoomUpdate.READIES);
    }
    void UpdateScores(object sender, ValueChangedEventArgs args)
    {
        try
        {
            if (args.DatabaseError != null && IsListening)
            {
                Debug.Log("DatabaseError: " + args.DatabaseError.Message);
            }
            else
            {
                Debug.Log("UpdateScores: " + args.Snapshot.GetRawJsonValue());
                ScoreKeeper scoreSnap;

                scoreSnap = JsonConvert.DeserializeObject<ScoreKeeper>(args.Snapshot.GetRawJsonValue());
                room.scores = scoreSnap;
            }
        }
        catch (Exception e) { Debug.Log("ERROR: " + e.Message + ", " + e.Source + ", " + e.StackTrace); }

        OnAnyUpdate(RoomUpdate.SCORES);

    }
    void UpdateTexts(object sender, ValueChangedEventArgs args)
    {
        try { 
        if  (args.DatabaseError != null && IsListening)
        {
            Debug.Log("DatabaseError: " + args.DatabaseError.Message);
        }
        else
        {
            Debug.Log("UpdateTexts: " + args.Snapshot.GetRawJsonValue());
            Dictionary<string, string> textSnap;

            textSnap = JsonConvert.DeserializeObject<Dictionary<string, string>>(args.Snapshot.GetRawJsonValue());
            room.playerTexts = textSnap;
        }

        }
        catch (Exception e) { Debug.Log("ERROR: " + e.Message + ", " + e.Source + ", " + e.StackTrace); }

        OnAnyUpdate(RoomUpdate.TEXTS);
    }
    void UpdateTargets(object sender, ValueChangedEventArgs args)
    {
        try { 
        if  (args.DatabaseError != null && IsListening)
        {
            Debug.Log("DatabaseError: " + args.DatabaseError.Message);
        }
        else
        {
            Debug.Log("UpdateTargets: " + args.Snapshot.GetRawJsonValue());
            Dictionary<string, string> targetSnap;

            targetSnap = JsonConvert.DeserializeObject<Dictionary<string, string>>(args.Snapshot.GetRawJsonValue());
            room.playerTargets = targetSnap;

    
        }

        }
        catch (Exception e) {
            Debug.Log("ERROR: " + e.Message + ", " + e.Source + ", " + e.StackTrace);
            room.playerTargets = null;
        }

        OnAnyUpdate(RoomUpdate.TARGETS);
    }
    void UpdateTruths(object sender, ValueChangedEventArgs args)
    {
        try { 
        if  (args.DatabaseError != null && IsListening)
        {
            Debug.Log("DatabaseError: " + args.DatabaseError.Message);
        }
        else
        {
            Debug.Log("UpdateTruths: " + args.Snapshot.GetRawJsonValue());
            Dictionary<string, bool> truthSnap;

            truthSnap = JsonConvert.DeserializeObject<Dictionary<string, bool>>(args.Snapshot.GetRawJsonValue());
            room.playerTruths = truthSnap;
           
        }

        }
        catch (Exception e) { Debug.Log("ERROR: " + e.Message + ", " + e.Source + ", " + e.StackTrace); }

        OnAnyUpdate(RoomUpdate.TRUTHS);

    }
    void UpdateVotes(object sender, ValueChangedEventArgs args)
    {
        try { 
        if  (args.DatabaseError != null && IsListening)
        {
            Debug.Log("DatabaseError: " + args.DatabaseError.Message);
        }
        else
        {
            Debug.Log("UpdateVotes: " + args.Snapshot.GetRawJsonValue());
            Dictionary<string, Dictionary<string, VoteData>> votesSnap;

            votesSnap = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string,VoteData>>>(args.Snapshot.GetRawJsonValue());
            room.playerVotes = votesSnap;
        }
        }
        catch (Exception e) {
            Debug.Log("ERROR: " + e.Message + ", " + e.Source + ", " + e.StackTrace);
            room.playerVotes = null;
        }


        OnAnyUpdate(RoomUpdate.VOTES);
    }
    void UpdateHostName(object sender, ValueChangedEventArgs args)
    {
        try { 
        if  (args.DatabaseError != null && IsListening)
        {
            Debug.Log("DatabaseError: " + args.DatabaseError.Message);
        }
        else
        {
            Debug.Log("UpdateHostName: " + args.Snapshot.Value);
            string hostnameSnap;

                hostnameSnap = (string)args.Snapshot.Value;

                room.hostName = hostnameSnap;

        }

        }
        catch (Exception e) { Debug.Log("ERROR: " + e.Message + ", " + e.Source + ", " + e.StackTrace); }


        OnAnyUpdate(RoomUpdate.HOSTNAME);
    }
    void UpdateRoomCode(object sender, ValueChangedEventArgs args)
    {
        try { 
        if  (args.DatabaseError != null && IsListening)
        {
            Debug.Log("DatabaseError: " + args.DatabaseError.Message);
        }
        else
        {
            Debug.Log("UpdateRoomCode: " + args.Snapshot.GetRawJsonValue());
            string roomCodeSnap;

                roomCodeSnap = (string)args.Snapshot.Value;
                room.roomCode = roomCodeSnap;
       
        }

        }
        catch (Exception e) { Debug.Log("ERROR: " + e.Message + ", " + e.Source + ", " + e.StackTrace); }

        OnAnyUpdate(RoomUpdate.ROOMCODE);
    }
    void UpdateSettings(object sender, ValueChangedEventArgs args)
    {
        try { 
        if  (args.DatabaseError != null && IsListening)
        {
            Debug.Log("DatabaseError: " + args.DatabaseError.Message);
        }
        else
        {
            Debug.Log("UpdateSettings: " + args.Snapshot.GetRawJsonValue());
            GameSettings settingsSnap;

                settingsSnap = JsonConvert.DeserializeObject<GameSettings>(args.Snapshot.GetRawJsonValue());
                room.settings = settingsSnap;
   
        }

        }
        catch (Exception e) { Debug.Log("ERROR: " + e.Message + ", " + e.Source + ", " + e.StackTrace); }

        OnAnyUpdate(RoomUpdate.SETTINGS);
    }
    void UpdateRevealsPageIndex(object sender, ValueChangedEventArgs args)
    {
        try
        {
            if  (args.DatabaseError != null && IsListening)
            {
                Debug.Log("DatabaseError: " + args.DatabaseError.Message);
            }
            else
            {
                Debug.Log("UpdateHostName: " + args.Snapshot.GetRawJsonValue());
                int[] revealPageInd;

                revealPageInd = (int[])JsonConvert.DeserializeObject<int[]>(args.Snapshot.GetRawJsonValue());
                room.revealsPageIndex = revealPageInd;



            }
        }
        catch (Exception e) { Debug.Log("ERROR: " + e.Message + ", " + e.Source + ", " + e.StackTrace); }

        OnAnyUpdate(RoomUpdate.REVEALSPAGEINDEX);
    }
    void UpdateRDI(object sender, ValueChangedEventArgs args)
    {
        try
        {
            if  (args.DatabaseError != null && IsListening)
            {
                Debug.Log("DatabaseError: " + args.DatabaseError.Message);
            }
            else
            {
                Debug.Log("UpdateRDI: " + args.Snapshot.GetRawJsonValue());
                bool? rdiReady;

                rdiReady = (bool?) args.Snapshot.Value;

                room.rdiReady = rdiReady;

            }

        }catch(Exception e) { Debug.Log("ERROR: " + e.Message + ", " + e.Source + ", " + e.StackTrace); }

        OnAnyUpdate(RoomUpdate.RDI);
    }

    void OnAnyUpdate(RoomUpdate update)
	{
        main.data.PushUpdate(this.room, update);
    }

    public enum RoomUpdate
    {
        PLAYERS,
        READIES,
        SCORES,
        TEXTS,
        TARGETS,
        TRUTHS,
        VOTES,
        ORDER,
        TURNNUM,
        HOSTNAME,
        ROOMCODE,
        PHASE,
        SETTINGS,
        RDI,
        REVEALSPAGEINDEX
    }

    public static Dictionary<string, Player> GetPlayersLookupFromRoom(Room room)
    {
        // Update room
        Dictionary<string, string> players = room.players;
        List<Player> playersList = new List<Player>();

        // Return object
        Dictionary<string, Player> playersLookup = new Dictionary<string, Player>();

        // Instantiate a player for every member of the DataSnapshot
        foreach (string playerName in players.Values)
        {
            playersList.Add(new Player(playerName));
        }

        // Populate lookup
        foreach (Player p in playersList)
        {
            playersLookup.Add(p.playerName, p);

            // Update fields
            p.isHosting = string.Equals(p.playerName, room.hostName);
        }

        return playersLookup;
    }
}
