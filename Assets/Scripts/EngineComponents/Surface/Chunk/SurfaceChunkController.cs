using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
public static class BlockIDConsts
{
    public const int blank = 0;
}

public class SurfaceChunkController
{
    public const int chunkSize = 64;
    public int mapWidth;
    public int mapHeight;
    public int xChunkCount;
    public int lastXChunkWidth;
    public string name = "nauvis";
    public float2 gridSize = new float2(0.5f, 0.5f);



    public bool IsViewed { get; private set; }

    
    public void PrepareNextFrame()
    {
        foreach (var a in chunks)
        {
            a?.CopyToNext();
        }
    }

    public void SetViewport(int2 bottmLeft, int2 topRight)
    {
        ArgumentVectorPair(ref bottmLeft, ref topRight);

        int maxXChunk = bottmLeft.x / chunkSize;
        if (topRight.x > 4200)
            maxXChunk = (topRight.x - 4200) / chunkSize + xChunkCount;

        int maxYChunk = topRight.y / chunkSize;
    }
    // Euclidian mod.
    int EucMod(int k, int n) { return ((k %= n) < 0) ? k + n : k; }

    /// <summary>
    /// input: min/max pair of arbiraty points
    /// modify to match argmin>argmax, argmin/argmax.x > 0
    /// </summary>
    public void ArgumentVectorPair(ref int2 argMin,ref int2 argMax)
    {
        argMin.x = EucMod(argMin.x, mapWidth);
        argMax.x = EucMod(argMax.x, mapWidth);
        if (argMin.x > argMax.x)
            argMax.x += mapWidth;
    }
    
    /// <summary>
    /// input: min/max pair of arb
    /// </summary>
    public void ForEachLastObjectsInChunkRange(int2 argMin, int2 argMax, Action<SurfaceObject> action)
    {
        ArgumentVectorPair(ref argMin, ref argMax);

        for (int x = argMin.x; x < argMax.x; x++)
        {
            for (int y = argMin.y; y < argMax.y; y++)
            {
                chunks[x % mapWidth, y].Current.ForEach(aa => action(sObjects.GetLast(aa)));
            }
        }
    }

    BufferedFixedIndexArray<int>[,] chunks;
    BufferedFixedIndexArray<SurfaceObject> sObjects;


    public SurfaceChunkController(BufferedFixedIndexArray<SurfaceObject> surfaceObejcts)
    {
        mapWidth = 4200;
        mapHeight = 1200;
        sObjects = surfaceObejcts;
        xChunkCount = mapWidth / chunkSize + 1;
        lastXChunkWidth = mapWidth % chunkSize;
        chunks = new BufferedFixedIndexArray<int>[mapWidth / chunkSize + 1, mapHeight / chunkSize + 1];
        //chunk 0: 0 ~ chunksize -1, chunk 1: chunksize ~ chunksize*2-1
    }

    public bool ChunkExists(int x, int y)
    {
        if (chunks[x, y] == null)
            return false;
        else
            return chunks[x, y].Current.Count != 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="type"></param>
    /// <returns>If placing block has succedeed</returns>
    public bool RegisterObject(int objectIdx)
    {
        SurfaceObject addObj = sObjects.GetCurrent(objectIdx);
        if (CanPlaceObject(addObj) == false)
            return false;

        int maxXChunk = addObj.MaxX / chunkSize;
        if (addObj.MaxX > 4200)
            maxXChunk = (addObj.MaxX - 4200) / chunkSize + xChunkCount;

        int maxYChunk = addObj.MaxY / chunkSize;


        for (int xChunk = addObj.MinX / chunkSize; xChunk <= maxXChunk; ++xChunk)
        { 
            for (int yChunk = addObj.MinY / chunkSize; yChunk <= maxYChunk / chunkSize; ++yChunk)
            {
                if (chunks[xChunk % xChunkCount, yChunk] == null)
                    chunks[xChunk % xChunkCount, yChunk] = new BufferedFixedIndexArray<int>();
                chunks[xChunk % xChunkCount, yChunk].Current.Add(objectIdx);
            }
        }
        return true;
    }

    public bool CanPlaceObject(int2 pos, int type)
    {
        SurfaceObject addObj = new SurfaceObject(pos, type);
        if (CanPlaceObject(addObj) == false)
            return false;
        return true;
    }
    public void ForEachObject(Action<SurfaceObject> action)
    {
        foreach ( var a in chunks)
        {
            a?.Current.ForEach(aa=>action(sObjects.GetCurrent(aa)));
        }
    }
    public bool CanPlaceObject(SurfaceObject addObj)
    {
        int maxXChunk = addObj.MaxX / chunkSize;
        if (addObj.MaxX > 4200)
            maxXChunk = (addObj.MaxX - 4200) / chunkSize + xChunkCount;

        for (int xChunk = addObj.MinX / chunkSize; xChunk <= maxXChunk; ++xChunk)
        {
            for (int yChunk = addObj.MinY / chunkSize; yChunk <= addObj.MaxY / chunkSize; ++yChunk)
            {
                if (chunks[xChunk % xChunkCount, yChunk] != null && chunks[xChunk % xChunkCount, yChunk].Current.Exists(obj => sObjects.GetCurrent(obj).IsCollideWith(addObj)))
                    return false;
            }
        }
        return true;
    }



}