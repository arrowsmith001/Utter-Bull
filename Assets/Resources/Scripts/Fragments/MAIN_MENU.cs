using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Threading;
using TMPro;
using Firebase.Firestore;

public class MAIN_MENU : Fragment
{
    public const string ID = "MM";
    public override string GetID() { return ID; }

    public GameObject Pos_Title;
    public GameObject Pos_Bottom;
    public GameObject Pos_Top;
    public PositionsHolder ph;

    public List<Sprite> soundOffOnSprites;

    public Button button_create;
    public Button button_join;
    public Button button_tutorial;
    public Button button_sound;
    public Button button_info;
    public Button button_removeads;

    public GameObject Create_Game_Panel;
    public GameObject Join_Game_Panel;
    public GameObject Top_Panel;

    public Image Image_Spiny1;
    public Image Image_Spiny2;
    public Image Image_Utter;
    public Image Image_Bull;
    public Image Image_BullPic;
    public Image Image_Bubble;

    public GameObject statusPanel;
    public Text statusText;
    public GameObject statusImg;
    public GameObject spinner;

    public LeanTweenType easeType;
    private Interpolator interp = new OvershootInterpolator(2);

    bool ShowButtonsNow;

    public Button query_test;

    public Image Img_Create;
    public Text Text_Create;
    public Image Img_Join;
    public Text Text_Join;

    public Text version;

    // Start is called before the first frame update
    void Start()
    {
        base.GetMain();

        // version.text = Application.version;

        try
        {
            // Subscribe method to status event TODO Remove listener onDestroy
            main.statusEvent.AddListener(StatusUpdate);

        }
        catch (Exception e) { Debug.Log(e.Message); }

        // Play music
        main.PlayMusic("utter_bull_main_theme", MainScript.MUSIC_VOLUME_NORMAL);

        // Refresh player prefs, act on "firstTime" condition
        bool firstTime = bool.Parse(main.GetPrefString("firstTime", "True"));
        if (firstTime)
        {
            ShowButtonsNow = false;
            main.SavePrefString("firstTime", "False");
        }
        else
        {
            ShowButtonsNow = true;
        }

        Debug.Log("ShowButtonsNow: " + ShowButtonsNow);

        InitialiseViews();

        SetListeners();

        StartIntroAnim();

        SetUIValues();
    }

    public override void Initialise()
    {

    }

    public override void RegisterEventListeners()
    {

    }

    public override void UnregisterEventListeners()
    {

    }


    Color32 grey = new Color32(122, 122, 122, 255);

    public void StatusUpdate(string text, bool isNotError)
    {
        try
        {
            statusPanel.SetActive(true);

            statusText.text = text;
            statusImg.SetActive(!isNotError);
            spinner.SetActive(isNotError);

            // Buttons
            button_create.interactable = !isNotError;
            button_join.interactable = !isNotError;
            button_tutorial.interactable = !isNotError;

            Text_Create.color = isNotError ? grey : white;
            Text_Join.color = isNotError ? grey : white;
            Img_Create.color = isNotError ? grey : white;
            Img_Join.color = isNotError ? grey : white;
            button_tutorial.GetComponent<Image>().color = isNotError ? grey : white;
        }
        catch (Exception e) { Debug.Log(e.Message); }
    }

    void StartIntroAnim()
    {
        try
        {

            // Make image transparent
            foreach (Image i in images)
            {
                Color c = i.color;
                c.a = 0;
                i.color = c;
            }

            float dur_utter = 0.5f;
            float del_utter = 0.5f;
            float dur_bubble = 0.3f;
            float del_bubble = del_utter + dur_utter - 0.25f;
            float dur_bull = 0.5f;
            float del_bull = del_bubble + dur_bubble - 0.25f;
            float dur_bullpic = 0.3f;
            float del_bullpic = del_bull + dur_bull - 0.25f;
            float dur_spinybig = 1f;
            float del_spinybig = del_bullpic + dur_bullpic - 0.25f;
            float dur_spinysml = 1f;
            float del_spinysml = del_spinybig + 0.5f - 0.25f;

            // Fade in UTTER
            // Move in UTTER
            LeanTween.value(Image_Utter.gameObject, 0, 1,
                dur_utter)
                .setDelay(del_utter)
                .setOnUpdate((float val) =>
            {
                try { if (Image_Utter != null) SetImageAlpha(Image_Utter, val); } catch { }
            });
            Tween(Image_Utter, "Utter_2", dur_utter, this, del_utter, LeanTweenType.easeSpring, true, true, null);

            // Pop in BUBBLE
            Tween(Image_Bubble, "Bubble_2", dur_bubble, this, del_bubble, LeanTweenType.easeSpring, true, true, null);
            LeanTween.value(0, 1, 0.5f).setDelay(del_bubble).setOnUpdate((float val) =>
            {
                try { if (Image_Bubble != null) SetImageAlpha(Image_Bubble, 1); } catch { }
            });

            // Fade in BULL
            // Move in BULL
            LeanTween.value(Image_Bull.gameObject, 0, 1,
                dur_bull)
                .setDelay(del_bull)
                .setOnUpdate((float val) =>
                {
                    try { if (Image_Bull != null) SetImageAlpha(Image_Bull, val); } catch { }
                });
            Tween(Image_Bull, "Bull_2", dur_bull, this, del_bull, LeanTweenType.easeSpring, true, true, null);

            // Pop in BULLPIC
            Tween(Image_BullPic, "BullPic_2", dur_bullpic, this, del_bullpic, LeanTweenType.easeSpring, true, true, null);
            LeanTween.value(9, 9, 9).setDelay(del_bullpic).setOnUpdate((float val) =>
              {
                  try { if (Image_BullPic != null) SetImageAlpha(Image_BullPic, 1); } catch { }
              });

            // Fade in SPINYBIG
            // Move in SPINYBIG
            LeanTween.value(Image_Spiny1.gameObject, 0, 1,
                dur_spinybig)
                .setDelay(del_spinybig)
                .setOnUpdate((float val) =>
                {
                    try { if (Image_Spiny1 != null) SetImageAlpha(Image_Spiny1, val); } catch { }
                });
            Tween(Image_Spiny1, "SpinyBig_2", dur_spinybig, this, del_spinybig, LeanTweenType.easeOutQuint, true, true, null);

            // Fade in SPINYSML
            // Move in SPINYSML
            LeanTween.value(Image_Spiny2.gameObject, 0, 1,
                dur_spinysml)
                .setDelay(del_spinysml)
                .setOnUpdate((float val) =>
                {
                    try { if (Image_Spiny2 != null) SetImageAlpha(Image_Spiny2, val); } catch { }
                });
            Tween(Image_Spiny2, "SpinySml_2", dur_spinysml, this, del_spinysml, LeanTweenType.easeOutQuint, true, true, null);

            // ONGOING ANIMS

            // SPINYS
            try
            {
                LeanTween.rotateAroundLocal(Image_Spiny1.gameObject, Vector3.back, 360, 25.0f).setRepeat(-1);
                LeanTween.rotateAroundLocal(Image_Spiny2.gameObject, Vector3.back, 360, 15.0f).setRepeat(-1);
            }
            catch { }

            // BULLPIC ROTATE
            BullPic_Start = true;

            // WORDS POP
            Words_Bounce_Start = true;

            // BUBBLE ROTATE
            Bubble_Start = true;

        }
        catch (Exception e) { Debug.Log(e.Message + ", " + e.Source + ", " + e.StackTrace); }
    }

    public static void SetImageAlpha(Image img, float val)
    {
        Color c = img.color;
        c.a = val;
        img.color = c;
    }


    public bool ReactToPhaseChange(string newPhase)
    {
        Debug.Log(this.GetID() + ": ReactToPhaseChange: newPhase: " + newPhase);
        try
        {
            if (newPhase == "Lobby")
            {
                main.RequestFragmentFromState("Lobby");
            }

            return true;
        }
        catch (Exception e)
        {
            return false;
        }

    }

    public bool SetUIValues()
    {
        try
        {

            button_removeads.gameObject.SetActive(main.isMember != MembershipChecker.MEMBER_TRUE);

            return true;
        }
        catch (Exception e)
        {
            return false;
        }

    }


    List<Image> images = new List<Image>();

    void InitialiseViews()
    {
        ph = new PositionsHolder();
        ph.AddPositions(Pos_Title);
        ph.AddPositions(Pos_Bottom);
        ph.AddPositions(Pos_Top);

        SetTransformPos(Image_Bubble, "Bubble_1", this);
        SetTransformPos(Image_Bull, "Bull_1", this);
        SetTransformPos(Image_BullPic, "BullPic_1", this);
        SetTransformPos(Image_Spiny1, "SpinyBig_1", this);
        SetTransformPos(Image_Spiny2, "SpinySml_1", this);
        SetTransformPos(Image_Utter, "Utter_1", this);

        images.Add(Image_Bubble);
        images.Add(Image_Bull);
        images.Add(Image_BullPic);
        images.Add(Image_Spiny1);
        images.Add(Image_Spiny2);
        images.Add(Image_Utter);

        // Set panels into place
        SetTransformPos(Create_Game_Panel.transform, "Create_1", this);
        SetTransformPos(Join_Game_Panel.transform, "Join_1", this);
        SetTransformPos(button_tutorial.transform, "Tutorial_1", this);
        SetTransformPos(Top_Panel.transform, "Top_1", this);

        Create_Game_Panel.gameObject.SetActive(false);
        Join_Game_Panel.gameObject.SetActive(false);
        button_tutorial.gameObject.SetActive(false);
        Top_Panel.gameObject.SetActive(false);

        Tween(Create_Game_Panel.transform, "Create_2", 1, this, ShowButtonsNow ? 0 : 1, LeanTweenType.easeOutCirc, true, true, null);
        Tween(Join_Game_Panel.transform, "Join_2", 1, this, ShowButtonsNow ? 0 : 1, LeanTweenType.easeOutCirc, true, true, null);
        Tween(button_tutorial.transform, "Tutorial_2", 1, this, ShowButtonsNow ? 0 : 1, LeanTweenType.easeOutCirc, true, true, null);
        Tween(Top_Panel.transform, "Top_2", 1, this, ShowButtonsNow ? 0 : 1, LeanTweenType.easeOutCirc, true, true, null);

        query_test.gameObject.SetActive(false);
    }

    void SetListeners()
    {
        // Set audio button
        SetMuteIcon(main.isMuted);

        button_create.onClick.AddListener(() => OnCreateGame());
        button_join.onClick.AddListener(() => OnJoinGame());
        button_tutorial.onClick.AddListener(() => main.RequestFragmentFromState("TutorialRevisit"));
        button_sound.onClick.AddListener(() =>
        {
            main.SetMute(null);
            SetMuteIcon(main.isMuted);
        });
        button_info.onClick.AddListener(() =>
        {
            main.OnDialogRequested("INFO_DIALOG");
        });

        query_test.onClick.AddListener(() =>
        {
            //FirebaseFirestore fs = main.fs;
            //QueryHelper qh = new QueryHelper(false, fs, null);
            //// qh.setCustomIdea("i met @ [weird_professional] with <20-100>.");
            //qh.BeginQuery();
        });
        button_removeads.onClick.AddListener(() =>
        {
            if (main.iapOn) main.purch.BuyAdFree();
            else main.OnRemoveAdsPressed();
        });
    }

    private void SetMuteIcon(bool isMuted)
    {
        Image img = button_sound.gameObject.GetComponent<Image>();
        img.sprite = soundOffOnSprites[isMuted ? 0 : 1];
    }

    void OnCreateGame()
    {
        StartCoroutine(FlashCreateGame());
        main.PlaySfx("button_start_game", 0.9f);
        main.OnDialogRequested("CREATE_GAME");
    }

    void OnJoinGame()
    {
        StartCoroutine(FlashJoinGame());
        main.PlaySfx("button_start_game", 0.9f);
        main.OnDialogRequested("JOIN_GAME");
    }

    Color32 white = new Color32(255, 255, 255, 255);
    Color32 yellow = new Color32(255, 245, 0, 255);

    IEnumerator FlashCreateGame()
    {
        yield return null;
        try
        {
            LeanTween.value(0, 1, 0.05f).setOnUpdate((float t) =>
            {
                try
                {
                    Text_Create.color = Color.Lerp(white, yellow, t);
                    Img_Create.color = Color.Lerp(white, yellow, t);
                }
                catch { }
            })
                .setOnComplete(() =>
                {
                    try
                    {
                        LeanTween.value(0, 1, 0.25f).setOnUpdate((float t) =>
                        {
                            try
                            {
                                Text_Create.color = Color.Lerp(yellow, white, t);
                                Img_Create.color = Color.Lerp(yellow, white, t);
                            }
                            catch { }
                        });
                    }
                    catch { }
                });


        }
        catch { }


    }

    IEnumerator FlashJoinGame()
    {
        yield return null;
        try
        {
            LeanTween.value(0, 1, 0.05f).setOnUpdate((float t) =>
            {
                try
                {
                    Text_Join.color = Color.Lerp(white, yellow, t);
                    Img_Join.color = Color.Lerp(white, yellow, t);
                }
                catch { }
            })
                .setOnComplete(() =>
                {
                    try
                    {
                        LeanTween.value(0, 1, 0.25f).setOnUpdate((float t) =>
                        {
                            try
                            {
                                Text_Join.color = Color.Lerp(yellow, white, t);
                                Img_Join.color = Color.Lerp(yellow, white, t);
                            }
                            catch { }
                        });
                    }
                    catch { }
                });


        }
        catch { }
    }

    bool BullPic_Start = false;
    bool Words_Bounce_Start = false;
    bool Bubble_Start = false;

    // Update is called once per frame
    void Update()
    {
        if (BullPic_Start)
        {
            try
            {
                BullPic_Start = false;
                LeanTween.rotateAroundLocal(Image_BullPic.gameObject, Vector3.forward, -5, 1).setDelay(0f);//.setEase(LeanTweenType.punch);
                LeanTween.rotateAroundLocal(Image_BullPic.gameObject, Vector3.forward, 20, 3).setDelay(1f);//.setEase(LeanTweenType.punch);
                LeanTween.rotateAroundLocal(Image_BullPic.gameObject, Vector3.forward, -20, 6).setDelay(4f);//.setEase(LeanTweenType.punch);
                LeanTween.rotateAroundLocal(Image_BullPic.gameObject, Vector3.forward, 5, 2).setDelay(10f)//.setEase(LeanTweenType.punch)
                    .setOnComplete(() => BullPic_Start = true);

            }
            catch (Exception e) { Debug.Log(e.Message + ", " + e.Source + ", " + e.StackTrace); }
        }

        if (Words_Bounce_Start)
        {
            try
            {
                Words_Bounce_Start = false;
                LeanTween.scale(Image_Utter.gameObject.GetComponent<RectTransform>(), new Vector3(1, 1, 1), 0.3f).setEase(LeanTweenType.easeOutBounce)
                    .setDelay(3f)
                    .setFrom(new Vector3(0.75f, 0.75f));

                LeanTween.scale(Image_Bull.gameObject.GetComponent<RectTransform>(), new Vector3(1, 1, 1), 0.3f).setEase(LeanTweenType.easeOutBounce)
                    .setDelay(4f)
                    .setFrom(new Vector3(0.75f, 0.75f))
                    .setOnComplete(() => { try { Words_Bounce_Start = true; } catch { } });

            }
            catch (Exception e) { Debug.Log(e.Message + ", " + e.Source + ", " + e.StackTrace); }
        }
        if (Bubble_Start)
        {
            try
            {
                Bubble_Start = false;
                LeanTween.rotateAroundLocal(Image_Bubble.gameObject, Vector3.forward, -30, 1.5f).setDelay(0f).setEase(LeanTweenType.punch);
                LeanTween.rotateAroundLocal(Image_Bubble.gameObject, Vector3.forward, 30, 1.5f).setDelay(1.5f).setEase(LeanTweenType.punch)
                    .setOnComplete(() => { try { Bubble_Start = true; } catch { } });


            }
            catch (Exception e) { Debug.Log(e.Message + ", " + e.Source + ", " + e.StackTrace); }
        }
    }

    public override void RoomChangeResult(string code, Task task, bool success, object args)
    {


    }

    // UTILS

    void SetTransformPos(Component cmp, string posName, MAIN_MENU mm)
    {
        cmp.gameObject.GetComponent<RectTransform>().localPosition = mm.ph.positions[posName].GetComponent<RectTransform>().localPosition;
        cmp.gameObject.GetComponent<RectTransform>().anchorMin = mm.ph.positions[posName].GetComponent<RectTransform>().anchorMin;
        cmp.gameObject.GetComponent<RectTransform>().anchorMax = mm.ph.positions[posName].GetComponent<RectTransform>().anchorMax;
    }

    void Tween(Component cmp, string posName, float duration, MAIN_MENU mm,
        float delay, LeanTweenType easeType, bool ActivenessOnStart, bool ActivenessOnEnd,
        Action endAction)
    {
        try
        {
            Vector3 startPos = cmp.gameObject.transform.localPosition;
            Vector3 endPos = mm.ph.positions[posName].localPosition;

            Vector2 startSize = cmp.gameObject.GetComponent<RectTransform>().sizeDelta;
            Vector2 endSize = mm.ph.positions[posName].gameObject.GetComponent<RectTransform>().sizeDelta;

            LeanTween.value(0, 1, duration).setDelay(delay)
                .setOnUpdate((float t) =>
                {
                    try
                    {
                        Vector3 lerpedPos = interp.getVec3Interpolation(startPos, endPos, t);
                        Vector3 lerpedSize = interp.getVec3Interpolation(startSize, endSize, t);

                        if (cmp != null) cmp.gameObject.transform.localPosition = lerpedPos;
                        if (cmp != null) cmp.gameObject.GetComponent<RectTransform>().sizeDelta = lerpedSize;
                    }
                    catch { }
                })
                .setOnStart(() =>
                {
                    try { if (cmp != null) cmp.gameObject.SetActive(ActivenessOnStart); } catch { }
                })
                .setOnComplete(() =>
                {
                    try
                    {
                        if (cmp != null) cmp.gameObject.SetActive(ActivenessOnEnd);
                        if (endAction != null) endAction();
                    }
                    catch { }
                });

        }
        catch { }
    }

}
