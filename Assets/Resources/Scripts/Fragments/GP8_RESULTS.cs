using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using frame8.ScrollRectItemsAdapter.Classic.Examples;
using UnityEngine;
using UnityEngine.UI;

public class GP8_RESULTS : MonoBehaviour
{
    private MainScript main;
    private GP6_REVEALS_BASE gp6;

    public PositionsHolder ph = new PositionsHolder();
    public GameObject Pos;

    public List<Player> list = new List<Player>();
    public GP8_ListContent gp8_listContent;
    public VerticalListView_ResultsList listScript;
    public VerticalLayoutGroup vlg;

    public GameObject dummyList;

    public Button button_exp;
    public Button button_col;

    private Interpolator decel = new AccelerateInterpolator(2);

    public void Awake()
    {
        ph.AddPositions(Pos);
    }

    public void GiveRefs(MainScript main, GP6_REVEALS_BASE gp6)
    {
        this.main = main;
        this.gp6 = gp6;
        gp8_listContent.GiveRef(gp6);
    }

    public void HideAndCollapseItems()
    {
        listScript.CollapseItems();


        for (int i = 0; i < gp8_listContent.transform.childCount; i++)
        {
            Transform trans = gp8_listContent.transform.GetChild(i).transform;
            trans.gameObject.SetActive(false);
        }
    }

    float[] listYs;

    bool[] bs;
    int bInt = -1;

    public void AnimateListItems()
    {
        if (gp8_listContent.transform.childCount < WAITING_ROOM.PLAYER_MIN_FOR_GAME)
        {
            throw new Exception("RESULTS LIST NOT INITIALISED");
        }

        float space = vlg.spacing;

        // Store Y values
        listYs = new float[gp8_listContent.transform.childCount];
        for (int i = 0; i < gp8_listContent.transform.childCount; i++)
        {
            Transform trans = gp8_listContent.transform.GetChild(i).transform;
            float height = trans.Find("MainParent").GetComponent<RectTransform>().rect.height;

            // listYs[i] = trans.localPosition.y;
            listYs[i] = -(space + i*(height + 2*space));

            Debug.Log(i+": "+listYs[i]);
        }

        vlg.enabled = false; // TODO Re-enable vlg

        // Set Y values
        for (int i = 0; i < gp8_listContent.transform.childCount; i++)
        {
            Transform trans = gp8_listContent.transform.GetChild(i).transform;
            Vector3 pos = trans.localPosition;
            pos.y = listYs[i];
            trans.localPosition = pos;
        }

            for (int i = 0; i < gp8_listContent.transform.childCount; i++)
        {
            Transform trans = gp8_listContent.transform.GetChild(i).transform;
            Vector3 finalPos = trans.localPosition;

            trans.gameObject.SetActive(false);

            float finalX = trans.localPosition.x;
            float initialX = ph.positions["X"].localPosition.x;

            bool b = (i == gp8_listContent.transform.childCount - 1);

            StartCoroutine(Animate(0.3f + (i * 0.15f), finalX, initialX, finalPos, trans, b));

        }

    }

    private IEnumerator Animate(float time, float finalX, float initialX, Vector3 finalPos, Transform trans, bool last)
    {
        bool b = false;

        yield return new WaitForSeconds(time);

        Debug.Log("LAST: " + last);

        LeanTween.value(0, 1, 2f).setOnUpdate((float t) =>
        {
          try{
                Vector3 newPos = finalPos;
            newPos.x = finalX + (initialX - finalX) * decel.getInterpolation(1 - t);
            trans.localPosition = newPos;

            if(!b)
            {
                trans.gameObject.SetActive(true);
                b = true;
            }

            if(last)
            {
                if(!dummyShow)
                {
                    StartCoroutine(HideDummyList());
                    dummyShow = true;
                }
            }
             } catch { }
        }).setOnStart(() => { ; })
        .setOnComplete(() => {

            Vector3 newPos = finalPos;
            newPos.x = finalX - 2;
            trans.localPosition = newPos;

            if (last) vlg.enabled = true;
        }) ;
    }

    IEnumerator HideDummyList()
    {
        yield return new WaitForSeconds(0.25f);

        dummyList.SetActive(false);
    }

    bool dummyShow = false;

    private void Update()
    {

    }

    public GameObject block;

    public void ActivateBlock()
    {
        block.SetActive(true);
        button_exp.interactable = false;
        button_col.interactable = false;
    }
}