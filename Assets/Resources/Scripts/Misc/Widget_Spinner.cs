using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class Widget_Spinner : MonoBehaviour
{
    public Image spinner;

    Interpolator accel = new AccelerateInterpolator(1.2f);

    // Start is called before the first frame update
    void Start()
    {

        Rotate();

        Spin();

    }

    void Spin()
    {
        try
        {

            LeanTween.value(0.0f, 1, 0.8f).setOnUpdate((float t) =>
            {
                try
                {
                    spinner.fillAmount = accel.getInterpolation(t);
                }
                catch { }
            }).setOnComplete(() =>
            {
                try
                {
                    spinner.fillClockwise = !spinner.fillClockwise;

                    LeanTween.value(1f, 0.0f, 0.8f).setOnUpdate((float t) =>
                    {
                        try
                        {
                            spinner.fillAmount = accel.getInterpolation(t);
                        }
                        catch { }
                    }).setOnComplete(() =>
                    {
                        try
                        {
                            spinner.fillClockwise = !spinner.fillClockwise;
                            Spin();
                        }
                        catch { }
                    });
                }
                catch { }
            });
        }
        catch { }
    }

    void Rotate()
    {
        try
        {
            LeanTween.rotateAroundLocal(spinner.gameObject, Vector3.forward, 360, 2)
                .setOnComplete(() =>
                {
                    try { Rotate(); } catch { }
                });
        }
        catch { }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
