using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Firestore;
using frame8.ScrollRectItemsAdapter.Classic.Examples;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GP2_TEXT_ENTRY : Fragment
{
    public const string ID = "GP2";

    public override string GetID() { return ID; }

    public QueryHelper qh;

    #region FIELD VARS
    public bool debugVars = false;

    public GameObject Layout_PreMainSubmission;
    public GameObject Layout_PreMainSuggestion;
    public GameObject Layout_PreMain;
    public GameObject Layout_PreButtons;
    public GameObject Layout_PreSub;
    public GameObject Layout_WhenSub;

    public GameObject Pos_TopLevel;
    public GameObject Pos_PreSub;
    public GameObject Pos_PreMain;
    public PositionsHolder ph = new PositionsHolder();

    public GameObject Ready_Panel;
    public GameObject Waiting_Panel;

    private OvershootInterpolator overshootInterp;
    public float OvershootInterpFactor = 3;
    private DecelerateInterpolator decelInterp;
    public float DecelInterpFactor = 2;

    public Button Button_Submit;
    public Button Button_GenerateIdea;
    public Button Button_Unready;
    public Button Button_ReadyList;
    public Button Button_Dismiss_Suggestions;
    public Button Button_NewIdea;
    public Button Button_UseIdea;
    public Button Button_Quit;

    // Data UI
    public Text text_writea;
    public Text text_truthOrLie;
    public Text text_forOrAbout;
    public Text text_yourselfOrName;
    public Text text_youSubmitted;
    public Text readyCount;
    public Text playerCount;

    public TMP_InputField editText;
    public TextMeshProUGUI editText_text_UGUI;
    public TextMeshProUGUI editText_placeholder_UGUI;
    public TextMeshPro editText_text;
    public TextMeshPro editText_placeholder;
    public Text text_queryReceiver;

    public VerticalListView_ReadyList listScript;

    public LeanTweenType easeType;

    bool lewdnessAllowed = false;

    #endregion

    public override void Awake()
    {
        AddPositions();

        base.Awake();
    }

    public override void Start()
    {
        base.Start();

        //editText_text = editText;
        //editText_placeholder = editText_placeholder_UGUI.GetComponent<TextMeshPro>();

        overshootInterp = new OvershootInterpolator(OvershootInterpFactor);
        decelInterp = new DecelerateInterpolator(DecelInterpFactor);


        SetListeners();
    }

    void AddPositions()
    {
        ph.AddPositions(Pos_TopLevel);
        ph.AddPositions(Pos_PreSub);
        ph.AddPositions(Pos_PreMain);
    }

    void SetListeners()
    {
        Button_Submit.onClick.AddListener(() =>
        {
            OnSubmitClick();
        });

        Button_Unready.onClick.AddListener(() =>
        {
            OnUnreadyClick();
        });

        Button_GenerateIdea.onClick.AddListener(() =>
        {
            ToggleSuggestionsBox();
            GenerateIdea();
        });

        Button_ReadyList.onClick.AddListener(() =>
        {
            OnToggleList();
        });

        Button_Dismiss_Suggestions.onClick.AddListener(() =>
        {
            ToggleSuggestionsBox();
        });

        Button_NewIdea.onClick.AddListener(() =>
        {
            GenerateIdea();
        });

        Button_UseIdea.onClick.AddListener(() =>
        {
            ToggleSuggestionsBox();
            UseIdea();
        });
        Button_Quit.onClick.AddListener(() =>
        {
            main.OnDialogRequested("QUIT_GAME");
        });
    }

    public override void Initialise()
    {
        try { OnPlayersChanged(); } catch { }
        try { OnReadiesChanged(-1, -1); } catch { }

        OnGameSettingsChanged();
    }

    void OnGameSettingsChanged()
    {
        try { this.lewdnessAllowed = main.data.currentSettings.le; } catch { }
    }

    void OnPlayersChanged()
    {
        try { text_writea.text = "Write a"; } catch { }
        try { text_truthOrLie.text = (bool)main.data.playersLookup[main.playerName].isTruth ? "TRUTH" : "LIE"; } catch { }
        try { text_forOrAbout.text = (bool)main.data.playersLookup[main.playerName].isTruth ? "about" : "for"; } catch { }
        try
        {
            text_yourselfOrName.text = (bool)main.data.playersLookup[main.playerName].isTruth
                ? "yourself" : main.data.playersLookup[main.playerName].target;
        }
        catch { }
        try
        {
            qh.truthOrLie = (bool)main.data.playersLookup[main.playerName].isTruth;
        }
        catch { }
        try
        {
            string target = main.data.playersLookup[main.playerName].target;
            text_youSubmitted.text = "You submitted:\n" + main.data.playersLookup[target].text;
        } catch { }

        OnReadiesChanged(-1, -1);
    }

    void OnReadiesChanged(int from, int to)
    {
        try { readyCount.text = main.data.readyCount.ToString(); } catch { }
        try { playerCount.text = main.data.playersList.Count.ToString(); } catch { }

        try
        {
            listScript.playerList = main.data.readiesList;
            listScript.RefreshList();
        }
        catch { }

        ToggleReadyUp(main.data.playersLookup[main.playerName].isReady);

    }

    void AllReadyChanged(string forPhase, bool isHost)
    {
        if(forPhase == "TextEntryWriting")
        {
            ShowWaiting();

            if (isHost) main.rc.ChangePhase("Choose", this);
        }
    }

    public override void RegisterEventListeners()
    {
        main.data.gameSettingsChangedEvent.AddListener(OnGameSettingsChanged);
        main.data.playersChangeEvent.AddListener(OnPlayersChanged);
        main.data.readiesChangeEvent.AddListener(OnReadiesChanged);
        main.data.phaseChangeEvent.AddListener(OnPhaseChanged);
        main.data.allReadyEvent.AddListener(AllReadyChanged);
    }

    private void OnPhaseChanged(string from, string to)
    {
        main.data.ResetRDI();

        if (to == "Lobby") GoToLobby();
        if (to == "TextEntryWriting") { }
        if (to == "Choose")
        {
            main.RequestFragmentDirect(GP3_CHOOSE_WHOSE_TURN.ID);
        }
    }

    private void GoToLobby()
    {
        main.rc.ResetVars(this);
        main.RequestFragmentDirect(WAITING_ROOM.ID);
    }

    public override void UnregisterEventListeners()
    {
        main.data.gameSettingsChangedEvent.RemoveListener(OnGameSettingsChanged);
        main.data.playersChangeEvent.RemoveListener(OnPlayersChanged);
        main.data.readiesChangeEvent.RemoveListener(OnReadiesChanged);
        main.data.phaseChangeEvent.RemoveListener(OnPhaseChanged);
        main.data.allReadyEvent.RemoveListener(AllReadyChanged);
    }

    string phase;
    bool meReady;
    bool whoseTurnConfirmed;


    // UI /////////////////////////////////////////////

    bool IsShowing_SuggestionBox = false;
    bool IsReadiedUp = false;
    bool IsShowing_Readylist = false;

    private void OnSubmitClick()
    {
        main.PlaySfx("button_major", 0.7f);

        if(editText.text != "")
        {
            main.rc.ChangeTextEntry(editText.text,
                main.data.playersLookup[main.playerName].target
                , this);
        }
    }

    private void OnUnreadyClick()
    {
        main.rc.SetReadyStatus(false, this);

        if (IsShowing_Readylist) OnToggleList();
    }

    private void ToggleSuggestionsBox()
    {
        if(!IsReadiedUp)
        {
            if (IsShowing_SuggestionBox)
                    {
                        Button_GenerateIdea.interactable = true;
                        Button_Submit.interactable = true;

                        Tween(
                            Layout_PreMainSubmission.transform, "Sub_1", 
                            0.5f, this, 0, overshootInterp, true, true, null);
                        Tween(
                            Layout_PreMainSuggestion.transform, "Sug_1",
                            0.5f, this, 0, overshootInterp, true, true, null);

                    }
                    else
                    {

                        Button_GenerateIdea.interactable = false;
                        Button_Submit.interactable = false;

                        Tween(
                            Layout_PreMainSubmission.transform, "Sub_2", 
                            0.5f, this, 0, overshootInterp, true, true, null);
                        Tween(
                            Layout_PreMainSuggestion.transform, "Sug_2",
                            0.5f, this, 0, overshootInterp, true, true, null);
            }

                    IsShowing_SuggestionBox = !IsShowing_SuggestionBox;
        }
 
    }

    private void OnToggleList()
    {
        if (IsReadiedUp)
        {
            if (IsShowing_Readylist)
            {
                Tween(
                    Layout_WhenSub.transform, "WhenSub_2",
                    0.5f, this, 0, decelInterp, true, true, null);
                Tween(
                    Layout_PreMain.transform, "PreMain_2",
                    0.5f, this, 0, decelInterp, true, true, null);
            }
            else
            {
                Tween(
                    Layout_WhenSub.transform, "WhenSub_3",
                    0.5f, this, 0, decelInterp, true, true, null);
                Tween(
                    Layout_PreMain.transform, "PreMain_3",
                    0.5f, this, 0, decelInterp, true, true, null);
            }

            IsShowing_Readylist = !IsShowing_Readylist;
        }

    }

    private void GenerateIdea()
    {
        Button_NewIdea.interactable = false;
        Button_UseIdea.interactable = false;

        try
        {
                try
                {
                    qh.SetVars(false, main.fs, this);
                    qh.SetLewdnessAllowed(this.lewdnessAllowed);
                    qh.BeginQuery();
                }
                catch (Exception e)
                {
                    Debug.Log("GENERATE IDEA ERROR: " + e.Message);

                    Button_NewIdea.interactable = true;
                    Button_UseIdea.interactable = true;
                }
          

        }
        catch { }
    }


    public void SetHintText(string text)
    {
        StartCoroutine(SetTickerText(text));
    }

    string tickerText; 

    IEnumerator SetTickerText(string text)
    {
        for (int i = 0; i <= text.Length; i++)
        {
            try
            {
                text_queryReceiver.text = text.Substring(0, i);
            }
            catch (Exception e) { Debug.Log(e.Message); }

            yield return new WaitForSeconds(0.01f);
        }

        Button_NewIdea.interactable = true;
        Button_UseIdea.interactable = true;
    }


    private void UseIdea()
    {
        if(text_truthOrLie.text == "TRUTH")
        {
            editText.text = "";
            editText.placeholder.GetComponent<TMP_Text>().text = text_queryReceiver.text;
        }
        else if(text_truthOrLie.text == "LIE")
        {
            editText.text = text_queryReceiver.text;
        }

        text_queryReceiver.text = "";
    }


    // OVERRIDES ////////////////////////////////////////


    public override void RoomChangeResult(string code, Task task, bool success, object args)
    {
            switch (code)
            {
                case "ChangeTextEntry":

                    main.rc.SetReadyStatus(true, this); // Triggers below

                    break;
                case "ChangeReadyStatus":



                    break;
            }


    }

    private void ToggleReadyUp(bool isReadiedUp)
    {
        Debug.Log("ToggleReadyUp: arg (player isReady): " + isReadiedUp + ", is in readied up state: " + IsReadiedUp);

        if (isReadiedUp != IsReadiedUp)
        {

            if (!isReadiedUp)
            {

                try
                {
                    main.PlaySfx("ready_down", 0.8f);

                    Tween(
                        Layout_PreButtons.transform, "Buttons_1",
                        0.5f, this, 0, overshootInterp, true, true, null);
                    Tween(
                        Layout_PreMain.transform, "PreMain_1",
                        0.5f, this, 0, overshootInterp, true, true, null);
                    Tween(
                        Layout_WhenSub.transform, "WhenSub_1",
                        0.5f, this, 0, decelInterp, true, true, null);

                    IsReadiedUp = isReadiedUp;
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }
            }
            else
            {
                try
                {
                    main.PlaySfx("ready_up", 1);

                    Tween(
                        Layout_PreButtons.transform, "Buttons_2",
                        0.5f, this, 0, overshootInterp, true, true, null);
                    Tween(
                        Layout_PreMain.transform, "PreMain_2",
                        0.5f, this, 0, overshootInterp, true, true, null);
                    Tween(
                        Layout_WhenSub.transform, "WhenSub_2",
                        0.5f, this, 0, decelInterp, true, true, null);

                    IsReadiedUp = isReadiedUp;
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }
            }
        }
  
    }

    bool isHost;
    bool StopRefreshing = false;

    int readyCount_int;
    int playerCount_int;
    private List<Player> readyList;

    void ShowWaiting()
    {
        Button_Unready.gameObject.SetActive(false);
        Ready_Panel.gameObject.SetActive(false);

        Waiting_Panel.gameObject.SetActive(true);
    }


    // UTILS

    void Tween(Component cmp, string endPosName, float duration, GP2_TEXT_ENTRY gp2,
         float delay, Interpolator interp, bool ActivenessOnStart, bool ActivenessOnEnd,
         Action endAction)
    {
        try
        {
            GameObject go = cmp.gameObject;

            if (interp == null)
            {
                interp = new LinearInterpolator();
            }

            Vector2 startSize = new Vector2();
            Vector2 endSize = new Vector2();

            RectTransform startRect;
            RectTransform endRect;

            startRect = go.GetComponent<RectTransform>();
            endRect = gp2.ph.positions[endPosName].GetComponent<RectTransform>();


            Vector3 startPos = startRect.localPosition;
            Vector3 endPos = endRect.localPosition;

            Vector2 startMin = startRect.anchorMin;
            Vector2 endMin = endRect.anchorMin;
            Vector2 startMax = startRect.anchorMax;
            Vector2 endMax = endRect.anchorMax;

            LeanTween.value(0, 1, duration).setDelay(delay)
                .setOnUpdate((float t) =>
                {
                    try
                    {
                        if (go != null)
                        {

                            go.GetComponent<RectTransform>().localPosition = interp.getVec3Interpolation(startPos, endPos, t);
                            go.GetComponent<RectTransform>().anchorMin = interp.getVec2Interpolation(startMin, endMin, t);
                            go.GetComponent<RectTransform>().anchorMax = interp.getVec2Interpolation(startMax, endMax, t);

                        }
                    }
                    catch { }
                })
                .setOnStart(() =>
                {

                    go.GetComponent<RectTransform>().localPosition = startPos;
                    go.GetComponent<RectTransform>().anchorMin = startMin;
                    go.GetComponent<RectTransform>().anchorMax = startMax;
                })
                .setOnComplete(() =>
                {

                    go.GetComponent<RectTransform>().localPosition = endPos;
                    go.GetComponent<RectTransform>().anchorMin = endMin;
                    go.GetComponent<RectTransform>().anchorMax = endMax;
                });

        }
        catch (Exception e)
        {
            Debug.Log(e.Message + ", " + e.Source + ", " + e.StackTrace);
        }

    }


}