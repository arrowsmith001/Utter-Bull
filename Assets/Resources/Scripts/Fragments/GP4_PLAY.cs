using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using frame8.ScrollRectItemsAdapter.Classic.Examples;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GP4_PLAY : Fragment
{
    public const string ID = "GP4";
    public override string GetID() { return ID; }

    bool debugVars = false;

    public PositionsHolder ph = new PositionsHolder();
    public GameObject Pos_Main;
    public LeanTweenType easeType;

    public GameObject Layout_List;
    public GameObject Layout_MiddleMain;
    public GameObject Layout_TextBoxContent;
    public GameObject Layout_MiddlePanel;
    public GameObject Layout_BottomButtons;
    public GameObject Dialog_Panel;
    public GameObject Waiting_Panel;

    public Button Button_Continue;
    public Button Button_VoteBull;
    public Button Button_VoteTrue;
    public Button Button_Toggle_ReadyList;

    public VerticalListView_ReadyList listScript;

    public Image Img_Layout_ProgressBar;
    public Image ProgressBar;

    public Text Text_Statement;
    public Text Text_WhoseTurn;
    public Text Text_AboveButtons;
    public Text Text_Top;
    public Text Text_ClockLeft;
    public Text Text_ClockRight;
    public Text Text_NumReady;
    public Text Text_NumPlayers;
    public Text Text_OverContinue;

    public Image arrowLeft;
    public Image arrowRight;

    public List<GameObject> dialogs;
    GameObject RoundOverDialogGo;
    GameObject TimeOutDialogGo;

    bool uiInitialised = false;

    public override void Awake()
    {
        base.Awake();

        RoundOverDialogGo = dialogs[0];
        TimeOutDialogGo = dialogs[1];

        InitialiseViews();
    }

    public override void Start()
    {
        base.Start();

        // SetListeners();

    }

    public override void Initialise()
    {
        OnPlayersChanged();
        OnPhaseChanged(null, main.data.currentPhase ?? "");
        OnReadiesChanged(0, main.data.readyCount);
    }

    public override void RegisterEventListeners()
    {
        main.data.playersChangeEvent.AddListener(OnPlayersChanged);
        main.data.phaseChangeEvent.AddListener(OnPhaseChanged);
        main.data.votesChangedEvent.AddListener(OnVotesChanged);
        main.data.readiesChangeEvent.AddListener(OnReadiesChanged);
        main.data.allReadyEvent.AddListener(OnAllReady);
        main.data.whoseTurnChangedEvent.AddListener(OnWhoseTurnChanged);
    }

    private void OnWhoseTurnChanged()
    {
        OnReadiesChanged(0, main.data.readyCountMinusWhoseTurn);
    }

    private void OnAllReady(string forPhase, bool isHost)
    {
        if (forPhase == "Play") main.rc.ChangePhase("EndOfPlay", this);

        if(isHost)
        {
            if (forPhase == "EndOfPlay") main.rc.ChangePhase("ConfirmResetVariables", this);
            if (forPhase == "ConfirmResetVariables") main.rc.SetNewTurn(main.data.GetNextTurn(), this);
        }

    }

    private void OnPlayersChanged()
    {
        try { this.phase = main.rl.room.phase; } catch (Exception e) { Debug.Log("ERROR LINE 1: " + e.Message); }

        try{ rtm = main.data.currentSettings.rtm;} catch (Exception e) { Debug.Log("ERROR LINE 2: " + e.Message); }
        try { myTurn = main.data.GetMe().isTurn; } catch (Exception e) { Debug.Log("ERROR LINE 3: " + e.Message); }
        try{  myTarget = main.data.GetMe().target; } catch (Exception e) { Debug.Log("ERROR LINE 4: " + e.Message); }
        try{ whoseTurn = main.data.currentTurn.playerName;  }catch (Exception e) { Debug.Log("ERROR LINE 5: " + e.Message); }
        try {  phase = main.data.currentPhase; } catch (Exception e) { Debug.Log("ERROR LINE 6: " + e.Message); }
        try  {  isSabo = (myTarget == whoseTurn && myTarget != main.playerName); } catch (Exception e) { Debug.Log("ERROR LINE 7: " + e.Message); }
        try {   isReady = main.data.GetMe().isReady; } catch (Exception e) { Debug.Log("ERROR LINE 8: " + e.Message); }
        try {  if(!hasVoted) hasVoted = main.data.HaveIVoted();  } catch (Exception e) { Debug.Log("ERROR LINE 9: " + e.Message); }

        if (!uiInitialised)
        {
            try
            {
                ToggleReadyList(false, myTurn);
            }
            catch (Exception e) { Debug.Log("ERROR LINE 11: " + e.Message); }
            try
            {
                Button_Toggle_ReadyList.interactable = !myTurn;
            }
            catch (Exception e) { Debug.Log("ERROR LINE 12: " + e.Message); }

            uiInitialised = true;
        }

        try {  Text_OverContinue.text = "Waiting for " + whoseTurn + " to continue"; } catch (Exception e) { Debug.Log("ERROR LINE 13: " + e.Message); }

        try{  Button_Continue.gameObject.SetActive(myTurn && !Waiting_Panel.activeInHierarchy); } catch (Exception e) { Debug.Log("ERROR LINE 14: " + e.Message); }
        try {  Text_OverContinue.gameObject.SetActive(!myTurn && !Waiting_Panel.activeInHierarchy); } catch (Exception e) { Debug.Log("ERROR LINE 15: " + e.Message); }

        // Player vote
        try {
            if (myTurn && !hasVoted)
            {
                StartCoroutine(DelayedPlayerVote(0));
            }
        }catch (Exception e) { Debug.Log("ERROR LINE 16: " + e.Message); }

        try{ SetListeners();}catch (Exception e) { Debug.Log("ERROR LINE 17: " + e.Message); }

        try { if (hasVoted && !ReadyListShowing) VotedState(); } catch (Exception e) { Debug.Log("ERROR LINE 10: " + e.Message); }
    }

    private void OnPhaseChanged(string from, string to)
    {
        /**/
        this.phase = to;

        if (phase == "Lobby") GoToLobby();
        else if (phase == "Play")
        {
            try
            {
                Text_Statement.text = main.data.currentTurn.text;
                Text_WhoseTurn.text = main.data.currentTurn.playerName;
                Text_Top.text = "WAITING FOR EVERYONE TO VOTE";
            }
            catch (Exception e) { Debug.Log("ERROR LINE A: " + e.Message); }

            if (!Playing)
            {
                try { OnPlay(); } catch (Exception e) { Debug.Log("ERROR LINE C: " + e.Message); }
            }
        }
        else if (phase == "EndOfPlay")
        {
            Text_Top.text = "ROUND OVER";

            if (!EndingPlay)
            {
                try { OnEndOfPlay(); } catch (Exception e) { Debug.Log("ERROR LINE D: " + e.Message); }
            }
            else
            {
                if (isReady)
                {
                    try { ActivateWaiting(); } catch (Exception e) { Debug.Log("ERROR LINE E: " + e.Message); }
                }
            }
        }
        else if (phase == "ConfirmResetVariables")
        {
            if (!ConfirmingRVs) OnConfirmResetVariables();

            Text_Top.text = "ROUND OVER";

            Text_ClockLeft.text = "0:00";
            Text_ClockRight.text = ":00";

            ActivateWaiting();

            Debug.Log("WAITING PANEL");
        }
        else if(phase == "Choose")
        {
            main.RequestFragmentDirect(GP3_CHOOSE_WHOSE_TURN.ID);
        }
        else if(phase == "RevealsIntro")
        {
            main.RequestFragmentDirect(GP5_REVEALS_INTRO.ID);
        }
        

    }


    private void GoToLobby()
    {
        main.rc.ResetVars(this);
        main.RequestFragmentDirect(WAITING_ROOM.ID);
    }

    private void OnReadiesChanged(int from, int to)
    {
        try
        {
            //if (from < to)
            //{
            if(Text_NumReady.text == "-"
                || int.Parse(Text_NumReady.text) < main.data.readyCountMinusWhoseTurn)
            {
                Text_NumReady.text = main.data.readyCountMinusWhoseTurn.ToString();
                if (CountDown) main.PlaySfx("ready_up", 1);
            }
            //}
            Text_NumPlayers.text = (main.data.playersList.Count - 1).ToString();
        }
        catch (Exception e) { Debug.Log("ERROR LINE Z: " + e.Message); }


        try
        {
            if(main.data.currentPhase != "ConfirmResetVariables" && main.data.currentPhase != "Choose")
            {
                listScript.playerList = main.data.readiesListMinusWhoseTurn;
                listScript.RefreshList();
            }

        }
        catch (Exception e) { Debug.Log("ERROR LINE B: " + e.Message); }
    }

    private void OnVotesChanged()
    {

    }


    public override void UnregisterEventListeners()
    {
        main.data.playersChangeEvent.RemoveListener(OnPlayersChanged);
        main.data.phaseChangeEvent.RemoveListener(OnPhaseChanged);
        main.data.votesChangedEvent.RemoveListener(OnVotesChanged);
        main.data.readiesChangeEvent.RemoveListener(OnReadiesChanged);
        main.data.whoseTurnChangedEvent.RemoveListener(OnWhoseTurnChanged);
    }

    DateTime TurnStartTime;

    void InitialiseTime()
    {
        string TurnStartTimeSaved = main.GetPrefString("TurnStartTime", "");

        Debug.Log("TurnStartTimeSaved:" + TurnStartTimeSaved);

        if(TurnStartTimeSaved == "")
        {
            TurnStartTime = DateTime.UtcNow;
            string TurnTimeSerializable = TurnStartTime.ToString();

            main.SavePrefString("TurnStartTime", TurnTimeSerializable);

        }
        else
        {
            TurnStartTime = System.Convert.ToDateTime(TurnStartTimeSaved);
        }

        TimeSpan diff = DateTime.UtcNow - TurnStartTime;
        int msElapsed = (int)diff.TotalMilliseconds;
        int sElapsed = (int)Mathf.Ceil(((float)msElapsed) / 1000);

        if(main.data.GetMe().isTurn && sElapsed < 15)
        {
            try
            {
                bool b = (bool)main.data.GetMe().isTruth;

                if (b)
                {
                    main.dm.RequestDialog("CONFIRM_TRUTH");
                }
                else
                {
                    main.dm.RequestDialog("CONFIRM_LIE");
                }
            }
            catch { }
        }

        CountDown = true;
    }


    void InitialiseViews()
    {
        ph.AddPositions(Pos_Main);
    }


    bool myTurn;
    bool isSabo;
    string myTarget;
    string whoseTurn;
    string phase;

    bool BullButtonListenerAdded = false;
    bool TrueButtonListenerAdded = false;
    bool ContinueButtonListenerAdded = false;
    bool ReadylistButtonListenerAdded = false;

    // BUTTON TASKS
    void SetListeners()
    {

        // If not playing
        if (!myTurn)
        {

            if (!BullButtonListenerAdded)
            {
                Button_VoteBull.onClick.AddListener(OnButtonVoteBull);
                BullButtonListenerAdded = true;
            }

            if (!TrueButtonListenerAdded)
            {
                Button_VoteTrue.onClick.AddListener(OnButtonVoteTrue);
                TrueButtonListenerAdded = true;
            }

            
        }
        else
        {
            if (!ContinueButtonListenerAdded)
            {
                Button_Continue.onClick.AddListener(OnButtonContinue);
                ContinueButtonListenerAdded = true;
            }
        }

        if(phase == "Play")
        {
            if (!ReadylistButtonListenerAdded)
            {
                Button_Toggle_ReadyList.onClick.AddListener(OnButtonReadyList);
                ReadylistButtonListenerAdded = true;
            }
        }

    }
    void OnButtonContinue()
    {
        main.rc.ChangePhase("ConfirmResetVariables", this);
    }

    void OnButtonVoteBull()
    {
        hasVoted = true;
        main.SavePrefString("hasVoted", "True");

        if (!isSabo) main.rc.AddVote("L", sElapsed, this);
        else main.rc.AddVote("sL", sElapsed, this);

        SetVoteButtonsEnabled(false);

    }
    void OnButtonVoteTrue()
    {
        hasVoted = true;
        main.SavePrefString("hasVoted", "True");

        if (!isSabo) main.rc.AddVote("T", sElapsed, this);
        else main.rc.AddVote("sT", sElapsed, this);

        SetVoteButtonsEnabled(false);

    }
    void OnXVote()
    {
        Debug.Log("ON X VOTE CALLED");

        hasVoted = true;
        main.SavePrefString("hasVoted", "True");
        if(!isSabo) main.rc.AddVote("X", rtm*60, this);
        else main.rc.AddVote("sX", rtm * 60, this);

        SetVoteButtonsEnabled(false);
    }
    void OnButtonReadyList()
    {
        ToggleReadyList(true, null);
    }

    bool hasVoted = false;

    // UI ///////////////////////////////////////

    bool ReadyListShowing = false;
    void ToggleReadyList(bool animate, bool? set)
    {
        if(set == null)
        {
            if(ReadyListShowing) // If showing, hide
            {
                if (animate)
                {
                    Tween(Layout_MiddleMain.transform, "MiddleMain_1",
                        0.25f, this.ph, 0, new OvershootInterpolator(2), true, true, null);
                    Tween(Layout_List.transform, "MiddleMain_4",
                        0.25f, this.ph, 0, new OvershootInterpolator(2), true, true, null);
                }
                else
                {
                    SetTransformPos(Layout_MiddleMain.transform, "MiddleMain_1", this.ph);
                    SetTransformPos(Layout_List.transform, "MiddleMain_4", this.ph);
                }

                Color c1 = arrowLeft.color;
                c1.a = 1f;
                arrowLeft.color = c1;

                Color c2 = arrowRight.color;
                c2.a = 0.5f;
                arrowRight.color = c2;

            }
            else // If hiding, show
            {
                if (animate)
                {
                    Tween(Layout_MiddleMain.transform,"MiddleMain_4", 
                        0.25f, this.ph, 0, new OvershootInterpolator(2), true, true, null);
                    Tween(Layout_List.transform, "MiddleMain_1",
                        0.25f, this.ph, 0, new OvershootInterpolator(2), true, true, null);
                }
                else
                {
                    SetTransformPos(Layout_MiddleMain.transform, "MiddleMain_4", this.ph);
                    SetTransformPos(Layout_List.transform, "MiddleMain_1", this.ph);
                }

                Color c1 = arrowLeft.color;
                c1.a = 0.5f;
                arrowLeft.color = c1;

                Color c2 = arrowRight.color;
                c2.a = 1f;
                arrowRight.color = c2;
            }

            ReadyListShowing = !ReadyListShowing;
        }
        else
        {
            string pos1 = "MiddleMain_" + (((bool)set) ? "4" : "1");
            string pos2 = "MiddleMain_" + (((bool)set) ? "1" : "4");

            Debug.Log(Layout_MiddleMain.transform);
            Debug.Log(ph.positions[pos1]);

            Tween(Layout_MiddleMain.transform, pos1, 
                0.25f, this.ph, 0, new OvershootInterpolator(2), true, true, null);
            Tween(Layout_List.transform, pos2, 
                0.25f, this.ph, 0, new OvershootInterpolator(2), true, true, null);

            Color c1 = arrowLeft.color;
            c1.a = (bool)set ? 0.5f : 1f;
            arrowLeft.color = c1;

            Color c2 = arrowRight.color;
            c2.a = (bool)set ? 1f : 0.5f;
            arrowRight.color = c2;

            ReadyListShowing = (bool) set;

        }
       
    }

    void SetVoteButtonsEnabled(bool enable)
    {
        Button_VoteBull.interactable = enable;
        Button_VoteTrue.interactable = enable;
    }


    // PLAY STATE //////////////////////////////

    bool Playing = false;

    void OnPlay()
    {
        Playing = true;
        InitialiseTime();
    }


    // END OF PLAY //////////////////////////////
    bool EndingPlay = false;

    void OnEndOfPlay()
    {
        EndingPlay = true;

        try
        {

            main.PlayMusic(null, 0);

            if (rtm != 0)
            {
                TimeSpan diff = DateTime.UtcNow - TurnStartTime;

                int msElapsed = (int)diff.TotalMilliseconds;
                int msAllowed = rtm * 60000;

                msLeft = Mathf.Max(msAllowed - msElapsed, 0);

                if(msLeft == 0)
                {
                }
                else
                {
                    Bleed = true;
                }

            }

            CountDown = false;

            Tween(Layout_MiddleMain.transform, "MiddleMain_4", 
                0.5f, this.ph, 0, null, true, true, null);
            Tween(Layout_List.transform, "MiddleMain_4",
                0.5f, this.ph, 0, null, true, true, null);
            Tween(Layout_BottomButtons.transform,"BottomButtons_2", 
                0.5f, this.ph, 0, null, true, true, null);
            Tween(Layout_MiddlePanel.transform, "MiddlePanel_2", 
                0.5f, this.ph, 0, null, true, true, null);

        }catch(Exception e)
        {
            Debug.Log(e.Message + ", " + e.Source);
        }
    }

    bool ConfirmingRVs = false;

    void OnConfirmResetVariables()
    {
        ConfirmingRVs = true;
        uiFrozen = true;

        ActivateWaiting();

        main.SavePrefString("TurnStartTime", "");

        ConfirmResetVariables = true;
    }



    public override void RoomChangeResult(string code, Task task, bool succes, object args)
    {
        if(succes)
        {
            switch(code)
            {
                case "AddVote_p":

                    VotedState();

                    break;

                case "AddVote_sL":

                    VotedState();

                    break;
                case "AddVote_sT":

                    VotedState();

                    break;
                case "AddVote_L":

                    VotedState();

                    break;
                case "AddVote_T":

                    VotedState();

                    break;
                case "SetNewTurn":

                    if(args != null)
                    {
                        main.rc.ChangePhase("Choose", this);
                    }
                    else
                    {
                        main.rc.ChangePhase("RevealsIntro", this);
                    }


                    break;
            }
        }
    }

    void VotedState()
    {
        ToggleReadyList(true, true);
        Button_Toggle_ReadyList.interactable = false;
        arrowLeft.gameObject.SetActive(false);
    }


    bool isReady = false;

    IEnumerator DelayedPlayerVote(float t)
    {
        yield return new WaitForSeconds(t);

        hasVoted = true;
        main.rc.AddVote("p", 0, this);
    }

    Color32 white = new Color32(255, 255, 255, 255);
    Color32 red = new Color32(255, 64, 64, 255);
    Color32 brown = new Color32(75, 40, 40, 255);
    Color32 grey = new Color32(175, 175, 175, 255);


    void NormalFlash()
    {
        try
        {
            LeanTween.value(0, 1, 0.75f).setDelay(3).setOnUpdate((float t) =>
            {
               try{ Color c = Color.Lerp(brown, grey, t);
                if(Img_Layout_ProgressBar!= null) Img_Layout_ProgressBar.color = c;
                } catch { }
            }).setOnComplete(() =>
            {
                LeanTween.value(0, 1, 0.75f).setOnUpdate((float t2) =>
                {
                   try{ Color c = Color.Lerp(grey, brown, t2);
                    if(Img_Layout_ProgressBar!=null) Img_Layout_ProgressBar.color = c; }catch { }
                })
                .setOnComplete(() =>
                {
                  try{  if (CountDown && !tenSecsLeft && Img_Layout_ProgressBar!= null) NormalFlash(); } catch { }

                });

            });
        }
        catch(Exception e)
        {
            Debug.Log(e.Message + ", " + e.Source + ", " + e.StackTrace);
        }

    }

    void TenSecondFlash()
    {
        try
        {
            LeanTween.value(0, 1, 0.2f).setOnUpdate((float t) =>
            {
              try{  Color c = Color.Lerp(white, red, t);
                Text_ClockLeft.color = c;
                Text_ClockRight.color = c;
                ProgressBar.color = c;  } catch { }
            }).setOnComplete(() =>
            {
                LeanTween.value(0, 1, 0.2f).setOnUpdate((float t) =>
                {
                  try{  Color c = Color.Lerp(red, white, t);
                    Text_ClockLeft.color = c;
                    Text_ClockRight.color = c;
                    ProgressBar.color = c; } catch { }
                })
                .setOnComplete(() =>
                {
                  try{  if (CountDown) TenSecondFlash(); } catch { }
                });

            });
        }catch(Exception e) { }
    }

    void OnTimerEnd()
    {
        main.PlayMusic(null, 0);

        if (!hasVoted)
        {
            // DIALOG BOX TODO

            main.PlaySfx("timer_end_alarm", 0.7f);
            OnXVote();

            TimeRanOutDialog();
        }
        else
        {
            RoundOverDialog();
            main.PlaySfx("play_end", 0.7f);

        }
        
    }

    void ActivateWaiting()
    {
        Text_OverContinue.gameObject.SetActive(false);
        Button_Continue.gameObject.SetActive(false);
        Waiting_Panel.SetActive(true);
    }

    GameObject currentDialog;

    void TimeRanOutDialog()
    {
        if (currentDialog != null) Destroy(currentDialog.gameObject);
        currentDialog = Instantiate(TimeOutDialogGo, Dialog_Panel.transform);
    }

    void RoundOverDialog()
    {
        if (currentDialog != null) Destroy(currentDialog.gameObject);
        currentDialog = Instantiate(RoundOverDialogGo, Dialog_Panel.transform);
    }

    void SetClockNumber(int m, int s, int ms)
    {
        Text_ClockLeft.text = m + ":" + s % 60;
        Text_ClockRight.text = ":" + Mathf.Floor((ms % 1000) / 10);
    }

    bool ConfirmResetVariables = false;
    bool uiFrozen = false;
    bool EndedAbruptly = false;

    bool CountDown = false;
    bool Bleed = false;

    int rtm;
    int msLeft;
    int sLeft;
    int mLeft;
    int sElapsed;
    float fractionLeft;

    bool MusicEstablished = false;
    bool tenSecsLeft = false;

    private void Update()
    {
        if(ConfirmResetVariables)
        {
            string TurnStartTime = main.GetPrefString("TurnStartTime", "");

            if (TurnStartTime == "")
            {
                main.rc.SetReadyStatus(true, this);
                ConfirmResetVariables = false;
            }
        }


        if (CountDown)
        {
            TimeSpan diff = DateTime.UtcNow - TurnStartTime;

            int msElapsed = (int) diff.TotalMilliseconds;
            sElapsed = (int) Mathf.Ceil(((float) msElapsed) / 1000);
            int mElapsed = (int) Mathf.Ceil(((float) sElapsed) / 60);

            int msAllowed = rtm * 60000;
            int sAllowed = rtm * 60;
            int mAllowed = rtm;

            msLeft = Mathf.Max(msAllowed - msElapsed, 0);
            sLeft = Mathf.Max(sAllowed - sElapsed, 0);
            mLeft = Mathf.Max(mAllowed - mElapsed, 0);

            if (sLeft < 10 && !tenSecsLeft)
            {
                MusicEstablished = false;
                tenSecsLeft = true;

                TenSecondFlash();
            }

            // ESTABLISH MUSIC
            if(!MusicEstablished)
            {
                if(sLeft > 10)
                {
                    main.PlayMusic("timer_music", 0.7f);
                }
                else
                {
                    main.PlayMusic("timer_10_secs", 0.7f);
                }

                NormalFlash();

                MusicEstablished = true;
            }

            

            fractionLeft = Mathf.Min(((float) msLeft) / msAllowed, 1);

            SetClockNumber(mLeft, sLeft, msLeft);

            ProgressBar.fillAmount = fractionLeft;

            //Debug.Log("fraction:" + fractionLeft);
            //Debug.Log("elapsed: m: " + mElapsed + ", s:" + sElapsed + ", ms: " + msElapsed);
            //Debug.Log("allowed: m: " + mAllowed + ", s:" + sAllowed + ", ms: " + msAllowed);
            //Debug.Log("DATETIME NOW: "+DateTime.UtcNow.ToString()+", TURNSTARTTIME: "+TurnStartTime.ToString());

            if (msLeft == 0)
            {
                CountDown = false;
                EndedAbruptly = true;
                OnTimerEnd();
            }
        }

        if(Bleed)
        {
            
            TimeSpan diff = DateTime.UtcNow - TurnStartTime;

            int msElapsed = (int)diff.TotalMilliseconds;
            sElapsed = (int)Mathf.Ceil(((float)msElapsed) / 1000);
            int mElapsed = (int)Mathf.Ceil(((float)sElapsed) / 60);

            int msAllowed = rtm * 60000;
            int sAllowed = rtm * 60;
            int mAllowed = rtm;

            msLeft = Mathf.Max(msAllowed - msElapsed, 0);
            sLeft = Mathf.Max(sAllowed - sElapsed, 0);
            mLeft = Mathf.Max(mAllowed - mElapsed, 0);

            try
            {
                LeanTween.value(0, 1, 1)
                    .setOnStart(() => { try{main.PlaySfx("timer_bleed", 0.7f); } catch { } })
                    .setOnUpdate((float t) =>
                {
                 try{   float ms = msLeft * (1 - t);
                    float s = sLeft * (1 - t);
                    float m = mLeft * (1 - t);

                    ProgressBar.fillAmount = fractionLeft * (1 - t);

                    SetClockNumber((int)m, (int)s, (int)ms); } catch { }
                })
                    .setOnComplete(() =>
                    {
                     try{   main.PlaySfx("play_end", 0.7f);
                        RoundOverDialog(); } catch { }
                    }); ;

            }catch(Exception e) { }

            Bleed = false;
        }


        
    }
   
}