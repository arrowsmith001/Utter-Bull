using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionManager : MonoBehaviour
{
    public GameObject mainCanvas;
    private MainScript main;

    public List<GameObject> fragments;
    Dictionary<string, GameObject> fragmentLookup = new Dictionary<string, GameObject>();

    public string currentFragId = LOADING.ID;

    public const int TRANSITION_SPECIAL = -99;
    public const int TRANSITION_APPEAR = -1;
    public const int TRANSITION_SLIDE_L2R = 0;
    public const int TRANSITION_SLIDE_R2L = 1;
    public const int TRANSITION_SLIDE_U2D = 2;
    public const int TRANSITION_SLIDE_D2U = 3;
    public const int TRANSITION_POP = 4;

    // Public
    public GameObject mainPanel; // Parent to-be

    public GameObject activePosition;
    public GameObject offscreenPositionLeft;
    public GameObject offscreenPositionRight;
    public GameObject offscreenPositionUp;
    public GameObject offscreenPositionDown;
    public GameObject popEntry;
    public GameObject popFinal;

    public float animSpeed;

    private Interpolator interp;
    public float interpFactor;

    public bool isTransitioning;
    private bool isAnimating;

    public GameObject newFrag;
    public GameObject currentFrag;
    public LeanTweenType easeType;

    // Private
    //private List<int[3]> transitionQueue;

    private Dictionary<string, Transform> pos;
    private float t;

    // Start is called before the first frame update
    void Start()
    {
        main = mainCanvas.GetComponent<MainScript>();

        isTransitioning = false;
        isAnimating = false;
        //transitionQueue = new List<int[3]>();

        foreach(GameObject go in this.fragments)
        {
            this.fragmentLookup.Add(go.GetComponent<Fragment>().GetID(), go);
        }

        pos = new Dictionary<string, Transform>();
        pos.Add("UP", offscreenPositionUp.transform);
        pos.Add("DOWN", offscreenPositionDown.transform);
        pos.Add("LEFT", offscreenPositionLeft.transform);
        pos.Add("RIGHT", offscreenPositionRight.transform);
        pos.Add("CENTER", activePosition.transform);
        pos.Add("POP_ENTRY", popEntry.transform);
        pos.Add("POP_FINAL", popFinal.transform);

        RegisterEventListeners();
    }

    void RegisterEventListeners()
    {
        main.data.phaseChangeEvent.AddListener(OnPhaseChanged);
    }

    private void OnPhaseChanged(string lastPhase, string newPhase)
    {
        if (lastPhase == "") InterpretState(newPhase);
    }

    public void InterpretState(string state)
	{

        switch(state)
		{
			case "Tutorial":
				TransitionTo(TUTORIAL.ID);
				break;
			case "TutorialRevisit":
                TransitionTo(TUTORIAL.ID);
                break;
			case "MainMenuLong":
                TransitionTo(MAIN_MENU.ID);
                break;
			case "MainMenuShort":
                TransitionTo(MAIN_MENU.ID);
                break;
			case "Lobby":
                TransitionTo(WAITING_ROOM.ID);
                break;
			case "ConfirmRoleInfo":
                TransitionTo(WAITING_ROOM.ID);
                break;
            case "TextEntryWriting":
                TransitionTo(GP2_TEXT_ENTRY.ID);
                break;
            case "TextEntryReady":
                TransitionTo(GP2_TEXT_ENTRY.ID);
                break;
            case "ConfirmWhoseTurn":
                TransitionTo(GP2_TEXT_ENTRY.ID);
                break;
            case "Choose":
                TransitionTo(GP3_CHOOSE_WHOSE_TURN.ID);
                break;
            case "Reading":
                TransitionTo(GP3_CHOOSE_WHOSE_TURN.ID);
                break;
            case "Play":
                TransitionTo(GP4_PLAY.ID);
                break;
            case "EndOfPlay":
                TransitionTo(GP4_PLAY.ID);
                break;
            case "ConfirmResetVariables":
                TransitionTo(GP4_PLAY.ID);
                break;
            case "RevealsIntro":
                TransitionTo(GP5_REVEALS_INTRO.ID);
                break;
            case "CheckRdi":
                TransitionTo(GP5_REVEALS_INTRO.ID);
                break;
            case "Reveals":
                TransitionTo(GP6_REVEALS_BASE.ID);
                break;
            //case "NextReveal":
            //    TransitionTo("GP6_REVEALS_BASE");
            //    break;
            //case "PostReveal":
            //    TransitionTo("GP6_REVEALS_BASE");
            //break;
            case "NextPage":
                TransitionTo(GP6_REVEALS_BASE.ID);
                break;
            case "LastPage":
                TransitionTo(GP6_REVEALS_BASE.ID);
                break;
            default:
				Debug.Log("ERROR: Invalid state provided");
				break;
		}
	}


    public void TransitionTo(string id)
    {
        try
        {
        isAnimating = false;
        t = 0;

        Transition transition = new Transition(currentFragId, id);

            // Determine transition type

        int transType = GetTransition(currentFragId, id);
        transition.transType = transType;

        // Determine positions based on type
        transition = InterpretTransition(transType, transition);

        newFrag = fragmentLookup[id];
        currentFragId = transition.fragGoingTo;

        main.CloseCurrentDialog();

        // Instantiate above fragment
        newFrag = Instantiate(newFrag, transition.pos_in_1.position, Quaternion.identity, mainPanel.transform);

        //currentFrag = mainPanel.transform.GetChild(mainPanel.transform.childCount - 1).gameObject;

        Tween(currentFrag.transform,
            transition.pos_out_1.transform, transition.pos_out_2.transform,
            transition.pos_out_1.localPosition, transition.pos_out_2.transform.localPosition,
            0.5f, 0, transition.interp, true, true, null);

        Tween(newFrag.transform,
            transition.pos_in_1.transform, transition.pos_in_2.transform,
            transition.pos_in_1.localPosition, transition.pos_in_2.transform.localPosition,
            0.5f, 0, transition.interp, true, true, KillFragments);


            currentFrag = newFrag;
            currentFragId = newFrag.GetComponent<Fragment>().GetID();

            //transitionQ.Remove(transition);

        }catch(Exception e)
        {
            print(e.Message);
        }

    }

    private int GetTransition(string fragComingFrom, string fragGoingTo)
    {
        Debug.Log("GETTING TRANSITION: " + fragComingFrom + " to " + fragGoingTo);

        switch(fragComingFrom)
        {
            case LOADING.ID:
                return TransitionManager.TRANSITION_APPEAR;

            case TUTORIAL.ID:
                switch (fragGoingTo)
                {
                    case MAIN_MENU.ID:
                        return TransitionManager.TRANSITION_SLIDE_R2L;
                    default:
                        return TransitionManager.TRANSITION_SLIDE_R2L;
                }
            case MAIN_MENU.ID:
                switch (fragGoingTo)
                {
                    case TUTORIAL.ID:
                        return TransitionManager.TRANSITION_SLIDE_L2R;
                    default:
                        return TransitionManager.TRANSITION_SLIDE_R2L;
                }
            case WAITING_ROOM.ID:
                switch (fragGoingTo)
                {
                    case MAIN_MENU.ID:
                        return TransitionManager.TRANSITION_SLIDE_L2R;
                    default:
                        return TransitionManager.TRANSITION_SLIDE_R2L;
                }
            case GP2_TEXT_ENTRY.ID:
                switch (fragGoingTo)
                {
                    case WAITING_ROOM.ID:
                        return TransitionManager.TRANSITION_SLIDE_L2R;
                    default:
                        return TransitionManager.TRANSITION_SLIDE_R2L;
                }
            case GP3_CHOOSE_WHOSE_TURN.ID:
                switch (fragGoingTo)
                {
                    case GP4_PLAY.ID:
                        return TransitionManager.TRANSITION_POP;
                    default:
                        return TransitionManager.TRANSITION_SLIDE_R2L;
                }
            case GP4_PLAY.ID:
                switch (fragGoingTo)
                {
                    case GP5_REVEALS_INTRO.ID:
                        return TransitionManager.TRANSITION_POP;
                    default:
                        return TransitionManager.TRANSITION_SLIDE_R2L;
                }
            case GP6_REVEALS_BASE.ID:
                switch (fragGoingTo)
                {
                    case WAITING_ROOM.ID:
                        return TransitionManager.TRANSITION_SLIDE_L2R;
                    default:
                        return TransitionManager.TRANSITION_SLIDE_R2L;
                }
            default: // DEFAULT FOR ALL CASES
                return TransitionManager.TRANSITION_SLIDE_R2L;
        }
    }

    Transition InterpretTransition(int transType, Transition t)
	{
        switch(transType)
        {
            case TransitionManager.TRANSITION_SLIDE_L2R:
                t.pos_in_1 = pos["LEFT"];
                t.pos_in_2 = pos["CENTER"];
                t.pos_out_1 = pos["CENTER"];
                t.pos_out_2 = pos["RIGHT"];

                t.interp = new OvershootInterpolator(2);
                t.interp.factor = interpFactor;


                break;
            case TransitionManager.TRANSITION_SLIDE_R2L:
                t.pos_in_1 = pos["RIGHT"];
                t.pos_in_2 = pos["CENTER"];
                t.pos_out_1 = pos["CENTER"];
                t.pos_out_2 = pos["LEFT"];

                t.interp = new OvershootInterpolator(2);
                t.interp.factor = interpFactor;


                break;
            case TransitionManager.TRANSITION_SLIDE_U2D:
                t.pos_in_1 = pos["UP"];
                t.pos_in_2 = pos["CENTER"];
                t.pos_out_1 = pos["CENTER"];
                t.pos_out_2 = pos["DOWN"];

                t.interp = new OvershootInterpolator(2);
                t.interp.factor = interpFactor;


                break;
            case TransitionManager.TRANSITION_SLIDE_D2U:
                t.pos_in_1 = pos["DOWN"];
                t.pos_in_2 = pos["CENTER"];
                t.pos_out_1 = pos["CENTER"];
                t.pos_out_2 = pos["UP"];

                t.interp = new OvershootInterpolator(2);
                t.interp.factor = interpFactor;


                break;
            case TransitionManager.TRANSITION_APPEAR:
                t.pos_in_1 = pos["CENTER"];
                t.pos_in_2 = pos["CENTER"];
                t.pos_out_1 = pos["CENTER"];
                t.pos_out_2 = pos["CENTER"];

                t.interp = new OvershootInterpolator(2);
                t.interp.factor = interpFactor;


                break;
            case TransitionManager.TRANSITION_POP:
                t.pos_in_1 = pos["POP_ENTRY"];
                t.pos_in_2 = pos["POP_FINAL"];
                t.pos_out_1 = pos["CENTER"];
                t.pos_out_2 = pos["DOWN"];

                t.interp = new OvershootInterpolator(2);
                t.interp.factor = interpFactor;

                break;
            default:
                print("ERROR: Invalid transition supplied");
                break;
        }

        return t;

	}

    void KillFragments()
    {
        Debug.Log("COMPLETE");

        if (mainPanel.transform.childCount > 1)
        {
            int startKilling = 0;
            while (startKilling < mainPanel.transform.childCount - 1)
            {
                Destroy(mainPanel.transform.GetChild(startKilling).gameObject);
                startKilling++;
            }

        };

        // newFrag = mainPanel.transform.GetChild(0).gameObject;
    }

    //private List<Transition> transitionQ = new List<Transition>(); // The queue for transitions


        void Tween(Component cmp, Transform entryTrans, Transform finalTrans, Vector3 startPos, Vector3 finalPos, float duration,
    float delay, Interpolator interp, bool ActivenessOnStart, bool ActivenessOnEnd,
    Action endAction)
        {
            try
            {
                Vector3 endPos = finalPos;

                Vector2 startSize = entryTrans.GetComponent<RectTransform>().sizeDelta;
                Vector2 endSize = finalTrans.GetComponent<RectTransform>().sizeDelta;

                LeanTween.value(0, 1, duration).setDelay(delay)
                    .setOnUpdate((float t) =>
                    {
                      try{  Vector3 lerpedPos = interp.getVec3Interpolation(startPos, endPos, t);
                        Vector3 lerpedSize = interp.getVec3Interpolation(startSize, endSize, t);

                        // Debug.Log(lerpedPos.x + " "+ lerpedPos.y + " " + lerpedPos.z);

                        if (cmp != null) cmp.gameObject.transform.localPosition = lerpedPos;
                        if (cmp != null) cmp.gameObject.GetComponent<RectTransform>().sizeDelta = lerpedSize;  } catch { }
                    })
                    .setOnStart(() =>
                    {
                      try{  if (cmp != null) cmp.gameObject.SetActive(ActivenessOnStart); } catch { }
                    })
                    .setOnComplete(() =>
                    {
                     try{   if (cmp != null) cmp.gameObject.SetActive(ActivenessOnEnd);
                        if (endAction != null) endAction();
                        }
                        catch { }
                    });
            }
            catch(Exception e) { Debug.Log(e.Message + ", " + e.Source); }

        }

    
}