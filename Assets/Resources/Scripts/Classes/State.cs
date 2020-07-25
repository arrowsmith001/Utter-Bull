using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State {
    
    //  Whether game is in session, or not
    public bool InSession;
    
    //  State of the game
    public string phase;

    
    public State() {
        
    }
    
    public State(bool InSession) {
        this.InSession = InSession;
    }
    
}