using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecelerateInterpolator : Interpolator
{
    public float factor = 0.5f;

    //  default
    public DecelerateInterpolator()
    {

    }

    public DecelerateInterpolator(float factor)
    {
        this.factor = factor;
    }

    public override float getInterpolation(float input)
    {
        return 1 - Mathf.Pow(1-input, 2*factor);
    }
}