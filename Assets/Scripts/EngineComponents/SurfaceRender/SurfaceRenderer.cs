using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceRenderer : MonoBehaviour
{
    List<SurfaceGameObject> gameObjects = new List<SurfaceGameObject>();
    Vector3 blockOffset = new Vector3(0, 0, 0);


    public void UpdateObject(SurfaceObject? obj, int index)
    {
        while (gameObjects.Count <= index)
            gameObjects.Add(null);
        if (obj == null)
        {
            if (gameObjects[index] != null)
            {
                Destroy(gameObjects[index]);
            }
            return;
        }

        if (obj.Value.objectType == 0)
            return;

        if (gameObjects[index] != null && gameObjects[index].sObjectType != obj.Value.objectType)
        {
            Destroy(gameObjects[index]);
        }
        if (gameObjects[index] == null)
            gameObjects[index] = Instantiate(SurfaceGameObjectPrefabs.Instance[obj.Value.objectType], transform).GetComponent<SurfaceGameObject>();

        gameObjects[index].transform.localPosition = new Vector3(obj.Value.MidX, obj.Value.MidY, 0);
    }

}
