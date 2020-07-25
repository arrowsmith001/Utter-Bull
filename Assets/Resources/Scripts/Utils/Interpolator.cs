using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interpolator
{
    public float factor { get; set; }

    public Vector3 getVec3Interpolation(Vector3 start, Vector3 end, float input)
    {
        return start + (end - start) * getInterpolation(input);
    }

    public Vector2 getVec2Interpolation(Vector2 start, Vector2 end, float input)
    {
        return start + (end - start) * getInterpolation(input);
    }

    public virtual float getInterpolation(float input) { return input; }

}