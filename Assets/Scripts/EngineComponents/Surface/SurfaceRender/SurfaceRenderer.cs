using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using static MathExtension;

public class SurfaceRenderer : MonoBehaviour
{
    List<SurfaceGameObject> gameObjects;
    Vector3 blockOffset = new Vector3(0, 0, 0);

    Transform[,] chunks;
    ChunkData[,] chunkData;
    HashSet<int2> enabledChunks;
    HashSet<int2> updatedChunks;
    struct ChunkData
    {
        public ulong lastUpdatedFrame;
        public bool forceUpdate;
        public SurfaceGameObject[,] blocks;
    }

    public void Init(int xChunkCount, int yChunkCount)
    {
        gameObjects = new List<SurfaceGameObject>();
        InitalizeChunks(xChunkCount, yChunkCount);

        updatedChunks = new HashSet<int2>();
        enabledChunks = new HashSet<int2>();
    }

    private void InitalizeChunks(int xChunkCount, int yChunkCount)
    {
        chunks = new Transform[xChunkCount, yChunkCount];
        chunkData = new ChunkData[xChunkCount, yChunkCount];
        for (int x = 0; x < xChunkCount; x++)
        {
            for (int y = 0; y < yChunkCount; y++)
            {
                GameObject go = new GameObject();
                chunks[x, y] = go.transform;
                chunkData[x, y].blocks = new SurfaceGameObject[SurfaceChunkController.chunkSize, SurfaceChunkController.chunkSize];
                go.name = $"chunk {x}, {y}";
                go.transform.SetParent(this.transform);

                go.SetActive(false);
            }

        }
    }

    public void StartObjectUpdate() {
        updatedChunks.Clear();
    }

    public void DoRender(IEnumerable<(SurfaceObject?, int)> renderObj, IEnumerable<SurfaceObject> renderBlock)
    {
        StartObjectUpdate();
        foreach ((var obj, int idx) in renderObj)
            UpdateObject(obj, idx);
        foreach (var obj in renderBlock)
            UpdateBlock(obj);
        FinishObjectUpdate();
    }

    void ExpandObjectList(int index)
    {
        if (gameObjects.Count <= index)
        {
            gameObjects.Capacity = index + 1;
            while (gameObjects.Count <= index)
                gameObjects.Add(null);
        }
    }
    public void UpdateBlock(SurfaceObject objData)
    {
        var chunkID = new int2(objData.BelongsToChunkX, objData.BelongsToChunkY);
        var chunkLocalX = objData.ChunkLocalPosX;
        var chunkLocalY = objData.ChunkLocalPosY;

        

        SetChunkAsUpdated(chunkID);

        SurfaceGameObject go = chunkData[chunkID.x, chunkID.y].blocks[chunkLocalX, chunkLocalY];

        if (objData.objectType == 0)
        {
            if (go != null)
            {
                Destroy(go);
            }
            return;
        }

        if (go != null && (go.sObjectType != objData.objectType))
        {
            Destroy(go.gameObject);
        }

        if (go == null)
        {
            go = chunkData[chunkID.x, chunkID.y].blocks[chunkLocalX, chunkLocalY] 
                = Instantiate(SurfaceGameObjectPrefabs.Instance[objData.objectType], chunks[objData.BelongsToChunkX, objData.BelongsToChunkY]).GetComponent<SurfaceGameObject>();
            go.name = $"block [{chunkLocalX}, {chunkLocalY}] @chk[{chunkID.x}, {chunkID.y}]";
        }

        if (go.UpdateMe(objData))
        {
            updatedChunks.Add(chunkID);
            //go.transform.SetParent(chunks[objData.Value.BelongsToChunkX, objData.Value.BelongsToChunkY]);
        }



    }
    public void UpdateObject(SurfaceObject? obj, int index)
    {
        ExpandObjectList(index);
        
        if (obj == null || obj.Value.objectType == 0)
        {
            if (gameObjects[index] != null)
            {
                Destroy(gameObjects[index]);
            }
            return;
        }

        SetChunkAsUpdated(new int2(obj.Value.BelongsToChunkX, obj.Value.BelongsToChunkY));

        if (gameObjects[index] != null && (gameObjects[index].sObjectType != obj.Value.objectType))
        {
            Destroy(gameObjects[index].gameObject);
        }

        if (gameObjects[index] == null)
        {
            gameObjects[index] = Instantiate(SurfaceGameObjectPrefabs.Instance[obj.Value.objectType], chunks[obj.Value.BelongsToChunkX, obj.Value.BelongsToChunkY]).GetComponent<SurfaceGameObject>();
            gameObjects[index].name = $"go #{index}";
        }

        if (gameObjects[index].UpdateMe(obj.Value))
        {
            updatedChunks.Add(new int2(obj.Value.BelongsToChunkX, obj.Value.BelongsToChunkY));
            gameObjects[index].transform.SetParent(chunks[obj.Value.BelongsToChunkX, obj.Value.BelongsToChunkY]);
        }



    }

    private void SetChunkAsUpdated(int2 pos)
    {
        if (enabledChunks.Add(pos))
        {
            chunkData[pos.x, pos.y].forceUpdate = true;
            chunks[pos.x, pos.y].gameObject.SetActive(true);
        }
        chunkData[pos.x, pos.y].lastUpdatedFrame = UpdateManager.UpdatingFrameNo;
    }

    public void FinishObjectUpdate()
    {
        enabledChunks.Where(n => chunkData[n.x, n.y].lastUpdatedFrame != UpdateManager.UpdatingFrameNo)
            .ToList().ForEach(n => chunks[n.x, n.y].gameObject.SetActive(false));

        enabledChunks.RemoveWhere(n => chunkData[n.x, n.y].lastUpdatedFrame != UpdateManager.UpdatingFrameNo);
        foreach(var n in enabledChunks)
        {
            chunkData[n.x, n.y].forceUpdate = false;
            chunks[n.x, n.y].SetAsFirstSibling();
        }
    }

}
