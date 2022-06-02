using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public static class SObjectTypes
{
    public static IReadOnlyList<SObjectType> sObjectTypes { get; private set; }
    static bool isInitalized = false;
    public static void Init()
    {
        if (isInitalized) return;
        isInitalized = true;
        List<SObjectType> tmp = new List<SObjectType>(3);//todo: build from file 
        tmp.Add(new SObjectType() { shape = new SObejctShape() { size = new int2(0, 0), layer = Layer.None }, name = "none" });
        tmp.Add(new SObjectType() { shape = new SObejctShape() { size = new int2(1, 1), layer = Layer.Block }, name = "stone" });
        tmp.Add(new SObjectType() { shape = new SObejctShape() { size = new int2(1, 1), layer = Layer.BackgroundOnly }, name = "white" }); 
        tmp.Add(new SObjectType() { shape = new SObejctShape() { size = new int2(2, 2), layer = Layer.SpriteOnly }, name = "chest" });
        tmp.Add(new SObjectType() { shape = new SObejctShape() { size = new int2(3, 3), layer = Layer.SpriteOnly }, name = "" });
        sObjectTypes = tmp.AsReadOnly();
    }

}