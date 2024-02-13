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

    public static string GetName(int player)
    {
        return player switch
        {
            0 => "Gray",
            1 => "Blue",
            2 => "Red",
            3 => "Green",
            4 => "Yellow",
            _ => "Gray",
        };
    }
}
