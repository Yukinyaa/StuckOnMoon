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

public class SurfaceObjectPool
{
    [SerializeField]
    public Vector2 gridSize = new Vector2(1, 1);
    [SerializeField]
    public Vector3 gridOffset;


    // 이거 청크로딩, 트리플버퍼링 해야함 :)
    FixedIndexArray<SurfaceObject> objects = new FixedIndexArray<SurfaceObject>();


    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="type"></param>
    /// <returns>If placing block has succedeed</returns>


    public int TryPlaceObject(int2 pos, int type)
    {
        SurfaceObject addObj = new SurfaceObject(pos, type);
        if (CanPlaceObject(addObj) == false)
            return -1;
        
        return objects.Add(addObj);
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
        objects.ForEach(action);
    }
    bool CanPlaceObject(SurfaceObject addObj)
    {
        //todo: chunk
        if (objects.Exists(obj => obj.IsCollideWith(addObj)))
            return false;
        return true;
    }



}