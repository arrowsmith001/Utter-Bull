using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class POPUP_BULL : MonoBehaviour
{

    public GP7_REVEAL gp7;
    public Text TrueOrBull;
    public Button button;
    public Image img;
    public GameObject Pos_Main;
    public GameObject Pos_Ach;
    public PositionsHolder ph = new PositionsHolder();

    public GameObject PointsPopup;
    public Text Text_Points;
    public Text Text_Name;
    public Text Text_Msg;

    private MainScript main;
    private List<Achievement> AchList;

    private void Awake()
    {
        this.main = gp7.main;
        this.AchList = gp7.AchList;

        Text_Points.text = "";
        Text_Name.text = "";
        Text_Msg.text = "";

        ph.AddPositions(Pos_Main);
        ph.AddPositions(Pos_Ach);

        button.onClick.AddListener(() =>
        {
            gp7.SetPosition(4);
            Destroy();
        });
    }

    // Start is called before the first frame update
    void Start()
    {
        Anim();

        StartCoroutine(AchPopup());
    }


    private void Anim()
    {
        main.PlaySfx("bull_reveal", 1);


        LeanTween.value(0, 1, 0.5f).setOnUpdate((float v) =>
        {
            try
            {
                if (img != null && TrueOrBull.gameObject != null)
                {

                    TrueOrBull.gameObject.transform.localScale = new Vector2(1, 1) * (3 - (2 * v));
                    img.gameObject.transform.localPosition
                    = Vector3.Lerp(ph.positions["Image_Initial"].localPosition, ph.positions["Image_Final"].localPosition, v);

                    Color c = img.color;
                    c.a = v;
                    img.color = c;
                }
            }
            catch { }

        }).setOnComplete(() =>
        {
            try
            {
                if (img != null) LeanTween.rotateAroundLocal(img.gameObject, Vector3.forward, 10, 0.5f);
                if (img != null) LeanTween.rotateAroundLocal(img.gameObject, Vector3.back, 20, 0.5f).setDelay(0.5f);
                if (img != null) LeanTween.rotateAroundLocal(img.gameObject, Vector3.forward, 10, 0.5f).setDelay(1f).setOnComplete(() => img.gameObject.SetActive(false));
            }
            catch { }
        });
    }

    IEnumerator AchPopup()
    {
        if (AchList != null && AchList.Count > 0)
        {
            foreach (Achievement ach in AchList)
            {

                FormatPopup(ach);

                if (ach.pointsWorth <= 50) main.PlaySfx("points_gained_low", 1);
                else main.PlaySfx("points_gained_high", 1);

                try
                {
                    LeanTween.scale(Text_Points.gameObject, new Vector3(1, 1), 0.2f).setFrom(new Vector3(0.5f, 0.5f)).setDelay(0)
                        .setOnStart(() =>
                        {
                            try
                            {
                                PointsPopup.transform.localPosition = ph.positions["Start"].localPosition;
                            }
                            catch { }
                        });
                    LeanTween.scale(Text_Name.gameObject, new Vector3(1, 1), 0.2f).setFrom(new Vector3(0.5f, 0.5f)).setDelay(0.1f);
                    LeanTween.scale(Text_Msg.gameObject, new Vector3(1, 1), 0.2f).setFrom(new Vector3(0.5f, 0.5f)).setDelay(0.2f);

                    LeanTween.move(PointsPopup, ph.positions["End"], 3);

                    LeanTween.scale(Text_Points.gameObject, new Vector3(0, 0), 0.2f).setFrom(new Vector3(1f, 1f)).setDelay(2.4f);
                    LeanTween.scale(Text_Name.gameObject, new Vector3(0, 0), 0.2f).setFrom(new Vector3(1f, 1f)).setDelay(2.6f);
                    LeanTween.scale(Text_Msg.gameObject, new Vector3(0, 0), 0.2f).setFrom(new Vector3(1f, 1f)).setDelay(2.8f);
                }
                catch { }



                yield return new WaitForSeconds(3);
            }

            Destroy();
        }
        else
        {
            yield return new WaitForSeconds(3);
            Destroy();
        }

    }

    void FormatPopup(Achievement ach)
    {
        Text_Points.text = (ach.pointsWorth < 0 ? "-" : "+") + Mathf.Abs(ach.pointsWorth);
        Text_Name.text = ach.title;
        Text_Msg.text = ach.msg;
    }


    public void Destroy()
    {
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
