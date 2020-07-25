using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Player {
    
    public Player() {
        
    }

    public Player(string playerName, bool isHosting, int points)
    {
        this.playerName = playerName;
        this.target = playerName;
        this.isHosting = isHosting;
        this.points = points;
    }
    
    public Player(string playerName) {
        this.playerName = playerName;
        this.target = playerName;
        this.points = 0;
    }
    
    public Player(int points) {
        this.points = points;
    }


    public Player(Player p, RoundDataInterpretter rdi)
    {

        this.playerName = p.playerName;
        this.points = p.points;
        this.rdi = rdi;
    }

    //  Player name
    [SerializeField]
    public string playerName { get; set; }
    
    //  Whether player is hosting or not
    [SerializeField]
    public bool isHosting { get; set; }
    
    //  Whether player is ready or not
    [SerializeField]
    public bool isReady { get; set; }

    //  Whether player is telling truth or not
    [SerializeField]
    public bool? isTruth { get; set; }

    //  Whether its the player's turn
    [SerializeField]
    public bool isTurn { get; set; }

    //  The content this player must read out
    [SerializeField]
    public string text { get; set; }
    
    //  How many points the player has earned
    [SerializeField]
    public int points { get; set; }
    
    //  Target of lie
    [SerializeField]
    public string target { get; set; }
    
    //  Whether player corresponds to "me"
    public bool isMe { get; set; }

    //  Keeps tracks of votes cast / round tracking info (T,L,s,p)
    [SerializeField]
    public List<VoteData> votes { get; set; }
    
    
    public void addVote(string vote, int time) {
        if ((this.votes == null)) {
            this.votes = new List<VoteData>();
        }
        
        switch (vote) {
            case "T":
                this.votes.Add(new VoteData(vote, time));
                break;
            case "L":
                this.votes.Add(new VoteData(vote, time));
                break;
            case "sX":
                this.votes.Add(new VoteData(vote, time));
                break;
            case "sL":
                this.votes.Add(new VoteData(vote, time));
                break;
            case "sT":
                this.votes.Add(new VoteData(vote, time));
                break;
            case "p":
                this.votes.Add(new VoteData(vote, time));
                break;
            case "X":
                this.votes.Add(new VoteData(vote, time));
                break;
        }
    }
    
    public void addPoints(int newPoints)
    {
        this.points = this.points + newPoints;
    }

    public string key;

    // As Voter...
    public int? voteTime;
    public bool? isAmongFastest;
    public bool? fakeVoter;

    // As results object...
    public RoundDataInterpretter rdi;
}