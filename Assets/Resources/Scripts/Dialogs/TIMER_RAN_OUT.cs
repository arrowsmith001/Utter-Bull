using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TIMER_RAN_OUT : MonoBehaviour
{
    public GameObject Pos;

    public Text text;
    public Image img;

    // Start is called before the first frame update
    void Start()
    {
        PositionsHolder ph = new PositionsHolder();
        ph.AddPositions(Pos);

        try { 
        LeanTween.value(0, 1, 0.5f).setOnUpdate((float v) =>
        {
            try { 
            text.gameObject.transform.localScale = new Vector2(1, 1) * (5 - (4 * v));
            img.gameObject.transform.localPosition
                = Vector3.Lerp(ph.positions["Image_Initial"].localPosition, ph.positions["Image_Final"].localPosition, v);

            Color c = img.color;
            c.a = v;
            img.color = c;

            }
            catch { }

        }).setOnComplete(() =>
        {
            try { 
            LeanTween.rotateAroundLocal(img.gameObject, Vector3.forward, 10, 0.5f);
            LeanTween.rotateAroundLocal(img.gameObject, Vector3.back, 20, 0.5f).setDelay(0.5f);
            LeanTween.rotateAroundLocal(img.gameObject, Vector3.forward, 10, 0.5f).setDelay(1f).setOnComplete(() =>
            {
                img.gameObject.SetActive(false);
                Destroy();

                });
            }
            catch { }
        });

        }
        catch { }
    }
    private void Destroy()
    {
        Destroy(this.gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
