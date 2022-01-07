using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System.Runtime.InteropServices;

public class SurfaceChunk
{
    public const int chunkSize = 16;
    FixedIndexArray<int> sobjIDs;
    public IEnumerable<int> GetObjectItor() => sobjIDs.Iterator();
    int2 gridOffset;
    public bool IsGenerated { get; private set; } = false;
    public void MarkAsGenertated() => IsGenerated = true;

    int[,] blocks;
    public IEnumerable<SurfaceObject> GetBlocksItor()
    { 
        for( int x = 0; x < chunkSize; x++ )
        {
            for (int y = 0; y < chunkSize; y++)
            { 
                if(blocks[x,y] != 0)
                    yield return new SurfaceObject(new int2( x + gridOffset.x, y + gridOffset.y), blocks[x,y] );
            }
        }
    }

    public SurfaceChunk(int xChunkPos, int yChunkPos)
    {
        blocks = new int[chunkSize, chunkSize];
        gridOffset = new int2(xChunkPos * chunkSize, yChunkPos * chunkSize);
        sobjIDs = new FixedIndexArray<int>();
        IsGenerated = false;
    }
    public void RegisterBlock(SurfaceObject obj)
    {
        Debug.Assert(obj.shape == SObejctShape.Block);

        obj.postion -= gridOffset;
        blocks[obj.postion.x, obj.postion.y] = obj.objectType;
    }
    public void RemoveBlock(SurfaceObject obj)
    {
        Debug.Assert(obj.shape == SObejctShape.Block);
        Debug.Assert(obj.objectType == blocks[obj.postion.x, obj.postion.y]);

        blocks[obj.postion.x, obj.postion.y] = 0;
        obj.postion -= gridOffset;   
    }
    public void RegisterObject(int objID)
    {
        sobjIDs.Add(objID);
    }
    public void RemoveObject(int objID)
    {
        sobjIDs.Remove(objID);
    }

    internal void CopyTo(SurfaceChunk surfaceChunk)
    {
        //blocks.CopyTo(surfaceChunk.blocks, 0);
        System.Buffer.BlockCopy(blocks, 0, surfaceChunk.blocks, 0, sizeof(int) * chunkSize * chunkSize);
        sobjIDs.CopyTo(surfaceChunk.sobjIDs);
        surfaceChunk.IsGenerated = IsGenerated;
    }
}
