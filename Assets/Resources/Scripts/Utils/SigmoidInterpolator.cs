using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SigmoidInterpolator : Interpolator
{

    public float factor = 0.5f;

    //  default
    public SigmoidInterpolator()
    {

    }

    public SigmoidInterpolator(float factor)
    {
        this.factor = factor;
    }

	//public override float getInterpolation(float input)
	//{
	//    float den = (float)Mathf.Pow(input / (1 - input), -factor);
	//    return ((float)1) / (1 + den);
	//}

	public override float getInterpolation(float input)
	{
		float den = (float)Mathf.Pow((((float)1) / input) - 1, factor) + 1;
		return (1 - ((float)1) / den);
	}
}