using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionsHolder
{

    public Dictionary<string, Transform> positions;

    public PositionsHolder()
    {
        positions = new Dictionary<string, Transform>();
    }

    public void AddPositions(GameObject pos)
    {
        for (int i = 0; i < pos.transform.childCount; i++)
        {
            if(!positions.ContainsKey(pos.transform.GetChild(i).gameObject.name))
            {

                positions.Add(
                    pos.transform.GetChild(i).gameObject.name,
                    pos.transform.GetChild(i).gameObject.transform
                    //.localPosition
                    );
            }
        }
    }
}
