using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class LOADING : Fragment
{
    public const string ID = "LOAD";
    public override string GetID() { return ID; }


    // Start is called before the first frame update
    void Start()
    {
        base.GetMain();

        //InitialiseViews();

        //SetListeners();


    }


    public bool ReactToPhaseChange(string newPhase)
    {
        Debug.Log(this.GetID() + ": ReactToPhaseChange: newPhase: " + newPhase);
        try
        {
            main.RequestFragmentFromState(newPhase);

            return true;
        }
        catch (Exception e)
        {
            return false;
        }

    }

    public override void RoomChangeResult(string code, Task task, bool succes, object args)
    {
        
    }

    public override void Initialise()
    {

    }

    public override void RegisterEventListeners()
    {

    }

    public override void UnregisterEventListeners()
    {

    }
}
