using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelerateInterpolator : Interpolator
{
    public float factor = 0.5f;

    //  default
    public AccelerateInterpolator()
    {

    }

    public AccelerateInterpolator(float factor)
    {
        this.factor = factor;
    }

    public override float getInterpolation(float input)
    {
        return Mathf.Pow(input, 2 * factor);
    }
}