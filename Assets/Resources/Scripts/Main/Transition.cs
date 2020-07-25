using UnityEngine;

public class Transition
{
    public string fragComingFrom;
    public string fragGoingTo;

    public int transType; // Type of transition
    public Interpolator interp; // Interpolator to be used

    public Transform pos_out_1; // The frag that is EXITING, it's START position
    public Transform pos_out_2; // The frag that is EXITING, it's END position
    public Transform pos_in_1;  // The frag that is ENTERING, it's START position
    public Transform pos_in_2; // The frag that is ENTERING, it's END position

    public GameObject frag;

    public bool ready = false;

    public Transition()
    {
    }

    public Transition(string fragComingFrom, string fragGoingTo)
    {
        this.fragComingFrom = fragComingFrom;
        this.fragGoingTo = fragGoingTo;
    }
}