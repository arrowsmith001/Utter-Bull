using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using DanielLochner.Assets.SimpleScrollSnap;
using UnityEngine.Video;

public class TUTORIAL : Fragment
{
    public const string ID = "TUT";
    public override string GetID() { return ID; }


    // PREFABS

    public GameObject layout1;
    public GameObject layout2;
    public GameObject layout3;
    public GameObject layout4;
    public GameObject layout5;
    public GameObject layout6;
    public GameObject layout7;
    public GameObject layout8;
    public GameObject layout9;
    public GameObject layout10;

    public GameObject Text_True;
    public GameObject Text_Bull;
    public GameObject Image_Sabo;


    public GameObject Pos;

    public SimpleScrollSnap scrollSnap;

    public List<GameObject> phones;


    public Button button_skip_tutorial;
    public Button button_next_tutorial;
    public Button Button_GetStarted;

    // Start is called before the first frame update
    void Start()
    {
        GameObject root = this.transform.root.gameObject;
        mainCanvas = root.transform.Find("MAIN_CANVAS").gameObject;
        main = mainCanvas.GetComponent<MainScript>();

        main.PlayMusic("utter_bull_main_theme", MainScript.MUSIC_VOLUME_LOW);

        beginningCase = bool.Parse(main.GetPrefString("firstTime", "True"));
        if (beginningCase)
        {
            button_skip_tutorial.transform.GetChild(0).transform.GetComponent<Text>().text = "SKIP TUTORIAL";
        }
        else
        {
            button_skip_tutorial.transform.GetChild(0).transform.GetComponent<Text>().text = "END TUTORIAL";
        }

        RemoveBlock(beginningCase);

        layouts.Add(layout1);
        layouts.Add(layout2);
        layouts.Add(layout3);
        layouts.Add(layout4);
        layouts.Add(layout5);
        layouts.Add(layout6);
        layouts.Add(layout7);
        layouts.Add(layout8);
        layouts.Add(layout9);
        layouts.Add(layout10);

        ph.AddPositions(Pos);

        SetListeners();


    }

    public GameObject block;

    private void RemoveBlock(bool animate)
    {
        if(animate)
        {
            try
            {
                LeanTween.value(1, 0, 0.5f)
                    .setOnUpdate((float t) =>
                    {
                        try
                        {
                        Color c = block.GetComponent<Image>().color;
                        c.a = t;
                        block.GetComponent<Image>().color = c;
                        }
                        catch { }
                    })
                    .setOnComplete(() =>
                    {
                        try
                        {
                            block.SetActive(false);
                        }
                        catch { }
                    });
            }
            catch { }
        }
        else
        {
            block.SetActive(false);
        }
    }

    private List<GameObject> layouts = new List<GameObject>();
    private PositionsHolder ph = new PositionsHolder();

    private int currentPageIndex = -1;
    private bool beginningCase;

    public void OnPageSelected()
    {
        int pNum = scrollSnap.TargetPanel;

        if(pNum != currentPageIndex)
        {
            // Kill kids of previous
            if(currentPageIndex != -1)
            {
                if(currentPageIndex == 1
                    || currentPageIndex == 2
                    || currentPageIndex == 3
                    || currentPageIndex == 4
                    || currentPageIndex == 5
                    || currentPageIndex == 6)
                {
                    KillChildren(layouts[currentPageIndex]);
                }
            }

            if(pNum == 9 && beginningCase)
            {
                StartCoroutine(ShowGetStarted());
            }


            // Set current page to new page
            currentPageIndex = pNum;
        }
        else
        {
            return;
        }

        GameObject layout = layouts[pNum];

        switch(pNum)
        {
            case 0: // 1


                break;
            case 1: // 2

                StartCoroutine(P2setup(layout));

                break;
            case 2: // 3

                StartCoroutine(P3setup(layout));

                break;
            case 3: // 4

                StartCoroutine(P4setup(layout));

                break;
            case 4: // 5

                StartCoroutine(P5setup(layout));

                break;
            case 5: // 6

                StartCoroutine(P6setup(layout));

                break;
            case 6: // 7

                StartCoroutine(P7setup(layout));

                break;
            case 7: // 8


                break;
            case 8: // 9


                break;
            case 9: // 10


                break;






        }

    }

    IEnumerator ShowGetStarted()
    {
        yield return new WaitForSeconds(2f);
        Button_GetStarted.gameObject.SetActive(true);

        try
        {
            LeanTween.scale(Button_GetStarted.gameObject, new Vector3(1, 1, 1), 0.3f).setFrom(new Vector3(0, 0, 0)).setEase(LeanTweenType.easeSpring);

            LeanTween.value(0, 1, 0.3f)
                .setOnUpdate((float t) =>
                {
                    try {

                        float skipX = button_skip_tutorial.transform.localPosition.x;
                        skipX -= 10;
                        button_skip_tutorial.transform.localPosition = new Vector3(skipX, button_skip_tutorial.transform.localPosition.y);

                        float nextX = button_next_tutorial.transform.localPosition.x;
                        nextX += 10;
                        button_next_tutorial.transform.localPosition = new Vector3(nextX, button_next_tutorial.transform.localPosition.y);

                        Color c1 = button_skip_tutorial.transform.GetComponent<Image>().color;
                        Color c2 = button_skip_tutorial.transform.GetChild(0).GetComponent<Text>().color;
                        Color c3 = button_next_tutorial.transform.GetComponent<Image>().color;
                        Color c4 = button_next_tutorial.transform.GetChild(0).GetComponent<Text>().color;

                        c1.a = 1 - t;
                        c2.a = 1 - t;
                        c3.a = 1 - t;
                        c4.a = 1 - t;

                        button_skip_tutorial.transform.GetComponent<Image>().color = c1;
                        button_skip_tutorial.transform.GetChild(0).GetComponent<Text>().color = c2;
                        button_next_tutorial.transform.GetComponent<Image>().color = c3;
                        button_next_tutorial.transform.GetChild(0).GetComponent<Text>().color = c4;


                    }
                    catch
                    {

                    }



                });


        }
        catch { }
    }

    private void KillChildren(GameObject layout)
    {
        for(int i = 0; i < layout.transform.childCount; i++)
        {
            if(layout.transform.GetChild(i).name != "POSITIONS")
            {
                Destroy(layout.transform.GetChild(i).gameObject);
            }
        }
    }

    #region 2

    IEnumerator P2setup(GameObject layout)
    {
        // INITIATE POSITION //

        GameObject phone1 = Instantiate(phones[0], layout.transform);
        GameObject phone2 = Instantiate(phones[1], layout.transform);
        GameObject phone3 = Instantiate(phones[2], layout.transform);

        Image shade1 = phone1.transform.Find("Shade").GetComponent<Image>();
        Image shade2 = phone2.transform.Find("Shade").GetComponent<Image>();
        Image shade3 = phone3.transform.Find("Shade").GetComponent<Image>();
        
        phone1.transform.GetComponent<RectTransform>().localPosition = ph.positions["3Left"].GetComponent<RectTransform>().localPosition;
        phone2.transform.GetComponent<RectTransform>().localPosition = ph.positions["Middle"].GetComponent<RectTransform>().localPosition;
        phone3.transform.GetComponent<RectTransform>().localPosition = ph.positions["3Right"].GetComponent<RectTransform>().localPosition;

        phone1.transform.GetComponent<RectTransform>().localScale = new Vector3(0.5f, 0.5f);
        phone2.transform.GetComponent<RectTransform>().localScale = new Vector3(0.5f, 0.5f);
        phone3.transform.GetComponent<RectTransform>().localScale = new Vector3(0.5f, 0.5f);

        // VIDEO //

        VideoPlayer vp1 = phone1.transform.Find("Video Player").GetComponent<VideoPlayer>();
        VideoPlayer vp2 = phone2.transform.Find("Video Player").GetComponent<VideoPlayer>();
        VideoPlayer vp3 = phone3.transform.Find("Video Player").GetComponent<VideoPlayer>();

        vp1.Prepare();
        vp2.Prepare();
        vp3.Prepare();

        while (!vp1.isPrepared || !vp2.isPrepared || !vp3.isPrepared)
        {
            Debug.Log("Preparing Video P2");
            yield return null;
        }

        vp1.targetTexture.Release();
        vp2.targetTexture.Release();
        vp3.targetTexture.Release();

        StartCoroutine(P2anim(phone1, phone2, phone3, vp1, vp2, vp3, shade1, shade2, shade3));
    }

    IEnumerator P2anim(GameObject phone1, GameObject phone2, GameObject phone3,
        VideoPlayer vp1, VideoPlayer vp2, VideoPlayer vp3,
        Image shade1, Image shade2, Image shade3)
{
        double time1 = TimeConv(3, 35, 50);
        double time2 = TimeConv(3, 7, 50);
        double time3 = TimeConv(4, 4, 50);
        try
        {

            vp1.time = time1;
            vp2.time = time2;
            vp3.time = time3;

            // Scale phone A in
            LeanTween.scale(phone1, new Vector3(1, 1, 1), 0.25f).setEase(LeanTweenType.easeSpring)
                .setOnStart(() =>
                {
                try{    if (vp1 != null) vp1.Play();
                    if (shade1 != null) shade1.enabled = false; } catch { }
                });

        }
        catch (Exception e) { Debug.Log("ERROR IN P2Anim: " + e.Message); }

        yield return new WaitForSeconds(3f);

        try
        { 
            // Scale phone A out...
            LeanTween.scale(phone1, new Vector3(0.5f, 0.5f, 0.5f), 0.25f).setEase(LeanTweenType.easeSpring)
                .setOnComplete(() =>
                {

                    if(vp1 != null) vp1.Pause();
                    if (vp1 != null) vp1.targetTexture.Release();
                    if (shade1 != null) shade1.enabled = true;

                // Scale phone B in
                LeanTween.scale(phone2, new Vector3(1, 1, 1), 0.25f).setEase(LeanTweenType.easeSpring)
                        .setOnStart(() =>
                        {
                            if(vp2 != null) vp2.Play();
                            if(shade2 != null) shade2.enabled = false;
                        });

                });

        }
        catch (Exception e) { Debug.Log("ERROR IN P2Anim: " + e.Message); }

        yield return new WaitForSeconds(3);

        try { 
            // Scale phone A out...
            LeanTween.scale(phone2, new Vector3(0.5f, 0.5f, 0.5f), 0.25f).setEase(LeanTweenType.easeSpring)
                .setOnComplete(() =>
                {
                    if(vp2 != null) vp2.Pause();
                    if (vp2 != null) vp2.targetTexture.Release();
                    if (shade2 != null) shade2.enabled = true;

                // Scale phone B in
                LeanTween.scale(phone3, new Vector3(1, 1, 1), 0.25f).setEase(LeanTweenType.easeSpring)
                        .setOnStart(() =>
                        {
                            if(vp3 != null) vp3.Play();
                            if(shade3 != null) shade3.enabled = false;
                        });

                });

        }
        catch (Exception e) { Debug.Log("ERROR IN P2Anim: " + e.Message); }

        yield return new WaitForSeconds(3);

        try { 
            // Scale phone A out...
            LeanTween.scale(phone3, new Vector3(0.5f, 0.5f, 0.5f), 0.25f).setEase(LeanTweenType.easeSpring)
                .setOnComplete(() =>
                {
                    if(vp3 != null) vp3.Pause();
                    if (vp3 != null) vp3.targetTexture.Release();
                    if (shade3 != null) shade3.enabled = true;

                //// Scale phone B in
                //LeanTween.scale(phone3, new Vector3(1, 1, 1), 0.25f)
                //    .setOnStart(() => shade3.enabled = false);

            });

        }
        catch (Exception e) { Debug.Log("ERROR IN P2Anim: " + e.Message); }

        yield return new WaitForSeconds(0.25f);

        try { 
            if(currentPageIndex == 1) StartCoroutine(P2anim(phone1, phone2, phone3, vp1, vp2, vp3, shade1, shade2, shade3));

        }
        catch (Exception e) { Debug.Log("ERROR IN P2Anim: " + e.Message); }

    }
    #endregion
    #region 3
    IEnumerator P3setup(GameObject layout)
    {
        // INITIATE POSITION //

        GameObject phone1 = Instantiate(phones[0], layout.transform);
        GameObject phone2 = Instantiate(phones[1], layout.transform);
        GameObject phone3 = Instantiate(phones[2], layout.transform);
        GameObject phone4 = Instantiate(phones[3], layout.transform);

        Image shade1 = phone1.transform.Find("Shade").GetComponent<Image>();
        Image shade2 = phone2.transform.Find("Shade").GetComponent<Image>();
        Image shade3 = phone3.transform.Find("Shade").GetComponent<Image>();
        Image shade4 = phone4.transform.Find("Shade").GetComponent<Image>();

        phone1.transform.GetComponent<RectTransform>().localPosition = ph.positions["3Left"].GetComponent<RectTransform>().localPosition;
        phone2.transform.GetComponent<RectTransform>().localPosition = ph.positions["Middle"].GetComponent<RectTransform>().localPosition;
        phone3.transform.GetComponent<RectTransform>().localPosition = ph.positions["3Right"].GetComponent<RectTransform>().localPosition;
        phone4.transform.GetComponent<RectTransform>().localPosition = ph.positions["Middle"].GetComponent<RectTransform>().localPosition;

        phone1.transform.GetComponent<RectTransform>().localScale = new Vector3(0.0f, 0.0f);
        phone2.transform.GetComponent<RectTransform>().localScale = new Vector3(0.0f, 0.0f);
        phone3.transform.GetComponent<RectTransform>().localScale = new Vector3(0.0f, 0.0f);
        phone4.transform.GetComponent<RectTransform>().localScale = new Vector3(0.0f, 0.0f);

        shade1.enabled = false;
        shade2.enabled = false;
        shade3.enabled = false;
        shade4.enabled = false;

        // VIDEO //

        VideoPlayer vp1 = phone1.transform.Find("Video Player").GetComponent<VideoPlayer>();
        VideoPlayer vp2 = phone2.transform.Find("Video Player").GetComponent<VideoPlayer>();
        VideoPlayer vp3 = phone3.transform.Find("Video Player").GetComponent<VideoPlayer>();
        VideoPlayer vp4 = phone4.transform.Find("Video Player").GetComponent<VideoPlayer>();

        vp1.Prepare();
        vp2.Prepare();
        vp3.Prepare();
        vp4.Prepare();

        while (!vp1.isPrepared || !vp2.isPrepared || !vp3.isPrepared || !vp4.isPrepared)
        {
            Debug.Log("Preparing Video P3");
            yield return null;
        }

        phone1.SetActive(false);
        phone2.SetActive(false);
        phone3.SetActive(false);
        phone4.SetActive(false);

        vp1.targetTexture.Release();
        vp2.targetTexture.Release();
        vp3.targetTexture.Release();
        vp4.targetTexture.Release();

        StartCoroutine(P3anim(phone1, phone2, phone3, phone4, vp1, vp2, vp3, vp4, shade1, shade2, shade3, shade4));
    }

    IEnumerator P3anim(GameObject phone1, GameObject phone2, GameObject phone3, GameObject phone4,
        VideoPlayer vp1, VideoPlayer vp2, VideoPlayer vp3, VideoPlayer vp4,
        Image shade1, Image shade2, Image shade3, Image shade4)
    {
        double time1 = TimeConv(2, 43, 0);
        double time2 = TimeConv(2, 43, 0);
        double time3 = TimeConv(2, 43, 0);

        double time4 = TimeConv(2, 37, 50);

        try {


            vp4.time = time4;

            vp1.time = time1;
            vp2.time = time2;
            vp3.time = time3;


            phone4.SetActive(true);

        // Scale phone D in
        LeanTween.scale(phone4, new Vector3(1, 1, 1), 0.25f).setEase(LeanTweenType.easeSpring)
            .setOnStart(() =>
            {
                 if(vp4 != null) vp4.Play();
            });


        }
        catch (Exception e) { Debug.Log("ERROR IN P3Anim: " + e.Message); }

        yield return new WaitForSeconds(4.5f);

        try {

        // Scale phone D out
        LeanTween.scale(phone4, new Vector3(0, 0, 0), 0.25f).setEase(LeanTweenType.easeSpring)
            .setOnStart(() => {
                if (vp4 != null) vp4.Pause();
                if (vp4 != null) vp4.targetTexture.Release();
            })
            .setOnComplete(() =>
            {
                phone4.SetActive(false);

                // Scale phones A, B and C in
                LeanTween.value(0, 0.75f, 0.25f).setEase(LeanTweenType.easeSpring)
                .setOnStart(() =>
                {
                    if(phone1 != null) phone1.SetActive(true);
                    if(phone2 != null) phone2.SetActive(true);
                    if(phone3 != null) phone3.SetActive(true);

                    if(vp1 != null) vp1.Play();
                    if(vp2 != null) vp2.Play();
                    if(vp3 != null) vp3.Play();
                })
                .setOnUpdate((float t) =>
                {
                    if(phone1 != null) phone1.transform.localScale = new Vector3(t, t);
                    if(phone2 != null) phone2.transform.localScale = new Vector3(t, t);
                    if(phone3 != null) phone3.transform.localScale = new Vector3(t, t);
                });

            });
        }
        catch (Exception e) { Debug.Log("ERROR IN P3Anim: " + e.Message); }

        yield return new WaitForSeconds(7);

        try {

        // Scale phones A, B and C out
        LeanTween.value(0.75f, 0f, 0.25f).setEase(LeanTweenType.easeSpring)
        .setOnUpdate((float t) =>
        {
            if(phone1 != null) phone1.transform.localScale = new Vector3(t, t);
            if(phone2 != null) phone2.transform.localScale = new Vector3(t, t);
            if(phone3 != null) phone3.transform.localScale = new Vector3(t, t);
        })
        .setOnComplete(() =>
        {

            if (vp1 != null) vp1.Pause();
            if (vp2 != null) vp2.Pause();
            if (vp3 != null) vp3.Pause();

            if (vp1 != null) vp1.targetTexture.Release();
            if (vp2 != null) vp2.targetTexture.Release();
            if (vp3 != null) vp3.targetTexture.Release();

            if (phone1 != null) phone1.SetActive(false);
            if(phone2 != null) phone2.SetActive(false);
            if(phone3 != null) phone3.SetActive(false);

            if(currentPageIndex == 2) StartCoroutine(P3anim(phone1, phone2, phone3, phone4, vp1, vp2, vp3, vp4, shade1, shade2, shade3, shade4));
        });

        }
        catch (Exception e) { Debug.Log("ERROR IN P3Anim: " + e.Message); }

    }

    #endregion
    #region 4
    IEnumerator P4setup(GameObject layout)
    {
        GameObject phone2 = Instantiate(phones[1], layout.transform);
        GameObject phone4 = Instantiate(phones[3], layout.transform);

        GameObject True1 = Instantiate(Text_True, layout.transform);
        GameObject True2 = Instantiate(Text_True, layout.transform);
        GameObject True3 = Instantiate(Text_True, layout.transform);
        GameObject Bull1 = Instantiate(Text_Bull, layout.transform);
        GameObject Bull2 = Instantiate(Text_Bull, layout.transform);
        GameObject Bull3 = Instantiate(Text_Bull, layout.transform);

        Image shade2 = phone2.transform.Find("Shade").GetComponent<Image>();
        Image shade4 = phone4.transform.Find("Shade").GetComponent<Image>();

        phone2.transform.GetComponent<RectTransform>().localPosition = ph.positions["2Right"].GetComponent<RectTransform>().localPosition;
        phone4.transform.GetComponent<RectTransform>().localPosition = ph.positions["2Right"].GetComponent<RectTransform>().localPosition;

        True1.transform.GetComponent<RectTransform>().localPosition = ph.positions["3TextL1"].GetComponent<RectTransform>().localPosition;
        True2.transform.GetComponent<RectTransform>().localPosition = ph.positions["3TextL2"].GetComponent<RectTransform>().localPosition;
        True3.transform.GetComponent<RectTransform>().localPosition = ph.positions["3TextL3"].GetComponent<RectTransform>().localPosition;
        Bull1.transform.GetComponent<RectTransform>().localPosition = ph.positions["3TextL1"].GetComponent<RectTransform>().localPosition;
        Bull2.transform.GetComponent<RectTransform>().localPosition = ph.positions["3TextL2"].GetComponent<RectTransform>().localPosition;
        Bull3.transform.GetComponent<RectTransform>().localPosition = ph.positions["3TextL3"].GetComponent<RectTransform>().localPosition;

        phone2.transform.GetComponent<RectTransform>().localScale = new Vector3(0.0f, 0.0f);
        phone4.transform.GetComponent<RectTransform>().localScale = new Vector3(0.0f, 0.0f);

        True1.transform.GetComponent<RectTransform>().localScale = new Vector3(0.0f, 0.0f);
        True2.transform.GetComponent<RectTransform>().localScale = new Vector3(0.0f, 0.0f);
        True3.transform.GetComponent<RectTransform>().localScale = new Vector3(0.0f, 0.0f);
        Bull1.transform.GetComponent<RectTransform>().localScale = new Vector3(0.0f, 0.0f);
        Bull2.transform.GetComponent<RectTransform>().localScale = new Vector3(0.0f, 0.0f);
        Bull3.transform.GetComponent<RectTransform>().localScale = new Vector3(0.0f, 0.0f);


        // VIDEO //

        VideoPlayer vp2 = phone2.transform.Find("Video Player").GetComponent<VideoPlayer>();
        VideoPlayer vp4 = phone4.transform.Find("Video Player").GetComponent<VideoPlayer>();

        vp2.Prepare();
        vp4.Prepare();

        while (!vp2.isPrepared || !vp4.isPrepared)
        {
            Debug.Log("Preparing Video P4");
            yield return null;
        }

        phone2.SetActive(false);
        phone4.SetActive(false);
        True1.SetActive(false);
        True2.SetActive(false);
        True3.SetActive(false);
        Bull1.SetActive(false);
        Bull2.SetActive(false);
        Bull3.SetActive(false);

        vp2.targetTexture.Release();
        vp4.targetTexture.Release();

        StartCoroutine(P4anim(phone2, phone4, vp2, vp4, shade2, shade4,
            True1, True2, True3, Bull1, Bull2, Bull3));
    }

    IEnumerator P4anim(GameObject phone2, GameObject phone4, VideoPlayer vp2, VideoPlayer vp4, Image shade2, Image shade4,
        GameObject true1, GameObject true2, GameObject true3, GameObject bull1, GameObject bull2, GameObject bull3)
    {
        try
        {
            vp2.time = TimeConv(4, 53, 0); // 7 secs
            vp4.time = TimeConv(4, 34, 0); // 4 secs

            phone2.SetActive(true);

            // Scale phone B in
            LeanTween.scale(phone2, new Vector3(1, 1, 1), 0.25f).setEase(LeanTweenType.easeSpring)
                .setOnStart(() =>
                {

                })
                .setOnComplete(() =>
                {
                    // True1 SHOW
                    LeanTween.scale(true1, new Vector3(1, 1), 0.25f).setEase(LeanTweenType.easeSpring)
                    .setOnStart(() =>
                    {
                        if(true1 != null) true1.SetActive(true);
                    })
                    .setOnComplete(() =>
                    {
                        // True2 SHOW
                        LeanTween.scale(true2, new Vector3(1, 1), 0.25f).setEase(LeanTweenType.easeSpring)
                        .setOnStart(() =>
                        {
                            if(true2 != null) true2.SetActive(true);
                        })
                        .setOnComplete(() =>
                        {
                            // True3 SHOW
                            LeanTween.scale(true3, new Vector3(1, 1), 0.25f).setEase(LeanTweenType.easeSpring)
                            .setOnStart(() =>
                            {
                                if(true3 != null) true3.SetActive(true);
                            })
                            .setOnComplete(() =>
                            {

                            });
                        });
                    });
                });
        }
        catch (Exception e) { Debug.Log("ERROR IN P4Anim: " + e.Message); }

        yield return new WaitForSeconds(1.5f);
        try { 
        shade2.enabled = false;

        Color c = true1.GetComponent<Text>().color;
        c.a = 0.33f;
        true1.GetComponent<Text>().color = c;
        true2.GetComponent<Text>().color = c;
        true3.GetComponent<Text>().color = c;

        vp2.Play();

    }
        catch (Exception e) { Debug.Log("ERROR IN P4Anim: " + e.Message); }
yield return new WaitForSeconds(3.5f);

        try { 
        // Scale phone B out
        LeanTween.scale(phone2, new Vector3(0, 0, 0), 0.25f).setEase(LeanTweenType.easeSpring)
            .setOnStart(() =>
            {

            })
            .setOnComplete(() =>
            {
                if(shade2 != null) shade2.enabled = true;
                if (vp2 != null) vp2.Pause();
                if(vp2!=null) vp2.targetTexture.Release();
                if (phone2 != null) phone2.SetActive(false);

                    // True1 HIDE
                    LeanTween.scale(true1, new Vector3(0, 0), 0.25f).setEase(LeanTweenType.easeSpring)
                .setOnComplete(() =>
                {
                    if (true1 != null)
                    {
                        Color c1 = true1.GetComponent<Text>().color;
                        c1.a = 1f;
                        true1.GetComponent<Text>().color = c1;

                        true1.SetActive(false);
                    }
                });

                    // True2 HIDE
                    LeanTween.scale(true2, new Vector3(0, 0), 0.25f).setEase(LeanTweenType.easeSpring)
                .setOnComplete(() =>
                {
                    if (true2 != null)
                    {
                        Color c1 = true2.GetComponent<Text>().color;
                        c1.a = 1f;
                        true2.GetComponent<Text>().color = c1;

                        true2.SetActive(false);
                    }
                });

                    // True3 HIDE
                    LeanTween.scale(true3, new Vector3(0, 0), 0.25f).setEase(LeanTweenType.easeSpring)
                .setOnComplete(() =>
                {
                    if (true3 != null)
                    {
                        Color c1 = true3.GetComponent<Text>().color;
                        c1.a = 1f;
                        true3.GetComponent<Text>().color = c1;

                        true3.SetActive(false);
                    }
                });
            });
    }
        catch (Exception e) { Debug.Log("ERROR IN P4Anim: " + e.Message); }

yield return new WaitForSeconds(0.75f);

        try
        {
            phone4.SetActive(true);

            // Scale phone D in
            LeanTween.scale(phone4, new Vector3(1, 1, 1), 0.25f).setEase(LeanTweenType.easeSpring)
                .setOnComplete(() =>
                {
                    // Bull1 SHOW
                    LeanTween.scale(bull1, new Vector3(1, 1), 0.25f).setEase(LeanTweenType.easeSpring)
                    .setOnStart(() =>
                    {
                        if(bull1 != null) bull1.SetActive(true);
                    })
                    .setOnComplete(() =>
                    {
                        // Bull2 SHOW
                        LeanTween.scale(bull2, new Vector3(1, 1), 0.25f).setEase(LeanTweenType.easeSpring)
                        .setOnStart(() =>
                        {
                            if(bull2 != null) bull2.SetActive(true);
                        })
                        .setOnComplete(() =>
                        {
                            // Bull3 SHOW
                            LeanTween.scale(bull3, new Vector3(1, 1), 0.25f).setEase(LeanTweenType.easeSpring)
                            .setOnStart(() =>
                            {
                                if (bull3 != null) bull3.SetActive(true);
                            });
                        });
                    });
                });
        }
        catch (Exception e) { Debug.Log("ERROR IN P4Anim: " + e.Message); }

        yield return new WaitForSeconds(1.5f);
        try { 
        shade4.enabled = false;

        Color c2 = bull1.GetComponent<Text>().color;
        c2.a = 0.33f;
        bull1.GetComponent<Text>().color = c2;
        bull2.GetComponent<Text>().color = c2;
        bull3.GetComponent<Text>().color = c2;

        vp4.Play();

    }
        catch (Exception e) { Debug.Log("ERROR IN P4Anim: " + e.Message); }
yield return new WaitForSeconds(3.5f);
        try { 
        // Scale phone D out
        LeanTween.scale(phone4, new Vector3(0, 0, 0), 0.25f).setEase(LeanTweenType.easeSpring)
            .setOnComplete(() =>
            {
                if(shade4 != null) shade4.enabled = true;
                if (vp4 != null) vp4.Pause();
                if (vp4 != null) vp4.targetTexture.Release();
                if (phone4 != null) phone4.SetActive(false);

                // Bull1 HIDE
                LeanTween.scale(bull1, new Vector3(0, 0), 0.25f).setEase(LeanTweenType.easeSpring)
            .setOnComplete(() =>
            {
                if (bull1 != null)
                {
                    Color c1 = bull1.GetComponent<Text>().color;
                    c1.a = 1f;
                    bull1.GetComponent<Text>().color = c1;

                    bull1.SetActive(false);
                }
            });

                // Bull2 HIDE
                LeanTween.scale(bull2, new Vector3(0, 0), 0.25f).setEase(LeanTweenType.easeSpring)
            .setOnComplete(() =>
            {
                if (bull2 != null)
                {
                    Color c1 = bull2.GetComponent<Text>().color;
                    c1.a = 1f;
                    bull2.GetComponent<Text>().color = c1;

                    bull2.SetActive(false);
                }
            });

                // Bull3 HIDE
                LeanTween.scale(bull3, new Vector3(0, 0), 0.25f).setEase(LeanTweenType.easeSpring)
            .setOnComplete(() =>
            {
                if (bull3 != null)
                {
                    Color c1 = bull3.GetComponent<Text>().color;
                    c1.a = 1f;
                    bull3.GetComponent<Text>().color = c1;

                    bull3.SetActive(false);
                }
            });
            });

        }
        catch (Exception e) { Debug.Log("ERROR IN P4Anim: " + e.Message); }

        yield return new WaitForSeconds(0.75f);

        try { 
        if(currentPageIndex == 3) StartCoroutine(P4anim(phone2, phone4, vp2, vp4, shade2, shade4,
            true1, true2, true3, bull1, bull2, bull3));

        }
        catch (Exception e) { Debug.Log("ERROR IN P4Anim: " + e.Message); }

    }

    #endregion
    #region 5
    IEnumerator P5setup(GameObject layout)
    {
        GameObject phone2 = Instantiate(phones[1], layout.transform);

        Image shade2 = phone2.transform.Find("Shade").GetComponent<Image>();
        shade2.enabled = false;

        phone2.transform.GetComponent<RectTransform>().localPosition = ph.positions["Middle"].GetComponent<RectTransform>().localPosition;

        phone2.transform.GetComponent<RectTransform>().localScale = new Vector3(0.0f, 0.0f);

        // VIDEO //

        VideoPlayer vp2 = phone2.transform.Find("Video Player").GetComponent<VideoPlayer>();

        vp2.Prepare();

        while (!vp2.isPrepared)
        {
            Debug.Log("Preparing Video P4");
            yield return null;
        }

        phone2.SetActive(false);

        vp2.targetTexture.Release();

        StartCoroutine(P5anim(phone2,  vp2,  shade2));
    }

    IEnumerator P5anim(GameObject phone2, VideoPlayer vp2, Image shade2)
    {
        double time2 = TimeConv(6, 51, 50);

        try
        {
            vp2.time = time2;

            phone2.SetActive(true);

            // Scale phone D in
            LeanTween.scale(phone2, new Vector3(1, 1, 1), 0.25f).setEase(LeanTweenType.easeSpring)
                .setOnStart(() =>
                {
                    if (vp2 != null) vp2.Play();
                });


        }
        catch (Exception e) { Debug.Log("ERROR IN P3Anim: " + e.Message); }

        yield return new WaitForSeconds(6);

        try
        {

            // Scale phone D out
            LeanTween.scale(phone2, new Vector3(0, 0, 0), 0.25f).setEase(LeanTweenType.easeSpring)
                .setOnStart(() =>
                {
                    if (vp2 != null) vp2.Pause();
                    if (vp2 != null) vp2.targetTexture.Release();
                })
                .setOnComplete(() =>
                {
                    if(phone2 != null) phone2.SetActive(false);

                    if(currentPageIndex == 4) StartCoroutine(P5anim(phone2, vp2, shade2));

                });
        }
        catch (Exception e) { Debug.Log("ERROR IN P5Anim: " + e.Message); }

    }

    #endregion
    #region 6
    IEnumerator P6setup(GameObject layout)
    {
        GameObject phone1 = Instantiate(phones[0], layout.transform);
        GameObject phone4 = Instantiate(phones[3], layout.transform);

        GameObject Sabo1 = Instantiate(Image_Sabo, layout.transform);
        GameObject Sabo2 = Instantiate(Image_Sabo, layout.transform);

        Image shade1 = phone4.transform.Find("Shade").GetComponent<Image>();
        Image shade4 = phone1.transform.Find("Shade").GetComponent<Image>();

        shade1.enabled = false;
        shade4.enabled = false;

        phone1.transform.GetComponent<RectTransform>().localPosition = ph.positions["2Left"].GetComponent<RectTransform>().localPosition;
        phone4.transform.GetComponent<RectTransform>().localPosition = ph.positions["2Right"].GetComponent<RectTransform>().localPosition;

        Sabo1.transform.GetComponent<RectTransform>().localPosition = ph.positions["2Left"].GetComponent<RectTransform>().localPosition;
        Sabo2.transform.GetComponent<RectTransform>().localPosition = ph.positions["2Right"].GetComponent<RectTransform>().localPosition;

        phone1.transform.GetComponent<RectTransform>().localScale = new Vector3(0.0f, 0.0f);
        phone4.transform.GetComponent<RectTransform>().localScale = new Vector3(0.0f, 0.0f);

        //Sabo1.transform.GetComponent<RectTransform>().localScale = new Vector3(0.0f, 0.0f);
        //Sabo2.transform.GetComponent<RectTransform>().localScale = new Vector3(0.0f, 0.0f);

        Sabo1.SetActive(false);
        Sabo2.SetActive(false);

        // VIDEO //

        VideoPlayer vp1 = phone1.transform.Find("Video Player").GetComponent<VideoPlayer>();
        VideoPlayer vp4 = phone4.transform.Find("Video Player").GetComponent<VideoPlayer>();

        vp1.Prepare();
        vp4.Prepare();

        while (!vp1.isPrepared || !vp4.isPrepared)
        {
            Debug.Log("Preparing Video P6");
            yield return null;
        }

        phone1.SetActive(false);
        phone4.SetActive(false);

        vp1.targetTexture.Release();
        vp4.targetTexture.Release();

        StartCoroutine(P6anim(phone4, vp4, phone1, vp1, shade1, shade4,
            Sabo1, Sabo2));
    }

    IEnumerator P6anim(GameObject phone4, VideoPlayer vp4, GameObject phone1, VideoPlayer vp1, Image shade1, Image shade4,
        GameObject sabo1, GameObject sabo2)
    {
        double time_start = TimeConv(6, 52, 0); // approx 6 secs

        double time_turnA = TimeConv(2, 37, 0); // approx 3 secs
        double time_turnD = TimeConv(3, 36, 0); // approx 3 secs

        try
        {
            phone1.SetActive(true);
            phone4.SetActive(true);

            vp1.time = time_start;
            vp4.time = time_start;

            vp1.Play();
            vp4.Play();


            // Scale phones in
            LeanTween.scale(phone1, new Vector3(1, 1, 1), 0.25f).setEase(LeanTweenType.easeSpring);

            LeanTween.scale(phone4, new Vector3(1, 1, 1), 0.25f).setEase(LeanTweenType.easeSpring);
        }
        catch (Exception e) { Debug.Log("ERROR IN P5Anim: " + e.Message); }

        yield return new WaitForSeconds(5f);
        try
        {
            // Pop back and in, changing to As turn scenario
            LeanTween.scale(phone1, new Vector3(0.7f, 0.7f, 0.7f), 0.25f).setEase(LeanTweenType.easeSpring)
                .setOnStart(() =>
                {
                    if (vp1 != null)
                    {
                        vp1.targetTexture.Release();
                        vp1.time = time_turnA;
                        vp1.Pause();
                    }
                })
                .setOnComplete(() =>
                {
                    LeanTween.scale(phone1, new Vector3(1, 1, 1), 0.25f).setEase(LeanTweenType.easeSpring)
                        .setOnStart(() =>
                        {
                            if(vp1 != null) vp1.Play();

                        })
                        .setOnComplete(() =>
                        {
                            // IsTurn scenario
                        });
                });

            LeanTween.scale(phone4, new Vector3(0.7f, 0.7f, 0.7f), 0.25f).setEase(LeanTweenType.easeSpring)
                .setOnStart(() =>
                {
                    if (vp4 != null)
                    {
                        vp4.targetTexture.Release();
                        vp4.time = time_turnA;
                        vp4.Pause();
                    }
                })
                .setOnComplete(() =>
                {
                    LeanTween.scale(phone4, new Vector3(1, 1, 1), 0.25f).setEase(LeanTweenType.easeSpring)
                        .setOnStart(() =>
                        {
                            if(vp4 != null) vp4.Play();

                        })
                        .setOnComplete(() =>
                        {
                            // Sabo scenario

                            if(shade4 != null) shade4.enabled = true;
                            if(sabo1 != null) sabo1.SetActive(true);

                        });

                });

        }
        catch (Exception e) { Debug.Log("ERROR IN P5Anim: " + e.Message); }

        yield return new WaitForSeconds(3f);
        try
        {
            // Pop back and in, changing to Ds turn scenario
            LeanTween.scale(phone1, new Vector3(0.7f, 0.7f, 0.7f), 0.25f).setEase(LeanTweenType.easeSpring)
                .setOnStart(() =>
                {
                    if (vp1 != null)
                    {
                        vp1.targetTexture.Release();
                        vp1.time = time_turnD;
                        vp1.Pause();
                    }
                })
                .setOnComplete(() =>
                {
                    LeanTween.scale(phone1, new Vector3(1, 1, 1), 0.25f).setEase(LeanTweenType.easeSpring)
                           .setOnStart(() =>
                           {
                               if(vp1 != null) vp1.Play();

                           })
                           .setOnComplete(() =>
                           {
                               // Sabo scenario
                                if(shade1 != null) shade1.enabled = true;
                                if(sabo2 != null) sabo2.SetActive(true);
                            });


                });

            LeanTween.scale(phone4, new Vector3(0.7f, 0.7f, 0.7f), 0.25f).setEase(LeanTweenType.easeSpring)
                .setOnStart(() =>
                {
                    if(shade4 != null) shade4.enabled = false;
                    if(sabo1 != null) sabo1.SetActive(false);

                    if (vp4 != null)
                    {
                        vp4.targetTexture.Release();
                        vp4.time = time_turnD;
                        vp4.Pause();
                    }

                })
                .setOnComplete(() =>
                {
                    LeanTween.scale(phone4, new Vector3(1, 1, 1), 0.25f).setEase(LeanTweenType.easeSpring)
                        .setOnStart(() =>
                        {
                            if(vp4 != null) vp4.Play();

                        })
                        .setOnComplete(() =>
                        {
                            LeanTween.scale(phone4, new Vector3(1, 1, 1), 0.25f).setEase(LeanTweenType.easeSpring)
                           .setOnStart(() =>
                           {


                           })
                           .setOnComplete(() =>
                           {
                            // IsTurn scenario
                            });

                        });

                });

        }
        catch (Exception e) { Debug.Log("ERROR IN P5Anim: " + e.Message); }

        yield return new WaitForSeconds(3f);
        try {

        shade1.enabled = false;
        sabo2.SetActive(false);

        // Scale phones OUT
        LeanTween.scale(phone1, new Vector3(0, 0, 0), 0.25f).setEase(LeanTweenType.easeSpring);

        LeanTween.scale(phone4, new Vector3(0, 0, 0), 0.25f).setEase(LeanTweenType.easeSpring)
            .setOnComplete(() =>
            {
                if(phone1 != null) phone1.SetActive(false);
                if(phone4 != null) phone4.SetActive(false);

                if(vp1 != null) vp1.Pause();
                if(vp4 != null) vp4.Pause();

                if(vp1 != null) vp1.targetTexture.Release();
                if(vp4 != null) vp4.targetTexture.Release();
            });

    }
        catch (Exception e) { Debug.Log("ERROR IN P5Anim: " + e.Message); }
yield return new WaitForSeconds(0.75f);
        try { 
        if(currentPageIndex == 5) StartCoroutine(P6anim(phone4, vp4, phone1, vp1, shade1, shade4,
            sabo1, sabo2));

        }
        catch (Exception e) { Debug.Log("ERROR IN P5Anim: " + e.Message); }
    }

    #endregion
    #region 7
    IEnumerator P7setup(GameObject layout)
    {
        GameObject phone4 = Instantiate(phones[3], layout.transform);

        Image shade4 = phone4.transform.Find("Shade").GetComponent<Image>();
        shade4.enabled = false;

        phone4.transform.GetComponent<RectTransform>().localPosition = ph.positions["Middle"].GetComponent<RectTransform>().localPosition;

        phone4.transform.GetComponent<RectTransform>().localScale = new Vector3(0.0f, 0.0f);

        // VIDEO //

        VideoPlayer vp4 = phone4.transform.Find("Video Player").GetComponent<VideoPlayer>();

        vp4.Prepare();

        while (!vp4.isPrepared)
        {
            Debug.Log("Preparing Video P7");
            yield return null;
        }

        phone4.SetActive(false);

        vp4.targetTexture.Release();

        StartCoroutine(P7anim(phone4, vp4, shade4));


    }

    IEnumerator P7anim(GameObject phone4, VideoPlayer vp4, Image shade4)
    {
        double time4 = TimeConv(2, 40, 0);

        try
        {
            vp4.time = time4;

            phone4.SetActive(true);

            // Scale phone D in
            LeanTween.scale(phone4, new Vector3(1, 1, 1), 0.25f).setEase(LeanTweenType.easeSpring)
                .setOnStart(() =>
                {
                    if (vp4 != null) vp4.Play();
                });


        }
        catch (Exception e) { Debug.Log("ERROR IN P7Anim: " + e.Message); }

        yield return new WaitForSeconds(10);

        try
        {

            // Scale phone D out
            LeanTween.scale(phone4, new Vector3(0, 0, 0), 0.25f).setEase(LeanTweenType.easeSpring)
                .setOnStart(() =>
                {
                    if (vp4 != null) vp4.Pause();
                    if (vp4 != null) vp4.targetTexture.Release();
                })
                .setOnComplete(() =>
                {
                    if(phone4 != null) phone4.SetActive(false);


                });
        }
        catch (Exception e) { Debug.Log("ERROR IN P7Anim: " + e.Message); }

        yield return new WaitForSeconds(0.5f);

        try
        {
            if (currentPageIndex == 6) StartCoroutine(P7anim(phone4, vp4, shade4));

        }catch (Exception e) { Debug.Log("ERROR IN P7Anim: " + e.Message); }
    }

    #endregion

    double TimeConv(int m, int s, int ms)
    {
        return (double)(60 * m + s +(((double) ms) / 100));
    }

    void SetListeners()
    {
        button_skip_tutorial.onClick.AddListener(() => {
            main.RequestFragmentDirect(MAIN_MENU.ID);
        });

        Button_GetStarted.onClick.AddListener(() => {
            main.RequestFragmentDirect(MAIN_MENU.ID);
        });

        button_next_tutorial.onClick.AddListener(() =>
        {
            main.PlaySfx("button_major", 0.7f);
        });
    }

    // Update is called once per frame
    void Update()
    {

    }


    public override void RoomChangeResult(string code, Task task, bool success, object args)
    {


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
}
