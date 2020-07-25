using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using UnityEngine;

public class RoundDataInterpretter
{

    // INPUTS
    private List<Player> playersOrder;
    private Dictionary<string, Player> playersLookup;
    private Dictionary<string, List<VoteData>> votesLookup;
    private ScoreKeeper scores;
    private int rtm;

    // CONSTRUCTOR
    public RoundDataInterpretter(
        Dictionary<string, List<VoteData>> votesLookup,
        Dictionary<string, Player> playersLookup, List<Player> playersOrder,
        int rtm, ScoreKeeper scores)
    {
        this.votesLookup = votesLookup;
        this.playersLookup = playersLookup;
        this.playersOrder = playersOrder;
        this.rtm = rtm;
        this.scores = scores;

    }


    // ADMIN VARS
    public bool SufficientInfoConfirmed = false;
    public string failureMessage = "";


    // WORKING VARS
    int playersNum;


    // OUTPUTS
    public List<TurnData> turnsData;
    public ResultsData resultsData;
    private bool? scoresUpdated;

    public static ConfirmInfo ConfirmSufficientInformation(
        Dictionary<string, List<VoteData>> votesLookup,
        Dictionary<string, Player> playersLookup, List<Player> playersOrder,
        GameSettings gs, ScoreKeeper scores)
    {
        try
        {
            // Check variables aren't null
            if (votesLookup == null || playersLookup == null || playersOrder == null || scores == null || gs == null)
            {
                return new ConfirmInfo(false,
                    "votesLookup NULL: " + (votesLookup == null)
                    + ", playersLookup NULL: " + (playersLookup == null)
                    + ", playersOrder NULL: " + (playersOrder == null)
                    + ", scores NULL: " + (scores == null)
                    + ", gs NULL: " + (gs == null));
            }

            if (votesLookup.Count + playersLookup.Count + playersOrder.Count == 0)
            {
                return new ConfirmInfo(false,
                    "votesLookup.Count: " + (votesLookup.Count)
                    + ", playersLookup.Count: " + (playersLookup.Count)
                    + ", playersOrder.Count: " + (playersOrder.Count));
            }

            int playerNum = playersLookup.Count;
            if (votesLookup.Count != playerNum)
                return new ConfirmInfo(false,
                "votesLookup.Count: " + votesLookup.Count
                + ", playerNum: " + playerNum);
            if (playersOrder.Count != playerNum)
                return new ConfirmInfo(false,
                "playersOrder.Count: " + playersOrder.Count
                + ", playerNum: " + playerNum);

            // Count p votes match playerNum, and indexes unique
            List<int> pIndices = new List<int>();

            foreach(string pName in votesLookup.Keys)
            {
                List<VoteData> votes = votesLookup[pName];

                if (votes.Count != playerNum)
                    return new ConfirmInfo(false,
                        "votes.Count: " + votes.Count
                        + ", playerNum: " + playerNum);

                List<int> list = VoteData.WhereIsVote(votes, "p");

                if (list.Count != 1)
                {
                    return new ConfirmInfo(false, "(list of Ps) list.Count: "+ list.Count + " in "+pName);
                }
                else
                {
                    pIndices.Add(list[0]);
                }
            }

            bool duplicateFound = false;

            for(int i = 0; i < pIndices.Count; i++)
            {
                int thisInd = pIndices[i];

                for (int j = 0; j < pIndices.Count; j++)
                {
                    if(i != j && thisInd == pIndices[j])
                    {
                        duplicateFound = true;
                    }
                }
            }

            // Checks of above
            if(duplicateFound)
            {
                return new ConfirmInfo(false, "duplicateFound");
            }

            return new ConfirmInfo(true, null);

        }
        catch(Exception e)
        {
            return new ConfirmInfo(false, "RDI CONFIRM ERROR:" + e.Message + ", " + e.Source + ", " + e.StackTrace); ;
        }

    }

    public void InterpretRound()
    {
        Debug.Log("InterpretRound CALLED");

        try
        {


            turnsData = new List<TurnData>();

            for (int i = 0; i < playersOrder.Count; i++)
            // i represents turn index
            {
                // Turn variables
                int turnNumber;
                string readersName;
                string readersText;
                bool wasInFact;
                string writtenBy;
                List<string> trueVoters = new List<string>();
                List<string> lieVoters = new List<string>();
                List<string> xVoters = new List<string>();

                List<string> sTrueVoters = new List<string>();
                List<string> sLieVoters = new List<string>();
                List<string> sXVoters = new List<string>();

                List<string> correctVoters = new List<string>();
                List<string> incorrectVoters = new List<string>();
                List<string> didntVoteVoters = new List<string>();
                List<string> fastestCorrectVoters = new List<string>();

                Dictionary<string, int> playerTimesLookup = new Dictionary<string, int>();
                Dictionary<string, List<Achievement>> achievementsUnlocked = new Dictionary<string, List<Achievement>>();


                string saboName; // NULL if no saboteur
                bool s_badSabo = false; // Lie subverted
                bool s_mostEarned = false;
                bool s_allEarned = false;

                bool p_fiftyfiftyEarned = false;
                bool p_mostEarned = false;
                bool p_allEarned = false;
                bool p_nobodyVoted = false;
                //

                // Player whose turn
                Player p = playersOrder[i];

                turnNumber = i + 1;
                readersName = p.playerName;
                readersText = p.text;
                wasInFact = p.isTruth ?? false;

                // Find who wrote
                writtenBy = "";
                foreach (string k in playersLookup.Keys)
                {
                    Player p1 = playersLookup[k];

                    if (p1.target == readersName)
                    {
                        writtenBy = p1.playerName;
                    }
                }

                // Sabo logic
                saboName = (writtenBy == readersName) ? null : writtenBy;
                s_badSabo = (saboName != null && wasInFact); // True if: Sabo exists & True round

                // Construct voter name lists
                foreach (string k in playersLookup.Keys)
                {
                    Player p1 = playersLookup[k];

                    switch (p1.votes[i].vote)
                    {
                        case "p":
                            break;
                        case "L":
                            lieVoters.Add(p1.playerName);
                            break;
                        case "T":
                            trueVoters.Add(p1.playerName);
                            break;
                        case "X":
                            xVoters.Add(p1.playerName);
                            break;
                        case "sL":
                            sLieVoters.Add(p1.playerName);
                            break;
                        case "sT":
                            sTrueVoters.Add(p1.playerName);
                            break;
                        case "sX":
                            sXVoters.Add(p1.playerName);
                            break;
                    }

                    playerTimesLookup.Add(k, p1.votes[i].time);
                }

                // Voter lists
                if (wasInFact)
                {
                    correctVoters.AddRange(trueVoters);
                    incorrectVoters.AddRange(lieVoters);
                }
                else
                {
                    correctVoters.AddRange(lieVoters);
                    incorrectVoters.AddRange(trueVoters);
                }

                didntVoteVoters.AddRange(xVoters);

                // Calculate fastest (correct) voters
                int quickestTime = int.MaxValue;
                foreach (string pName in correctVoters) // Calculate quickest time
                {
                    int myTime = playersLookup[pName].votes[i].time;
                    if (myTime < quickestTime) quickestTime = myTime;
                }

                if(correctVoters.Count > 1)
                {
                    foreach (string pName in correctVoters) // Add quickest voters
                    {
                        int myTime = playersLookup[pName].votes[i].time;
                        if (myTime == quickestTime) fastestCorrectVoters.Add(pName);
                    }
                }


                // Apply basic achievement logic
                int trueVotes = trueVoters.Count;
                int lieVotes = lieVoters.Count;
                int totalVotes = trueVotes + lieVotes;

                p_nobodyVoted = (totalVotes == 0);
                p_fiftyfiftyEarned = (trueVotes == lieVotes);

                if (totalVotes > 0)
                {
                    if (wasInFact) // True case
                    {
                        p_mostEarned = (lieVotes > trueVotes) && (trueVotes != 0);
                        p_allEarned = (trueVotes == 0);
                    }
                    else // Lie case
                    {
                        p_mostEarned = (lieVotes < trueVotes) && (lieVotes != 0);
                        p_allEarned = (lieVotes == 0);

                        if (saboName != null && !s_badSabo)
                        {
                            s_mostEarned = p_mostEarned;
                            s_allEarned = p_allEarned;
                        }
                    }
                }

                // Apply full achievements logic
                AchievementAwarder aa = new AchievementAwarder(this.playersOrder, readersName, wasInFact, writtenBy, saboName,
                    s_badSabo, s_mostEarned, s_allEarned, p_fiftyfiftyEarned, p_mostEarned, p_allEarned, p_nobodyVoted,
                    trueVoters, lieVoters, xVoters, correctVoters, incorrectVoters, didntVoteVoters, fastestCorrectVoters);
                aa.ApplyLogic();
                achievementsUnlocked = aa.achievementsUnlocked;

                // Add to TurnData
                TurnData td = new TurnData(turnNumber, readersName, readersText, wasInFact, writtenBy,
                    trueVoters, lieVoters, xVoters, fastestCorrectVoters, playerTimesLookup,
                    quickestTime, this.rtm,
                    correctVoters, incorrectVoters, didntVoteVoters,
                    saboName, s_badSabo, s_mostEarned, s_allEarned,
                    sTrueVoters, sLieVoters, sXVoters,
                    achievementsUnlocked,
                    p_fiftyfiftyEarned, p_mostEarned, p_allEarned, p_nobodyVoted);

                td.MakeVoters();
                turnsData.Add(td);
            }

            // Calculate results data

            resultsData = new ResultsData(turnsData, playersOrder, this.scores);
            resultsData.Calculate();

        }catch(Exception e)
        {

            Debug.Log("Interpret round ERROR: "+e.Message+", "+e.Source+", "+e.StackTrace);
        }
    }

    private void AddToFailureMessage(string msg)
    {
        failureMessage += DateTime.UtcNow + " : " + msg + "\n";
    }




}
