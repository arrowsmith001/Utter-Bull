using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DanielLochner.Assets.SimpleScrollSnap;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class GP6_REVEALS_BASE : Fragment
{
    public const string ID = "GP6";
    public override string GetID() { return ID; }

    #region DEBUG MODE ON/OFF
    // DEBUG ONLY
    public bool debugVars = false;
    #endregion

    #region LAYOUT VARS
    public PositionsHolder ph = new PositionsHolder();
    public GameObject Pos_Main;
    public GameObject Pager;
    public SimpleScrollSnap scrollSnap;
    public GameObject Layout_Pager;
    public GameObject Layout_Pagination;
    public GameObject Pager_Content;
    public GameObject Layout_ButtonPanel;
    public GameObject Layout_Quit;
    public Button Button_Left;
    public Button Button_Center;
    public Button Button_Right;
    public Button Button_Quit;
    public Text TextNumPlayers;
    public Text TextNumReady;
    public GameObject Waiting_Panel;
    public List<GameObject> gps;

    public RoleAssigner ra;
    #endregion

    public static Color32 dark_red = new Color32(187, 32, 32, 255);
    public static Color32 dark_blue = new Color32(32, 32, 187, 255);


    #region START
    public override void Awake()
    {
        ph.AddPositions(Pos_Main);
        
        base.Awake();

        gp7_page = gps[0];
        gp8_page = gps[1];

        
    }

    public override void Start()
    {
        base.Start();

        Button_Quit.onClick.AddListener(() =>
        {
            main.OnDialogRequested("QUIT_GAME");
        });
    }

    public override void Initialise()
    {
        try {  SetRDIData(); }catch { }
        try { OnPhaseChanged(null, main.data.currentPhase); }catch { }
        try {   OnReadiesChanged(0, main.data.readyCount); }catch { }
        try { OnRevealsPageIndexChanged(main.data.currentReveal[0], main.data.currentReveal[1]); }catch { }
    }

    public bool RDIDataSet = false;
    private void SetRDIData()
    {
        if (main.data.rdi == null) return;
        if (RDIDataSet) return;

        RDIDataSet = true;

        this.rdi = main.data.rdi;
        this.turnsData = this.rdi.turnsData;
        this.resultsData = this.rdi.resultsData;

        this.revealsNum = this.rdi.turnsData.Count;

        for (int i = 0; i < revealsNum; i++)
        {
            AddRevealPage();
        }

        AddResultPage();

        // Sets UI for each page with turnData
        for (int j = 0; j < revealsNum; j++)
        {
            gp7s[j].SetUI(turnsData[j]);
        }

        //this.scrollSnap.swipeGestures = true;

        //Debug.Log("SET UI VALUES GP6");

        //this.phase = main.data.currentPhase;
        //isReady = main.data.playersLookup[main.playerName].isReady;

        //if (phase != "LastPage" && phase != "TextEntryWriting")
        //{
        //    if (main.rl.room.revealsPageIndex == null)
        //    {
        //        throw new Exception("NULL INDEX");
        //    }
        //    else
        //    {
        //        this.index = main.rl.room.revealsPageIndex[0];
        //        this.pos = main.rl.room.revealsPageIndex[1];
        //    }

        //}

        //this.isHost = main.rl.playersLookup[main.playerName].isHosting;

        //revealsNum = turnsData.Count;

        //if (phase == "LastPage")
        //{
        //    numReady = main.rl.readyCount;
        //    numPlayers = main.rl.room.players.Count;

        //    TextNumReady.text = numReady.ToString();
        //    TextNumPlayers.text = numPlayers.ToString();

        //    if (!LastPageReached)
        //    {
        //        JumpToPage(gp7s.Count);
        //    }

        //    if (isReady)
        //    {
        //        JumpToPage(gp7s.Count);
        //        DisableScrollSnapButtons();
        //    }
        //}
    }

    public override void RegisterEventListeners()
    {
        this.main.data.rdiReadyEvent.AddListener(OnRDIReady);
        this.main.data.revealsPageIndexChanged.AddListener(OnRevealsPageIndexChanged);
        this.main.data.readiesChangeEvent.AddListener(OnReadiesChanged);
        this.main.data.allReadyEvent.AddListener(OnAllReadyChanged);
        this.main.data.phaseChangeEvent.AddListener(OnPhaseChanged);
    }

    public override void UnregisterEventListeners()
    {
        this.main.data.rdiReadyEvent.RemoveListener(OnRDIReady);
        this.main.data.revealsPageIndexChanged.RemoveListener(OnRevealsPageIndexChanged);
        this.main.data.readiesChangeEvent.RemoveListener(OnReadiesChanged);
        this.main.data.allReadyEvent.RemoveListener(OnAllReadyChanged);
        this.main.data.phaseChangeEvent.RemoveListener(OnPhaseChanged);
    }

    private void OnPhaseChanged(string from, string to)
    {
        switch(to)
        {  
            case "Lobby": // ALL SHOULD IMPLEMENT THIS

                restartRound = false;
                main.rc.ResetVars(this);
                main.RequestFragmentDirect(WAITING_ROOM.ID);

                break;
            case "Reveals":

                //gp7s[thisPage].SetPosition(GP7_REVEAL.POS_INITIAL);

                //StartCoroutine(InitialPos2PreReveal(main.data.currentReveal[0]));
                
                if(main.data.currentReveal[1] < 2 && !PreRevealStarting)
                {
                    StartCoroutine(InitialPos2PreReveal(main.data.currentReveal[0]));
                }
                if (main.data.currentReveal[1] > 3 && !PostRevealStarting)
                {
                    StartCoroutine(PostReveal2End(main.data.currentReveal[0]));
                }

                break;
            case "NextPage":

                if (main.data.AmIHost())
                {
                    AdvancePage();
                }

                break;
            case "LastPage":

                if (!LastPageReached)
                {
                    LastPageBool = true;
                }

                break;
            case "TextEntryWriting":

                main.RequestFragmentDirect(GP1_DELEGATE_ROLES.ID);

                break;
        }
    
    }

    bool LastPageBool = false;

    private void OnAllReadyChanged(string forPhase, bool isHost)
    {
        if (!isHost) return;

        if(forPhase == "Reveals")
        {
            main.rc.ChangePhase("NextPage", this);
        }

        if(forPhase == "LastPage")
        {
            main.rc.ResetVars(this);
        }
    }

    private void OnReadiesChanged(int from, int to)
    {
        TextNumReady.text = to.ToString();
        try { TextNumPlayers.text = main.data.playersList.Count.ToString(); } catch { }
    }

    private void OnRevealsPageIndexChanged(int page, int pos)
    {
        if (main.data.currentPhase != "LastPage" && main.data.currentPhase != "TextEntryWriting") 
        {
            JumpToPage(page);
            try { this.gp7s[page].SetPosition(pos); } catch { }


                // HOST CONTROLS
            if (main.data.AmIHost())
            {
                if (pos == 1 && !PreRevealStarting)
                {
                    StartCoroutine(InitialPos2PreReveal(page));
                }
                if (pos == 3 && !PostRevealStarting)
                {
                    StartCoroutine(PostReveal2End(page));
                }

                Debug.Log("SET UI VALUES REVEALS CASE: index: " + page + ", ISHOST: " + main.data.AmIHost());
            }
        }

    }

    private void OnRDIReady()
    {
        SetRDIData();
    }


    /// <summary>
    /// Sets center button listener, in the case of pager being on final page or not.
    /// </summary>
    /// <param name="isFinal"></param>
    private void SetCenterButton(bool isFinal)
    {
        Button_Center.onClick.RemoveAllListeners();

        if (!isFinal)
        {
            try
            {
                Button_Center.transform.Find("Text").GetComponent<Text>().text = "RESULTS PAGE";
                Button_Center.GetComponent<Image>().color = GP6_REVEALS_BASE.dark_red;

                Button_Center.onClick.AddListener(() =>
                {
                    scrollSnap.GoToPanel(turnsData.Count);
                });
            }
            catch(Exception e) { Debug.Log("SetCenterButton failed 1: "+e.Message); }
        }
        else
        {
            try
            {
                Button_Center.transform.Find("Text").GetComponent<Text>().text = "NEW ROUND";
                Button_Center.GetComponent<Image>().color = GP6_REVEALS_BASE.dark_blue;

                Button_Center.onClick.AddListener(() =>
                {
                    main.rc.SetReadyStatus(true, this);
                    DisableScrollSnapButtons();
                });

            }
            catch(Exception e) { Debug.Log("SetCenterButton failed 2: " + e.Message); }
        }
    }

    private void DisableScrollSnapButtons()
    {
        Button_Center.gameObject.SetActive(false);
        Button_Left.gameObject.SetActive(false);
        Button_Right.gameObject.SetActive(false);
        Waiting_Panel.gameObject.SetActive(true);

        scrollSnap.swipeGestures = false;

        try
        {
            // gp8.listScript.CollapseItems();
            gp8.ActivateBlock();
        }catch(Exception e) { Debug.Log("DisableScrollSnapButtons: "+e.Message); }

        Debug.Log("REACHED END OF DISABLE SCROLL SNAP BUTTONS");
    }
    #endregion


    #region INITIALISE PAGER

    private GameObject gp7_page;
    private GameObject gp8_page;

    private RoundDataInterpretter rdi;
    List<TurnData> turnsData;
    ResultsData resultsData;
    List<GP7_REVEAL> gp7s = new List<GP7_REVEAL>();
    int revealsNum;

    /// <summary>
    /// Adds a reveal page to the pager, saves page script to list (gp7s).
    /// Expects turnData to exist
    /// </summary>
    private void AddRevealPage()
    {
        try
        {
            scrollSnap.AddToBack(gp7_page);

            GameObject page = scrollSnap.Panels[scrollSnap.NumberOfPanels - 1];
            GP7_REVEAL gp7 = page.GetComponent<GP7_REVEAL>(); // Get script

            gp7.GiveRefs((MainScript) main, (GP6_REVEALS_BASE) this);
            gp7s.Add(gp7);


        }catch(Exception e)
        {
            Debug.Log("AddRevealPage: " + e.Message + ", " + e.Source + ", " + e.StackTrace);
        }

        // gp7.SetUI(turnData);
    }

    public GP8_RESULTS gp8; // Script for back page
    public List<Player> resultsList = new List<Player>(); // Data for results list

    /// <summary>
    /// Adds a result page to the back of the pager.
    /// Expects resultsData to exist
    /// </summary>
    private void AddResultPage()
    {
        try
        {
            // Is a Player list, I want a PlayerWithResults list
            foreach (Player p in resultsData.playersOrderedWithNewScores)
            {
                resultsList.Add(new Player(p, this.rdi));
            }

            // Adds GP8 page to pager
            scrollSnap.AddToBack(gp8_page);

            // Saves GP8 script
            GameObject page = scrollSnap.Panels[scrollSnap.NumberOfPanels - 1];
            gp8 = page.GetComponent<GP8_RESULTS>();

            // Pass main and GP6 refs to GP8
            gp8.GiveRefs((MainScript)main, (GP6_REVEALS_BASE)this);

            // Gives resultsList to GP8 script, to GP8 list, and refreshes the list TODO: Collapse list items
            gp8.list = resultsList;
            gp8.listScript.playerList = resultsList;
            gp8.listScript.RefreshList();

            gp8.HideAndCollapseItems();


        }catch(Exception e)
        {
            Debug.Log(e.Message + ", " + e.Source + ", " + e.StackTrace);
        }
    }
    #endregion

    #region PAGER CALLBACKS

    public void OnPanelSelecting()
    {


    }

    public void OnPanelSelected()
    {
        Debug.Log("ON PANEL SELECTED: CURRENT: " + scrollSnap.CurrentPanel + ", TARGET: " + scrollSnap.TargetPanel);

        thisPage = scrollSnap.TargetPanel;

        SetCenterButton(thisPage == turnsData.Count);

        if (main.data.currentPhase == "LastPage" && !main.data.AmIReady())
        {
            Button_Left.gameObject.SetActive(thisPage > 0);
            Button_Right.gameObject.SetActive(thisPage < turnsData.Count);
        }

        //if (thisPage == turnsData.Count && (main.data.currentPhase == "Reveals" || main.data.currentPhase == "NextPage"))
        //{
        //    main.rc.ChangePhase("LastPage", this);

        //    LastPageRoutine();
        //}

        //if(main.data.currentPhase == "LastPage" && !LastPageReached)
        //{
        //    LastPageRoutine();
        //}

        

    }

    private void LastPageRoutine()
    {
        if(!LastPageReached)
        {
            ShowButtonPanel();
            scrollSnap.swipeGestures = true;

            for (int i = 0; i < Mathf.Max(WAITING_ROOM.PLAYER_MIN_FOR_GAME, main.data.playersList.Count); i++)
            {
                gp7s[i].Reveal();
            }

            gp8.AnimateListItems();

            LastPageReached = true;
        }
    }

    public void OnPanelChanging()
    {
        Debug.Log("OnPanelChanging: current: " + scrollSnap.CurrentPanel);
        Debug.Log("OnPanelChanging: target: " + scrollSnap.TargetPanel);

    }

    public void OnPanelChanged()
    {
        Debug.Log("OnPanelChanged: current: " + scrollSnap.CurrentPanel);
        Debug.Log("OnPanelChanged: target: " + scrollSnap.TargetPanel);

    }


    #endregion

    #region TRAVERSE PAGER
    bool PreRevealStarting = false;

    IEnumerator InitialPos2PreReveal(int pageNum)
    {
        Debug.Log("InitialPos2PreReveal: " + pageNum);

        PreRevealStarting = true;

        yield return new WaitForSeconds(main.debugModeOn ? 1f : 2f);

        //gp7.SetPosition(GP7_REVEAL.POS_PRE_REVEAL);
        main.rc.ChangeRevealsPage(new int[2] { pageNum, 2 }, this);

    }

    bool PostRevealStarting = false;

    IEnumerator PostReveal2End(int pageNum)
    {
        Debug.Log("PostReveal2End: " + pageNum);

        PostRevealStarting = true;

        yield return new WaitForSeconds(main.debugModeOn ? 1 : 3);

        //gp7.SetPosition(4);
        main.rc.ChangeRevealsPage(new int[2] { pageNum, 4 }, this);

        yield return new WaitForSeconds(main.debugModeOn ? 1 : 1);

        //gp7.SetPosition(5);
        main.rc.ChangeRevealsPage(new int[2] { pageNum, 5 } , this);


        PostRevealStarting = false;

    }

    void ShowButtonPanel()
    {
        Tween(Layout_ButtonPanel, "Buttons_On", 0.5f, this, 0, new DecelerateInterpolator(2), true, true, null);
        Tween(Layout_Quit, "Quit_On", 0.5f, this, 0, new DecelerateInterpolator(2), true, true, null);
    }

    #endregion

    #region CALLBACKS

    bool Pager_Created = false;

    private void AdvancePage()
    {
        try
        {
            int nextPage = main.data.currentReveal[0] + 1;

            if (gp7s.Count == nextPage) main.rc.ChangePhase("LastPage", this); 

            if(nextPage <= scrollSnap.NumberOfPanels) main.rc.ChangeRevealsPage(new int[2] { nextPage, 1 }, this);
        }
        catch(Exception e) { Debug.Log("AdvancePageERROR: " + e.Message); }
    }

    bool LastPageReached = false;

    int thisPage = 0;

    bool restartRound = true;

    public override void RoomChangeResult(string code, Task task, bool succes, object args)
    {
        switch (code)
        {
            case "ChangeRevealsPage":

                    if(!succes)
                {
                    Debug.Log("ChangeRevealsPage FAIL");
                    int[] newArgs = (int[])args;
                    main.rc.ChangeRevealsPage(newArgs, this);
                }
                    else
                {

                    int[] newArgs = (int[])args;

                    if(newArgs[1] == 2)
                    {
                        PreRevealStarting = false;
                    }

                    if(newArgs[0] < gp7s.Count) main.rc.ChangePhase("Reveals", this);
                }

                break;
            case "ResetVars":

                if (!restartRound) return;

                ra.allTrueAllowed = main.data.currentSettings.ate;
                ra.AssignRoles(main.data.GetStringPlayersList());

                main.rc.AddRolesAndOrder(ra.playerTruths, ra.playerTargets, this);

                break;

            case "AddPlayerRoles":

                if(succes) main.rc.AddPlayerOrder(main.data.GetStringPlayersList(), this);

                break;
            case "AddPlayerOrder":

                if (succes) main.rc.ChangePhase("TextEntryWriting", this);

                break;


        }
    }

    public void JumpToPage(int page)
    {
        Debug.Log("JUMPED TO PAGE " + page);

        if (scrollSnap != null)
        {
            scrollSnap.GoToPanel(page);
        }
    }

    #endregion

    #region UPDATE
    bool UpdateRequired = false;
    bool rdiReady = false;

    float time = 0;
    int timeRunning = 0;
    private bool DisableScrollSnapBool = false;


    // Update is called once per frame
    void Update()
    {
        if(LastPageBool)
        {
            try
            {
                JumpToPage(gp7s.Count);
                LastPageRoutine();

                LastPageBool = false;
            }
            catch { }
        }
      

        //try{
        //    if(this.thisPage != (main.rl.room.revealsPageIndex ?? 0))
        //    {
        //        this.thisPage = (main.rl.room.revealsPageIndex ?? 0);
        //    }
        //}catch(Exception e){ }

        //time += Time.deltaTime;
        //if (time > 1)
        //{
        //    timeRunning++;
        //    try
        //    {
        //        string s = "";
        //        for (int i = 0; i < gp7s.Count; i++)
        //        {
        //            s += ", " + i + "-pos: " + gp7s[i].pos;
        //        }

        //        Debug.Log("revealsPageIndex: " + index + ", currentPanel: " + scrollSnap.CurrentPanel + s);
        //        // Debug.Log(timeRunning + "s: thisPage: " + thisPage + ", pos: " + gp7s[thisPage].pos + ", main.index: "+main.rl.room.revealsPageIndex);
        //    }
        //    catch (Exception e) { Debug.Log("ERROR AT TIME " + timeRunning + ": " + e.Message + ", " + e.Source + ", " + e.StackTrace); }
        //    time = 0;
        //}
    }
    #endregion

    #region DEBUG
    private RoundDataInterpretter DummyRDI()
    {
        string json = "{\"SufficientInfoConfirmed\":true,\"failureMessage\":\"\",\"turnsData\":[{\"turnNumber\":1,\"readersName\":\"A\",\"readersText\":\"Lie for A written by C.\",\"wasInFact\":false,\"writtenBy\":\"C\",\"trueVoters\":[],\"lieVoters\":[\"B\"],\"xVoters\":[],\"fastestCorrectVoters\":[\"B\"],\"correctVoters\":[\"B\"],\"incorrectVoters\":[],\"didntVoteVoters\":[],\"saboName\":\"C\",\"s_badSabo\":false,\"s_mostEarned\":false,\"s_allEarned\":false,\"p_fiftyfiftyEarned\":false,\"p_mostEarned\":false,\"p_allEarned\":false,\"p_nobodyVoted\":false,\"achievementsUnlocked\":{\"A\":[],\"B\":[{\"title\":\"Lie Detector\",\"msg\":\"You voted correctly!\",\"pointsWorth\":30,\"simpleString\":\"+30 - You voted correctly!\"},{\"title\":\"Finger On The Button\",\"msg\":\"You voted correctly the quickest!\",\"pointsWorth\":20,\"simpleString\":\"+20 - You voted correctly the quickest!\"}],\"C\":[]},\"votersTrue\":[],\"votersBull\":[{\"playerName\":\"B\",\"voteTime\":7,\"isAmongFastest\":false}]},{\"turnNumber\":2,\"readersName\":\"B\",\"readersText\":\"Truth about B written by B.\",\"wasInFact\":true,\"writtenBy\":\"B\",\"trueVoters\":[\"A\",\"C\"],\"lieVoters\":[],\"xVoters\":[],\"fastestCorrectVoters\":[\"A\"],\"correctVoters\":[\"A\",\"C\"],\"incorrectVoters\":[],\"didntVoteVoters\":[],\"saboName\":null,\"s_badSabo\":false,\"s_mostEarned\":false,\"s_allEarned\":false,\"p_fiftyfiftyEarned\":false,\"p_mostEarned\":false,\"p_allEarned\":false,\"p_nobodyVoted\":false,\"achievementsUnlocked\":{\"A\":[{\"title\":\"Got To The Truth\",\"msg\":\"You voted correctly!\",\"pointsWorth\":30,\"simpleString\":\"+30 - You voted correctly!\"},{\"title\":\"Finger On The Button\",\"msg\":\"You voted correctly the quickest!\",\"pointsWorth\":20,\"simpleString\":\"+20 - You voted correctly the quickest!\"}],\"B\":[],\"C\":[{\"title\":\"Got To The Truth\",\"msg\":\"You voted correctly!\",\"pointsWorth\":30,\"simpleString\":\"+30 - You voted correctly!\"}]},\"votersTrue\":[{\"playerName\":\"A\",\"voteTime\":5,\"isAmongFastest\":true},{\"playerName\":\"C\",\"voteTime\":9,\"isAmongFastest\":false}],\"votersBull\":[]},{\"turnNumber\":3,\"readersName\":\"C\",\"readersText\":\"Lie for C written by A.\",\"wasInFact\":false,\"writtenBy\":\"A\",\"trueVoters\":[\"B\"],\"lieVoters\":[],\"xVoters\":[],\"fastestCorrectVoters\":[],\"correctVoters\":[],\"incorrectVoters\":[\"B\"],\"didntVoteVoters\":[],\"saboName\":\"A\",\"s_badSabo\":false,\"s_mostEarned\":false,\"s_allEarned\":true,\"p_fiftyfiftyEarned\":false,\"p_mostEarned\":false,\"p_allEarned\":true,\"p_nobodyVoted\":false,\"achievementsUnlocked\":{\"A\":[{\"title\":\"Biggest Lie Ever Sold\",\"msg\":\"Everyone fell for a lie you wrote!\",\"pointsWorth\":50,\"simpleString\":\"+50 - Everyone fell for a lie you wrote!\"}],\"B\":[],\"C\":[{\"title\":\"Oscar-Worthy\",\"msg\":\"You fooled everyone!\",\"pointsWorth\":100,\"simpleString\":\"+100 - You fooled everyone!\"}]},\"votersTrue\":[{\"playerName\":\"B\",\"voteTime\":6,\"isAmongFastest\":false}],\"votersBull\":[]}],\"resultsData\":{\"newScores\":{\"A\":100,\"B\":50,\"C\":130},\"scoreDiff\":{\"A\":100,\"B\":50,\"C\":130},\"playersOrderedWithNewScores\":[{\"playerName\":\"C\",\"isHosting\":false,\"isReady\":false,\"isTruth\":false,\"isTurn\":true,\"text\":\"Lie for C written by A.\",\"readyFor\":null,\"points\":130,\"target\":\"A\",\"isMe\":false,\"votes\":[\"s\",\"T\",\"p\"],\"voteTimes\":[8,9,0]},{\"playerName\":\"A\",\"isHosting\":true,\"isReady\":false,\"isTruth\":false,\"isTurn\":false,\"text\":\"Lie for A written by C.\",\"readyFor\":null,\"points\":100,\"target\":\"C\",\"isMe\":true,\"votes\":[\"p\",\"T\",\"s\"],\"voteTimes\":[0,5,7]},{\"playerName\":\"B\",\"isHosting\":false,\"isReady\":false,\"isTruth\":true,\"isTurn\":false,\"text\":\"Truth about B written by B.\",\"readyFor\":null,\"points\":50,\"target\":\"B\",\"isMe\":false,\"votes\":[\"L\",\"p\",\"T\"],\"voteTimes\":[7,0,6]}]}}";
        RoundDataInterpretter dummyRdi = JsonConvert.DeserializeObject<RoundDataInterpretter>(json);
        return dummyRdi;
    }
    #endregion

    void SetTransformPos(GameObject go, string posName, GP6_REVEALS_BASE gp6)
    {
        go.GetComponent<RectTransform>().localPosition = gp6.ph.positions[posName].GetComponent<RectTransform>().localPosition;
        go.GetComponent<RectTransform>().anchorMin = gp6.ph.positions[posName].GetComponent<RectTransform>().anchorMin;
        go.GetComponent<RectTransform>().anchorMax = gp6.ph.positions[posName].GetComponent<RectTransform>().anchorMax;
    }

    void Tween(GameObject go, string posName, float duration, GP6_REVEALS_BASE gp6,
        float delay, Interpolator interp, bool ActivenessOnStart, bool ActivenessOnEnd,
        Action endAction)
    {
        try
        {
            if (interp == null)
            {
                interp = new LinearInterpolator();
            }

            Vector3 startPos = new Vector3();
            Vector3 endPos = new Vector3();

            Vector2 startSize = new Vector2();
            Vector2 endSize = new Vector2();

            LeanTween.value(0, 1, duration).setDelay(delay)
                .setOnUpdate((float t) =>
                {
                 try{   if (go != null)
                    {
                        Vector3 lerpedPos = Vector3.Lerp(startPos, endPos, t);
                        Vector2 lerpedSize = Vector2.Lerp(startSize, endSize, t);

                        lerpedPos = interp.getVec3Interpolation(startPos, endPos, t);
                        lerpedSize = interp.getVec2Interpolation(startSize, endSize, t);

                        if (go != null) go.transform.localPosition = lerpedPos;
                        if (go != null) go.GetComponent<RectTransform>().sizeDelta = lerpedSize;
                    }  } catch { }
                })
                .setOnStart(() =>
                {
                 try{   if (go != null)
                    {
                        if (go != null) startPos = go.transform.localPosition;
                        endPos = gp6.ph.positions[posName].localPosition;

                        if (go != null) startSize = go.GetComponent<RectTransform>().sizeDelta;
                        endSize = gp6.ph.positions[posName].GetComponent<RectTransform>().sizeDelta;

                        if (go != null) go.SetActive(ActivenessOnStart);
                    }  } catch { }
                })
                .setOnComplete(() =>
                {
                 try{   if (go != null)
                    {
                        if (go != null) go.transform.localPosition = endPos;
                        if (go != null) go.GetComponent<RectTransform>().sizeDelta = endSize;

                        if (go != null) go.SetActive(ActivenessOnEnd);
                        if (endAction != null) endAction();
                    } } catch { }
                });

        }
        catch (Exception e)
        {
            Debug.Log(e.Message + ", " + e.Source + ", " + e.StackTrace);
        }

    }


}
