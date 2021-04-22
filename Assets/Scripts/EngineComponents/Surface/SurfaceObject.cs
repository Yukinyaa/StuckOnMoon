using Unity.Mathematics;
using UnityEngine;


public struct SurfaceObject
{
    public int2 postion;
    public int objectType;
    public byte status;//is built, enabled is ghost etc
    //derives from these
    public SObejctShape shape;

    public SurfaceObject(int2 position, int objectType)
    {
        this.postion = position;
        this.objectType = objectType;
        this.status = 0;
        this.shape = SObjectTypes.sObjectTypes[objectType].shape;

        this.factoryObjectID = -1;
    }

    public override string ToString()
    {
        return $"{postion.x}, {postion.y}: {SObjectTypes.sObjectTypes[objectType].name}";
    }

    #region shapes and positions helper

    public int Width { get => shape.size.x; }
    public int Height { get => shape.size.y; }
    public int MinX { get => postion.x; }
    public int MinY { get => postion.y; }
    public int MaxX { get => postion.x + shape.size.x; }
    public int MaxY { get => postion.y + shape.size.y; }

    public float MidX { get => postion.x + shape.size.x/2f; }
    public float MidY { get => postion.y + shape.size.y/2f; }
    public Vector2 Middle { get => new Vector2(MidX, MidY); }

    public bool IsCollideWith(SurfaceObject other) //basically aabb
    {
        if ((this.shape.layer & other.shape.layer) == 0)
        {
            return false;
        }
        //todo : add shape logic
        if (MaxX <= other.MinX || MinX >= other.MaxX) return false;
        if (MaxY <= other.MinY || MinY >= other.MaxY) return false;
        return true;
    }
    public bool IsCollideWith(int2 position, SObejctShape other) //basically aabb
    {
        //todo : add shape logic
        if (MaxX <= position.x || MinX >= position.x + other.size.x) return false;
        if (MaxY <= position.y || MinY >= position.y + other.size.y) return false;
        return true;
    }

    #endregion

    #region system obejects

    // public uint factorySystemID; there is only one factory system.
    public int factoryObjectID;

    #endregion
}
