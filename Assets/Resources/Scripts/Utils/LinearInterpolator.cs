using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearInterpolator : Interpolator
{

    public float factor = 0.5f;

    //  default
    public LinearInterpolator()
    {

    }

    public LinearInterpolator(float factor)
    {
        this.factor = factor;
    }

    public override float getInterpolation(float input)
    {
        return input;
    }
}