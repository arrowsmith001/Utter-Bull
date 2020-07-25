using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Room {

    public string roomCode;

    // Ordered players list
    public Dictionary<string, string> players;

    // Players states lookup
    public Dictionary<string, bool> playerReadies;

    // Players score lookup
    public ScoreKeeper scores;

    // Players text content lookup
    public Dictionary<string, string> playerTexts;

    // Players target lookup
    public Dictionary<string, string> playerTargets;

    // Players truth lookup
    public Dictionary<string, bool> playerTruths;

    // Players votes lookup
    public Dictionary<string, Dictionary<string, VoteData>> playerVotes;

    public Order order;

    // This is the host's name
    public string hostName;

    // State of room
    public string phase;

    // Data for RDI available
    public bool? rdiReady;

    // Data for Endgame available
    public int[] revealsPageIndex;

    public string turnTimeStart;

    // Room settings
    public GameSettings settings;

    //public Room(Room room)
    //{
    //    Room copy = new Room();

    //    copy.players = room.players.c
    //}

    public Room()
    {

    }

    public Room(string roomCode)
    {
        this.roomCode = roomCode;
    }

    public Room(string roomCode,  string hostName, GameSettings gameSettings, string hostKey) {

        Initialise(roomCode, hostName, gameSettings, hostKey);
    }

    public void Initialise(string roomCode, string hostName, GameSettings gameSettings, string hostKey)
	{
        this.roomCode = roomCode;
        this.hostName = hostName;
        this.settings = gameSettings;

        players = new Dictionary<string, string>();
        scores = new ScoreKeeper();
        playerReadies = new Dictionary<string, bool>();

        players.Add(hostKey,hostName);
        scores.Add(hostName, 0);
        playerReadies.Add(hostName, false);

        this.phase = "Lobby";
        //  TODO: Make settings dynamic based on user preferences
    }

}