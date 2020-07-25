using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class GP1_DELEGATE_ROLES : Fragment
{
    public const string ID = "GP1";
    public override string GetID() { return ID; }

    // DEBUG ONLY
    bool debugVars = false;


    public GameObject Main_Panel;

    public GameObject Pos_Main;
    public GameObject Pos_Top;
    public PositionsHolder ph = new PositionsHolder();

    public Button Button_Skip;

    // Data vars
    List<Player> playersList;
    bool myTruth;
    string myTarget;
    string myName;

    Interpolator interp = new OvershootInterpolator(3f);

    public override void Awake()
    {
        base.Awake();

        ph.AddPositions(Pos_Main);

        main.PlayMusic(null, 0);

        SetListeners();
    }

    void SetListeners()
    {
        Button_Skip.onClick.AddListener(() =>
        {
            GoToGp2();
        });
    }

    public override void Initialise()
    {
        TryStartAnim();
    }

    void OnPlayersChangedEvent()
    {
        TryStartAnim();
    }

    void OnPhaseChangedEvent(string from, string to)
    {
        if (to == "Lobby") GoToLobby();
    }

    private void GoToLobby()
    {
        main.rc.ResetVars(this);
        main.RequestFragmentDirect(WAITING_ROOM.ID);
    }

    public override void RegisterEventListeners()
    {
        main.data.playersChangeEvent.AddListener(OnPlayersChangedEvent);
        main.data.phaseChangeEvent.AddListener(OnPhaseChangedEvent);
    }

    public override void UnregisterEventListeners()
    {
        main.data.playersChangeEvent.RemoveListener(OnPlayersChangedEvent);
        main.data.phaseChangeEvent.RemoveListener(OnPhaseChangedEvent);
    }


    private bool CheckSufficientInformation(Dictionary<string, Player> playersLookup, string playerName)
    {
        return (playersLookup.ContainsKey(playerName)
            && playersLookup[playerName].target != null
            && playersLookup[playerName].isTruth != null);
    }



    List<Card> cards = new List<Card>();
    List<Card> lieCards = new List<Card>();
    List<Card> trueCards = new List<Card>();

    List<GameObject> cardViews = new List<GameObject>();
    Sprite bg_lieCard;
    Sprite bg_trueCard;
    Sprite bg_cardBack;
    GameObject roleCard;

    Dictionary<Card, Vector3> cardsToInitialPos = new Dictionary<Card, Vector3>();

    bool AnimStarted = false;
    bool Transitioning = false;


    void TryStartAnim()
    {
        if (!AnimStarted && CheckSufficientInformation(main.data.playersLookup, main.playerName))
        {
            this.playersList = main.data.playersList;
            this.myTarget = main.data.playersLookup[main.playerName].target;
            this.myTruth = (bool)main.data.playersLookup[main.playerName].isTruth;
            this.myName = main.playerName;

            StartAnim();
        }
    }

    // FIRST ANIM (Cards fly in to initial position)
    void StartAnim()
    {
        AnimStarted = true;

        try
        {
            bg_lieCard = Resources.Load<Sprite>("GUI/Drawables/New Drawables/Inkscape/role_card_lie");
            bg_trueCard = Resources.Load<Sprite>("GUI/Drawables/New Drawables/Inkscape/role_card_true");
            bg_cardBack = Resources.Load<Sprite>("GUI/Drawables/New Drawables/Inkscape/role_card_back");
            roleCard = Resources.Load<GameObject>("Prefabs/Misc/View_RoleCard");

            float delayLie = 0;
            float delayTrue = 0.5f;

            // LIE CARDS
            int j = 0;
            for(int i = 0; i < playersList.Count; i++)
            {
                Player p = playersList[i];


                if (p.playerName != myName)
                {
                    j++;
                    float t = ((float) j) / (playersList.Count - 1);

                //Debug.Log("lieCards t:" + t);

                    Card card = new Card(p.playerName, false);
                    lieCards.Add(card);

                    Vector3 finalPos = Vector3.Lerp(ph.positions["Upper_Start"].localPosition, ph.positions["Upper_End"].localPosition, t);
                    cardsToInitialPos.Add(card, finalPos);
                }
            }

            // TRUE CARDS
            for (int i = 0; i < (playersList.Count - 1); i++)
            {
                float t = ((float)i) / (playersList.Count - 1);

                //Debug.Log("truCards t:" + t);

                Card card = new Card(myName, true);
                trueCards.Add(card);

                Vector3 finalPos = Vector3.Lerp(ph.positions["Lower_Start"].localPosition, ph.positions["Lower_End"].localPosition, t);
                cardsToInitialPos.Add(card, finalPos);
            }

            //Debug.Log("truCards count :" + trueCards.Count + ", liecard count: " + lieCards.Count);

            // ADD ALTERNATING
            int lieCount = lieCards.Count;
            int trueCount = trueCards.Count;

            bool b = true;
            int ll = 0;
            int tt = 0;

            for (int i = 0; i < (2 * lieCards.Count); i++)
            {
                if (b)
                {
                    cards.Add(lieCards[ll]);
                    ll++; 
                }
                else
                {
                    cards.Add(trueCards[tt]);
                    tt++;
                }

                b = !b;
            }

            //Debug.Log("Cards count: " + cards.Count);


            // FLY IN ALTERNATING
            float delay2 = 0;
            int k = -1;


            foreach (Card card in cards)
            {
                k++;

                GameObject cardView = Instantiate(roleCard,
                    ph.positions["Outside"].localPosition,
                    //ph.positions["Upper_Start"].localPosition,
                    Quaternion.identity, Main_Panel.transform);


                cardView.SetActive(false);
                cardViews.Add(cardView);

                FormatCard(cardView, card.name, card.truthCard);

                Vector3 pos = cardsToInitialPos[card];

                if(k < cards.Count - 1)
                {
                    StartCoroutine(CardSound(delay2));
                    Tween(cardView.transform, cardView.transform.localPosition, cardsToInitialPos[card],  0.3f, this, delay2, LeanTweenType.notUsed, true, true, null);
;

                }
                else
                {
                    StartCoroutine(CardSound(delay2));
                    Tween(cardView.transform, cardView.transform.localPosition, cardsToInitialPos[card],  0.5f, this, delay2, LeanTweenType.notUsed, true, true, Anim2);
                }


                delay2 += 0.4f;
            }


        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    IEnumerator CardSound(float waitTime)
    {
        //for (int i = 0; i < number; i++)
        //{
            yield return new WaitForSeconds(waitTime);
        if (!Transitioning) main.PlaySfx("card_draw", 0.5f);
        //}
    }

    Dictionary<GameObject, Vector3> cardToSecondPos = new Dictionary<GameObject, Vector3>();

    // SECOND ANIMATION - Cards fly in to single file diagonal
    void Anim2()
    {
        float delay = 0;

        for(int i = 0; i < cardViews.Count; i++)
        {
            //Debug.Log("Anim2: "+i);

            float t = ((float)i) / (cardViews.Count - 1);

            Vector3 interPos = new Vector3(
                1000,
                cardViews[i].transform.localPosition.y,
                cardViews[i].transform.localPosition.z);

            Vector3 finalPos = Vector3.Lerp(ph.positions["Upper_Start"].localPosition, ph.positions["Lower_End"].localPosition, t);

            if(i < cardViews.Count - 1)
            {

                StartCoroutine(CardSound(delay));
                Tween(cardViews[i].transform, cardViews[i].transform.localPosition, interPos, 0.2f, this, delay, LeanTweenType.notUsed, true, true, null);
                Tween(cardViews[i].transform, interPos, finalPos, 0.2f, this, delay + 0.2f, LeanTweenType.notUsed, true, true, null);

                //Tween(cardViews[i].transform, finalPos, 0.4f, this, delay, LeanTweenType.notUsed, true, true, null);

            }
            else
            {
                StartCoroutine(CardSound(delay));
                Tween(cardViews[i].transform, cardViews[i].transform.localPosition, interPos, 0.2f, this, delay, LeanTweenType.notUsed, true, true, null);
                Tween(cardViews[i].transform, interPos, finalPos, 0.2f + 0.2f, this, delay, LeanTweenType.notUsed, true, true, Anim3);
            }

            delay += 0.1f;
        }

    }

    List<GameObject> cardViewsReversed = new List<GameObject>();

    // THIRD ANIMATION - Cards "flip over"
    void Anim3()
    {
        for(int i = (cardViews.Count - 1); i >= 0; i--)
        {
            cardViewsReversed.Add(cardViews[i]);
        }

        float delay = 0;

        int k = -1;
        foreach(GameObject card in cardViewsReversed)
        {

            k++;


            try{
            LeanTween.scale(card, new Vector3(0.1f, 1), 0.25f).setDelay(delay).
                setOnComplete(() => FlipCard(card));
            LeanTween.scale(card, new Vector3(1, 1), 0.25f).setDelay(delay + 0.25f).
                setOnComplete(() =>
                {
                    
                });
            }
            catch { }

            if (k < (cardViewsReversed.Count - 1))
            {
                try { 
                LeanTween.scale(card, new Vector3(1, 1), 0.25f).setDelay(delay + 0.25f);
                }
                catch { }
            }
            else
            {
                try {
                LeanTween.scale(card, new Vector3(1, 1), 0.25f).setDelay(delay + 0.25f).
                setOnComplete(() =>
                {
                    Anim4();
                });
                }
                catch { }
            }

            delay += 0.1f;
        }
    }

    // FOURTH ANIMATION - Collapse cards to center
    void Anim4()
    {
        float delay = 0;
        int k = -1;
        foreach (GameObject card in cardViews)
        {
            k++;

            StartCoroutine(CardSound(delay));

            if (k < (cardViews.Count - 1))
            {
                Tween(card.transform, card.transform.localPosition, ph.positions["Center"].localPosition, 0.25f, this, delay, LeanTweenType.notUsed, true, true, null);
            }
            else
            {
                Tween(card.transform, card.transform.localPosition, ph.positions["Center"].localPosition, 0.25f, this, delay, LeanTweenType.notUsed, true, true, Anim5);
            }

            delay += 0.1f;
        }
    }

    List<GameObject> cardsExceptTop = new List<GameObject>();
    List<GameObject> cardsToShuffle = new List<GameObject>();

    // FIFTH ANIMATION - SHUFFLE
    void Anim5()
    {
        try
        {
            if (!Transitioning) main.PlaySfx("shuffle_role_select", 0.6f);

            // Card views except top card
            int k = 0;
            foreach (GameObject c in cardViewsReversed)
            {
                k++;
                if (k != cardViewsReversed.Count) cardsExceptTop.Add(c);

            }

            // Shuffle deck
            cardsToShuffle.AddRange(cardsExceptTop);

            while (cardsToShuffle.Count < 8)
            {
                cardsToShuffle.AddRange(cardsExceptTop);
            }

            // Shuffle positions
            List<Vector3> shufflePos = new List<Vector3>();
            shufflePos.Add(ph.positions["Upper_Start"].localPosition);
            shufflePos.Add(ph.positions["Upper_End"].localPosition);
            shufflePos.Add(ph.positions["Lower_End"].localPosition);
            shufflePos.Add(ph.positions["Lower_Start"].localPosition);

            System.Random rand = new System.Random();
            int randomStartIndex = (int)(rand.NextDouble() * 4);
            int index = randomStartIndex;

            float delay = 0;

            SigmoidInterpolator interp = new SigmoidInterpolator(5);

            Vector3 posCenter = ph.positions["Center"].localPosition;

            for (int i = 0; i < 8; i++)
            {
                index++;
                index = index % 4;

                Vector3 posOuter = shufflePos[index];
                GameObject card = cardsToShuffle[i];

                if (i != 7)
                {
                    try {
                     LeanTween.value(0, 1, 0.3f).setDelay(delay)
                        .setOnUpdate((float v) =>
                        {
                            if (card != null) card.transform.localPosition = Vector3.Lerp(posCenter, posOuter, v);
                        })
                        .setOnComplete(() =>
                        {
                                LeanTween.value(1, 0, 0.3f).setDelay(0).setOnUpdate((float v) => {
                                    if (card != null) card.transform.localPosition = Vector3.Lerp(posCenter, posOuter, v);
                                });
                        });
                    }
                    catch { }

                }
                else
                {
                    try { 
                    LeanTween.value(0, 1, 0.3f).setDelay(delay)
                        .setOnUpdate((float v) =>
                        {
                            if (card != null) card.transform.localPosition = Vector3.Lerp(posCenter, posOuter, v);
                        })
                        .setOnComplete(() =>
                        {
                            try { 
                            LeanTween.value(1, 0, 0.3f).setDelay(0).setOnUpdate((float v) => {
                                if (card != null) card.transform.localPosition = Vector3.Lerp(posCenter, posOuter, v);
                            })
                            .setOnComplete(() => Anim6());
                            }
                            catch { }
                        });
                    }
                    catch { }

                }
                       

                delay += 0.2f;

            }
        }catch(Exception e)
        {
            Debug.Log(e.Message);
        }

    }

    List<Vector3> explosionPos = new List<Vector3>();

    // SIXTH ANIMATION - Cards explode, reveal final card
    void Anim6()
    {
        Debug.Log("Anim6 CALLED");

        if(!Transitioning) main.PlaySfx("player_selected", 0.8f);

        Vector3 posCenter = ph.positions["Center"].localPosition;

        float explosionDur = 1;

        explosionPos.Add(ph.positions["Upper_Start"].localPosition);
        explosionPos.Add(Vector3.Lerp(ph.positions["Upper_Start"].localPosition, ph.positions["Upper_End"].localPosition, 0.5f));
        explosionPos.Add(ph.positions["Upper_End"].localPosition);
        explosionPos.Add(Vector3.Lerp(ph.positions["Upper_End"].localPosition, ph.positions["Lower_End"].localPosition, 0.5f));
        explosionPos.Add(ph.positions["Lower_End"].localPosition);
        explosionPos.Add(Vector3.Lerp(ph.positions["Lower_End"].localPosition, ph.positions["Lower_Start"].localPosition, 0.5f));
        explosionPos.Add(ph.positions["Lower_Start"].localPosition);
        explosionPos.Add(Vector3.Lerp(ph.positions["Lower_Start"].localPosition, ph.positions["Upper_Start"].localPosition, 0.5f));

        for(int i = 0; i < cardViews.Count; i++)
        {

            GameObject card = cardViews[i];

            if (i >= 8)
            {
                card.gameObject.SetActive(false);
            }
            else
            {

                Vector3 pos = explosionPos[(i*3) % 8];

                try { 
                LeanTween.value(0, 1, explosionDur).setOnUpdate((float v) =>
                {
                    if (card != null) card.transform.localPosition = Vector3.Lerp(posCenter, pos, v);
                    if (card != null) card.transform.localScale = Vector2.Lerp(new Vector2(1, 1), new Vector2(0.5f, 0.5f), v);
                    if (card != null) SetCardAlpha(card, 1 - v);
                });

                }
                catch { }
            }
        }


        GameObject myCard = Instantiate(roleCard, Main_Panel.transform);

        myCard.gameObject.SetActive(false);

        myCard.transform.localPosition = ph.positions["Center"].localPosition;
        myCard.transform.localScale = new Vector3(0.5f, 0.5f);

        FormatCard(myCard, myTarget, myTruth);

        // Pop card in
        try
        {
        LeanTween.scale(myCard, new Vector3(1,1), 0.3f)
            .setOnStart(() => { if (myCard != null) myCard.gameObject.SetActive(true); });

        }
        catch { }

        Interpolator antiInterp = new AnticipateInterpolator(1);

        // Card exit right
        LeanTween.value(0, 1, 0.5f)
            .setDelay(1.5f)
            .setOnUpdate((float v) =>
            {
                try { 
                if(v >= 0.5f)
                {
                    GoToGp2();
                }
                if(myCard != null) myCard.transform.localPosition = antiInterp.getVec3Interpolation(
                    ph.positions["Center"].localPosition, ph.positions["Outside"].localPosition
                    , v);
                }
                catch { }
            });
        try { 
        LeanTween.rotateAroundLocal(myCard, Vector3.back, 540, 0.5f).setDelay(1.5f);
        }
        catch { }

    }


    void GoToGp2()
    {
        if(!Transitioning)
        {
            main.RequestFragmentDirect(GP2_TEXT_ENTRY.ID);
            Transitioning = true;
        }
    }

    private void SetCardAlpha(GameObject card, float v)
    {
        Image Image_bg = card.transform.Find("bg").GetComponent<Image>();
        Text Text_Name = card.transform.Find("Text_Name").GetComponent<Text>();
        Text Text_Top = card.transform.Find("Text_TrueOrBull_Top").GetComponent<Text>();
        Text Text_Bottom = card.transform.Find("Text_TrueOrBull_Bottom").GetComponent<Text>();

        Color c = Image_bg.color;
        c.a = v;
        Image_bg.color = c;

        c = Text_Name.color;
        c.a = v;
        Text_Name.color = c;

        c = Text_Top.color;
        c.a = v;
        Text_Top.color = c;

        c = Text_Bottom.color;
        c.a = v;
        Text_Bottom.color = c;
    }

    private void FlipCard(GameObject card)
    {
        Image Image_bg = card.transform.Find("bg").GetComponent<Image>();
        Text Text_Name = card.transform.Find("Text_Name").GetComponent<Text>();
        Text Text_Top = card.transform.Find("Text_TrueOrBull_Top").GetComponent<Text>();
        Text Text_Bottom = card.transform.Find("Text_TrueOrBull_Bottom").GetComponent<Text>();

        Image_bg.sprite = bg_cardBack;

        Text_Name.text = "";
        Text_Top.text = "";
        Text_Bottom.text = "";


    }

    private void FormatCard(GameObject card, string playerName, bool isTruth)
    {
        try
        {
            Image Image_bg = card.transform.Find("bg").GetComponent<Image>();
            Text Text_Name = card.transform.Find("Text_Name").GetComponent<Text>();
            Text Text_Top = card.transform.Find("Text_TrueOrBull_Top").GetComponent<Text>();
            Text Text_Bottom = card.transform.Find("Text_TrueOrBull_Bottom").GetComponent<Text>();
            
            if (isTruth)
            {
                Image_bg.sprite = bg_trueCard;
            }
            else
            {
                
                Image_bg.sprite = bg_lieCard;
            }

            
            Text_Name.text = playerName;
            
            Text_Top.text = isTruth ? "TRUE" : "BULL";
            
            Text_Bottom.text = isTruth ? "TRUE" : "BULL";
            

        }
        catch(Exception e)
        {
            Debug.Log(e.Message + ", " + e.StackTrace);
        }
    }



    public override void RoomChangeResult(string code, Task task, bool succes, object args)
    {
        
    }


    // UTILS

    void SetTransformPos(GameObject go, string posName, GP1_DELEGATE_ROLES gp1)
    {
        go.GetComponent<RectTransform>().localPosition = gp1.ph.positions[posName].GetComponent<RectTransform>().localPosition;
        go.GetComponent<RectTransform>().anchorMin = gp1.ph.positions[posName].GetComponent<RectTransform>().anchorMin;
        go.GetComponent<RectTransform>().anchorMax = gp1.ph.positions[posName].GetComponent<RectTransform>().anchorMax;
    }

    void Tween(Component cmp, Vector3 startPos, Vector3 finalPos, float duration, GP1_DELEGATE_ROLES gp1,
        float delay, LeanTweenType easeType, bool ActivenessOnStart, bool ActivenessOnEnd,
        Action endAction)
    {
        try
        {
            Vector3 endPos = finalPos;

            Vector2 startSize = cmp.gameObject.GetComponent<RectTransform>().sizeDelta;
            Vector2 endSize = cmp.gameObject.GetComponent<RectTransform>().sizeDelta;

            LeanTween.value(0, 1, duration).setDelay(delay)
                .setOnUpdate((float t) =>
                {
                    Vector3 lerpedPos = interp.getVec3Interpolation(startPos, endPos, t);
                    Vector3 lerpedSize = interp.getVec3Interpolation(startSize, endSize, t);

                    if (cmp != null) cmp.gameObject.transform.localPosition = lerpedPos;
                    if (cmp != null) cmp.gameObject.GetComponent<RectTransform>().sizeDelta = lerpedSize;
                })
                .setOnStart(() =>
                {
                    if (cmp != null) cmp.gameObject.SetActive(ActivenessOnStart);
                })
                .setOnComplete(() =>
                 {
                     if (cmp != null) cmp.gameObject.SetActive(ActivenessOnEnd);
                     if (endAction != null) endAction();
                 });
        }
        catch { }
    }

    
}
class Card
{
    public string name;
    public bool truthCard;

    public Card(string name, bool truthCard)
    {
        this.name = name;
        this.truthCard = truthCard;
    }
}
