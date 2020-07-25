using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

using Firebase;
using Firebase.Database;
using Firebase.Analytics;
using Firebase.Unity.Editor;
using Newtonsoft.Json;
using System.Linq;
using Firebase.Firestore;
using System.Net;
using System.Globalization;

using UnityEngine.Events;
using UnityEngine.UI;
using Firebase.Extensions;

[System.Serializable] public class StatusEvent : UnityEvent<string, bool> { }

public class MainScript : MonoBehaviour
{
    #region FIELD VARS

    // ADMIN VARS
    public bool debugModeOn = false;
    public bool AdsOn = true;
    public bool iapOn = false;

    public bool killTimeActive = true;
    public DateTime killTime = new DateTime(2020, 6, 13,
        23, 59, 0, 0, DateTimeKind.Utc);

    public bool PrefsWithName = true;
    public List<string> credits = new List<string>();

    // FIELD VARS
    public DatabaseReference databaseRef;
    public FirebaseFirestore fs;

    public PlayerNameHolder playerNameHolder;

    public UIData data;
    public TransitionManager tm;
    public DialogManager dm;
    public RoomListener rl;
    public RoomChanger rc;
    public Purchaser purch;

    public List<AudioClip> soundsList;
    public Dictionary<string, AudioClip> soundsLookup = new Dictionary<string, AudioClip>();
    public AudioSource sfx;
    public AudioSource music;

    public static Color true_blue = new Color32(2, 209, 192, 255);
    public static Color true_blue_dark = new Color32(88, 159, 133, 255);
    public static Color bull_red = new Color32(255, 166, 158, 255);
    public static Color bull_red_dark = new Color32(161, 31, 44, 255);

    public GameObject mouseClick;
    public GameObject Main_Canvas;
    public GameObject DebugObj;

    // Panels to resize for ad
    public GameObject Pos_Panel;
    public GameObject Dialog_Panel;
    public GameObject Main_Panel;
    public GameObject Notif_Panel;
    public GameObject Adspace;

    /// <summary>
    /// "True" means an update, "false" means an error
    /// </summary>
    public StatusEvent statusEvent = new StatusEvent();

    #endregion

    string deviceID;
    private RuntimePlatform devicePlatform;

    // HELPER CLASSES
    public GameChecker gcheck;
    public FirebaseInitialiser finit;
    public MembershipChecker mcheck;
    public MembershipAdder madd;
    public GameCreator gcreat;
    public GameJoiner gjoin;
    public RoomExiter rexit;

    #region START
    private void Awake()
    {
        gcheck = gameObject.AddComponent<GameChecker>();
        finit = gameObject.AddComponent<FirebaseInitialiser>();
        mcheck = gameObject.AddComponent<MembershipChecker>();
        madd = gameObject.AddComponent<MembershipAdder>();
        gcreat = gameObject.AddComponent<GameCreator>();
        gjoin = gameObject.AddComponent<GameJoiner>();
        rexit = gameObject.AddComponent<RoomExiter>();
    }

    // Start is called before the first frame update
    void Start()
    {

        deviceID = SystemInfo.deviceUniqueIdentifier;
        devicePlatform = Application.platform;

        if (killTimeActive) Kill(false);

        GetPlayerPrefs();

        InitialiseSound();

        InternetCheck();
    }


    public void MakeTopSpace(float h)
    {
        List<GameObject> panels = new List<GameObject>();
        panels.Add(Main_Panel);
        panels.Add(Dialog_Panel);
        panels.Add(Pos_Panel);
        panels.Add(Notif_Panel);

        float newH = h;

        // Find fraction of height
        float frac = newH / Main_Canvas.GetComponent<RectTransform>().rect.height;

        foreach (GameObject p in panels)
        {
            RectTransform rt = p.GetComponent<RectTransform>();

            // Set anchor
            rt.anchorMax = new Vector2(rt.anchorMax.x, 1 - frac);

            // Set position
            rt.offsetMax = new Vector2(rt.offsetMax.x, -newH);
        }

        // ADSPACE
        RectTransform rt2 = Adspace.GetComponent<RectTransform>();

        // Set anchor
        rt2.anchorMin = new Vector2(rt2.anchorMin.x, 1 - frac);

        // Set position
        rt2.offsetMin = new Vector2(rt2.offsetMin.x, -newH);

        Debug.Log("h: " + h
            + ", dpi: " + Screen.dpi
            + ", rect.top: " + panels[0].GetComponent<RectTransform>().rect.yMin);
    }

    public void OnRemoveAdsPressed()
    {
        MakeMember();
    }

    public void UndoAdsSpace()
    {
        List<GameObject> panels = new List<GameObject>();
        panels.Add(Main_Panel);
        panels.Add(Dialog_Panel);
        panels.Add(Pos_Panel);
        panels.Add(Notif_Panel);

        foreach (GameObject p in panels)
        {
            RectTransform rt = p.GetComponent<RectTransform>();

            // Set anchor
            rt.anchorMax = new Vector2(rt.anchorMax.x, 1);

            // Set position
            rt.offsetMax = new Vector2(rt.offsetMax.x, 0);
        }

        // ADSPACE
        RectTransform rt2 = Adspace.GetComponent<RectTransform>();

        // Set anchor
        rt2.anchorMin = new Vector2(rt2.anchorMin.x, 1);

        // Set position
        rt2.offsetMin = new Vector2(rt2.offsetMin.x, 0);
    }

    public void Kill(bool network)
    {
        if (network)
        {
            if (killTimeActive && killTime < GetNetTime()) Destroy(this.gameObject);
        }
        else
        {
            if (killTimeActive && killTime < DateTime.UtcNow) Destroy(this.gameObject);
        }
    }

    public void OnGameExistenceConfirmed(bool confirmed)
    {
        Debug.Log("Existence of " + gcheck.roomCodePref + "confirmed");

        if (confirmed)
        {
            Debug.Log("OnGameExistenceConfirmed confirmed");

            try
            {
                ResumeGame();

            } catch (Exception e) { Debug.Log(e.Message); }
        }
        else
        {
            Debug.Log("OnGameExistenceConfirmed NOT confirmed");
            InitialState(true);
        }
    }

    void ResumeGame()
    {
        dm.RequestDialog("RESUME_GAME");
    }

    public void OnGameResume(bool resume)
    {
        if (resume)
        {
            RequestRoomListeningBegin(gcheck.roomCodePref);
        }
        else
        {
            LeaveRoom(true, gcheck.room);
            InitialState(true);
        }
    }

    public void InitialState(bool requestState)
    {
        SavePrefString("roomCode", "");
        SavePrefString("TurnStartTime", ""); // Reset time prefs
        SavePrefString("hasVoted", "False"); // Reset key prefs


        if (requestState)
        {
            if (firstTime)
            {
                RequestFragmentDirect(TUTORIAL.ID);
            }
            else
            {
                RequestFragmentDirect(MAIN_MENU.ID);
            }
        }

    }

    public void QueryTest()
    {
        // TEST
        //QueryHelper qh = new QueryHelper(false, fs, null);
        //qh.BeginQuery();
        // TEST
    }

    #endregion


    #region SOUND
    public bool isMuted = false;
    public float recentMusicVol = 0;
    public static float MUSIC_VOLUME_NORMAL = 0.8f;
    public static float MUSIC_VOLUME_LOW = 0.3f;

    public void SetMute(bool? b)
    {
        if (b != null) // Set
        {
            isMuted = (bool)b;
        }
        else // Toggle
        {
            isMuted = !isMuted;
        }

        // Save pref
        SavePrefString("isMuted", isMuted ? "True" : "False");

        if (!isMuted) music.volume = recentMusicVol;
        else
        {
            music.volume = 0;
        }
    }

    private void InitialiseSound()
    {
        SetMute(isMuted);

        // BUTTONS / UI
        soundsLookup.Add("button_minor", soundsList[6]); // Minor button press sound
        soundsLookup.Add("button_major", soundsList[1]); // Major button press sound
        soundsLookup.Add("button_start_game", soundsList[3]); // When "CREATE GAME" or "JOIN GAME" is pressed on main menu

        // MAIN
        soundsLookup.Add("utter_bull_main_theme", soundsList[20]); // Main menu music

        // WAITING ROOM
        soundsLookup.Add("player_joined_lobby", soundsList[7]); // On player joining lobby

        // TEXT ENTRY
        soundsLookup.Add("ready_down", soundsList[12]); // Player readied down
        soundsLookup.Add("ready_up", soundsList[13]); // Player readied up

        // ROLES
        soundsLookup.Add("card_draw", soundsList[4]); // When a card is moved
        soundsLookup.Add("shuffle_role_select", soundsList[14]); // Cards shuffled
        soundsLookup.Add("player_selected", soundsList[9]); // Player landed on in ticker

        // CHOOSE
        soundsLookup.Add("tick_player_ticker", soundsList[8]); // A name ticked through on ticker

        // PLAY
        soundsLookup.Add("timer_music", soundsList[17]); // Timer main music
        soundsLookup.Add("timer_10_secs", soundsList[0]); // When timer reaches 10 seconds left
        soundsLookup.Add("play_end", soundsList[5]); // Play end after everyone has voted
        soundsLookup.Add("timer_end_alarm", soundsList[16]); // Timer reached zero alarm
        soundsLookup.Add("timer_bleed", soundsList[15]); // On timer end, the remaining time bleeding out

        // REVEAL
        soundsLookup.Add("tribal_drums", soundsList[18]); // On reveals intro
        soundsLookup.Add("true_reveal", soundsList[19]); // When a statement is revealed to be true
        soundsLookup.Add("bull_reveal", soundsList[2]); // When a statement is revealed to be bull
        soundsLookup.Add("points_gained_high", soundsList[10]); // Points gained high pitched sound
        soundsLookup.Add("points_gained_low", soundsList[11]); // Points gained lower pitched sound

        // UNDETERMINED
        soundsLookup.Add("victory_tone", soundsList[21]); // ?
    }

    public void PlaySfx(string name, float volume)
    {
        AudioClip ac = soundsLookup[name];

        if (!isMuted) sfx.volume = volume;
        else sfx.volume = 0;

        //sfx.clip = ac;
        sfx.PlayOneShot(ac);
    }

    public void PlayMusic(string name, float volume)
    {
        if (name != null)
        {
            AudioClip ac = soundsLookup[name];
            recentMusicVol = volume;

            if (!isMuted) music.volume = volume;

            if (music.clip != ac)
            {
                music.clip = ac;
                music.Play();
            }
        }
        else
        {
            music.volume = volume;
        }


    }

    #endregion

    #region PREFS

    public string playerName = "";
    public GameSettings gameSettings;
    public string roomCodePref;
    public bool firstTime;
    public string isMember;

    public void GetPlayerPrefs()
    {
        // Get preferred player name (empty string by default)

        //PlayerPrefs.DeleteAll();

        if (playerNameHolder.active)
        {
            playerName = playerNameHolder.playerName;
        }
        else
        {
            playerName = GetPrefString("playerName", "");
        }

        roomCodePref = GetPrefString("roomCode", "");

        // Get preferred GameSettings
        gameSettings = JsonConvert.DeserializeObject<GameSettings>(
             GetPrefString("gameSettings", JsonConvert.SerializeObject(new GameSettings(3, true, false))));

        // Mute pref
        isMuted = bool.Parse(GetPrefString("isMuted", "False"));

        firstTime = bool.Parse(GetPrefString("firstTime", "True"));

        isMember = GetPrefString("isMember", MembershipChecker.MEMBER_UNKNOWN);

        Debug.Log("isMember: " + isMember);
    }

    List<Dictionary<string, string>> prefsToUpdate;
    public void SavePrefString(string tag, string stringPref)
    {
        if (prefsToUpdate == null) prefsToUpdate = new List<Dictionary<string, string>>();

        Dictionary<string, string> d = new Dictionary<string, string>();
        d.Add((PrefsWithName ? playerName : "") + tag, stringPref);
        prefsToUpdate.Add(d);

        PlayerPrefUpdateRequired = true;
    }
    public string GetPrefString(string tag, string defaultVal)
    {
        return PlayerPrefs.GetString((PrefsWithName ? playerName : "") + tag, defaultVal);
    }

    #endregion

    #region FIREBASE INITIALISATION

    public void InternetCheck()
    {
        bool c = !(Application.internetReachability == NetworkReachability.NotReachable);

        OnInternetChecked(c);
    }

    private void OnInternetChecked(bool c)
    {
        if (c)
        {
            FirebaseInit();
        }
        else
        {
            dm.RequestDialog("CHECK_INTERNET");
        }
    }

    public void FirebaseInit()
    {
        Debug.Log("FirebaseInit: CALLED");

        finit.BeginInitialising(this, FirebaseInitialiser.MODE_FIRST);
    }

    // Completion callback from FirebaseInitialiser
    internal void OnFirebaseInitComplete(bool success)
    {
        Debug.Log("OnFirebaseInitComplete: CALLED");
        if (success)
        {
            this.databaseRef = finit.databaseRef;
            this.fs = finit.fs;


            // AD SETTINGS
            if (AdsOn &&
            (devicePlatform == RuntimePlatform.Android || devicePlatform == RuntimePlatform.IPhonePlayer))
            {
                try
                {
                    if (isMember == MembershipChecker.MEMBER_UNKNOWN)
                    {
                        Debug.Log("2 isMember UNKNOWN");
                        mcheck.BeginChecking(this, databaseRef, this.deviceID);
                    }
                    else
                    {
                        bool b = (isMember == MembershipChecker.MEMBER_TRUE ? true : false);
                        Debug.Log("3 isMember: " + isMember);

                        if (!b) AdManager.instance.RequestBanner();
                        else UndoAdsSpace();
                    }
                }
                catch (Exception e) { Debug.Log(e.Message); }
            }
            else
            {
                this.isMember = debugModeOn ? MembershipChecker.MEMBER_FALSE : MembershipChecker.MEMBER_TRUE;
            }

            // ROOM PERSISTANCE
            if (roomCodePref != "" && roomCodePref.Length == 5)
            {
                // Check room still exists
                Debug.Log("Checking existence of " + roomCodePref);
                gcheck.BeginChecking(this, databaseRef, roomCodePref);
            }
            else
            {
                InitialState(true);
            }

            // CREDITS
            databaseRef.Child("credits").GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted && !task.IsFaulted)
                {
                    credits = JsonConvert.DeserializeObject<List<string>>(task.Result.GetRawJsonValue());
                }
            });

        }
        else
        {
            InvokeStatusEvent("There was an error initialising the server. Check your connection, restart the app, or try again later.", false);
            dm.RequestDialog("CHECK_FIREBASE");
        }
    }


    internal void OnFirebaseReinitComplete(bool success)
    {
        Debug.Log("OnFirebaseReinitComplete: CALLED");
        if (success)
        {
            Debug.Log("FB REINIT SUCCESS");

            this.databaseRef = finit.databaseRef;
            this.fs = finit.fs;
        }
        else
        {
            InvokeStatusEvent("There was an error initialising the server. Check your connection, restart the app, or try again later.", false);
            dm.RequestDialog("CHECK_FIREBASE");
        }
    }

    #endregion
    #region MEMBERSHIP
    public void OnMembershipConfirmed()
    {
        this.isMember = (mcheck.isMember ? MembershipChecker.MEMBER_TRUE : MembershipChecker.MEMBER_FALSE);
        try
        {
            SavePrefString("isMember", this.isMember);

            Debug.Log("5 isMember: " + isMember);

            if (this.isMember == MembershipChecker.MEMBER_TRUE)
            {
                Debug.Log("6 isMember: TRUE");
                AdManager.instance.RemoveAds();
            }
            else
            {
                Debug.Log("6 isMember: FALSE");
                AdManager.instance.RequestBanner();
            }

        }
        catch (Exception e) { Debug.Log("OnMembershipconfirmed: " + e.Message); }
        Debug.Log("4 isMember: " + isMember);

    }

    public void MakeMember()
    {
        madd.BeginAdding(this, this.databaseRef, this.deviceID);
    }

    public void OnMemberAdded()
    {
        if (madd.success)
        {
            this.isMember = MembershipChecker.MEMBER_TRUE;
            SavePrefString("isMember", MembershipChecker.MEMBER_TRUE);
            AdManager.instance.RemoveAds();
            RequestFragmentDirect(MAIN_MENU.ID);
        }
    }

    #endregion

    #region SCENE MANAGEMENT


    public void RequestFragmentFromState(string stateName)
    {
        //SetScene(scene);
        //PlayerPrefs.SetString("scene", JsonConvert.SerializeObject(scene));

        // this.statePref = state;

        tm.InterpretState(stateName);
    }

    internal void RequestFragmentDirect(string fragName)
    {
        tm.TransitionTo(fragName);
    }

    #endregion

    #region ALL DIALOGS

    public void OnDialogRequested(string dialogName)
    {
        dm.RequestDialog(dialogName);
    }



    public void CloseCurrentDialog()
    {
        dm.CloseCurrentDialog();
    }

    #endregion


    #region CREATE_GAME DIALOG

    public void OnCreateGame(string playerName)
    {
        Debug.Log("OnCreateGame: playerName: " + playerName);

        dm.CloseCurrentDialog();

        this.playerName = playerName;
        SavePrefString("playerName", playerName);

        gameSettings = JsonConvert.DeserializeObject<GameSettings>(
             GetPrefString("gameSettings", JsonConvert.SerializeObject(new GameSettings(3, true, false))));

        if (databaseRef != null)
        {
            InvokeStatusEvent("Generating game code", true);
            gcreat.CreateGame(this, databaseRef, playerName, gameSettings);
        }
        else
        {
            OnGameCreationComplete("Failed to initialise servers.");
        }

    }

    // Update callback from GameCreator
    public void GameCreationUpdate(string message)
    {
        InvokeStatusEvent(message, true);
    }

    // Completion callback from GameCreator
    public void OnGameCreationComplete(string str)
    {
        if (killTimeActive) Kill(false);

        if (str == null)
        {
            RequestRoomListeningBegin(gcreat.room.roomCode);
        }
        else
        {
            InvokeStatusEvent(str, false);
        }
    }

    #endregion

    #region JOIN_GAME DIALOG


    public void OnJoinGame(string playerName, string gameCode)
    {

        Debug.Log("OnJoinGame: playerName: " + playerName);

        dm.CloseCurrentDialog();

        this.playerName = playerName;
        SavePrefString("playerName", playerName);

        if (databaseRef != null)
        {
            InvokeStatusEvent("Searching rooms", true);
            
            gjoin.JoinGame(this, databaseRef, playerName, gameCode);
        }
        else
        {
            OnGameJoinComplete("Failed to initialise servers");
        }
    }

    // Update callback from GameJoiner
    public void GameJoinUpdate(string message)
    {
        InvokeStatusEvent(message, true);
    }

    public void InvokeStatusEvent(string message, bool isError)
    {

        statusEvent.Invoke(message, isError);
    }

    // Completion callback from GameJoiner
    public void OnGameJoinComplete(string str)
    {
        Debug.Log("OnGameJoinComplete: CALLED: gj.gameCode: " + gjoin.gameCode + ", success: " + (str==null));

        try
        {
            if (str == null)
            {
                RequestRoomListeningBegin(gjoin.gameCode);
            }
            else
            {
                InvokeStatusEvent(str, false);
            }

        }
        catch (Exception e) { Debug.Log("OnGameJoinComplete ERROR: " + e.Message + ", " + e.Source + ", " + e.StackTrace); }

    }

    #endregion


    #region ROOM LISTENER

    void RequestRoomListeningBegin(string roomCode)
    {
        try
        {
            Debug.Log("RequestRoomListeningBegin: !rl.IsListening: " + (!rl.IsListening));
            Debug.Log("RequestRoomListeningBegin: databaseRef != null: " + (databaseRef != null));
            Debug.Log("RequestRoomListeningBegin: roomCode != null: " + (roomCode != null));
            Debug.Log("RequestRoomListeningBegin: roomCode.Length == 5: " + (roomCode.Length == 5));
            Debug.Log("RequestRoomListeningBegin: ALL ABOVE: " + (!rl.IsListening && databaseRef != null && roomCode != null && roomCode.Length == 5));

            if (!rl.IsListening && databaseRef != null && roomCode != null && roomCode.Length == 5)
            {
                //Debug.Log("pp BEFORE: roomCode: "+roomCode);
                //PlayerPrefs.SetString("roomCode", roomCode);
                //Debug.Log("pp AFTER");

                Debug.Log("Configuring room: " + roomCode);
                rl.ConfigureListeners(roomCode, databaseRef);
            }
        }
        catch (Exception e) { Debug.Log("RequestRoomListeningBegin ERROR: " + e.Message + ", " + e.Source + ", " + e.StackTrace); }
    }

    // LEAVE ROOM
    public void LeaveRoom(bool ToLobby, Room _room)
    {
        Room room = null;
        Dictionary<string, Player> playersLookup = null;
        DatabaseReference roomRef = null;

        if (_room == null)
        {
            room = rl.room;
            roomRef = rl.roomRef;

            playersLookup = this.data.playersLookup;
        }
        else
        {
            room = _room;
            roomRef = databaseRef.Child("Rooms/" + room.roomCode);

            playersLookup = RoomListener.GetPlayersLookupFromRoom(room);
        }



        rexit.toLobby = ToLobby;
        rexit.BeginExit(this, room, playersLookup, roomRef, databaseRef, playerName);

    }

    public void OnLeaveRoomUpdate(string msg)
    {

    }

    public void OnExitRoomComplete()
    {
        Debug.Log("OnExitRoomComplete: CALLED");
        try
        {
            ReplaceRoomListener();
            ReplaceRoomChanger();
            this.data.Clear(); 

            RequestFragmentDirect(MAIN_MENU.ID);

            InitialState(false);

            //finit.BeginInitialising(this, FirebaseInitialiser.MODE_REINIT);

        }
        catch (Exception e) { Debug.Log("OnExitRoomComplete ERROR: " + e.Message + ", " + e.StackTrace); }
    }

    void ReplaceRoomListener()
    {
        GameObject rlgo = rl.gameObject;

        Destroy(rl);

        rl = rlgo.AddComponent<RoomListener>();
        rl.main = this;
    }

    void ReplaceRoomChanger()
    {
        GameObject rcgo = rc.gameObject;

        Destroy(rc);

        rc = rcgo.AddComponent<RoomChanger>();
        rc.main = this;
    }

    // DATE TIME

    public static DateTime GetNetTime()
    {
        var myHttpWebRequest = (HttpWebRequest)WebRequest.Create("http://www.microsoft.com");
        var response = myHttpWebRequest.GetResponse();
        string todaysDates = response.Headers["date"];
        return DateTime.ParseExact(todaysDates,
                                   "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                                   CultureInfo.InvariantCulture.DateTimeFormat,
                                   DateTimeStyles.AssumeUniversal);
    }


    #endregion


    #region UPDATE

    bool PlayerPrefUpdateRequired = false;
    bool DebugConsoleOn = false;



    void Update()
    {
        if (PlayerPrefUpdateRequired)
        {
            try
            {
                Dictionary<string, string> d = prefsToUpdate[prefsToUpdate.Count - 1];
                string tag = d.Keys.First<string>();
                string stringPref = d[tag];

                PlayerPrefs.SetString(tag, stringPref);
                Debug.Log("PlayerPrefs.SetString " + tag + " to " + stringPref);

                prefsToUpdate.Remove(d);

                if (prefsToUpdate.Count == 0) PlayerPrefUpdateRequired = false;

            } catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {

            // STANDALONE ONLY
            if (Application.platform == RuntimePlatform.OSXPlayer
                || Application.platform == RuntimePlatform.WindowsPlayer
                || Application.isEditor)
            {

                Vector3 mousePos = Input.mousePosition;
                Instantiate(mouseClick, mousePos, Quaternion.identity, Main_Canvas.transform);
            }

        }

        //if (Input.GetMouseButtonDown(1))
        //{
        //    DebugConsoleOn = !DebugConsoleOn;
        //    DebugObj.SetActive(DebugConsoleOn);
        //}




        //time += Time.deltaTime;
        //if(time > 1)
        //{
        //    time = 0;
        //    Debug.Log("firstTime?: " + PlayerPrefs.GetString(playerName+"firstTime", "True"));
        //}

    }

    float time = 0;

    #endregion
}
