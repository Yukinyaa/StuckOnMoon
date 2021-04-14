using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct SObejctShape
{
    public Vector2Int size;
    public int layer;// bitmask idk , 1(collision),2(building),4(background)
    public Vector2Int[,] shape;// if null, then full rectangle shape
}
public struct SObjectType
{
    public string name;
    public SObejctShape shape;
}

public struct SObjectTypeDataAsset
{
    public BlobArray<SObjectType> objectTypes;
}
