using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Achievement
{
    public string title;
    public string msg;
    public int pointsWorth;

    public string simpleString;
    public string simpleColouredString;

    public static Color points_colour = new Color(0, 0, 255, 255);
    public static Color neg_colour = new Color(255, 0, 0, 255);
    public static Color grey_colour = new Color(122, 122, 122, 255);

    public Achievement(string title, string msg, int pointsWorth)
    {
        this.title = title;
        this.msg = msg;
        this.pointsWorth = pointsWorth;

        simpleString = (pointsWorth > 0 ? "+" : "-")
            + Mathf.Abs(pointsWorth)
            + " - "
            + msg;


        string colString = "";

        if(pointsWorth > 0)
        {
            colString = ColorUtility.ToHtmlStringRGB(points_colour);
        }
        else if(pointsWorth == 0)
        {

            colString = ColorUtility.ToHtmlStringRGB(grey_colour);
        }
        else if (pointsWorth < 0)
        {
            colString = ColorUtility.ToHtmlStringRGB(neg_colour);
        }


        simpleColouredString = "<color=#" + colString +">"
            + (pointsWorth > 0 ? "+" : "-")
            + Mathf.Abs(pointsWorth)
            +"</color>"
            + " - "
            + msg;

    }

    
}
