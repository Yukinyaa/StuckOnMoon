using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Collections;


public static class BlockIDConsts
{
    public const int blank = 0;
}

class SurfaceObjectPool
{
    [SerializeField]
    public Vector2 gridSize = new Vector2(1, 1);
    [SerializeField]
    public Vector3 gridOffset;


    // 이거 청크로딩, 트리플버퍼링 해야함 :)
    Dictionary<int, SurfaceObejct> Objects = new Dictionary<int, SurfaceObejct>();


    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="id"></param>
    /// <returns>If placing block has succedeed</returns>


    bool TryPlaceObject(Vector2Int pos, int id)
    {
        SurfaceObejct addObj = new SurfaceObejct() { postion = pos, objectType = id };
        if (CanPlaceObject(in addObj) == false)
            return false;
        
        return true;
    }

    bool CanPlaceObject(Vector2Int pos, int id)
    {
        SurfaceObejct addObj = new SurfaceObejct() { postion = pos, objectType = id };
        if (CanPlaceObject(in addObj) == false)
            return false;
        return true;
    }

    bool CanPlaceObject(in SurfaceObejct addObj)
    {

        foreach (var o in Objects)//todo: chunk
        {
            if (o.Value.IsCollideWith(addObj))
                return false;
        }
        return true;
    }



}