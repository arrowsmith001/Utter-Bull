using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GP3_CHOOSE_WHOSE_TURN : Fragment
{
    public const string ID = "GP3";
    public override string GetID() { return ID; }

    #region FIELD VARS

    bool debugVars = false;
    public LeanTweenType easeType;

    public GameObject Pos_All;
    public GameObject Pos_Top;
    public GameObject Pos_Middle;
    public GameObject Pos_Ticker;
    public GameObject Pos_Bottom;
    public PositionsHolder ph;

    public GameObject LayoutTop;
    public GameObject LayoutMiddle;
    public GameObject LayoutBottom;
    public GameObject LayoutTicker;
    public GameObject ArrowsLeft;
    public GameObject ArrowsRight;

    public Button Button_Go;
    public Button Button_Start;
    public Image radialProgress;
    public GameObject Layout_radialProgress;

    // Data UI
    public Text Text_ChoosingPlayer;
    public Text Text_ReadThisOut;
    public Text Text_Text;

    public Text Text_WhoseTurn;

    public Button Button_SaboPrompt;
    bool saboPrompt_dismissed = false;

    public Text Text_Statement;
    public Text Text_Statement_2;
    public Text Text_ReadOutTimer;

    bool isTurn = false;
    bool isSabo = false;
    private List<Player> playersToPlay;

    private OvershootInterpolator overshootInterp;
    public float OvershootInterpFactor = 3;
    private DecelerateInterpolator decelInterp;
    public float DecelInterpFactor = 2;

    public GameObject tickerNamePrefab;

    #endregion

    #region SETUP
    public override void Awake()
    {
        base.Awake();

        overshootInterp = new OvershootInterpolator(OvershootInterpFactor);
        decelInterp = new DecelerateInterpolator(DecelInterpFactor);

        LayoutTicker.gameObject.SetActive(false);

        AddPositions();

        SetListeners();
    }

    void AddPositions()
    {
        // Create position dictionary
        ph = new PositionsHolder();

        ph.AddPositions(Pos_All);
        ph.AddPositions(Pos_Middle);
        ph.AddPositions(Pos_Top);
        ph.AddPositions(Pos_Ticker);
        ph.AddPositions(Pos_Bottom);
    }

    void SetListeners()
    {
        Button_Go.onClick.AddListener(() =>
        {
            OnGo();
        });

        Button_SaboPrompt.onClick.AddListener(() =>
        {
            ToggleSaboPrompt(false, true);
        });
        Button_Start.onClick.AddListener(() =>
        {
            Button_Start.interactable = false;
            OnReadoutAnimEnd();
        });

    }

    #endregion

    public override void Initialise()
    {
        OnPlayerChange();
        OnPhaseChange(null, main.data.currentPhase);

    }

    void OnPlayerChange()
    {
        try
        {
            this.playersToPlay = main.data.playersLeftToPlay;

            string whoseTurn = main.data.currentTurn.playerName;
            string text = main.data.currentTurn.text;

            Text_WhoseTurn.text = whoseTurn;
            Text_Statement.text = text;


            // If its my turn
            if (main.data.currentTurn.playerName == main.playerName)
            {
                isTurn = true;
                Text_Statement.text = "Press GO! to reveal your statement";
                Text_Statement_2.text = Text_Statement.text;
            }
            else
            {
                isTurn = false;
                Text_Statement.text = "Waiting for " + whoseTurn + " to read out statement";
                Text_Statement_2.text = Text_Statement.text;
            }

            isSabo = (main.data.playersLookup[main.playerName].target == whoseTurn
                && main.data.playersLookup[main.playerName].target != main.playerName);

            Text_Statement.gameObject.SetActive(isSabo);
            Button_SaboPrompt.gameObject.SetActive(isSabo);
            Text_Statement_2.gameObject.SetActive(!isSabo);


        }
        catch { }
    }

    void OnPhaseChange(string from, string to)
    {

        this.phase = to;

        if (to == "Lobby") GoToLobby();
        if (to == "Choose" && !Choosing) OnChooseState();
        if (to == "Reading" && !Reading) OnReadingState();
        if (to == "Play") OnPlayState();


        //Text_Statement.text = text_pressgo_text;
        //Text_ChoosingPlayer.text = "";

        //Text_WhoseTurn.gameObject.SetActive(true);
        //Text_WhoseTurn.text = text_whoseturn_text;
    }


    private void GoToLobby()
    {
        main.rc.ResetVars(this);
        main.RequestFragmentDirect(WAITING_ROOM.ID);
    }

    public override void RegisterEventListeners()
    {
        main.data.playersChangeEvent.AddListener(OnPlayerChange);
        main.data.phaseChangeEvent.AddListener(OnPhaseChange);
    }

    public override void UnregisterEventListeners()
    {
        main.data.playersChangeEvent.RemoveListener(OnPlayerChange);
        main.data.phaseChangeEvent.RemoveListener(OnPhaseChange);
    }

    #region ON CHOOSE STATE

    List<GameObject> tickNames = new List<GameObject>();
    GameObject go = null;

    private List<Player> SeedShuffle(List<Player> playersToPlay)
    {
        // DEBUG ONLY
        //return playersToPlay;

        List<Player> playersShuffled = new List<Player>();
        Dictionary<double, Player> dic = new Dictionary<double, Player>();
        List<double> randomDoubles = new List<double>();

        foreach(Player p in playersToPlay)
        {
            System.Random rand = new System.Random(p.playerName.GetHashCode());
            double d = rand.NextDouble();

            dic.Add(d, p);
            randomDoubles.Add(d);

            Debug.Log("HASH: " + p.playerName + ", D: " + d);
        }

        randomDoubles.Sort();

        foreach(double d in randomDoubles)
        {
            playersShuffled.Add(dic[d]);
        }

        return playersShuffled;
    }

    bool Choosing = false;

    void OnChooseState()
    {
        Choosing = true;

        //SetTransformPos(LayoutTop.transform, "Top_1", this.ph);
        //SetTransformPos(LayoutMiddle.transform, "Middle_1", this.ph);
        //SetTransformPos(LayoutBottom.transform, "Bottom_1", this.ph);

        if (!main.debugModeOn) StartCoroutine(TickerAnim1());
        //else if(main.rl.playersLookup[main.playerName].isHosting) main.rc.ChangePhase("Play", this);
    }

    // Instantiate, set up
    IEnumerator TickerAnim1()
    {
        yield return new WaitForSeconds(1);

        Tween(LayoutTop.transform, "Top_2", 0.5f, this.ph,
            0, overshootInterp, true, true, null);

        LayoutTicker.SetActive(true);

        // Generate shuffled extended playerlist
        playersToPlay = SeedShuffle(playersToPlay);

        int runningCount = 0;
        List<Player> playersExt = new List<Player>(playersToPlay);
        while(runningCount < 30)
        {
            playersExt.AddRange(playersToPlay);
            runningCount += playersToPlay.Count;
        }
        int incr = -1;
        bool added = false;
        while(!added)
        {
            incr++;
            playersExt.Add(playersToPlay[incr]);

            added = (playersToPlay[incr].playerName == main.data.currentTurn.playerName);
        }

        // Instantiate to starting positions
        bool b = true;

        foreach(Player p in playersExt)
        {
            GameObject go = Instantiate(tickerNamePrefab, LayoutTicker.transform);
            go.SetActive(false);

            Text text = go.GetComponent<Text>();
            text.text = p.playerName;

            string posName = b ? "Tick_TopLeft" : "Tick_TopRight";
            Tween(go.transform, posName, 0.01f, this.ph, 0, null, false, false, null);
            b = !b;

            tickNames.Add(go);
        }

        LayoutTicker.SetActive(true);

        StartCoroutine(TickerAnim2());

       // OnTickerAnimEnd();
    }

    // Part 2
    IEnumerator TickerAnim2()
    {
        yield return null;

        int i = 0;
        int total = tickNames.Count;
        float delay = 0; // initial
        float timeToMiddle = 0.2f; // initial
      
        Interpolator interp = new LinearInterpolator();
        Interpolator accel = new AccelerateInterpolator(1f);
        Interpolator decel = new DecelerateInterpolator(1f);

        Interpolator decay = new DecelerateInterpolator(3f);

        bool b = true;

        Transform middle = ph.positions["Tick_Middle"];

        foreach (GameObject go in tickNames)
        {
            i++;
            float f = ((float) i) / total;
            float df = decay.getInterpolation(f);

            Vector3 startPos;
            Vector3 endPos;
            Vector2 startSize;
            Vector2 endSize;

            // Custom tween function

            // TOP TO MIDDLE
            startPos = go.transform.localPosition;
            endPos = middle.localPosition;

            startSize = go.GetComponent<RectTransform>().sizeDelta;
            endSize = middle.GetComponent<RectTransform>().sizeDelta;

            float t1 = timeToMiddle;

            LeanTween.value(0, 1, t1).setDelay(delay)
                .setOnUpdate((float t) =>
                {
                   try{ if (go != null)
                    {
                        Vector3 lerpedPos = decel.getVec3Interpolation(startPos, endPos, t);

                        float tSin = (float)System.Math.Sin(decel.getInterpolation(t) * (Mathf.PI / 2));
                        lerpedPos.x = startPos.x + tSin * (endPos.x - startPos.x);

                        Vector3 lerpedSize = decel.getVec3Interpolation(startSize, endSize, t);

                        if (go != null) go.transform.localPosition = lerpedPos;
                        if (go != null) go.GetComponent<RectTransform>().sizeDelta = lerpedSize;

                        Color c = go.GetComponent<Text>().color;
                        c.a = t;
                        go.GetComponent<Text>().color = c;
                    }
                    } catch { }
                })
                .setOnStart(() =>
                {
                    if (go != null) go.SetActive(true);
                })
                .setOnComplete(() =>
                {
                    if (go != null && tickNames.IndexOf(go) != (tickNames.Count - 1))
                    {
                        main.PlaySfx("tick_player_ticker", 1);

                        // MIDDLE TO BOTTOM
                        startPos = middle.localPosition;
                        endPos = b ? ph.positions["Tick_BottomLeft"].localPosition : ph.positions["Tick_BottomRight"].localPosition;

                        startSize = middle.GetComponent<RectTransform>().sizeDelta;
                        endSize = b ? ph.positions["Tick_BottomLeft"].GetComponent<RectTransform>().sizeDelta
                            : ph.positions["Tick_BottomRight"].GetComponent<RectTransform>().sizeDelta;

                        float t2 = timeToMiddle;

                        LeanTween.value(0, 1, t2).setDelay(0)
                            .setOnUpdate((float t) =>
                            {
                             try{   if (go != null)
                                {
                                    Vector3 lerpedPos = accel.getVec3Interpolation(startPos, endPos, t);

                                    float tSin = 1 - (float)System.Math.Sin((Mathf.PI / 2) + (accel.getInterpolation(t) * (Mathf.PI / 2)));
                                    lerpedPos.x = startPos.x + tSin * (endPos.x - startPos.x);

                                    Vector3 lerpedSize = accel.getVec3Interpolation(startSize, endSize, t);

                                    go.transform.localPosition = lerpedPos;
                                    go.GetComponent<RectTransform>().sizeDelta = lerpedSize;

                                    Color c = go.GetComponent<Text>().color;
                                    c.a = 1 - t;
                                    go.GetComponent<Text>().color = c;

                                    b = !b;
                                    }
                                }catch { }
                            })
                            .setOnStart(() =>
                            {
                                if (go != null) go.SetActive(true);
                            })
                            .setOnComplete(() =>
                            {
                                if (go != null) Destroy(go.gameObject);
                            });

                    }
                    else if (tickNames.IndexOf(go) == (tickNames.Count - 1))
                    {
                        this.go = go;
                        try { 
                        LeanTween.scale(go, new Vector3(1, 1), 0.25f).setFrom(new Vector3(2f, 2f));
                        } catch { }
                        StartCoroutine(OnTickerAnimEnd(true));
                    }
                });

            
            delay += timeToMiddle * f;
        }

    }

    // On end
    IEnumerator OnTickerAnimEnd(bool playSound)
    {
        if(playSound) main.PlaySfx("player_selected", 1);

        yield return new WaitForSeconds(main.debugModeOn ? 0 : 1);

        if (go != null)
        {
            Vector3 startPos = go.transform.localPosition;
            LeanTween.value(0, 1, 0.5f).setOnUpdate((float t) =>
            { try { 
                go.transform.localPosition = Vector3.Lerp(startPos, LayoutTop.transform.localPosition, t);

                Color c = go.GetComponent<Text>().color;
                c.a = Mathf.Max(0, 1 - (t * 2));
                go.GetComponent<Text>().color = c;} catch { }
            })
                .setOnComplete(() =>
                {
                   try{ LayoutTicker.SetActive(false);
                    go.SetActive(false);
                    Text_WhoseTurn.gameObject.SetActive(true);
                    LeanTween.value(0, 1, 0.3f).setOnUpdate((float t) =>
                    {
                       try{ Text_WhoseTurn.gameObject.transform.localScale
                        = new Vector3(overshootInterp.getInterpolation(t), overshootInterp.getInterpolation(t));

                        Color c = Text_ChoosingPlayer.color;
                        c.a = 1 - t;
                        Text_ChoosingPlayer.color = c;} catch { }
                    })
                    .setOnComplete(() =>
                    {
                        try{Text_ChoosingPlayer.gameObject.SetActive(false); } catch { }
                    });} catch { }
                });
        }


        if (isTurn)
        {
            try{

                Tween(LayoutBottom.transform, "Bottom_2", 0.5f, this.ph, 0, new OvershootInterpolator(2), true, true, null);
                Tween(LayoutMiddle.transform, "Middle_2", 0.5f, this.ph, 0, new OvershootInterpolator(2), true, true, null);
            }
            catch { }
        }
        else
        {
          try{
            SetTransformPos(LayoutMiddle.transform, "Middle_3", this.ph);
            Tween(LayoutMiddle.transform, "Middle_4", 0.5f, this.ph, 0, new OvershootInterpolator(2), true, true, null);
            }
            catch { }
        }
   

        
    }
    #endregion

    #region ON READING STATE

    void ToggleSaboPrompt(bool saboShow, bool animate)
    {

        Button_SaboPrompt.gameObject.SetActive(saboShow);

        if (saboShow)
        {
            if (!animate)
            {
                SetTransformPos(Text_Statement, "Statement_1", this.ph);
            }
            else
            {
                Tween(Text_Statement.transform, "Statement_1", 
                    0.5f, this.ph, 0, overshootInterp, true, true, null); // TODO
            }
        }
        else
        {
            saboPrompt_dismissed = true;

            if (!animate)
            {
                SetTransformPos(Text_Statement, "Statement_2", this.ph);
            }
            else
            {
                Tween(Text_Statement.transform, "Statement_2",
                     0.5f, this.ph, 0, overshootInterp, true, true, null); 
            }
        }

    }

    bool Reading = false;

    void OnReadingState()
    {
        Reading = true;

        SetTransformPos(LayoutTop.transform,"Top_2", this.ph);

        if (isTurn)
        {
            //LeanTween.move(LayoutBottom, positions["Bottom_2"], 0.5f).setEase(easeType);
            //LeanTween.move(LayoutMiddle, positions["Middle_2"], 0.5f).setEase(easeType);

            SetTransformPos(LayoutBottom.transform, "Bottom_2", this.ph);
            SetTransformPos(LayoutMiddle.transform, "Middle_2", this.ph);

            LayoutBottom.SetActive(true);
            LayoutMiddle.SetActive(true);
        }
        else
        {
            //LayoutMiddle.transform.position = positions["Middle_3"].position;
            //LeanTween.move(LayoutMiddle, positions["Middle_4"], 0.5f).setEase(easeType);

            SetTransformPos(LayoutMiddle.transform, "Middle_4", this.ph);
            LayoutBottom.SetActive(true);
        }
    }

    void OnGo()
    {
        Button_Go.gameObject.SetActive(false);
        Text_WhoseTurn.gameObject.SetActive(false);

        float timeToRead = main.debugModeOn ? 0.5f : 15; // TODO f(text.Length)

        ReadoutAnim(timeToRead);

    }


    float timeToAllowSkip = 3;
    //float timeToRead;

    public List<Image> ArrowsLeftList;
    public List<Image> ArrowsRightList;


    void ReadoutAnim(float timeToRead)
    {

        this.time = timeToRead;
        this.timeToRead = timeToRead;

        try
        {
            ArrowsLeft.SetActive(true);
            ArrowsRight.SetActive(true);

            // Arrows
            Tween(ArrowsLeft.transform, "ArrowsLeft_2", 
                0.5f, this.ph, 0, overshootInterp, true, true, null);

            Tween(ArrowsRight.transform, "ArrowsRight_2", 
                0.5f, this.ph, 0, overshootInterp, true, true, null);

            StartCoroutine(ArrowFlash());

            // READ THIS OUT
            Text_ReadThisOut.gameObject.SetActive(true);
            Text_ChoosingPlayer.gameObject.SetActive(false);

            // Set statement text
            Text_Text.text = main.data.currentTurn.text;
            Text_Text.gameObject.SetActive(true);
            Text_Statement.gameObject.SetActive(false);
            Text_Statement_2.gameObject.SetActive(false);

            // Make statement pop
            try
            {
                LeanTween.value(0.5f, 1, 0.25f).setOnUpdate((float t) =>
                {
                   try{ float _t = overshootInterp.getInterpolation(t);
                    Text_Text.gameObject.transform.localScale = new Vector3(_t, _t);}
                    catch { }
                });

            }
            catch (Exception e) { }

            // Initialise progress bar
            radialProgress.fillClockwise = true;
            ReadingOutTimer = true;

            // OnReadoutAnimEnd();

        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

        // main.RequestFragmentDirect("GP2_TEXT_ENTRY");

    }

    IEnumerator ArrowFlash()
    {
        List<Image> arrows = new List<Image>();
        arrows.AddRange(ArrowsLeftList);
        arrows.AddRange(ArrowsRightList);

        foreach(Image img in arrows)
        {
            Color c = img.color;
            c.a = 0.5f;
            img.color = c;
        }

        for(int i = 0; i < 12; i++)
        {
            foreach (Image img in arrows)
            {
                Color c = img.color;

                int r = arrows.IndexOf(img) % 3;
                int target = i % 3;

                c.a = (r == target) ? 1f : 0.35f;
                img.color = c;
            }

            yield return new WaitForSeconds(0.1f);
        }

        foreach (Image img in arrows)
        {
            Color c = img.color;
            c.a = 1f;
            img.color = c;
        }
    }

    void PresentStartOption()
    {
        Tween(Layout_radialProgress.transform, "Circle_2", 
            0.5f, this.ph, 0, new OvershootInterpolator(2), true, true, null);
        Button_Start.gameObject.SetActive(true);
    }

    bool Transitioning = false;

    void OnReadoutAnimEnd()
    {
        if (!debugVars && !Transitioning)
        {
            Transitioning = true;
            Button_Start.interactable = false;

            main.rc.ChangePhase("Play", this);
            //StartCoroutine(SetTurnStartTime());
        }
    }

    IEnumerator SetTurnStartTime()
    {
        yield return null;

        main.rc.SetTurnTimeStart(MainScript.GetNetTime(), this);
    }

    #endregion


    #region ON PLAY STATE

    void OnPlayState()
    {
        main.RequestFragmentDirect(GP4_PLAY.ID);

        //// Launch dialog if it's your turn
        //if (main.rl.room.whoseTurn == main.playerName)
        //{
        //    //if (!(main.rl.playersLookup[main.playerName].isTruth ?? false))
        //    //{
        //    //    main.OnDialogRequested("CONFIRM_LIE");
        //    //}
        //    //else
        //    //{
        //    //    main.OnDialogRequested("CONFIRM_TRUTH");
        //    //}
        //}
    }

    #endregion



    #region OVERRIDES

    public override void RoomChangeResult(string code, Task task, bool succes, object args)
    {
        //if(code == "SetTurnTimeStart")
        //{
        //    if(succes)
        //    {
        //        main.rc.ChangePhase("Play", this);
        //    }
        //    else
        //    {
        //        StartCoroutine(SetTurnStartTime());
        //    }
        //}
    }

    #endregion

    #region UPDATE

    bool UpdateRequired = true;
    public bool ReadingOutTimer = false;
    bool StartPresented = false;
    public float timeToRead = 0;
    public float time = 0;
    private string phase;

    bool SetUIValuesBool = false;

    private void Update()
    {
   

        if(ReadingOutTimer)
        {
            time -= Time.deltaTime;

            // Decrement progress bar
            radialProgress.fillAmount = (time / timeToRead);

            string t1 = Text_ReadOutTimer.text;
            string t2 = Mathf.Ceil(time).ToString();

            if(t1 != t2)
            {
                Text_ReadOutTimer.text = Mathf.Ceil(time).ToString();

                try
                {
                    LeanTween.value(0.5f, 1, 0.25f).setOnUpdate((float t) =>
                      {
                          try
                          {
                              float t3 = overshootInterp.getInterpolation(t);
                              if (Text_ReadOutTimer.gameObject != null) Text_ReadOutTimer.gameObject.transform.localScale = new Vector3(t3, t3, t3);
                          }
                          catch(Exception e) { }
                      });

                }catch (Exception e) { }
            }

            if(!StartPresented && time < (timeToRead - timeToAllowSkip))
            {
                PresentStartOption();
                StartPresented = true;

            }

            if (time < 0)
            {
                time = 0;
                ReadingOutTimer = false;

                OnReadoutAnimEnd();
            }
        }

       
 
    }

    #endregion

    #region UTILS


    #endregion
}
