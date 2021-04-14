using UnityEngine;

public struct SurfaceObejct
{
    public Vector2Int postion;
    public int objectType;
    public byte status;//is built, enabled is ghost etc
    //derives from these
    public SObejctShape shape;

    public SurfaceObejct(Vector2Int position, int objectType)
    {
        this.postion = position;
        this.objectType = objectType;
        this.status = 0;
        this.shape = SObejctTypes.SObjectTypes[objectType].shape;

        this.factoryObjectID = -1;
    }
    

    #region shapes and positions helper

    public int Width { get => shape.size.x; }
    public int Height { get => shape.size.y; }
    public int MinX { get => postion.x; }
    public int MinY { get => postion.y; }
    public int MaxX { get => postion.x + shape.size.x; }
    public int MaxY { get => postion.y + shape.size.y; }
    
    public bool IsCollideWith(SurfaceObejct other) //basically aabb
    {
        //todo : add shape logic
        if (MaxX < other.MinX || MinX > other.MaxX) return false;
        if (MaxY < other.MinY || MinY > other.MinY) return false;
        //if (a.max.z < b.min.z || a.min.z > b.max.z) return false;
        return true;
    }

    #endregion

    #region system obejects

    // public uint factorySystemID; there is only one factory system.
    public int factoryObjectID;

    #endregion
}
