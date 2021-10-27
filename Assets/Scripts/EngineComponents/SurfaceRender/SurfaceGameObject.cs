using System.Collections;
using UnityEngine;

public class SurfaceGameObject : MonoBehaviour
{
    public int sObjectType => data.objectType;
    public SurfaceObject data;
    public SpriteRenderer sprite;
    public void UpdateMe(SurfaceObject data)
    {
        if (data == this.data)
            return;

        transform.localPosition = new Vector3(data.MidX, data.MidY, 0);
    }
}