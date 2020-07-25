using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnticipateInterpolator : Interpolator
{

    public float factor = 0.3f;

    //  default
    public AnticipateInterpolator()
    {

    }

    public AnticipateInterpolator(float factor)
    {
        this.factor = factor;
    }

    public override float getInterpolation(float input)
    {
        return (float)(
            (factor + 1) * Mathf.Pow(input, 3)
            - factor * Mathf.Pow(input, 2)
        );
    }
}