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

public class SurfaceObejctsController
{
    public const int chunkSize = 16;
    public int mapWidth;
    public int surfaceNo { get; private set; }
    public int mapHeight;
    public int xChunkCount;
    public int lastXChunkWidth;
    public string name = "nauvis";
    public float2 gridSize = new float2(0.5f, 0.5f);

    BufferedFixedIndexArray<int>[,] chunks;
    BufferedFixedIndexArray<SurfaceObject> sObjects;
    BufferedReference<int[]>[,] sBlocks;

    bool[,] isGenerated;

    public int countX => chunks.GetUpperBound(0) + 1;
    public int countY => chunks.GetUpperBound(1) + 1;


    public bool IsViewed { get; private set; }

    public SurfaceObejctsController(int surfaceNo)
    {
        mapWidth = 4200;
        mapHeight = 1200;
        this.surfaceNo = surfaceNo;
        xChunkCount = mapWidth / chunkSize + 1;
        lastXChunkWidth = mapWidth % chunkSize;
        chunks = new BufferedFixedIndexArray<int>[mapWidth / chunkSize + 1, mapHeight / chunkSize + 1];
        isGenerated = new bool[mapWidth / chunkSize + 1, mapHeight / chunkSize + 1];
        sObjects = new BufferedFixedIndexArray<SurfaceObject>();
        //chunk 0: 0 ~ chunksize -1, chunk 1: chunksize ~ chunksize*2-1
    }


    /// <summary>
    /// input: min/max pair of arbiraty points
    /// modify to match argmin>argmax, argmin/argmax.x > 0
    /// </summary>
    public void ArgumentVectorPair(ref int2 argMin, ref int2 argMax)
    {
        argMin.x = EucMod(argMin.x, mapWidth);
        argMax.x = EucMod(argMax.x, mapWidth);
        argMin.y = Mathf.Max(argMin.y, 0);
        argMax.y = Mathf.Max(argMax.y, 0);
        if (argMin.x > argMax.x)
            argMax.x += mapWidth;
    }

    IEnumerable<(int, int)> ChunkRange(int2 min, int2 max)
    {
        ArgumentVectorPair(ref min, ref max);

        int maxXChunk = max.x / chunkSize;
        int maxYChunk = max.y / chunkSize;

        for (int xChunk = min.x / chunkSize; xChunk <= maxXChunk; ++xChunk)
        {
            for (int yChunk = min.y / chunkSize; yChunk <= maxYChunk; ++yChunk)
            {
                yield return (xChunk % xChunkCount, yChunk);
            }
        }
    }

    public void PrepareNextFrame()
    {
        sObjects.CopyUpdateToNext();
        for(int x = chunks.GetUpperBound(0); x>=0 ; --x)
            for (int y = chunks.GetUpperBound(1); y >= 0; --y)
                chunks[x,y]?.CopyUpdateToNext();
    }

    // Euclidian mod.
    int EucMod(int k, int n)
    {
        if (k < 0)
            return (k % n) + n;
        else
            return k % n;
    }


    public void GenerateUnknownChunkChunkInRange(int2 min, int2 max)
    {
        foreach( (int xChunk, int yChunk) in ChunkRange(min, max))
        {
            if (IsChunkGenerated(xChunk, yChunk) == false)
            {
                Debug.Log($"ev: {xChunk}, {yChunk}");
                EventManager.Instance.RegisterLocalEvent(new SurfaceGenerateMapEvent(surfaceNo, new int2(xChunk, yChunk)));
                return;
            }
        }
    }
    public IEnumerable<(SurfaceObject?, int)> GetRenderingObjectsInChunkRangeItor(int2 min, int2 max)
    {
        foreach((int xChunk, int yChunk) in ChunkRange(min, max))
        {
            if (chunks[xChunk, yChunk] == null || chunks[xChunk, yChunk].createdTime > UpdateManager.RenderingFrame)
                continue;
            foreach (var aa in chunks[xChunk, yChunk].Rendering.Iterator())
            {
                bool isnull = sObjects.SafeGetRendering(aa, out var rendering);

                yield return (isnull ? rendering : (SurfaceObject?)null, aa);
            }
        }
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
    public bool TryPlaceObject(int2 position, int blockType, out int newObjectIndex)
    {
        newObjectIndex = -1;
        SurfaceObject newObject = new SurfaceObject(position, blockType);

        if (CanPlaceObject(newObject) == false)
            return false;

        newObjectIndex = sObjects.Updating.Add(newObject);

        
        foreach( (int xChunk, int yChunk) in ChunkRange( newObject.MinAABBPos, newObject.MaxAABBPos ))
        {
            if (chunks[xChunk , yChunk] == null)
                chunks[xChunk , yChunk] = new BufferedFixedIndexArray<int>(); // UNDO IF CHUNK NOT GENERATED?
            chunks[xChunk, yChunk].Updating.Add(newObjectIndex);
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

    public bool CanPlaceObject(SurfaceObject addObj)
    {
        foreach ((int xChunk, int yChunk) in ChunkRange(addObj.MinAABBPos, addObj.MaxAABBPos))
        {
            if (chunks[xChunk, yChunk] != null)
                if(chunks[xChunk, yChunk].Updating.Exists(obj =>sObjects.GetUpdating(obj).IsCollideWith(addObj)))
                    return false;
        }
        return true;
    }
}