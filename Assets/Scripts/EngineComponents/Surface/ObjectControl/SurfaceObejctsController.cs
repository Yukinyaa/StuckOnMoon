using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
public struct SurfaceInfo
{
    public int surfaceNo;

    public int width;
    public int height;
    public string name;
}

public class SurfaceObejctsController
{
    public float2 gridSize = new float2(0.5f, 0.5f);

    BufferedFixedIndexArray<SurfaceObject> sObjects;
    SurfaceChunkController chunkController;
    SurfaceInfo surfaceInfo;
    public SurfaceInfo GetSurfaceInfo => surfaceInfo;

    public bool IsChunkGenerated(int chunkPosx, int chunkPosy) => chunkController.IsChunkGenerated(chunkPosx, chunkPosy);
    public void MarkMapGenerated(int x, int y) => chunkController.MarkChunkAsGenerated(x, y);



    public bool IsViewed { get; private set; }

    public SurfaceObejctsController(SurfaceInfo surfaceinfo)
    {
        this.surfaceInfo = surfaceinfo;

        chunkController = new SurfaceChunkController(surfaceInfo);
        sObjects = new BufferedFixedIndexArray<SurfaceObject>();
        //chunk 0: 0 ~ chunksize -1, chunk 1: chunksize ~ chunksize*2-1
    }

    public void PrepareNextFrame()
    {
        sObjects.CopyUpdateToNext();

        chunkController.CopyUpdateToNext();
    }



    public void GenerateUnknownChunkChunkInRange(int2 min, int2 max)
    {
        foreach( (int xChunk, int yChunk) in chunkController.ChunkRange(min, max))
        {
            if (chunkController.IsChunkGenerated(xChunk, yChunk) == false)
            {
                Debug.Log($"ev: {xChunk}, {yChunk}");
                EventManager.Instance.RegisterLocalEvent(new SurfaceGenerateMapEvent(surfaceInfo.surfaceNo, new int2(xChunk, yChunk)));
                return;
            }
        }
    }
    public IEnumerable<(SurfaceObject?, int)> GetRenderingObjectsInChunkRangeItor(int2 min, int2 max)
    {
        foreach (int oID in chunkController.GetRenderingObjInRange(min, max))
        {
            bool isSucess = sObjects.SafeGetRendering(oID, out var rendering);
            yield return (isSucess ? rendering : (SurfaceObject?)null, oID);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="type"></param>
    /// <returns>If placing block has succedeed</returns>
    public bool TryPlaceObject(int2 position, int blockType, out int newObjectID)
    {
        newObjectID = -1;
        SurfaceObject newObject = new SurfaceObject(position, blockType);

        if (CanPlaceObject(newObject) == false)
            return false;

        newObjectID = sObjects.Updating.Add(newObject);
        chunkController.RegisterObject(newObject, newObjectID);
        
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
        if (chunkController.GetBlockInRange(addObj.MinAABBPos, addObj.MaxAABBPos).Any(x => x.IsCollideWith(addObj)))
            return false;
        if (chunkController.GetObjectsInRange(addObj.MinAABBPos, addObj.MaxAABBPos).Any(oID => sObjects.GetUpdating(oID).IsCollideWith(addObj)))
            return false;
        return true;
    }

}