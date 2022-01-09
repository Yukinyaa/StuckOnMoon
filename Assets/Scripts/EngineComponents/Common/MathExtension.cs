using System.Collections;
using UnityEngine;

public static class MathExtension
{
    // Euclidian mod.
    public static int EucMod(int k, int n)
    {
        if (k < 0)
            return (k % n) + n;
        else
            return k % n;
    }
}