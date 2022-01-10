using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using static MathExtension;
public class SurfaceChunkController
{
    public const int chunkSize = 16;
    PaddedRefBuffer<SurfaceChunk[,]> chunks;
    SurfaceInfo surfaceInfo;
    
    public int lastXChunkWidth;
    public int CountX { get; private set; }
    public int CountY { get; private set; }

    static public int XChunkCountFromSurfaceInfo(SurfaceInfo surfaceInfo)
    {
        return surfaceInfo.width / chunkSize + 1;
    }
    static public int YChunkCountFromSurfaceInfo(SurfaceInfo surfaceInfo)
    {
        return surfaceInfo.height / chunkSize + 1;
    }
    public SurfaceChunkController(SurfaceInfo surfaceInfo)
    {
        this.surfaceInfo = surfaceInfo;
        CountX = XChunkCountFromSurfaceInfo(surfaceInfo);
        CountY = YChunkCountFromSurfaceInfo(surfaceInfo);
        lastXChunkWidth = surfaceInfo.width % chunkSize;
        chunks = new PaddedRefBuffer<SurfaceChunk[,]>();
        for (int i = 0; i < UpdateManager.BufferCount; i++)
            chunks[i] = new SurfaceChunk[CountX, CountY];
    }
    public void CopyUpdateToNext()
    {
        for (int x = 0; x < CountX; x++)
        {
            for (int y = 0; y < CountY; y++)
            {
                if (chunks.Updating[x, y] == null) continue;
                if (chunks.Next[x, y] == null)
                    chunks.Next[x, y] = new SurfaceChunk(x, y);

                chunks.Updating[x, y].CopyTo(chunks.Next[x, y]);
            }
        }
    }

    public IEnumerable<int> GetObjectsInRange(int2 min, int2 max)
    {
        foreach ((int xChunk, int yChunk) in ChunkRange(min, max))
        {
            if (chunks.Updating[xChunk, yChunk] != null)
            {
                foreach (int oID in chunks.Updating[xChunk, yChunk].GetObjectItor())
                { 
                    yield return oID;
                }
            }
        }
    }
    public IEnumerable<int> GetRenderingObjInRange(int2 min, int2 max)
    {
        foreach ((int xChunk, int yChunk) in ChunkRange(min, max))
        {
            if (chunks.Rendering[xChunk, yChunk] != null)
            {
                foreach (int oID in chunks.Updating[xChunk, yChunk].GetObjectItor())
                {
                    yield return oID;
                }
            }
        }
    }

    public IEnumerable<SurfaceObject> GetBlockInRange(int2 min, int2 max)
    {
        foreach ((int xChunk, int yChunk) in ChunkRange(min, max))
        {
            if (chunks.Updating[xChunk, yChunk] != null)
            {
                foreach (var block in chunks.Updating[xChunk, yChunk].GetBlocksItor())
                {
                    yield return block;
                }
            }
        }
    }

    public IEnumerable<SurfaceObject> GetRenderingBlockInRange(int2 min, int2 max)
    {
        foreach ((int xChunk, int yChunk) in ChunkRange(min, max))
        {
            if (chunks.Updating[xChunk, yChunk] != null)
            {
                foreach (var block in chunks.Rendering[xChunk, yChunk].GetBlocksItor())
                {
                    yield return block;
                }
            }
        }
    }
    public void RegisterObject(SurfaceObject o, int oID)
    {
        chunks.Updating[o.BelongsToChunkX, o.BelongsToChunkY].RegisterObject(oID);
    }
    public void RegisterBlock(SurfaceObject o)
    {
        chunks.Updating[o.BelongsToChunkX, o.BelongsToChunkY].RegisterBlock(o);
    }
    public IEnumerable<(int, int)> ChunkRange(int2 min, int2 max)
    {
        ArgumentVectorPair(ref min, ref max);

        int maxXChunk = (max.x - 1) / chunkSize;
        int maxYChunk = (max.y - 1) / chunkSize;

        for (int xChunk = min.x / chunkSize; xChunk <= maxXChunk; ++xChunk)
        {
            for (int yChunk = min.y / chunkSize; yChunk <= maxYChunk; ++yChunk)
            {
                yield return (xChunk % CountX, yChunk);
            }
        }
    }

    internal void MarkChunkAsGenerated(int xChunkPos, int yChunkPos, bool forceReset = false)
    {
        if (chunks.Updating[xChunkPos, yChunkPos] == null || forceReset)
            chunks.Updating[xChunkPos, yChunkPos] = new SurfaceChunk(xChunkPos, yChunkPos);
        chunks.Updating[xChunkPos, yChunkPos].MarkAsGenertated();
    }


    internal bool IsChunkGenerated(int xChunkPos, int yChunkPos)
    {
        if (chunks.Updating[xChunkPos, yChunkPos] == null) return false;
        return chunks.Updating[xChunkPos, yChunkPos].IsGenerated;
    }

    public bool ChunkExists(int xChunkPos, int yChunkPos)
    {
        if (chunks.Updating[xChunkPos, yChunkPos] == null)
            return false;
        else return true;
    }

    /// <summary>
    /// input: min/max pair of arbiraty points
    /// modify to match argmin>argmax, argmin/argmax.x > 0
    /// </summary>
    public void ArgumentVectorPair(ref int2 argMin, ref int2 argMax)
    {
        argMin.x = EucMod(argMin.x, surfaceInfo.width);
        argMax.x = EucMod(argMax.x, surfaceInfo.width);
        argMin.y = Mathf.Max(argMin.y, 0);
        argMax.y = Mathf.Max(argMax.y, 0);
        if (argMin.x > argMax.x)
            argMax.x += surfaceInfo.width;
    }
}
