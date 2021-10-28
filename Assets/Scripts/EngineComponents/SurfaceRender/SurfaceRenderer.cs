using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

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
                go.name = $"chunk {x}, {y}";
                go.transform.SetParent(this.transform);

                go.SetActive(false);
            }

        }
    }

    public void StartObjectUpdate() {
        updatedChunks.Clear();
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
    public void UpdateObject(SurfaceObject? obj, int index)
    {
        ExpandObjectList(index);

        if (enabledChunks.Add(new int2(obj.Value.BelongsToChunkX, obj.Value.BelongsToChunkY)))
        {
            chunkData[obj.Value.BelongsToChunkX, obj.Value.BelongsToChunkY].forceUpdate = true;
            chunks[obj.Value.BelongsToChunkX, obj.Value.BelongsToChunkY].gameObject.SetActive(true);
        }
        chunkData[obj.Value.BelongsToChunkX, obj.Value.BelongsToChunkY].lastUpdatedFrame = UpdateManager.UpdatingFrameNo;

        if (obj == null || obj.Value.objectType == 0)
        {
            if (gameObjects[index] != null)
            {
                Destroy(gameObjects[index]);
            }
            return;
        }



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
