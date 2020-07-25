using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class GP5_REVEALS_INTRO : Fragment
{
    public const string ID = "GP5";
    public override string GetID() { return ID; }


    public GameObject Main_Panel;
    public GameObject Pos_Main;
    public PositionsHolder ph;

    public Button Button_ReadyForTruth;


    public override void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        main.PlayMusic(null, 0);
        main.PlaySfx("tribal_drums", 1);


        SetListeners();
    }

    void SetListeners()
    {
        Button_ReadyForTruth.onClick.AddListener(() =>
        {
            Button_ReadyForTruth.interactable = false;
            main.rc.SetReadyStatus(true, this);
            main.PlaySfx("button_major", 1);
        });
    }


    public override void Initialise()
    {
        if(main.data.rdi != null)
        {
            ShowButton();
        }
    }

    public override void RegisterEventListeners()
    {
        main.data.rdiReadyEvent.AddListener(OnRDIReady);
        main.data.phaseChangeEvent.AddListener(OnPhaseChanged);
        main.data.allReadyEvent.AddListener(AllReadyChanged);
    }

    private void AllReadyChanged(string forPhase, bool isHost)
    {
        if(forPhase == "RevealsIntro" && isHost) main.rc.ChangeRevealsPage(new int[2] { 0, 1 }, this);
    }

    private void OnPhaseChanged(string from, string to)
    {
        if (to == "Lobby") GoToLobby();
        if (to == "Reveals") main.RequestFragmentDirect(GP6_REVEALS_BASE.ID);

    }

    private void GoToLobby()
    {
        main.rc.ResetVars(this);
        main.RequestFragmentDirect(WAITING_ROOM.ID);
    }

    private void OnRDIReady()
    {
        ShowButton();
    }

    public override void UnregisterEventListeners()
    {
        main.data.rdiReadyEvent.RemoveListener(OnRDIReady);
        main.data.phaseChangeEvent.RemoveListener(OnPhaseChanged);
    }

    private bool buttonShowInitiated = false;
    private void ShowButton()
    {
        if (buttonShowInitiated) return;

        buttonShowInitiated = true;
        StartCoroutine(ShowButtonCoroutine());
    }

    IEnumerator ShowButtonCoroutine()
    {
        yield return new WaitForSeconds(2);

        main.PlaySfx("tribal_drums", 1);
        Button_ReadyForTruth.gameObject.SetActive(true);

    }

    public override void RoomChangeResult(string code, Task task, bool succes, object args)
    {
        switch (code)
        {
            case "ChangeRevealsPage":

                if(succes)
                {
                    main.rc.ChangeScores(main.data.rdi.resultsData.newScores, true, this);
                }

                break;
            case "ChangeScores":

                if (succes)
                {
                    main.rc.ChangePhase("Reveals", this);
                }
                else
                {
                    // TODO
                }

                break;
        }
        
    }

}
