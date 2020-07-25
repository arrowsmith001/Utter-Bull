using System;
using System.Collections.Generic;
using UnityEngine;

public class TurnData
{
    public int turnNumber;
    public string readersName;
    public string readersText;
    public bool wasInFact;
    public string writtenBy;
    public List<string> trueVoters;
    public List<string> lieVoters;
    public List<string> xVoters;
    public List<string> fastestCorrectVoters;
    public int quickestTime;
    public int rtm;
    public List<string> correctVoters;
    public List<string> incorrectVoters;
    public List<string> didntVoteVoters;
    public Dictionary<string, int> playerTimesLookup;
    public string saboName;
    public bool s_badSabo;
    public bool s_mostEarned;
    public bool s_allEarned;
    public List<string> sTrueVoters;
    public List<string> sLieVoters;
    public List<string> sXVoters;
    public bool p_fiftyfiftyEarned;
    public bool p_mostEarned;
    public bool p_allEarned;
    public bool p_nobodyVoted;

    public Dictionary<string, List<Achievement>> achievementsUnlocked;

    public List<Player> votersTrue;
    public List<Player> votersBull;

    public List<Player> votersTrueBLANK;
    public List<Player> votersBullBLANK;
    public List<Player> votersTrueNOFAKE;
    public List<Player> votersBullNOFAKE;

    public int numTrueACTUAL;
    public int numTrueFAKE;
    public int numLieACTUAL;
    public int numLieFAKE;

    public TurnData(int turnNumber, string readersName, string readersText, bool wasInFact, string writtenBy, List<string> trueVoters, List<string> lieVoters, List<string> xVoters, List<string> fastestCorrectVoters, Dictionary<string, int> playerTimesLookup, int quickestTime, int rtm, List<string> correctVoters, List<string> incorrectVoters, List<string> didntVoteVoters, string saboName, bool badSabo, bool mostEarned, bool allEarned, List<string> sTrueVoters, List<string> sLieVoters, List<string> sXVoters, Dictionary<string, List<Achievement>> achievementsUnlocked, bool p_fiftyfiftyEarned, bool p_mostEarned, bool p_allEarned, bool p_nobodyVoted)
    {
        this.turnNumber = turnNumber;
        this.readersName = readersName;
        this.readersText = readersText;
        this.wasInFact = wasInFact;
        this.writtenBy = writtenBy;
        this.trueVoters = trueVoters;
        this.lieVoters = lieVoters;
        this.xVoters = xVoters;
        this.fastestCorrectVoters = fastestCorrectVoters;
        this.quickestTime = quickestTime;
        this.rtm = rtm;
        this.correctVoters = correctVoters;
        this.incorrectVoters = incorrectVoters;
        this.didntVoteVoters = didntVoteVoters;
        this.playerTimesLookup = playerTimesLookup;
        this.saboName = saboName;
        this.s_badSabo = badSabo;
        this.s_mostEarned = mostEarned;
        this.s_allEarned = allEarned;

        this.sTrueVoters = sTrueVoters;
        this.sLieVoters = sLieVoters;
        this.sXVoters = sXVoters;

        this.achievementsUnlocked = achievementsUnlocked;
        this.p_fiftyfiftyEarned = p_fiftyfiftyEarned;
        this.p_mostEarned = p_mostEarned;
        this.p_allEarned = p_allEarned;
        this.p_nobodyVoted = p_nobodyVoted;

    }

    public void MakeVoters()
    {
        // REAL VOTERS
        votersTrue = new List<Player>();
        votersBull = new List<Player>();

        // COMPLETELY BLANK, NO FASTEST, NO FAKES
        votersTrueBLANK = new List<Player>();
        votersBullBLANK = new List<Player>();

        // NO FAKES ONLY
        votersTrueNOFAKE = new List<Player>();
        votersBullNOFAKE = new List<Player>();

        Player v;

        try
        {
            foreach (string pName in trueVoters)
            {
                Debug.Log("VOTER ADDING: " + pName);
                v = new Player(pName);
                v.voteTime = playerTimesLookup[pName];
                v.isAmongFastest = fastestCorrectVoters.Contains(pName);
                v.fakeVoter = false;
                votersTrue.Add(v);
                numTrueACTUAL++;
            }

            foreach (string pName in lieVoters)
            {
                Debug.Log("VOTER ADDING: " + pName);
                v = new Player(pName);
                v.voteTime = playerTimesLookup[pName];
                v.isAmongFastest = fastestCorrectVoters.Contains(pName);
                v.fakeVoter = false;
                votersBull.Add(v);
                numLieACTUAL++;
            }
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
        }

        Debug.Log("VOTERLIST COUNTS: TRUE: " + votersTrue.Count + ", LIE: " + votersBull.Count);

        // ADD FAKE VOTERS

        numTrueFAKE = numTrueACTUAL;
        numLieFAKE = numLieACTUAL;

        if (sTrueVoters.Count > 0)
        {
            foreach (string pName in sTrueVoters)
            {
                Debug.Log("FAKE VOTER ADDING: " + pName);
                v = new Player(pName);

                v.voteTime = playerTimesLookup[pName];

                v.isAmongFastest = false;
                v.fakeVoter = true;
                votersTrue.Add(v);
                numTrueFAKE++;
            }
        }

        if (sLieVoters.Count > 0)
        {
            foreach (string pName in sLieVoters)
            {
                Debug.Log("FAKE VOTER ADDING: " + pName);
                v = new Player(pName);

                v.voteTime = playerTimesLookup[pName];

                v.isAmongFastest = false;
                v.fakeVoter = true;
                votersBull.Add(v);
                numLieFAKE++;
            }
        }

        // FINAL SHUFFLE, REAL VOTERS FINALISED

        votersTrue = SeedShuffleVariant(votersTrue);
        votersBull = SeedShuffleVariant(votersBull);

        MakeFakes();
    }

    void MakeFakes()
    {

        System.Random rand = new System.Random();
        double r = rand.NextDouble();
        Player n;

        // NO DATA FAKES (NAME AND TIME ONLY)
        foreach (Player p in votersTrue)
        {
            n = new Player(p.playerName);
            n.voteTime = p.voteTime;
            if (p.fakeVoter ?? false) n.voteTime = GetFakeTime(p.playerName, r);
            votersTrueBLANK.Add(n);
        }
        foreach (Player p in votersBull)
        {
            n = new Player(p.playerName);
            n.voteTime = p.voteTime;
            if (p.fakeVoter ?? false) n.voteTime = GetFakeTime(p.playerName, r);
            votersBullBLANK.Add(n);
        }

        // PARTIAL FAKES (includes FASTEST)
        foreach (Player p in votersTrue)
        {
            n = new Player(p.playerName);
            n.voteTime = p.voteTime;
            if (p.fakeVoter ?? false) n.voteTime = GetFakeTime(p.playerName, r);
            n.isAmongFastest = p.isAmongFastest;
            votersTrueNOFAKE.Add(n);
        }
        foreach (Player p in votersBull)
        {
            n = new Player(p.playerName);
            n.voteTime = p.voteTime;
            if (p.fakeVoter ?? false) n.voteTime = GetFakeTime(p.playerName, r);
            n.isAmongFastest = p.isAmongFastest;
            votersBullNOFAKE.Add(n);
        }


    }

    int GetFakeTime(string pName, double rand)
    {
        int fakeTime;
        if (quickestTime < playerTimesLookup[pName] || !wasInFact)
        {
            fakeTime = playerTimesLookup[pName];
        }
        else
        {
            quickestTime = playerTimesLookup[pName];
            fakeTime = quickestTime + 1 + ((int)(rand * 10));
        }

        return fakeTime;
    }

    private List<Player> SeedShuffleVariant(List<Player> playersToPlay)
    {
        // DEBUG ONLY
        //return playersToPlay;

        List<Player> playersShuffled = new List<Player>();
        Dictionary<double, Player> dic = new Dictionary<double, Player>();
        List<double> randomDoubles = new List<double>();

        foreach (Player p in playersToPlay)
        {
            System.Random rand = new System.Random(
                (p.playerName + p.points + p.key).GetHashCode());
            double d = rand.NextDouble();

            dic.Add(d, p);
            randomDoubles.Add(d);

            Debug.Log("HASH: " + p.playerName + ", D: " + d);
        }

        randomDoubles.Sort();

        foreach (double d in randomDoubles)
        {
            playersShuffled.Add(dic[d]);
        }

        return playersShuffled;
    }
}