using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTextPulse : MonoBehaviour
{

    public RectTransform textRect;

    Color32 c1 = new Color32(0, 29, 179, 255);
    Color32 c2 = new Color32(0, 42, 255, 255);

    // Start is called before the first frame update
    void Start()
    {
        Anim();
    }

    void Anim()
    {
        try
        {
            LeanTween.colorText(textRect, c1, 1.25f)
                .setOnComplete(() =>
                {
                    try
                    {
                        LeanTween.colorText(textRect, c2, 1.25f)
                            .setOnComplete(() =>
                            {
                                try { Anim(); } catch { }
                            });
                    }
                    catch { }
                });

        }
        catch { }
    }


}
