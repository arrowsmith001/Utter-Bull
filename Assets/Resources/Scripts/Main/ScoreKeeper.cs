using System;
using System.Collections.Generic;

public class ScoreKeeper
{
    public ScoreKeeper()
    {

    }

    public Dictionary<string, int> playerScores = new Dictionary<string, int>();
    public bool updated = false;

    public void Add(string hostName, int v)
    {
        playerScores.Add(hostName, v);
    }
}