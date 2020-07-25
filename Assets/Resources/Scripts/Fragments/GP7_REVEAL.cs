using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using frame8.ScrollRectItemsAdapter.Classic.Examples;
using UnityEngine;
using UnityEngine.UI;

public class GP7_REVEAL : MonoBehaviour
{
    public string ID_TAG = "GP7";

    #region LAYOUT VARS
    public MainScript main;
    public GP6_REVEALS_BASE gp6;
    public GameObject Dialog_Panel;

    public PositionsHolder ph = new PositionsHolder();
    public GameObject Pos_Main;
    public GameObject Pos_Middle;

    public GameObject Layout_Top;                       // I

    public GameObject Layout_TopText;                   // II
    public Text Text_PlayerName;                        // III
    public Text Text_Content;                           // III
    public GameObject Layout_SideNumber;                // II
    public Text Text_TurnNumber;                        // III

    public GameObject Layout_YouVoted;                  // I
    public Text Text_YouVoted;                          // II

    public GameObject Layout_Middle;                    // I

    public GameObject Layout_Middle_WasInFact;          // II
    public Text Text_WasInFact;                         // III
    public Text Text_TrueOrBull;                        // III
    public Text Text_WrittenBy;                         // III

    public GameObject Layout_Middle_Reveal;             // II
    public Button Button_Reveal;                        // III
    public Text Text_Reveal;                            // III

    public GameObject Layout_Middle_Points;             // II
    public Text Text_PointsSummary;                     // III

    public GameObject Layout_VoteLists;                 // I

    public GameObject Layout_VoteLists_Left;            // II
    public GameObject Layout_VoteLists_LeftHeader;      // III
    public Text Text_LeftListHeader_VotedTrue;          // IV
    public Text Text_LeftListHeader_WasInFact;          // IV
    public GameObject ListLeft;                         // III

    public GameObject LeftShade;                        // III
    public GameObject RightShade;                       // III

    public GameObject Layout_VoteLists_Right;           // II
    public GameObject Layout_VoteLists_RightHeader;     // III
    public Text Text_RightListHeader_VotedBull;         // IV
    public Text Text_RightListHeader_WasInFact;         // IV
    public GameObject ListRight;                        // III

    public GameObject Layout_Continue;                  // I

    public VerticalClassicListView_VoterList listLeft;
    public VerticalClassicListView_VoterList listRight;
    public List<Player> votersTrue;
    public List<Player> votersBull;

    public bool revealed = false;

    public Button Button_Continue;
    public GameObject Waiting_Panel;

    public GameObject POPUP_BULL_GO;
    public GameObject POPUP_TRUE_GO;
    public POPUP_BULL POPUP_BULL_SCRIPT;
    public POPUP_TRUE POPUP_TRUE_SCRIPT;

    public void GiveRefs(MainScript main, GP6_REVEALS_BASE gp6)
    {
        this.main = main;
        this.gp6 = gp6;
    }
    #endregion

    private void Awake()
    {

        ph.AddPositions(Pos_Main);
        ph.AddPositions(Pos_Middle);

    }

    // Start is called before the first frame update
    void Start()
    {


        Button_Reveal.onClick.AddListener(() =>
        {
            main.rc.ChangeRevealsPage(
                new int[2] { this.td.turnNumber - 1, 3 }, gp6);
        });

        Button_Continue.onClick.AddListener(() =>
        {
            Button_Continue.gameObject.SetActive(false);
            Waiting_Panel.gameObject.SetActive(true);
            main.rc.SetReadyStatus(true, gp6);
        });
    }

    int positionCycler = 4;
    public static int POS_INITIAL = 1;
    public static int POS_PRE_REVEAL = 2;
    public static int POS_POST_REVEAL = 3;
    public static int POS_FINAL = 4;

    public int pos = 1;

    public void SetPosition(int? position)
    {
        int pos;

        if (position == null)
        {
            positionCycler = (positionCycler + 1) % 5;

            pos = positionCycler + 1;
        }
        else
        {
            pos = position ?? 1;
            positionCycler = pos - 1;
        }

        Debug.Log("Setting to position " + pos);
        if (this.pos < pos)
        {

            this.pos = pos;
            try
            {
                Debug.Log("...Setting " + this.td.turnNumber + " to position " + pos);
            }
            catch { }

            AddToPosQ(pos);

        }else
            {
        Debug.Log("POS WONT CHANGE: requested: " + pos+", current: "+this.pos);
            }
        }

    List<int> posList = new List<int>();
    bool GoToPos = false;

    private void AddToPosQ(int pos)
    {
        posList.Add(pos);
        GoToPos = true;
    }

    private void ActuallySetPosition(int pos)
    {
        Debug.Log("ActuallySetPosition: " + pos);

        UpdateLists(td);

        float factor = 2f;
        Interpolator interp = new OvershootInterpolator(factor);

        float delay_highlevel = 0.00f;
        float delay_midlevel = 0.33f;
        float delay_lowlevel = 0.66f;

        float dur_highlevel = 0.33f;
        float dur_midlevel = 0.33f;
        float dur_lowlevel = 0.33f;

        switch (pos)
        {
            case 1:
                // (1) TOP LEVEL
                Tween(Layout_Top, "1_Top", dur_highlevel, this, delay_highlevel, interp, true, true, null);
                Tween(Layout_YouVoted, "1_YouVoted", dur_highlevel, this, delay_highlevel, interp, true, true, null);
                Tween(Layout_Middle, "1_Middle", dur_highlevel, this, delay_highlevel, interp, true, true, null);
                Tween(Layout_VoteLists, "1_VoteLists", dur_highlevel, this, delay_highlevel, interp, true, true, null);

                // (2) MIDDLE LEVEL
                Tween(Layout_Middle_WasInFact, "2_MiddleWasInFact", dur_midlevel, this, delay_midlevel, interp, true, true, null);

                //  MIDDLE - reveal
                Layout_Middle_Reveal.SetActive(false); // TODO Fade in?
                Text_TrueOrBull.gameObject.SetActive(false); // TODO Fade in?
                Text_WrittenBy.gameObject.SetActive(false); // TODO Fade in?
                Button_Reveal.gameObject.SetActive(td.readersName == myName);
                Text_Reveal.gameObject.SetActive(td.readersName != myName);

                //  MIDDLE - wasinfact L/R
                //Tween(Layout_Middle_WasInFact, "4_Middle_" + (td.wasInFact ? "Left" : "Right"), 0.5f, this, 0, interp, true, true, null);
                SetTransformPos(Layout_Middle_Points, "4_Middle_" + (td.wasInFact ? "Right" : "Left"), this.ph);
                Tween(Layout_Middle_Points, "4_Middle_" + (td.wasInFact ? "Right" : "Left"), dur_midlevel, this, delay_midlevel, interp, false, false, null);

                // (3) MIDDLE_WASINFACT LEVEL
                //Tween(Text_WasInFact.gameObject, "2_TextWasInFact", dur_lowlevel, this, delay_lowlevel, interp, true, true, null);
                //Tween(Text_TrueOrBull.gameObject, "2_TextTrueOrBull", dur_lowlevel, this, delay_lowlevel, interp, false, false, null);

                Layout_Continue.SetActive(false);

                break;
            case 2:
                // (1) TOP LEVEL
                Tween(Layout_Top, "2_Top", dur_highlevel, this, delay_highlevel, interp, true, true, null);
                Tween(Layout_YouVoted, "2_YouVoted", dur_highlevel, this, delay_highlevel, interp, true, true, null);
                Tween(Layout_Middle, "2_Middle", dur_highlevel, this, delay_highlevel, interp, true, true, null);
                Tween(Layout_VoteLists, "2_VoteLists", dur_highlevel, this, delay_highlevel, interp, true, true, null);

                // (2) MIDDLE LEVEL
                Tween(Layout_Middle_WasInFact, "2_MiddleWasInFact", dur_midlevel, this, delay_midlevel, interp, true, true, null);

                //  MIDDLE - reveal
                Layout_Middle_Reveal.SetActive(true); // TODO Fade in?
                Text_TrueOrBull.gameObject.SetActive(false); // TODO Fade in?
                Text_WrittenBy.gameObject.SetActive(false); // TODO Fade in?
                Button_Reveal.gameObject.SetActive(td.readersName == myName);
                Text_Reveal.gameObject.SetActive(td.readersName != myName);

                //  MIDDLE - wasinfact L/R
                //Tween(Layout_Middle_WasInFact, "4_Middle_" + (td.wasInFact ? "Left" : "Right"), 0.5f, this, 0, interp, true, true, null);
                SetTransformPos(Layout_Middle_Points, "4_Middle_" + (td.wasInFact ? "Right" : "Left"), this.ph);

                Tween(Layout_Middle_Points, "4_Middle_" + (td.wasInFact ? "Right" : "Left"), dur_midlevel, this, delay_midlevel, interp, false, false, null);

                // (3) MIDDLE_WASINFACT LEVEL
                //Tween(Text_WasInFact.gameObject, "2_TextWasInFact", dur_lowlevel, this, delay_lowlevel, interp, true, true, null);
                //Tween(Text_TrueOrBull.gameObject, "2_TextTrueOrBull", dur_lowlevel, this, delay_lowlevel, interp, false, false, null);


                Layout_Continue.SetActive(false);

                break;
            case 3:
                // (1) TOP LEVEL
                Tween(Layout_Top, "2_Top", dur_highlevel, this, delay_highlevel, interp, true, true, null);
                Tween(Layout_YouVoted, "2_YouVoted", dur_highlevel, this, delay_highlevel, interp, true, true, null);
                Tween(Layout_Middle, "2_Middle", dur_highlevel, this, delay_highlevel, interp, true, true, null);
                Tween(Layout_VoteLists, "2_VoteLists", dur_highlevel, this, delay_highlevel, interp, true, true, null);

                // (2) MIDDLE LEVEL
                Tween(Layout_Middle_WasInFact, "2_MiddleWasInFact", dur_midlevel, this, delay_midlevel, interp, true, true, null);

                //  MIDDLE - reveal
                Layout_Middle_Reveal.SetActive(false); // TODO Fade in?
                Text_TrueOrBull.gameObject.SetActive(false); // TODO Fade in?
                Text_WrittenBy.gameObject.SetActive(true); // TODO Fade in?
                Button_Reveal.gameObject.SetActive(td.readersName == myName);
                Text_Reveal.gameObject.SetActive(td.readersName != myName);

                //  MIDDLE - wasinfact L/R
                //Tween(Layout_Middle_WasInFact, "4_Middle_" + (td.wasInFact ? "Left" : "Right"), 0.5f, this, 0, interp, true, true, null);
                Tween(Layout_Middle_Points, "4_Middle_" + (td.wasInFact ? "Right" : "Left"), dur_midlevel, this, delay_midlevel, interp, false, false, null);

                // (3) MIDDLE_WASINFACT LEVEL
                //Tween(Text_WasInFact.gameObject, "3_TextWasInFact", dur_lowlevel, this, delay_lowlevel, interp, true, true, null);

                //Tween(Text_TrueOrBull.gameObject, "3_TextTrueOrBull", dur_lowlevel, this, delay_lowlevel, interp, false, false, null);

                Layout_Continue.SetActive(false);

                StartCoroutine(PostRevealRoutine());

                break;
            case 4:

                // (1) TOP LEVEL
                Tween(Layout_Top, "2_Top", dur_highlevel, this, delay_highlevel, interp, true, true, null);
                Tween(Layout_YouVoted, "2_YouVoted", dur_highlevel, this, delay_highlevel, interp, true, true, null);
                Tween(Layout_Middle, "4_Middle", dur_highlevel, this, delay_highlevel, interp, true, true, null);
                Tween(Layout_VoteLists, "4_VoteLists", dur_highlevel, this, delay_highlevel, interp, true, true, null);

                // (2) MIDDLE LEVEL
                //Tween(Layout_Middle_WasInFact, "2_MiddleWasInFact", dur_midlevel, this, 0, interp, true, true, null);

                //  MIDDLE - reveal
                Layout_Middle_Reveal.SetActive(false); // TODO Fade in?
                Text_TrueOrBull.gameObject.SetActive(true); // TODO Fade in?
                Text_WrittenBy.gameObject.SetActive(true); // TODO Fade in?
                Button_Reveal.gameObject.SetActive(td.readersName == myName);
                Text_Reveal.gameObject.SetActive(td.readersName != myName);

                //  MIDDLE - wasinfact L/R
                Tween(Layout_Middle_WasInFact, "4_Middle_" + (td.wasInFact ? "Left" : "Right"), dur_midlevel, this, delay_midlevel, new AccelerateInterpolator(2), true, true, null);
                
                Tween(Layout_Middle_Points, "4_Middle_" + (td.wasInFact ? "Right" : "Left"), dur_midlevel, this, delay_midlevel, new AccelerateInterpolator(2), false, true, null);

                Layout_Middle_Points.SetActive(true);
                Layout_Middle_Points.transform.localScale = new Vector3(0, 0);
                SetTransformPos(Layout_Middle_Points, "4_Middle_" + (td.wasInFact ? "Right" : "Left"), this.ph);
                LeanTween.scale(Layout_Middle_Points, new Vector3(1, 1), 0.3f)
                    .setDelay(dur_midlevel)
                    .setFrom(new Vector3(0.7f, 0.7f))
                    .setEaseSpring()
                    .setOnStart(()=>
                    {
                        try
                        {
                            Layout_Middle_Points.transform.localScale = new Vector3(0.7f, 0.7f);
                        }
                        catch { }
                    });

                // (3) MIDDLE_WASINFACT LEVEL
                //Tween(Text_WasInFact.gameObject, "3_TextWasInFact", dur_lowlevel, this, delay_lowlevel, new DecelerateInterpolator(2), true, true, null);
                //Tween(Text_TrueOrBull.gameObject, "3_TextTrueOrBull", dur_lowlevel, this, delay_lowlevel, new DecelerateInterpolator(2), true, true, null);

                Layout_Continue.SetActive(false);


                break;
            case 5:

                // (1) TOP LEVEL
                Tween(Layout_Top, "2_Top", dur_highlevel, this, delay_highlevel, interp, true, true, null);
                Tween(Layout_YouVoted, "2_YouVoted", dur_highlevel, this, delay_highlevel, interp, true, true, null);
                Tween(Layout_Middle, "4_Middle", dur_highlevel, this, delay_highlevel, interp, true, true, null);
                Tween(Layout_VoteLists, "5_VoteLists", dur_highlevel, this, delay_highlevel, interp, true, true, null);

                // (2) MIDDLE LEVEL
                //Tween(Layout_Middle_WasInFact, "2_MiddleWasInFact", dur_midlevel, this, 0, interp, true, true, null);

                //  MIDDLE - reveal
                Layout_Middle_Reveal.SetActive(false); // TODO Fade in?
                Text_TrueOrBull.gameObject.SetActive(true); // TODO Fade in?
                Text_WrittenBy.gameObject.SetActive(true); // TODO Fade in?
                Button_Reveal.gameObject.SetActive(false);
                Text_Reveal.gameObject.SetActive(false);

                //  MIDDLE - wasinfact L/R
                Tween(Layout_Middle_WasInFact, "4_Middle_" + (td.wasInFact ? "Left" : "Right"), dur_midlevel, this, delay_midlevel, interp, true, true, null);
                SetTransformPos(Layout_Middle_Points, "4_Middle_" + (td.wasInFact ? "Right" : "Left"), this.ph);
                Tween(Layout_Middle_Points, "4_Middle_" + (td.wasInFact ? "Right" : "Left"), dur_midlevel, this, delay_midlevel, interp, false, true, null);

                // (3) MIDDLE_WASINFACT LEVEL
                //Tween(Text_WasInFact.gameObject, "3_TextWasInFact", dur_lowlevel, this, delay_lowlevel, interp, true, true, null);
                //Tween(Text_TrueOrBull.gameObject, "3_TextTrueOrBull", dur_lowlevel, this, delay_lowlevel, interp, true, true, null);

                Layout_Continue.SetActive(true);
                Button_Continue.gameObject.SetActive(true);

                Layout_Middle_Points.SetActive(true);
                Layout_Middle_Points.transform.localScale = new Vector3(1, 1);

                break;
        }
    }

    void SetFinalPosition()
    {

        pos = 6;
        UpdateLists(td);

        float factor = 2f;
        Interpolator interp = new OvershootInterpolator(factor);

        float delay_highlevel = 0.00f;
        float delay_midlevel = 0.1f;
        float delay_lowlevel = 0.2f;

        float dur_highlevel = 0.1f;
        float dur_midlevel = 0.1f;
        float dur_lowlevel = 0.1f;

        // (1) TOP LEVEL
        Tween(Layout_Top, "2_Top", dur_highlevel, this, delay_highlevel, interp, true, true, null);
        Tween(Layout_YouVoted, "2_YouVoted", dur_highlevel, this, delay_highlevel, interp, true, true, null);
        Tween(Layout_Middle, "4_Middle", dur_highlevel, this, delay_highlevel, interp, true, true, null);
        Tween(Layout_VoteLists, "5_VoteLists", dur_highlevel, this, delay_highlevel, interp, true, true, null);

        // (2) MIDDLE LEVEL
        //Tween(Layout_Middle_WasInFact, "2_MiddleWasInFact", dur_midlevel, this, 0, interp, true, true, null);

        //  MIDDLE - reveal
        Layout_Middle_Reveal.SetActive(false); // TODO Fade in?
        Text_TrueOrBull.gameObject.SetActive(true); // TODO Fade in?
        Text_WrittenBy.gameObject.SetActive(true); // TODO Fade in?
        Button_Reveal.gameObject.SetActive(false);
        Text_Reveal.gameObject.SetActive(false);

        //  MIDDLE - wasinfact L/R
        Tween(Layout_Middle_WasInFact, "4_Middle_" + (td.wasInFact ? "Left" : "Right"), dur_midlevel, this, delay_midlevel, interp, true, true, null);
        SetTransformPos(Layout_Middle_Points, "4_Middle_" + (td.wasInFact ? "Right" : "Left"), this.ph);
        Tween(Layout_Middle_Points, "4_Middle_" + (td.wasInFact ? "Right" : "Left"), dur_midlevel, this, delay_midlevel, interp, false, true, null);

        // (3) MIDDLE_WASINFACT LEVEL
        //Tween(Text_WasInFact.gameObject, "3_TextWasInFact", dur_lowlevel, this, delay_lowlevel, interp, true, true, null);
        //Tween(Text_TrueOrBull.gameObject, "3_TextTrueOrBull", dur_lowlevel, this, delay_lowlevel, interp, true, true, null);

        //Layout_Continue.SetActive(true);
        //Button_Continue.gameObject.SetActive(true);

        Layout_Middle_Points.SetActive(true);
        Layout_Middle_Points.transform.localScale = new Vector3(1, 1);

        Layout_Continue.SetActive(false);

    }

    // Controls dialog box
    IEnumerator PostRevealRoutine()
    {
        yield return null;

        try
        {
            UpdateLists(td);

            if (td.wasInFact)
            {
                Destroy(POPUP_BULL_GO.gameObject);

                POPUP_TRUE_GO.SetActive(true);
                POPUP_TRUE_SCRIPT.enabled = true;
            }
            else
            {
                Destroy(POPUP_TRUE_GO.gameObject);

                POPUP_BULL_GO.SetActive(true);
                POPUP_BULL_SCRIPT.enabled = true;
            }


        }
        catch(Exception e) { Debug.Log(e.Message); };
    }

    public List<Achievement> AchList;
    public float POPUP_LIFE_SECS = 3;

    string myName;
    public TurnData td;

    public void Reveal()
    {
        revealed = true;
        Layout_Continue.gameObject.SetActive(false);

        SetUI(td);
        SetFinalPosition();
    }

    public void SetUI(TurnData td)
    {
        // Start();

        try
        {
            this.td = td;

            // if (gp6.debugVars) myName = "A";

            myName = main.playerName;

            Text_PlayerName.text = td.readersName;
            Text_Content.text = td.readersText;
            Text_TurnNumber.text = td.turnNumber.ToString();

            Text_YouVoted.text = " - ";
            if (myName == td.readersName)
            {
                Text_YouVoted.text = "You played this";
            }
            else if (td.saboName != null && td.saboName == myName)
            {
                Text_YouVoted.text = "You wrote this";
            }
            else
            {
                if (td.trueVoters.Contains(myName))
                {
                    Text_YouVoted.text = "You voted <color=#"+ColorUtility.ToHtmlStringRGB(MainScript.true_blue_dark)+"> TRUE</color>";
                }
                else if (td.lieVoters.Contains(myName))
                {
                    Text_YouVoted.text = "You voted <color=#" + ColorUtility.ToHtmlStringRGB(MainScript.bull_red_dark) + "> BULL</color>";
                }
            }

            Text_TrueOrBull.text = td.wasInFact ? "TRUE" : "BULL";
            Text_TrueOrBull.color = td.wasInFact ? MainScript.true_blue : MainScript.bull_red;
            Text_WrittenBy.text = td.saboName == null ? "Written by " + td.writtenBy :
                (!revealed ? "Written by " + "TBA" : "Written by " + td.writtenBy);
            Text_Reveal.text = "Waiting for " + td.readersName + " to reveal...";

            this.AchList = td.achievementsUnlocked[myName];

            string scoreText = "";
            if (AchList != null && AchList.Count > 0)
            {
                foreach (Achievement a in td.achievementsUnlocked[myName])
                {
                    scoreText += a.simpleString + "\n";
                }
            }
            if (scoreText == "") scoreText = "You scored nothing this round";


            Text_PointsSummary.text = scoreText;
            Text_LeftListHeader_WasInFact.text = revealed ? td.numTrueACTUAL.ToString() : td.numTrueFAKE.ToString();
            Text_RightListHeader_WasInFact.text = revealed ? td.numLieACTUAL.ToString() : td.numLieFAKE.ToString();

            // Lists
            UpdateLists(td);

        }
        catch (Exception e)
        {
            Debug.Log("GP7 SETUI: "+ e.Message + ", " + e.Source + ", " + e.StackTrace);
        }
    }

    private void UpdateLists(TurnData td)
    {
        Color32 white = new Color32(255, 255, 255, 255);
        Color32 grey = new Color32(171, 171, 171, 255);

        if (pos <= GP7_REVEAL.POS_PRE_REVEAL)
        {
            listLeft.playerList = td.votersTrueBLANK;
            listRight.playerList = td.votersBullBLANK;

            // Shade
            LeftShade.SetActive(false);
            RightShade.SetActive(false);
            Text_LeftListHeader_VotedTrue.color = white;
            Text_RightListHeader_VotedBull.color = white;
            Text_LeftListHeader_WasInFact.color = white;
            Text_RightListHeader_WasInFact.color = white;
        }
        else if (!revealed)
        {
            listLeft.playerList = td.votersTrueNOFAKE;
            listRight.playerList = td.votersBullNOFAKE;

            //Shade
            LeftShade.SetActive(!td.wasInFact);
            RightShade.SetActive(td.wasInFact);
            Text_LeftListHeader_VotedTrue.color = td.wasInFact ? white : grey;
            Text_LeftListHeader_WasInFact.color = td.wasInFact ? white : grey;
            Text_RightListHeader_VotedBull.color = !td.wasInFact ? white : grey;
            Text_RightListHeader_WasInFact.color = !td.wasInFact ? white : grey;
        }
        else
        {
            listLeft.playerList = td.votersTrue;
            listRight.playerList = td.votersBull;

            // Shade
            LeftShade.SetActive(!td.wasInFact);
            RightShade.SetActive(td.wasInFact);
            Text_LeftListHeader_VotedTrue.color = td.wasInFact ? white : grey;
            Text_LeftListHeader_WasInFact.color = td.wasInFact ? white : grey;
            Text_RightListHeader_VotedBull.color = !td.wasInFact ? white : grey;
            Text_RightListHeader_WasInFact.color = !td.wasInFact ? white : grey;
        }

        try
        {
            listLeft.RefreshList();
            listRight.RefreshList();
        }
        catch(Exception e) { Debug.Log("LISTS UPDATE: "+e.Message); }

        }


    #region INSTANTIATE FORCEFULLY


    // Update is called once per frame
    void Update()
    {
       


        if(GoToPos)
        {
            int i = posList[0];

            if(!Layout_Top.LeanIsTweening()
                && !Layout_YouVoted.LeanIsTweening()
                && !Layout_Middle.LeanIsTweening()
                && !Layout_VoteLists.LeanIsTweening()
                && !Layout_Middle_WasInFact.LeanIsTweening()
                && !Layout_Middle_Points.LeanIsTweening()
                && !Text_WasInFact.gameObject.LeanIsTweening()
                && !Text_TrueOrBull.gameObject.LeanIsTweening())
            {
                ActuallySetPosition(i);
                posList.Remove(i);
                GoToPos = (posList.Count > 0);
            }
            else
            {
                Debug.Log("AVOIDED MULTI TWEEN");
            }


        }

        //try
        //{

        //    listLeft.Refresh();
        //    listRight.Refresh();
        //}
        //catch (Exception e)
        //{
        //    Debug.Log("LIST REFRESH: " + e.Message + ", " + e.Source + ", " + e.StackTrace);
        //}
    }
    #endregion

    public void SetTransformPos(GameObject go, string posName, PositionsHolder ph)
    {
        go.GetComponent<RectTransform>().localPosition = ph.positions[posName].GetComponent<RectTransform>().localPosition;
        go.GetComponent<RectTransform>().anchorMin = ph.positions[posName].GetComponent<RectTransform>().anchorMin;
        go.GetComponent<RectTransform>().anchorMax = ph.positions[posName].GetComponent<RectTransform>().anchorMax;
    }

    void Tween(GameObject go, string posName, float duration, GP7_REVEAL gp7,
        float delay, Interpolator interp, bool ActivenessOnStart, bool ActivenessOnEnd,
        Action endAction)
    {
        try
        {
            if (interp == null)
            {
                interp = new LinearInterpolator();
            }

            Vector2 startSize = new Vector2();
            Vector2 endSize = new Vector2();

            RectTransform startRect;
            RectTransform endRect;

            startRect = go.GetComponent<RectTransform>();
            endRect = gp7.ph.positions[posName].GetComponent<RectTransform>();


            Vector3 startPos = startRect.localPosition;
            Vector3 endPos = endRect.localPosition;

            Vector2 startMin = startRect.anchorMin;
            Vector2 endMin = endRect.anchorMin;
            Vector2 startMax = startRect.anchorMax;
            Vector2 endMax = endRect.anchorMax;

            LeanTween.value(0, 1, duration).setDelay(delay)
                .setOnUpdate((float t) =>
                {
                try{    if (go != null)
                    {
                            //Vector3 lerpedPos = Vector3.Lerp(startPos, endPos, t);
                            //Vector2 lerpedSize = Vector2.Lerp(startSize, endSize, t);

                            //lerpedPos = interp.getVec3Interpolation(startPos, endPos, t);
                            //lerpedSize = interp.getVec2Interpolation(startSize, endSize, t);

                            // if (go != null) go.transform.localPosition = lerpedPos;
                            //     if (go != null) go.GetComponent<RectTransform>().sizeDelta = lerpedSize;

                            go.GetComponent<RectTransform>().localPosition = interp.getVec3Interpolation(startPos, endPos, t);
                            go.GetComponent<RectTransform>().anchorMin = interp.getVec2Interpolation(startMin, endMin, t);
                            go.GetComponent<RectTransform>().anchorMax = interp.getVec2Interpolation(startMax, endMax, t);

                        } } catch { }
                })
                .setOnStart(() =>
                {
                    //try{   if (go != null)
                    //   {
                    //       if (go != null) startPos = go.transform.localPosition;
                    //       endPos = gp7.ph.positions[posName].localPosition;

                    //       if (go != null) startSize = go.GetComponent<RectTransform>().sizeDelta;
                    //       endSize = gp7.ph.positions[posName].GetComponent<RectTransform>().sizeDelta;

                    //       if (go != null) go.SetActive(ActivenessOnStart);
                    //   } } catch { }


                    go.GetComponent<RectTransform>().localPosition = startPos;
                    go.GetComponent<RectTransform>().anchorMin = startMin;
                    go.GetComponent<RectTransform>().anchorMax = startMax;
                })
                .setOnComplete(() =>
                {
                    //try{   if (go != null)
                    //   {
                    //       if (go != null) go.transform.localPosition = endPos;
                    //       if (go != null) go.GetComponent<RectTransform>().sizeDelta = endSize;

                    //       if (go != null) go.SetActive(ActivenessOnEnd);
                    //       if (endAction != null) endAction();
                    //   } } catch { }


                    go.GetComponent<RectTransform>().localPosition = endPos;
                    go.GetComponent<RectTransform>().anchorMin = endMin;
                    go.GetComponent<RectTransform>().anchorMax = endMax;
                });

        }catch(Exception e)
        {
            Debug.Log(e.Message + ", " + e.Source + ", " + e.StackTrace);
        }

    }
}
