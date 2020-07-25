using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionRequester : MonoBehaviour
{
    // Reference to the Prefab. Drag a Prefab into this field in the Inspector.
    public string FragPrefabName;
    public TransitionManager TransitionManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Request()
    {
        TransitionManager.TransitionTo(FragPrefabName);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
