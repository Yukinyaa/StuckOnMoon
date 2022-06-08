using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FixMath.NET;
using static FixMath.NET.Fix64;

public class SurfaceGen
{
    //Fix64[] surfaceNormal;
    //Fix64[] surfaceX;
    //Fix64[] surfaceY;
    Fix64[] offset;

    Fix64[] EuilerRotMatrix = new Fix64[3]; // zyz rotation
    Fix64 zoffset;


    public SurfaceGen(byte[] seed)
    {
        Debug.Assert(seed.Length >= 3);
        int len = seed.Length;
        Fix64 x, y, z;
        offset = new Fix64[] {
            x = Fix64.FromRaw( ((long)seed[len-3]) << 32 ) * Pi,
            y = Fix64.FromRaw( ((long)seed[len-2]) << 32 ) * Pi,
            z = Fix64.FromRaw( ((long)seed[len-1]) << 32 ) * Pi
        };
        //EuilerRotMatrix = new Fix64[] {
        //    x = Fix64.FromRaw( ((long)seed[len-3]) << 24 ) * Pi,
        //    y = Fix64.FromRaw( ((long)seed[len-2]) << 24 ) * Pi,
        //    z = Fix64.FromRaw( ((long)seed[len-1]) << 24 ) * Pi
        //};
        //zoffset = Fix64.FromRaw(((long)seed[len - 1]) << 26);
        // https://en.wikipedia.org/wiki/Conversion_between_quaternions_and_Euler_angles
        // normal = (0,1,0)
        //surfaceNormal = new Fix64[] {
        //    -Cos(y) * Sin(x) + Sin(z) * Sin(x) * Cos(y),
        //     Cos(y) * Cos(x) + Sin(z) * Sin(x) * Sin(y),
        //     Sin(z) * Cos(y)
        //};
        //// x = (1,0,0)
        //surfaceX = new Fix64[] {
        //     Cos(y) * Cos(x),
        //     Cos(y) * Sin(x),
        //    -Sin(y)
        //};
        //// y = front(0,0,1)
        //surfaceY = new Fix64[] {
        //     Sin(y) * Sin(x) + Cos(z) * Sin(x) * Cos(y),
        //    -Sin(y) * Cos(x) + Cos(z) * Sin(x) * Sin(y),
        //     Cos(z) * Cos(y)
        //};
        //surfaceNormal = new Fix64[] {
        //    -Cos(y) * Sin(x) + Sin(z) * Sin(x) * Cos(y),
        //     Cos(y) * Cos(x) + Sin(z) * Sin(x) * Sin(y),
        //     Sin(z) * Cos(y)
        //};
        //// x = (1,0,0)
        //surfaceX = new Fix64[] {
        //     Cos(y) * Cos(x),
        //     Cos(y) * Sin(x),
        //    -Sin(y)
        //};
        //// y = front(0,0,1)
        //surfaceY = new Fix64[] {
        //     Sin(y) * Sin(x) + Cos(z) * Sin(x) * Cos(y),
        //    -Sin(y) * Cos(x) + Cos(z) * Sin(x) * Sin(y),
        //     Cos(z) * Cos(y)
        //};
    }

    Fix64 med = (Fix64)32;
    Fix64 var = (Fix64)64; //0x0000 000F 0000 0000
    public int SurfaceGenV0(int x, int y)
    {
        var surfacePos = ValAt((Fix64)x, (Fix64)0, (Fix64)0, (Fix64)128) * var + med;
        if (surfacePos > (Fix64)y)
            return 1;
        else
            return 0;
    }
    public int SurfaceGenFlatty(int x, int y)
    {
        if (med > (Fix64)y)
            return 1;
        else
            return 0;
    }


    Fix64 ValAt(Fix64 x, Fix64 y, Fix64 z, Fix64 Scale)
    {
        return OctavePerlin(
            x / Scale + offset[0],
            y / Scale + offset[1],
            z / Scale + offset[2],
            3, (Fix64)5 / (Fix64)10);
    //    return perlin(
    //        surfaceX[0] * Scale * x + surfaceY[0] * Scale * y + surfaceNormal[0] * Scale * z,
    //        surfaceX[1] * Scale * x + surfaceY[1] * Scale * y + surfaceNormal[1] * Scale * z,
    //        surfaceX[2] * Scale * x + surfaceY[2] * Scale * y + surfaceNormal[2] * Scale * z
    //        );
    }
    

    /// <summary>
    /// returns test texture from 0 to size/scale
    /// </summary>
    /// <param name="z">z offset</param>
    /// <param name="scale">pixels per unit</param>
    /// <param name="size">size in pixel</param>
    /// <returns></returns>
    public Sprite TestPerlin(float z, float scale, int size)
    {
        Fix64 fz = (Fix64)z;
        Fix64 fs = (Fix64)scale;
        Fix64 fss = (Fix64)size;
        Texture2D tex = new Texture2D(size, size);

        float starttime = Time.realtimeSinceStartup;
        for (Fix64 x = Zero; x < fss; x+=One)
        {
            for (Fix64 y = Zero; y < fss; y+=One)
            {
                float val = (float)ValAt(x / fs, y / fs, fz, One);
                tex.SetPixel((int)x, (int)y, new Color(val, val, val));
            }
        }
        Debug.Log($"per call time: {(Time.realtimeSinceStartup - starttime) / size / size * 60 * 100}% of 1/60s");
        tex.Apply();
        return Sprite.Create(tex,
                             new Rect(0, 0, size, size),
                             new Vector2(0.5f, 0.5f)); 
    }
}
