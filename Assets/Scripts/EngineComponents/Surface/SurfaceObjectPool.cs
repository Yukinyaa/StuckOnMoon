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
    FixedIndexArray<SurfaceObejct> Objects = new FixedIndexArray<SurfaceObejct>();


    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="type"></param>
    /// <returns>If placing block has succedeed</returns>


    bool TryPlaceObject(Vector2Int pos, int type)
    {
        SurfaceObejct addObj = new SurfaceObejct() { postion = pos, objectType = type };
        if (CanPlaceObject(addObj) == false)
            return false;
        
        return true;
    }

    bool CanPlaceObject(Vector2Int pos, int type)
    {
        SurfaceObejct addObj = new SurfaceObejct() { postion = pos, objectType = type };
        if (CanPlaceObject(addObj) == false)
            return false;
        return true;
    }

    bool CanPlaceObject(SurfaceObejct addObj)
    {
        //todo: chunk
        if (Objects.Exists(obj => obj.IsCollideWith(addObj)))
            return false;
        return true;
    }



}