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

public class ChunkManager
{
    public const int chunkSize = 64;
    public int mapWidth;
    public int mapHeight;
    public int xChunkCount;
    public int lastXChunkWidth;



    
    public Vector2 gridSize = new Vector2(1, 1);
    public void PrepareFrame()
    {
        foreach (var a in chunks)
        {
            a?.CopyLast();
        }
    }

    BufferedFixedIndexArray<SurfaceObject>[,] chunks;

    public ChunkManager()
    {
        mapWidth = 4200;
        mapHeight = 1200;
        xChunkCount = mapWidth / chunkSize + 1;
        lastXChunkWidth = mapWidth % chunkSize;
        chunks = new BufferedFixedIndexArray<SurfaceObject>[mapWidth / chunkSize + 1, mapHeight / chunkSize + 1];
        //chunk 0: 0 ~ chunksize -1, 1: chunksize ~ chunksize*2-1
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="type"></param>
    /// <returns>If placing block has succedeed</returns>
    public bool PlaceObject(int2 pos, int type)
    {
        SurfaceObject addObj = new SurfaceObject(pos, type);
        if (CanPlaceObject(addObj) == false)
            return false;


        int maxXChunk = addObj.MaxX / chunkSize;
        if (addObj.MaxX > 4200)
            maxXChunk = (addObj.MaxX - 4200) / chunkSize + xChunkCount;


        for (int xChunk = addObj.MinX / chunkSize; xChunk <= maxXChunk; ++xChunk)
        { 
            for (int yChunk = addObj.MaxY / chunkSize; yChunk <= addObj.MaxY / chunkSize; ++yChunk)
            {
                if (chunks[xChunk % xChunkCount, yChunk] == null)
                    chunks[xChunk % xChunkCount, yChunk] = new BufferedFixedIndexArray<SurfaceObject>();
                chunks[xChunk, yChunk].Current.Add(addObj);
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
            a?.Current.ForEach(action);
        }
    }
    bool CanPlaceObject(SurfaceObject addObj)
    {
        int maxXChunk = addObj.MaxX / chunkSize;
        if (addObj.MaxX > 4200)
            maxXChunk = (addObj.MaxX - 4200) / chunkSize + xChunkCount;

        for (int xChunk = addObj.MinX / chunkSize; xChunk <= maxXChunk; ++xChunk)
        {
            for (int yChunk = addObj.MaxY / chunkSize; yChunk <= addObj.MaxY / chunkSize; ++yChunk)
            {
                if (chunks[xChunk % xChunkCount, yChunk] != null && chunks[xChunk, yChunk].Current.Exists(obj => obj.IsCollideWith(addObj)))
                    return false;
            }
        }
        return true;
    }



}