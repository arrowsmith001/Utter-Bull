using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoleAssigner : MonoBehaviour
{
    // INPUTS
    List<string> playersList = new List<string>();
    public bool allTrueAllowed;

    // OUTPUTS
    public Dictionary<string, bool?> playerTruths = new Dictionary<string, bool?>();
    public Dictionary<string, string> playerTargets = new Dictionary<string, string>();

    System.Random rand;

    public void AssignRoles(List<string> list)
    {
        this.playersList = list;

        rand = new System.Random();

        int numberOfPlayers = this.playersList.Count;
        List<int> lieIndices = new List<int>();
        List<int> trueIndices = new List<int>();

        int lieCount;
        do
        {
            lieCount = 0;
            lieIndices.Clear();
            trueIndices.Clear();
            for (int i = 0; (i < numberOfPlayers); i++)
            {
                double randomNum = rand.NextDouble();
                if ((randomNum <= 0.5))
                {
                    trueIndices.Add(i);
                }
                else
                {
                    lieCount++;
                    lieIndices.Add(i);
                }

            }

        } while ((!this.allTrueAllowed
                    && (lieCount == 0)));

        if ((lieCount == 1))
        {
            int newIndex = ((int)((trueIndices.Count * rand.NextDouble())));

            lieIndices.Add(trueIndices[newIndex]);
            trueIndices.Remove(trueIndices[newIndex]);

            lieCount++;
        }

        foreach (int i in trueIndices)
        {
            playerTruths.Add(playersList[i], true);
        }
        foreach (int i in lieIndices)
        {
            playerTruths.Add(playersList[i], false);
        }

        if ((lieCount != 0))
        {
            AssignTargets(lieIndices, trueIndices);
        }
        else
        {
            playerTargets = null;
        }

    }

    private void AssignTargets(List<int> lieIndices, List<int> trueIndices)
    {
        //  Creates permutations until a derangement is found
        List<int> targets = new List<int>(lieIndices);

        bool isDerangement = false;

        while (!isDerangement)
        {
            targets = Shuffle(targets);

            isDerangement = true;

            for (int i = 0; (i < targets.Count); i++)
            {
                if (lieIndices[i] == targets[i])
                {
                    isDerangement = false;
                }
            }

        }

        //  Assigns targets to players
        for (int i = 0; i < lieIndices.Count; i++)
        {
            string player = playersList[lieIndices[i]];
            string target = playersList[targets[i]];

            playerTargets.Add(player, target);
        }

        //  end
    }

    public List<T> Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rand.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }

        return list;
    }

    public void Reset()
    {
        this.playerTargets.Clear();
        this.playerTruths.Clear();
        this.playersList.Clear();
    }
}
