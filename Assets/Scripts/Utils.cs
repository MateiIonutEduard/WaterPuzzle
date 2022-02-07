using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public static int GetExtra(int level)
    {
        int glasses;
        int extra;

        if (level == 1) return 1;
        if (level < 4) return 2;

        Fs(level, out glasses, out extra);
        return extra;
    }

    public static void Fs(int level, out int glasses, out int extra)
    {
        int low = 4;
        int high = 8;

        glasses = 4;
        extra = 2;

        while (level < low || level > high)
        {
            glasses++;
            low = high;
            high <<= 1;
        }

        int middle = (high - low) >> 1;
        middle += low;
        if (level > middle) extra--;
    }
}
