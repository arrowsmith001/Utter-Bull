
using System.Collections.Generic;

public class VoteData
{
    public static bool ContainsVote(List<VoteData> list, string vote)
    {
        foreach (VoteData vd in list)
        {
            if (vd.vote == vote) return true;
        }

        return false;
    }

    public static List<int> WhereIsVote(List<VoteData> list, string vote)
    {
        List<int> outList = new List<int>();

        for(int i = 0; i < list.Count; i++)
        {
            if (list[i].vote == vote) outList.Add(i);
        }

        return outList;
    }

    public VoteData() { }

    public VoteData(string vote, int time)
    {
        this.vote = vote;
        this.time = time;
    }

    public string vote;
    public int time;

}