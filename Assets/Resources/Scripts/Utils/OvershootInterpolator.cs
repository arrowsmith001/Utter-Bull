using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OvershootInterpolator : Interpolator {
    
    public float factor = 0.3f;
    
    //  default
    public OvershootInterpolator() {
        
    }
    
    public OvershootInterpolator(float factor) {
        this.factor = factor;
    }

    public override float getInterpolation(float input) {
        return (float) (
            (factor + 1) * Mathf.Pow(input - 1, 3) 
            + factor * Mathf.Pow(input - 1, 2)
            + 1
        );
    }
}