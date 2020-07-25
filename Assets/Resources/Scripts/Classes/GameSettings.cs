using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings {
    
    public int rtm; // Round Time Minutes

    public bool ate; // All True Enabled

    public bool le; // Lewdness Enabled


    public GameSettings() {
        this.rtm = 3;
        this.ate = true;
        this.le = false;
    }
    
    public GameSettings(int roundTimeMins, bool allTrueEnabled, bool lewdNessEnabled) {
        this.rtm = roundTimeMins;
        this.ate = allTrueEnabled;
        this.le = lewdNessEnabled;
    }

    public bool Compare(GameSettings gs)
    {
        if (gs == null) return false;
        if (this.rtm == gs.rtm) return false;
        if (this.ate == gs.ate) return false;
        if (this.le == gs.le) return false;
        return true;
    }
    
}
