using System;
using System.Collections.Generic;

public class ResultsData
{

    // INPUTS
    public List<TurnData> turnsData;
    public Dictionary<string, int> oldScores;
    private bool newScoresAlreadyObtained;

    // OUTPUTS
    public Dictionary<string, int> newScores;
    public Dictionary<string, int> scoreDiff;
    public List<Player> playersOrderedWithNewScores;
    private List<Player> playersOrder;
    private ScoreKeeper scores;

    public ResultsData(List<TurnData> turnsData, List<Player> playersOrder, ScoreKeeper scores)
    {
        this.turnsData = turnsData;
        this.playersOrder = playersOrder;
        this.scores = scores;
        this.oldScores = new Dictionary<string, int>(this.scores.playerScores);
        this.newScoresAlreadyObtained = scores.updated;
    }

    internal void Calculate()
    {
        newScores = new Dictionary<string, int>();
        scoreDiff = new Dictionary<string, int>();

        foreach(Player p in playersOrder)
        {
            string pName = p.playerName;

            int score = 0;

            foreach(TurnData td in turnsData)
            {
                List<Achievement> achievements = td.achievementsUnlocked[pName];

                foreach(Achievement a in achievements)
                {
                    score += a.pointsWorth;
                }
            }

            if(newScoresAlreadyObtained)
            {
                // New scores are the old scores
                newScores.Add(pName, (oldScores[pName]));

                // Reverse engineer to get old scores
                oldScores.Remove(pName);
                oldScores.Add(pName, newScores[pName] - score);

            }
            else
            {
                // New scores are old scores + calculated score
                newScores.Add(pName, score + (oldScores[pName]));

                // Old scores can stay as they are
            }

            // Score diff simply is the score
            scoreDiff.Add(pName, score);
        }

        // Figure out order
        playersOrderedWithNewScores = new List<Player>();

        for(int i = 0; i < playersOrder.Count; i++)
        {
            Player highestScorer = null;
            int highScore = int.MinValue;

            foreach(Player p in playersOrder)
            {
                string pName1 = p.playerName;

                if(newScores[pName1] > highScore
                    && !playersOrderedWithNewScores.Contains(p))
                {
                    highestScorer = p;
                    highScore = newScores[pName1];
                    p.points = highScore;
                }
            }

            playersOrderedWithNewScores.Add(highestScorer);
        }





    }
}