using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UI;

public class ROUND_OVER : MonoBehaviour
{
    public Text text;

    // Start is called before the first frame update
    void Start()
    {
        Interpolator interp = new OvershootInterpolator(2);

        try
        {
            LeanTween.rotateAroundLocal(text.gameObject, Vector3.forward, 360 * 2, 1f).setEase(LeanTweenType.easeOutCubic);
            LeanTween.value(0, 1, 1).setOnUpdate((float t) =>
            {
                float t1 = interp.getInterpolation(t);
                if(text!= null) text.transform.localScale = new Vector3(t1, t1, t1);

            }).setOnComplete(() => { StartCoroutine(Death()); });

        }
        catch { }
    }

    private IEnumerator Death()
    {
        yield return new WaitForSeconds(1.5f);

        Destroy(this.gameObject);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
