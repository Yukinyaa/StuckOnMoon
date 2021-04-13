using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct SObejctShape
{
    public Vector2Int size;
    public Vector2Int[,] shape;// if null, then full rectangle shape
}
public struct SObjectType
{
    SObejctShape shape;
}

public struct SObjectListAsset
{
    public BlobArray<SObjectType> types;
}
