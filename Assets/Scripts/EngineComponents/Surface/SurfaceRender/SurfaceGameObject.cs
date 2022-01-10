using System.Collections;
using UnityEngine;

public class SurfaceGameObject : MonoBehaviour
{
    public int sObjectType => data.objectType;
    public SurfaceObject data;
    public SpriteRenderer sprite;
    public bool UpdateMe(SurfaceObject data)
    {
        if (data == this.data)
            return false;
        this.data = data;

        transform.localPosition = new Vector3(data.MidX, data.MidY, 0);
        return true;
    }
}