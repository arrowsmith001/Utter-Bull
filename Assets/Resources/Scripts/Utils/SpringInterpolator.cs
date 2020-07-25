using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringInterpolator : Interpolator {
    
    public float factor = 0.3f;
    
    //  default
    public SpringInterpolator() {
        
    }
    
    public SpringInterpolator(float factor) {
        this.factor = factor;
    }

    public override float getInterpolation(float input) {
        return ((float)(((Mathf.Pow(2, ((10 * input) 
                        * -1)) * Mathf.Sin((((2 * Mathf.PI) 
                        * (input 
                        - (this.factor / 4))) 
                        / this.factor))) 
                    + 1)));
    }
}