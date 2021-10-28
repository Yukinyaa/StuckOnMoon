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
    public const int chunkSize = 16;
    public int mapWidth;
    public int surfaceNo { get; private set; }
    public int mapHeight;
    public int xChunkCount;
    public int lastXChunkWidth;
    public string name = "nauvis";
    public float2 gridSize = new float2(0.5f, 0.5f);

    BufferedFixedIndexArray<int>[,] chu;

    BufferedFixedIndexArray<int>[,] chunks;
    BufferedFixedIndexArray<SurfaceObject> sObjects;

    bool[,] isGenerated;

    public int countX => chunks.GetUpperBound(0) + 1;
    public int countY => chunks.GetUpperBound(1) + 1;


    public bool IsViewed { get; private set; }


    public SurfaceChunkController(BufferedFixedIndexArray<SurfaceObject> surfaceObejcts, int surfaceNo)
    {
        mapWidth = 4200;
        mapHeight = 1200;
        this.surfaceNo = surfaceNo;
        sObjects = surfaceObejcts;
        xChunkCount = mapWidth / chunkSize + 1;
        lastXChunkWidth = mapWidth % chunkSize;
        chunks = new BufferedFixedIndexArray<int>[mapWidth / chunkSize + 1, mapHeight / chunkSize + 1];
        isGenerated = new bool[mapWidth / chunkSize + 1, mapHeight / chunkSize + 1];
        //chunk 0: 0 ~ chunksize -1, chunk 1: chunksize ~ chunksize*2-1
    }



    public void PrepareNextFrame()
    {
        for(int x = chunks.GetUpperBound(0); x>=0 ; --x)
            for (int y = chunks.GetUpperBound(1); y >= 0; --y)
                chunks[x,y]?.CopyUpdateToNext();
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
        argMin.y = Mathf.Max(argMin.y, 0);
        argMax.y = Mathf.Max(argMax.y, 0);
        if (argMin.x > argMax.x)
            argMax.x += mapWidth;
    }

    public void UnknownChunkChunkInRange(int2 argMin, int2 argMax)
    {
        ArgumentVectorPair(ref argMin, ref argMax);

        //for each chunk
        int maxXChunk = argMax.x / chunkSize;
        int maxYChunk = argMax.y / chunkSize;

        for (int xChunk = argMin.x / chunkSize; xChunk <= maxXChunk; ++xChunk)
        {
            for (int yChunk = argMin.y / chunkSize; yChunk <= maxYChunk; ++yChunk)
            {
                if (IsChunkGenerated(xChunk % xChunkCount, yChunk) == false)
                {
                    Debug.Log($"ev: {xChunk % xChunkCount}, {yChunk}");
                    EventManager.Instance.RegisterEvent(new SurfaceGenerateMapEvent(surfaceNo, new int2(xChunk % xChunkCount, yChunk)));
                    return;
                }
            }
        }
        return;
    }

    /// <summary>
    /// input: min/max pair of arb
    /// </summary>
    public void ForEachLastObjectsInChunkRange(int2 argMin, int2 argMax, Action<SurfaceObject?, int> action)
    {
        ArgumentVectorPair(ref argMin, ref argMax);

        //for each chunk


        int maxXChunk = argMax.x / chunkSize;
        int maxYChunk = argMax.y / chunkSize;
        int renderedObjectCount = 0, emptyObjectCount = 0;

        
        for (int xChunk = argMin.x / chunkSize; xChunk <= maxXChunk; ++xChunk)
        {
            for (int yChunk = argMin.y / chunkSize; yChunk <= maxYChunk; ++yChunk)
            {
                if (chunks[xChunk % xChunkCount, yChunk] == null || chunks[xChunk % xChunkCount, yChunk].createdTime > UpdateManager.RenderingFrame)
                    continue;
                chunks[xChunk % xChunkCount, yChunk].Rendering.ForEach(aa =>
                {
                    renderedObjectCount += 1;
                    bool isRenderingFilled = sObjects.SafeGetRendering(aa, out var rendering);
                    action(isRenderingFilled ? rendering : (SurfaceObject?)null, aa);
                    if (rendering.objectType == 0)
                        emptyObjectCount += 1;
                });
            }
        }

        Debug.Log($"Rendered {renderedObjectCount}({emptyObjectCount}) obejcts for frame #{UpdateManager.RenderingFrame}");
    }

    internal void MarkMapGenerated(int x, int y, bool forceReset = false)
    {
        isGenerated[x, y] = true;
        if (chunks[x, y] == null || forceReset)
            chunks[x, y] = new BufferedFixedIndexArray<int>();
    }


    internal bool IsChunkGenerated(int x, int y)
    {
        return isGenerated[x, y];
    }

    public bool ChunkExists(int x, int y)
    {
        if (chunks[x, y] == null)
            return false;
        else
            return chunks[x, y].createdTime <= UpdateManager.UpdatingFrame;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="type"></param>
    /// <returns>If placing block has succedeed</returns>
    public bool RegisterObject(int objectIdx)
    {
        SurfaceObject addObj = sObjects.GetUpdating(objectIdx);
        //Debug.Log($"Registered Object #{objectIdx}({addObj.objectType}) at {addObj.postion}");
        if (CanPlaceObject(addObj) == false)
            return false;

        int maxXChunk = addObj.MaxX / chunkSize;
        if (addObj.MaxX > 4200)
            maxXChunk = (addObj.MaxX - 4200) / chunkSize + xChunkCount;

        int maxYChunk = addObj.MaxY / chunkSize;


        for (int xChunk = addObj.MinX / chunkSize; xChunk <= maxXChunk; ++xChunk)
        { 
            for (int yChunk = addObj.MinY / chunkSize; yChunk <= maxYChunk; ++yChunk)
            {
                if (chunks[xChunk % xChunkCount, yChunk] == null)
                    chunks[xChunk % xChunkCount, yChunk] = new BufferedFixedIndexArray<int>(); //UNDO IF CUNOT GENERATED ?
                chunks[xChunk % xChunkCount, yChunk].Updating.Add(objectIdx);
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
            a?.Updating.ForEach(aa=>action(sObjects.GetUpdating(aa)));
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
                if (chunks[xChunk % xChunkCount, yChunk] != null && chunks[xChunk % xChunkCount, yChunk].Updating.Exists(obj => sObjects.GetUpdating(obj).IsCollideWith(addObj)))
                    return false;
            }
        }
        return true;
    }



}