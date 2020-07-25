using System.Collections.Generic;

public class Order
{
    // Order in which players will take their turns
    public List<string> playerOrder = new List<string>();

    // During play phase, this is whose turn it is
    public int turnNum = 0;
}