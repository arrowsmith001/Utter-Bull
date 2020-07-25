using System;
using System.Collections.Generic;
using UnityEngine;

public class LoadingDots : MonoBehaviour
{

    //the total time of the animation
    public float repeatTime = 1;

    //the time for a dot to bounce up and come back down
    public float bounceTime = 0.25f;

    //how far does each dot move
    public float bounceHeight = 10;

    public List<GameObject> dots;

    void Start()
    {
        if (repeatTime < dots.Count * bounceTime)
        {
            repeatTime = dots.Count * bounceTime;
        }

        InvokeRepeating("Animate", 0, repeatTime);

    }

    void Animate()
    {
        for (int i = 0; i < dots.Count; i++)
        {
            int dotIndex = i;

            try
            {
                LeanTween.moveLocalY(dots[dotIndex], dots[dotIndex].transform.localPosition.y + bounceHeight, bounceTime / 2)
                    .setDelay(dotIndex * bounceTime / 2)
                    .setEase(LeanTweenType.easeOutQuad)
                    .setOnComplete(() =>
                    {
                        try
                        {
                            LeanTween.moveLocalY(dots[dotIndex], dots[dotIndex].transform.localPosition.y - bounceHeight, bounceTime / 2)
                                .setEase(LeanTweenType.easeInQuad)
                                .setOnComplete(() =>
                                {

                                });
                        }
                        catch { }
                    });
            }
            catch (Exception e)
            {

            }
            //dots[dotIndex].transform
            //	.DOMoveY(dots[dotIndex].transform.position.y + bounceHeight, bounceTime / 2)
            //	.SetDelay(dotIndex * bounceTime / 2)
            //	.SetEase(Ease.OutQuad)
            //	.OnComplete(() =>
            //	{
            //		dots[dotIndex].transform
            //			.DOMoveY(dots[dotIndex].transform.position.y - bounceHeight, bounceTime / 2)
            //			.SetEase(Ease.InQuad);
            //	});
        }
    }
}