using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class TestGround : MonoBehaviour
{
    private Dictionary<string, List<VoteData>> votesLookup = new Dictionary<string, List<VoteData>>();
    private Dictionary<string, Player> pLookup = new Dictionary<string, Player>();
    private List<Player> playersOrder = new List<Player>();
    private ScoreKeeper scores = new ScoreKeeper();

    // Start is called before the first frame update
    void Start()
    {
        playersOrder.Add(new Player("B"));
        playersOrder.Add(new Player("A"));
        playersOrder.Add(new Player("C"));

        foreach(Player p in playersOrder)
        {
            this.votesLookup.Add(p.playerName, new List<VoteData>());
            this.pLookup.Add(p.playerName, p);

            pLookup[p.playerName].votes = votesLookup[p.playerName];
        }

        pLookup["A"].isTruth = false;
        pLookup["B"].isTruth = false;
        pLookup["C"].isTruth = true;

        pLookup["A"].text = "A text";
        pLookup["B"].text = "B text";
        pLookup["C"].text = "C text";

        pLookup["A"].target = "B";
        pLookup["B"].target = "A";

        votesLookup["A"].Add(new VoteData("p", 56));
        votesLookup["B"].Add(new VoteData("sT", 56));
        votesLookup["C"].Add(new VoteData("L", 56));

        votesLookup["A"].Add(new VoteData("sL", 56));
        votesLookup["B"].Add(new VoteData("p", 56));
        votesLookup["C"].Add(new VoteData("T", 56));

        votesLookup["A"].Add(new VoteData("T", 56));
        votesLookup["B"].Add(new VoteData("L", 56));
        votesLookup["C"].Add(new VoteData("p", 56));

        scores.Add("A", 0);
        scores.Add("B", 0);
        scores.Add("C", 0);
        scores.updated = false;

        try
            {

           // bool b = RoundDataInterpretter.ConfirmSufficientInformation(votesLookup, pLookup, playersOrder, new GameSettings(), scores);
          //  Debug.Log("PASSED: " + b);


            RoundDataInterpretter rdi = new RoundDataInterpretter(
                votesLookup, pLookup, playersOrder, 3, scores);

            rdi.InterpretRound();

            string json = JsonConvert.SerializeObject(rdi);
            Debug.Log(json);

        }catch(Exception e) { Debug.Log("RDI ERROR: " + e.Message); }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
