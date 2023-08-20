using UnityEngine;

public static class PlayerColors
{
    public static Color Get(int player)
    {
        return player switch
        {
            0 => Color.gray,
            1 => Color.blue,
            2 => Color.red,
            3 => Color.green,
            4 => Color.yellow,
            _ => Color.gray,
        };
    }
}
