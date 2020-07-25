using System;
using System.Collections.Generic;

internal class AchievementAwarder
{
    private List<Player> playersWhovePlayed;
    private string readersName;
    private bool wasInFact;
    private string writtenBy;
    private List<string> trueVoters;
    private List<string> lieVoters;
    private List<string> xVoters;
    private List<string> correctVoters;
    private List<string> incorrectVoters;
    private List<string> didntVoteVoters;
    private List<string> fastestCorrectVoters;

    private string saboName;
    private bool s_badSabo;
    private bool s_mostEarned;
    private bool s_allEarned;
    private bool p_fiftyfiftyEarned;
    private bool p_mostEarned;
    private bool p_allEarned;
    private bool p_nobodyVoted;

    public AchievementAwarder(List<Player> playersWhovePlayed, string readersName, bool wasInFact, string writtenBy, string saboName, bool badSabo, bool mostEarned, bool allEarned, bool p_fiftyfiftyEarned, bool p_mostEarned, bool p_allEarned, bool p_nobodyVoted, List<string> trueVoters, List<string> lieVoters, List<string> xVoters, List<string> correctVoters, List<string> incorrectVoters, List<string> didntVoteVoters, List<string> fastestCorrectVoters)
    {
        this.playersWhovePlayed = playersWhovePlayed;
        this.readersName = readersName;
        this.wasInFact = wasInFact;
        this.writtenBy = writtenBy;
        this.saboName = saboName;
        this.s_badSabo = badSabo;
        this.s_mostEarned = mostEarned;
        this.s_allEarned = allEarned;
        this.p_fiftyfiftyEarned = p_fiftyfiftyEarned;
        this.p_mostEarned = p_mostEarned;
        this.p_allEarned = p_allEarned;
        this.p_nobodyVoted = p_nobodyVoted;
        this.trueVoters = trueVoters;
        this.lieVoters = lieVoters;
        this.xVoters = xVoters;
        this.correctVoters = correctVoters;
        this.incorrectVoters = incorrectVoters;
        this.didntVoteVoters = didntVoteVoters;
        this.fastestCorrectVoters = fastestCorrectVoters;

        LoadAchievements();
    }


    internal Dictionary<string, List<Achievement>> achievementsUnlocked;

    Achievement v_correctVoteTrue, v_correctVoteLie, v_minorityVote, v_fastestVote;
    Achievement s_changedToTrue, s_mostVote, s_allVote;
    Achievement p_zeroVote, p_splitVote, p_mostVote, p_allVote;

    private void LoadAchievements()
    {
        v_correctVoteTrue = new Achievement("Got To The Truth", "You voted correctly!", 30); // Player voted correctly
        v_correctVoteLie = new Achievement("Lie Detector", "You voted correctly!", 30); // Player voted correctly
        v_minorityVote = new Achievement("Against The Grain", "You voted correctly in the minority!", 20); // Player voted correctly in minority of overall vote
        v_fastestVote = new Achievement("Finger On The Button", "You voted correctly the quickest!", 20);

        s_changedToTrue = new Achievement("Saboteur Sabotaged", "The lie you wrote turned out to be true!", -30); // Saboteurs lie turned out to be true
        s_mostVote = new Achievement("Great Work Of Fiction", "Most people fell for a lie you wrote!", 30); // Saboteurs lie mostly convinced the group true
        s_allVote = new Achievement("Biggest Lie Ever Sold", "Everyone fell for a lie you wrote!", 50); // Saboteurs lie wholly convinced the group true

        p_zeroVote = new Achievement("Complete Waste of Time", "Nobody voted. Oh well, here's some points.", 20);
        p_splitVote = new Achievement("House Divided", "You fooled half the room!", 20); // Player's statement split the group 50/50
        p_mostVote = new Achievement("Professional Actor", "You fooled most of the room!", 60); // Player's statement mostly convinced the group the opposite
        p_allVote = new Achievement("Oscar-Worthy", "You fooled everyone!", 100); // Player's statement wholly convinced the group the opposite

    }

    internal void ApplyLogic()
    {
        achievementsUnlocked = new Dictionary<string, List<Achievement>>();

        foreach(Player p in playersWhovePlayed)
        {
            List<Achievement> achievements = new List<Achievement>();

            bool isTurn = false;
            bool isSabo = false;
            bool isVoter = false;

            if (p.playerName == readersName) isTurn = true;
            else if (saboName != null && p.playerName == saboName) isSabo = true;
            else isVoter = true;

            if(isTurn)
            {
                if (p_nobodyVoted) achievements.Add(p_zeroVote);
                else if (p_fiftyfiftyEarned) achievements.Add(p_splitVote);
                else if (p_mostEarned) achievements.Add(p_mostVote);
                else if (p_allEarned) achievements.Add(p_allVote);
                        
            }
            else if(isSabo)
            {
                if(s_badSabo)
                {
                    achievements.Add(s_changedToTrue);
                }
                else
                {
                    if (s_mostEarned) achievements.Add(s_mostVote);
                    else if (s_allEarned) achievements.Add(s_allVote);
                }
            }
            else if(isVoter)
            {

                int trueCount = trueVoters.Count;
                int lieCount = lieVoters.Count;
                int totalCount = trueCount + lieCount;

                if(wasInFact)
                {
                    if(correctVoters.Contains(p.playerName))
                    {
                        achievements.Add(v_correctVoteTrue);

                        if(trueCount < lieCount)
                        {
                            achievements.Add(v_minorityVote);
                        }

                        if(fastestCorrectVoters.Contains(p.playerName))
                        {
                            achievements.Add(v_fastestVote);
                        }


                    }
                }
                else
                {
                    if (correctVoters.Contains(p.playerName))
                    {
                        achievements.Add(v_correctVoteLie);

                        if (trueCount > lieCount)
                        {
                            achievements.Add(v_minorityVote);
                        }

                        if (fastestCorrectVoters.Contains(p.playerName))
                        {
                            achievements.Add(v_fastestVote);
                        }

                    }
                }
            }

            // Add to dictionary
            achievementsUnlocked.Add(p.playerName, achievements);
        }



    }
}