using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using frame8.ScrollRectItemsAdapter.Classic.Examples;
using System.Numerics;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Unity.Mathematics;
using UnityEngine.Purchasing;
using System.Linq;
using Newtonsoft.Json;

public class WAITING_ROOM : Fragment
{
    public const string ID = "WR";
    public override string GetID() { return ID; }

    #region FIELD VARS
    public static int PLAYER_MIN_FOR_GAME = 2;

    public RoleAssigner ra;

    public GameObject Pos_Main;
    public PositionsHolder ph = new PositionsHolder();

    // VIEWS / VIEW VALUES
    public GameObject ListLayout;
    public GameObject ListView;
    public GameObject listContent;
    public GameObject Settings_Panel;
    public GameObject Begin_Panel;

    public Text text_code;

    public Button Button_TimerAdd;
    public Button Button_TimerMinus;
    public Button button_begin_game;
    public Button button_home;

    public Text text_timer;

    public Text text_descrip_angels;
    public Text text_descrip_lewd;

    public Toggle toggle_angels, toggle_lewd;

    public VerticalClassicListView_WaitingRoom listScript;

    private Interpolator interp = new OvershootInterpolator(2);

    public Image img_angels;
    public Image img_lewd;

    public List<Sprite> toggleAngelsSprites;
    public List<Sprite> toggleLewdSprites;

    Sprite AngelsOn;
    Sprite AngelsOff;

    Sprite LewdOn;
    Sprite LewdOff;

    public GameObject SettingsPrompts;
    public Text SettingsPromptsText;
    public Button Button_SettingsBlock;

    public GameObject PlayerNumPrompt;
    public Text PlayerNumText;

    #endregion

    public override void Awake()
    {
        base.Awake();

        AngelsOff = toggleAngelsSprites[0];
        AngelsOn = toggleAngelsSprites[1];

        LewdOff = toggleLewdSprites[0];
        LewdOn = toggleLewdSprites[1];

        ph.AddPositions(Pos_Main);
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        main.PlayMusic("utter_bull_main_theme", MainScript.MUSIC_VOLUME_LOW);

        SetListeners();
    }

    void SetListeners()
    {
        button_begin_game.onClick.AddListener(() =>
        {
            RoundStart();
        });
        Button_TimerAdd.onClick.AddListener(() =>
        {
            main.PlaySfx("button_minor", 0.7f);
            main.rc.ChangeTimer(
                Mathf.Min(int.Parse(text_timer.text) + 1, 10), this);
        });
        Button_TimerMinus.onClick.AddListener(() =>
        {
            main.PlaySfx("button_minor", 0.7f);
            main.rc.ChangeTimer(
                Mathf.Max(int.Parse(text_timer.text) - 1, 1), this);
        });
        toggle_angels.onValueChanged.AddListener((b) =>
        {
            main.PlaySfx("button_major", 0.7f);
            //if (b != toggle_angels_bool)
            //{
                main.rc.SetAllTrueEnabled(b, this);
            //}
        });
        toggle_lewd.onValueChanged.AddListener((b) =>
        {
            main.PlaySfx("button_major", 0.7f);
            //if (b != toggle_lewd_bool)
            //{
                main.rc.SetLewdEnabled(b, this);
            //}
        });
        button_home.onClick.AddListener(() =>
        {
            main.OnDialogRequested("QUIT_GAME_FROM_LOBBY");
        });
        Button_SettingsBlock.onClick.AddListener(() =>
        {
            StartCoroutine(FlashSettingsNotif());
        });


    }

    public override void Initialise()
    {
        
        OnPlayersListChanged();

        //if (main.data.currentSettings != null) OnGameSettingsChanged();
        //if (main.data.currentRoomCode != null) OnRoomCodeChangeEvent();
        //if (main.data.currentHost != null) OnHostChanged();

        try {
            OnGameSettingsChanged();

        }catch(Exception e) { Debug.Log(this.GetID() + " init error: " + e.Message); }
        try
        {
            OnRoomCodeChangeEvent();
        }
        catch (Exception e) { Debug.Log(this.GetID() + " init error: " + e.Message); }
        try
            {
                OnHostChanged();
        }
        catch (Exception e) { Debug.Log(this.GetID() + " init error: " + e.Message); }

        // TODO Do the rest
    }

    public override void RegisterEventListeners()
    {
        main.data.phaseChangeEvent.AddListener(OnPhaseChangeEvent);
        main.data.playersChangeEvent.AddListener(OnPlayersListChanged);
        main.data.playerNumChangeEvent.AddListener(OnPlayerNumChanged);
        main.data.gameSettingsChangedEvent.AddListener(OnGameSettingsChanged);
        main.data.hostChangedEvent.AddListener(OnHostChanged);
        main.data.allReadyEvent.AddListener(OnAllReadyEvent);
        main.data.roomCodeChangedEvent.AddListener(OnRoomCodeChangeEvent);
    }

    public override void UnregisterEventListeners()
    {
        main.data.phaseChangeEvent.RemoveListener(OnPhaseChangeEvent);
        main.data.playersChangeEvent.RemoveListener(OnPlayersListChanged);
        main.data.playerNumChangeEvent.RemoveListener(OnPlayerNumChanged);
        main.data.gameSettingsChangedEvent.RemoveListener(OnGameSettingsChanged);
        main.data.hostChangedEvent.RemoveListener(OnHostChanged);
        main.data.allReadyEvent.RemoveListener(OnAllReadyEvent);
        main.data.roomCodeChangedEvent.RemoveListener(OnRoomCodeChangeEvent);
    }

    void OnPhaseChangeEvent(string from, string to)
    {
        switch (to)
        {
            case "Lobby":

                break;
            case "TextEntryWriting":

                main.RequestFragmentDirect(GP1_DELEGATE_ROLES.ID);

                break;
        }

    }

    void OnPlayersListChanged()
    {
        List<Player> players = main.data.playersList;

        try
        {
            listScript.RefreshList();

        }
        catch (Exception e) { Debug.Log("List exception caught: " + e.Message); }

        // Set player prompts
        int diff = Mathf.Max(PLAYER_MIN_FOR_GAME - players.Count, 0);
        if (diff == 0) { PlayerNumText.text = "Begin a game!"; }
        else
        {
            PlayerNumText.text = "You need " + diff.ToString() + " more player"
                + ((diff == 1) ? "" : "s") + " for a game";
        }
    }

    int playerNum = 0;

    void OnPlayerNumChanged(int changeMode)
    {
        Debug.Log("PLAYERNUM CHANGED: " + changeMode.ToString());

        playerNum = main.data.playersLookup.Count;
        if(changeMode == 1) IncreaseNotHandled = true;

        //List<Player> players = main.data.playersList;

        //try
        //{
        //    listScript.RefreshList();

        //}
        //catch (Exception e) { Debug.Log("List exception caught: " + e.Message); }

        //// Handle increase (pop animation)
        //if (changeMode == 1)
        //{
        //    try
        //    {
        //        HandleIncrease(players.Count);
        //    }
        //    catch (Exception e) { Debug.Log("Handle increase exception: " + e.Message); }
        //}

    }

    void OnGameSettingsChanged()
    {
        GameSettings settings = main.data.currentSettings;

        int rtm = settings.rtm;
        bool ate = settings.ate;
        bool le = settings.le;

        text_timer.text = rtm.ToString();
        text_descrip_angels.text = ate ? "\"ALL TRUE\" ALLOWED" : "\"ALL TRUE\" DISALLOWED"; ;
        text_descrip_lewd.text = le ? "LEWD HINTS ENABLED" : "LEWD HINTS DISABLED";

        toggle_angels.isOn = ate;
        toggle_lewd.isOn = le;

        img_angels.sprite = ate ? AngelsOn : AngelsOff;
        img_lewd.sprite = le ? LewdOn : LewdOff;

        if (main.data.AmIHost()) main.SavePrefString("gameSettings", JsonConvert.SerializeObject(settings));

    }

    void OnHostChanged()
    {
        Debug.Log("HOST CHANGE REACT");
        SetHostPriveliges(main.data.AmIHost());
    }

    void OnAllReadyEvent(string phase, bool isHost)
    {
        if(isHost)
        {
            main.rc.ChangePhase("TextEntryWriting", this);
        }
    }

    void OnRoomCodeChangeEvent()
    {
        text_code.text = main.data.currentRoomCode;
    }

    // Pop list recent item
    void HandleIncrease(int len) // TODO Make it POP
    {
        Debug.Log("HANDLE INCREASE CALLED: items: "+ listScript.rects.Count.ToString() + " children: "+ listContent.transform.childCount.ToString()+", LEN: "+len.ToString());

        GameObject newAddition = listContent.transform.GetChild(listContent.transform.childCount - 1).gameObject; // listContent.transform.childCount - 1
        LeanTween.value(0, 1, 0.25f).setOnUpdate((float v) =>
        {
            try
            {
                float val = 0.25f;
                if (newAddition != null) newAddition.transform.localScale = new Vector2(val + (1 - val) * interp.getInterpolation(v), val + (1 - val) * interp.getInterpolation(v));
                }
            catch (Exception e) { Debug.Log("LeanTween exception: " + e.Message); }
        });

        main.PlaySfx("player_joined_lobby", 1);
    }

    // OVERRIDE: RoomChanger callback
    public override void RoomChangeResult(string code, Task task, bool success, object args)
    {
        switch (code)
        {
            case "AddPlayerRoles":

                if (success) // Player roles successfully added
                {
                    main.rc.AddPlayerOrder(main.data.GetStringPlayersList(), this);
                }
                else
                {
                    Debug.Log("Weird error: player role assignment failed");

                    button_begin_game.interactable = true;
                }

                break;
            case "AddPlayerOrder":

                if (success) // Player roles successfully added
                {
                    main.rc.ChangePhase("TextEntryWriting", this);
                }
                else
                {
                    Debug.Log("Weird error: player role assignment failed");

                    button_begin_game.interactable = true;
                }

                break;

                //case "ChangePhase_ConfirmRoleInfo":

                //    OnConfirmRoleInfo();

                //    break;

        }


    }

    void SetHostPriveliges(bool isHost)
    {
        button_begin_game.interactable = isHost;
        Button_TimerAdd.interactable = isHost;
        Button_TimerMinus.interactable = isHost;
        //toggle_angels.interactable = isHost;
        //toggle_lewd.interactable = isHost;

        Button_SettingsBlock.gameObject.SetActive(!isHost);

        Tween(ListLayout.transform, "List_" + (isHost ? "2" : "1"), 0.5f, this.ph, 0, new AccelerateInterpolator(1.2f), true, true, null);
        Tween(Settings_Panel.transform, "Settings_" + (isHost ? "2" : "1"), 0.5f, this.ph, 0, new AccelerateInterpolator(1.2f), true, true, null);
        Tween(Begin_Panel.transform, "Begin_" + (isHost ? "2" : "1"), 0.5f, this.ph, 0, new AccelerateInterpolator(1.2f), true, isHost, null);
    }

    void RoundStart()
    {
        button_begin_game.interactable = false;

        if (main.data.GetPlayerNum() < PLAYER_MIN_FOR_GAME)
        {
            try
            {
                StartCoroutine(FlashPlayernumNotif());
            }
            catch (Exception e) { Debug.Log(e.Message); }
        }
        else
        {
            AssignRoles();
        }
    }

    IEnumerator FlashPlayernumNotif()
    {
        try { button_begin_game.interactable = false; }
        catch { }
        try
        {
            PlayerNumPrompt.SetActive(true);
            LeanTween.value(0, 1, 0.3f)
                .setOnUpdate((float t) =>
                {
                    try
                    {
                        Color c1 = PlayerNumPrompt.GetComponent<Image>().color;
                        Color c2 = PlayerNumText.color;

                        c1.a = t;
                        c2.a = t;

                        PlayerNumPrompt.GetComponent<Image>().color = c1;
                        PlayerNumText.color = c2;
                    }
                    catch(Exception e) { Debug.Log(e.Message); }
                });
        }
        catch (Exception e) { Debug.Log(e.Message); }

        yield return new WaitForSeconds(1.5f);

        try
        {
            LeanTween.value(1, 0, 0.3f)
                .setOnUpdate((float t) =>
                {
                    try
                    {
                        Color c1 = PlayerNumPrompt.GetComponent<Image>().color;
                        Color c2 = PlayerNumText.color;

                        c1.a = t;
                        c2.a = t;

                        PlayerNumPrompt.GetComponent<Image>().color = c1;
                        PlayerNumText.color = c2;
                    }
                    catch (Exception e) { Debug.Log(e.Message); }
                })
                .setOnComplete(() =>
                {
                    try
                    {
                        button_begin_game.interactable = true;
                        PlayerNumPrompt.SetActive(false);
                    }
                    catch (Exception e) { Debug.Log(e.Message); }
                });
        }
        catch (Exception e) { Debug.Log(e.Message); }
    }

    IEnumerator FlashSettingsNotif()
    {
        try { Button_SettingsBlock.interactable = false; }
        catch { }
        try
        {
            SettingsPrompts.SetActive(true);
            LeanTween.value(0, 1, 0.3f)
                .setOnUpdate((float t) =>
                {
                    try
                    {
                        Color c1 = SettingsPrompts.GetComponent<Image>().color;
                        Color c2 = SettingsPromptsText.color;

                        c1.a = t;
                        c2.a = t;

                        SettingsPrompts.GetComponent<Image>().color = c1;
                        SettingsPromptsText.color = c2;
                    }
                    catch { }
                });
        }
        catch { }

        yield return new WaitForSeconds(1.5f);

        try
        {
            LeanTween.value(1, 0, 0.3f)
                .setOnUpdate((float t) =>
                {
                    try
                    {
                        Color c1 = SettingsPrompts.GetComponent<Image>().color;
                        Color c2 = SettingsPromptsText.color;

                        c1.a = t;
                        c2.a = t;

                        SettingsPrompts.GetComponent<Image>().color = c1;
                        SettingsPromptsText.color = c2;
                    }
                    catch { }
                })
                .setOnComplete(() =>
                {
                    try
                    {
                        SettingsPrompts.SetActive(false);
                        Button_SettingsBlock.interactable = true;
                    }
                    catch { }
                });
        }
        catch { }
    }

    void AssignRoles()
    {
        button_begin_game.interactable = false;

        ra.Reset();
        ra.allTrueAllowed = this.toggle_angels.isOn;
        ra.AssignRoles(main.data.GetStringPlayersList());

        main.rc.AddRolesAndOrder(ra.playerTruths, ra.playerTargets, this);
    }

    public void OnButton()
    {
        HandleIncrease(listContent.transform.childCount);
    }

    bool IncreaseNotHandled = false;

    private void Update()
    {
        ////listScript.RefreshList();
        //try
        //{

        //Debug.Log(listScript.rects.Count);
        //}
        //catch { }

        //Debug.Log(listContent.transform.childCount);

        if(IncreaseNotHandled)
        {

            if (listContent.transform.childCount == playerNum)
            {
                HandleIncrease(listContent.transform.childCount);
                IncreaseNotHandled = false;
            }


        }

    }


}