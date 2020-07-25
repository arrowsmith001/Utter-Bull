using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manifest<T> {
    
    private string refresh;
    
    private Dictionary<string, T> players;
    
    public Manifest() {
        
    }
    
    public Manifest(Dictionary<string, T> hashMap) {
        this.refresh = "";
        this.players = hashMap;
    }
    
    public Dictionary<string, T> getPlayers() {
        return this.players;
    }
    
    public void setPlayers(Dictionary<string, T> players) {
        this.players = players;
    }
}