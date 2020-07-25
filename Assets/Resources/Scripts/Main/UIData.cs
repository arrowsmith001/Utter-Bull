using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static RoomListener;

[System.Serializable] public class PhaseChangeEvent : UnityEvent<string, string> { } // <oldPhase, newPhase>
[System.Serializable] public class PlayersChangedEvent : UnityEvent { }
[System.Serializable] public class PlayerNumChangedEvent : UnityEvent<int> { } // <changeMode> {-1: Dec, 0: Reset, 1: Inc}
[System.Serializable] public class ReadiesChangedEvent : UnityEvent<int, int> { } // <from, to> 
[System.Serializable] public class AllReadyEvent : UnityEvent<string, bool> { } // <phase, isHost?>
[System.Serializable] public class OrderChangedEvent : UnityEvent { }
[System.Serializable] public class HostChangedEvent : UnityEvent { }  
[System.Serializable] public class WhoseTurnChangedEvent : UnityEvent { } 
[System.Serializable] public class RoomCodeChangedEvent : UnityEvent { }
[System.Serializable] public class GameSettingsChangedEvent : UnityEvent { }
[System.Serializable] public class VotesChangedEvent : UnityEvent { }
[System.Serializable] public class RDIReadyEvent : UnityEvent { }
[System.Serializable] public class RevealsPageIndexChanged : UnityEvent<int,int> { } // <page, position>

public class UIData : MonoBehaviour
{
    // Events
    public PhaseChangeEvent phaseChangeEvent = new PhaseChangeEvent();
    public PlayersChangedEvent playersChangeEvent = new PlayersChangedEvent();
    public PlayerNumChangedEvent playerNumChangeEvent = new PlayerNumChangedEvent();
    public ReadiesChangedEvent readiesChangeEvent = new ReadiesChangedEvent();
    public AllReadyEvent allReadyEvent = new AllReadyEvent();
    public OrderChangedEvent orderChangeEvent = new OrderChangedEvent();
    public HostChangedEvent hostChangedEvent = new HostChangedEvent();
    public WhoseTurnChangedEvent whoseTurnChangedEvent = new WhoseTurnChangedEvent();
    public RoomCodeChangedEvent roomCodeChangedEvent = new RoomCodeChangedEvent();
    public GameSettingsChangedEvent gameSettingsChangedEvent = new GameSettingsChangedEvent();
    public VotesChangedEvent votesChangedEvent = new VotesChangedEvent();
    public RDIReadyEvent rdiReadyEvent = new RDIReadyEvent();
    public RevealsPageIndexChanged revealsPageIndexChanged = new RevealsPageIndexChanged();

    public MainScript main;

    // DATA OBJECTS
    public List<Player> playersList = new List<Player>();
    public Dictionary<string, Player> playersLookup = new Dictionary<string, Player>();
    public List<Player> playersOrder = new List<Player>();
    public int turnNum = 0;

    public List<Player> readiesList = new List<Player>();
    public List<Player> readiesListMinusWhoseTurn = new List<Player>();
    public int readyCount = 0;
    public int readyCountMinusWhoseTurn = 0;

    public Dictionary<string, List<VoteData>> votesLookup = new Dictionary<string, List<VoteData>>();

    public List<Player> playersWhovePlayed = new List<Player>();
    public List<Player> playersLeftToPlay = new List<Player>();

    public string currentRoomCode = "";
    public string currentPhase = "";
    public Player currentHost = null;
    public Player currentTurn = null;
    public GameSettings currentSettings = null;
    public int[] currentReveal = null;

    public RoundDataInterpretter rdi = null;
    private ScoreKeeper scores = null;

    //public bool[] tracker = new bool[16];

    internal void Clear()
    {
        playersList = new List<Player>();
        playersLookup = new Dictionary<string, Player>();
        playersOrder = new List<Player>();
        turnNum = 0;

        readiesList = new List<Player>();
        readiesListMinusWhoseTurn = new List<Player>();
        readyCount = 0;
        readyCountMinusWhoseTurn = 0;

        votesLookup = new Dictionary<string, List<VoteData>>();

        playersWhovePlayed = new List<Player>();
        playersLeftToPlay = new List<Player>();

        currentRoomCode = "";
        currentPhase = "";
        currentHost = null;
        currentTurn = null;
        currentSettings = null;
        currentReveal = null;

        rdi = null;
        scores = null;
}

    public int GetPlayerNum() { return this.playersList.Count; }
    public Player GetMe()
    {
        if (this.playersLookup.ContainsKey(main.playerName)) return this.playersLookup[main.playerName];
        else return null;
    }
    public int? GetNextTurn()
    {
        int nextTurn = this.turnNum + 1;
        if (this.playersList.Count <= nextTurn) return null;
        else return nextTurn;
    }
    public bool HaveIVoted()
    {
        return (GetMe() == null || GetMe().votes == null ? false :
            GetMe().votes.Count == this.turnNum + 1);
    }

    private void RemovePlayer(string pName)
    {
        Player p = this.playersLookup[pName];
        this.playersList.Remove(p);
        this.playersLookup.Remove(pName);
    }
    private void AddPlayer(string pName)
    {
        Player p = new Player(pName);
        this.playersList.Add(p);
        this.playersLookup.Add(pName, p);

        p.isMe = (p.playerName == main.playerName);
    }

    private void ResolvePlayerEnumerable(IEnumerable<string> list, bool complete)
    {
        if (list == null || list.Count() == 0) return;

        int numInList = this.playersLookup.Count;

        foreach (string pName in list)
        {
            if (!playersLookup.ContainsKey(pName))
            {
                AddPlayer(pName);
            }
        }

        if(complete)
        {
            List<string> toRemove = new List<string>();

            foreach(string pName in this.playersLookup.Keys)
            {
                if(!list.Contains(pName)) toRemove.Add(pName);
            }

            foreach(string pName in toRemove)
            {
                RemovePlayer(pName);
            }

            // EVENT - Player list change
            int finalNum = this.playersLookup.Count;
            int? changeMode = null;

            if (finalNum < numInList) changeMode = -1;
            if (finalNum > numInList) changeMode = 1;

            Debug.Log("player enum: NUMINLIST: " + numInList + ", FINALNUM: " + finalNum);

            if(changeMode != null) this.playerNumChangeEvent.Invoke((int) changeMode);
        }
    }

    public void PushUpdate(Room room, RoomUpdate update)
    {
        switch(update)
        {
            case RoomUpdate.PLAYERS:
                if (room == null || room.players == null) return;
                ResolvePlayerEnumerable(room.players.Values, true);

                foreach(string pKey in room.players.Keys)
                {
                    playersLookup[room.players[pKey]].key = pKey;
                }

                ResolvePlayReadies();
                TryMakeRDI();
                this.playersChangeEvent.Invoke();

                break;
            case RoomUpdate.READIES:
                if (room == null || room.playerReadies == null) return;
                ResolvePlayerEnumerable(room.playerReadies.Keys, true);

                bool readiesChanged = false;
                bool allReady = false;

                List<Player> playerReadiesTEMP = new List<Player>();
                int readyCountTEMP = 0;

                foreach(Player p in playersList) // Add ready players
                {
                    p.isReady = room.playerReadies.ContainsKey(p.playerName) ? room.playerReadies[p.playerName] : false;
                    if (p.isReady)
                    {
                        playerReadiesTEMP.Add(p);
                        readyCountTEMP++;
                    }
                }

                foreach(Player p in playersList) // Add unready players
                {
                    if (!p.isReady) playerReadiesTEMP.Add(p);
                }

                if(this.readyCount != readyCountTEMP) // Readies Changed event
                {
                    readiesChanged = true;
                }

                if(this.playersList.Count > 0 && readyCountTEMP == this.playersList.Count) // All Ready event
                {
                    allReady = true;
                }

                // Pass references
                this.readiesList = playerReadiesTEMP;
                int readyCountPREV = this.readyCount;
                this.readyCount = readyCountTEMP;
                
                ResolvePlayReadies();

                if(readiesChanged)
                {
                    this.readiesChangeEvent.Invoke(readyCountPREV, readyCountTEMP);
                }

                if(allReady)
                {
                    bool b = (currentHost == null ? false : currentHost.isMe);
                    this.allReadyEvent.Invoke(this.currentPhase, b);
                }

                this.playersChangeEvent.Invoke();

                break;
            case RoomUpdate.SCORES:
                if (room == null || room.scores == null) return;
                ResolvePlayerEnumerable(room.scores.playerScores.Keys, true);

                foreach (Player p in playersList)
                {
                    p.points = room.scores.playerScores.ContainsKey(p.playerName) ? room.scores.playerScores[p.playerName] : 0;
                }

                this.scores = room.scores;

                TryMakeRDI();
                this.playersChangeEvent.Invoke();

                break;
            case RoomUpdate.TEXTS:
                if (room == null || room.playerTexts == null) return;
                ResolvePlayerEnumerable(room.playerTexts.Keys, false);

                foreach (Player p in playersList)
                {
                    p.text = room.playerTexts.ContainsKey(p.playerName) ? room.playerTexts[p.playerName] : null;
                }
               
                this.playersChangeEvent.Invoke();

                break;
            case RoomUpdate.TARGETS:
                if (room == null || room.playerTargets == null)
                {
                    foreach (Player p in playersList)
                    {
                        p.target = p.playerName;
                    }

                    return;
                }
                ResolvePlayerEnumerable(room.playerTargets.Keys, false);

                foreach (Player p in playersList)
                {
                    p.target = room.playerTargets.ContainsKey(p.playerName) ? room.playerTargets[p.playerName] : p.playerName;
                }
                
                this.playersChangeEvent.Invoke();

                break;
            case RoomUpdate.TRUTHS:
                if (room == null || room.playerTruths == null) return;
                ResolvePlayerEnumerable(room.playerTruths.Keys, true);

                if (room.playerTruths != null)
                {
                    foreach (Player p in playersList)
                    {
                        if(room.playerTruths.ContainsKey(p.playerName)) p.isTruth = room.playerTruths[p.playerName];
                    }
                }

                this.playersChangeEvent.Invoke();

                break;
            case RoomUpdate.VOTES:
                if (room == null || room.playerVotes == null)
                {
                    foreach (Player p in playersList)
                    {
                        p.votes = null;
                    }

                    ResetRDI();

                    return;
                }
                ResolvePlayerEnumerable(room.playerVotes.Keys, false);

                votesLookup.Clear();

                foreach (Player p in playersList) // Get their list of votes
                {
                    p.votes = room.playerVotes.ContainsKey(p.playerName) ? room.playerVotes[p.playerName].Values.ToList<VoteData>() : null;
                    votesLookup.Add(p.playerName, p.votes);
                }

                try
                {
                    playersWhovePlayed = new List<Player>();
                    playersLeftToPlay = new List<Player>();

                    foreach (Player p in playersList)
                    {
                        if (VoteData.ContainsVote(p.votes.ToList<VoteData>(), "p"))
                        {
                            playersWhovePlayed.Add(p);
                        }
                        else
                        {
                            playersLeftToPlay.Add(p);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("ON VOTES UPDATED: " + e.Message + ", " + e.Source + ", " + e.StackTrace);
                }


                TryMakeRDI();
                this.playersChangeEvent.Invoke();
                this.votesChangedEvent.Invoke();

                break;
            case RoomUpdate.ORDER:

                if (room == null || room.order == null) return;
                ResolvePlayerEnumerable(room.order.playerOrder, true);
                ResolveOrder(room);
                this.turnNum = room.order.turnNum;

                TryMakeRDI();
                this.orderChangeEvent.Invoke();

                break;
            case RoomUpdate.HOSTNAME:

                if (room == null || room.hostName == null) return;
                ResolvePlayerEnumerable(new List<string>() { room.hostName }, false);
                ResolveHost(room);

                this.playersChangeEvent.Invoke();

                break;
            case RoomUpdate.ROOMCODE:
                if (room == null || room.roomCode == null) return;

                if (this.currentRoomCode != room.roomCode)
                {
                    this.currentRoomCode = room.roomCode;
                    this.roomCodeChangedEvent.Invoke();
                }

                break;
            case RoomUpdate.PHASE:
                if (room == null || room.phase == null) return;

                if (room.phase != this.currentPhase)
                {
                    string oldPhase = this.currentPhase;
                    this.currentPhase = room.phase;
                    this.phaseChangeEvent.Invoke(oldPhase, this.currentPhase);
                }

                break;
            case RoomUpdate.SETTINGS:
                if (room == null || room.settings == null) return;

                if (!room.settings.Compare(this.currentSettings))
                {
                    this.currentSettings = room.settings;
                    this.gameSettingsChangedEvent.Invoke();
                }

                TryMakeRDI();
                break;
    
            case RoomUpdate.RDI:
                if (room == null || room.rdiReady == null) return;

                break;
    
            case RoomUpdate.REVEALSPAGEINDEX:
                if (room == null || room.revealsPageIndex == null) return;

                if(this.currentReveal == null || !Enumerable.SequenceEqual(this.currentReveal, room.revealsPageIndex))
                {
                    this.currentReveal = room.revealsPageIndex;
                    revealsPageIndexChanged.Invoke(this.currentReveal[0], this.currentReveal[1]);
                }

                break;
    }
}

    public void ResetRDI()
    {
        playersLeftToPlay = playersList; // WARNING - Passes playersList reference so dont manipulate directly
        playersWhovePlayed = new List<Player>();

        this.rdi = null;
    }

    public bool AmIReady()
    {
        return GetMe().isReady;
    }

    private void TryMakeRDI()
    {
        if (this.rdi != null) return;

        try
        {

            ConfirmInfo ci = RoundDataInterpretter.ConfirmSufficientInformation(
                this.votesLookup, this.playersLookup, this.playersOrder,
                this.currentSettings, this.scores);

            if (!ci.success) throw new Exception(ci.msg);

            rdi = new RoundDataInterpretter(
                this.votesLookup, this.playersLookup, this.playersOrder,
                this.currentSettings.rtm, this.scores);

            Debug.Log("Actually making RDI");
            rdi.InterpretRound();
            rdiReadyEvent.Invoke();

            string json = JsonConvert.SerializeObject(rdi);
            Debug.Log(json);
        }
        catch(Exception e)
        {
            rdi = null;
            Debug.Log("RDI Attempt failed: " + e.Message);
        }
    }

    private void ResolveOrder(Room room)
    {
        ResolvePlayerEnumerable(room.order.playerOrder, true);
        bool invokeWhoseTurn = false;

        List<Player> newList = new List<Player>();
        foreach(string s in room.order.playerOrder)
        {
            newList.Add(this.playersLookup[s]);
        }
        this.playersOrder = newList;

        foreach (Player p in playersList)
        {
            p.isTurn = (this.playersOrder[room.order.turnNum] == p);

            if (p.isTurn)
            {
                if (p != this.currentTurn)
                {
                    this.currentTurn = p;
                    invokeWhoseTurn = true;
                }
            }
        }

        ResolvePlayReadies();

        if (invokeWhoseTurn) whoseTurnChangedEvent.Invoke(); // EVENT turn change
    }

    private void ResolveHost(Room room)
    {
        foreach (Player p in playersList)
        {
            p.isHosting = (room.hostName == null) ? false : (room.hostName == p.playerName);

            if (p.isHosting)
            {
                if (p != this.currentHost)
                {
                    this.currentHost = p;

                    Debug.Log("CHANGE HOST INVOKE");
                    hostChangedEvent.Invoke();
                }
            }
        }
    }

    private void ResolvePlayReadies()
    {

        List<Player> playerReadiesMinusWhoseTurnTEMP = new List<Player>();
        int readyCountMinusWhoseTurnTEMP = 0;

        foreach (Player p in this.playersList)
        {
            if (p.isReady && !p.isTurn)
            {
                playerReadiesMinusWhoseTurnTEMP.Add(p);
                readyCountMinusWhoseTurnTEMP++;
            }
        }
        foreach (Player p in this.playersList)
        {
            if (!p.isReady && !p.isTurn)
            {
                playerReadiesMinusWhoseTurnTEMP.Add(p);
            }
        }

        // Pass references
        this.readiesListMinusWhoseTurn = playerReadiesMinusWhoseTurnTEMP;
        this.readyCountMinusWhoseTurn = readyCountMinusWhoseTurnTEMP;
    }

    internal List<string> GetStringPlayersList()
    {
        List<string> outList = new List<string>();

        foreach (Player p in this.playersList) outList.Add(p.playerName);

        return outList;
    }

    internal bool AmIHost()
    {
        return this.currentHost.isMe;
    }


    //// RDI

    //bool ConstructRoundDataBool = false;

    //public void ConstructRoundData()
    //{
    //    ConstructRoundDataBool = true;
    //}

    //void OnPhaseChanged(string newPhase)
    //{
    //    main.SavePrefString("statePref", newPhase);
    //    this.newPhase = newPhase;
    //    PhaseChangeReactionRequired = true;
    //}

    ///// <summary>
    ///// Only field populated is "Host"
    ///// </summary>
    ///// <param name="room"></param>
    ///// <returns></returns>
    //internal static Dictionary<string, Player> GetPlayersLookupFromRoom(Room room)
    //{
    //    // Update room
    //    Dictionary<string, string> players = room.players;
    //    List<Player> playersList = new List<Player>();

    //    // Return object
    //    Dictionary<string, Player> playersLookup = new Dictionary<string, Player>();

    //    // Instantiate a player for every member of the DataSnapshot
    //    foreach (string playerName in players.Values)
    //    {
    //        playersList.Add(new Player(playerName));
    //    }

    //    // Populate lookup
    //    foreach (Player p in playersList)
    //    {
    //        playersLookup.Add(p.playerName, p);

    //        // Update fields
    //        p.isHosting = string.Equals(p.playerName, room.hostName);
    //    }

    //    return playersLookup;
    //}
    //private Dictionary<string, List<T>> DicToList<T>(Dictionary<string, Dictionary<string, T>> dic)
    //{
    //    if (dic == null) return null;

    //    Dictionary<string, List<T>> output = new Dictionary<string, List<T>>();

    //    foreach (string pk in dic.Keys)
    //    {
    //        List<string> keys = new List<string>();
    //        foreach (string k in dic[pk].Keys)
    //        {
    //            keys.Add(k);
    //        }
    //        List<string> orderedKeys = keys;
    //        //keys.OrderBy(q => q).ToList<string>(); // C# ordering =/= Firebase key ordering

    //        // Votes / Votetimes
    //        List<T> votes = new List<T>();
    //        foreach (string key in orderedKeys)
    //        {
    //            votes.Add(dic[pk][key]);
    //        }

    //        output.Add(pk, votes);
    //    }
    //    return output;
    //}

    //// Construct playersWhovePlayed and playersLeftToPlay
    //void OnVotesUpdated()
    //{
    //    Debug.Log("ON VOTES UPDATED CALLED");




    //}

    //// UPDATE PUSH
    //string oldPhase;
    //string newPhase;
    //public bool UpdateRequired = false;
    //public bool PhaseChangeReactionRequired = false;
    //public bool AllReadyReactionRequired = false;
    //public List<Player> readiesListDISPLAY;


    //public bool ConfirmPhase = false;


    //void Update()
    //{
    //    if (IsListening && (UpdateRequired || PhaseChangeReactionRequired || AllReadyReactionRequired))
    //    {
    //        if (UpdateRequired)
    //        {
    //            Debug.Log("RoomListener: UpdateRequired");
    //            if (main.tm.newFrag.GetComponent<Fragment>().SetUIValues()) UpdateRequired = false;
    //        }
    //        else if (PhaseChangeReactionRequired)
    //        {
    //            Debug.Log("RoomListener: PhaseChangeReactionRequired");
    //            if (main.tm.newFrag.GetComponent<Fragment>().ReactToPhaseChange(this.newPhase)) PhaseChangeReactionRequired = false;
    //        }
    //        else if (AllReadyReactionRequired)
    //        {
    //            Debug.Log("RoomListener: AllReadyReactionRequired");
    //            if (main.tm.newFrag.GetComponent<Fragment>().ReactToAllReady(this.oldPhase)) AllReadyReactionRequired = false;
    //        }

    //    }

    //    if (ConfirmPhase)
    //    {
    //        if (room.phase != null)
    //        {
    //            this.oldPhase = room.phase;
    //            AllReadyReactionRequired = true;
    //            ConfirmPhase = false;
    //        }
    //    }

    //    if ((ConstructRoundDataBool && !rdiDataAvailable)
    //        //&&
    //        //(playersWhovePlayed.Count > 0 && playersLookup.Count > 0
    //        //&& votesLookup.Count > 0 && timesLookup.Count > 0)
    //        )
    //    {
    //        try
    //        {
    //            Debug.Log("RDI CONSTRUCTION ATTEMPT...");

    //            rdi = new RoundDataInterpretter(true, room,
    //                playersWhovePlayed, playersLookup,
    //                votesLookup, timesLookup);

    //            if (rdi.ConfirmSufficientInformation())
    //            {
    //                rdi.InterpretRound();
    //            }


    //            Debug.Log("ConstructRoundData: rdi constructed successfully ? = "
    //                + rdi.SufficientInfoConfirmed + ", msg: " + rdi.failureMessage);

    //            string json = JsonConvert.SerializeObject(rdi);

    //            Debug.Log("(RL) RDI SERIALISED: " + json);

    //            rdiDataAvailable = rdi.SufficientInfoConfirmed;
    //            ConstructRoundDataBool = !rdi.SufficientInfoConfirmed;
    //        }
    //        catch
    //        {
    //            MakePlayerLookup();
    //            OnVotesUpdated();
    //        }
    //    }

    //}

}
