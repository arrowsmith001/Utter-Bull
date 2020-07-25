using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class CONFIRM_LIE : MonoBehaviour
{
    private GameObject mainCanvas;
    private MainScript main;

    public Button Button_Confirm_True;
    public Button Button_Confirm_Bull;
    public Text Text_Statement;

    // Start is called before the first frame update
    void Start()
    {

        GameObject root = this.transform.root.gameObject;
        mainCanvas = root.transform.Find("MAIN_CANVAS").gameObject;
        main = mainCanvas.GetComponent<MainScript>();

        SetListeners();
    }

    void SetListeners()
    {
        Button_Confirm_True.onClick.AddListener(() =>
        {
            main.rc.ChangeTruth(true, main.playerName, main.tm.newFrag.GetComponent<Fragment>());
            main.CloseCurrentDialog();
        });
        Button_Confirm_Bull.onClick.AddListener(() =>
        {
            main.rc.ChangeTruth(false, main.playerName, main.tm.newFrag.GetComponent<Fragment>());
            main.CloseCurrentDialog();
        });
    }

    bool statementSet = false;

    // Update is called once per frame
    void Update()
    {
        while(!statementSet)
        {
            try
            {
                Text_Statement.text = main.data.playersLookup[main.playerName].text;

                statementSet = true;

            }catch(Exception e)
            {
                Debug.Log(e.Message);
            }
        }
        
    }
}
