using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;

public static class SObejctTypes
{
    public static IReadOnlyList<SObjectType> SObjectTypes;
    static bool isInitalized = false;
    public static void Init()
    {
        if (isInitalized) return;
        isInitalized = true;
        List<SObjectType> tmp = new List<SObjectType>(3);//todo: build from file
        tmp.Add(new SObjectType() { shape = new SObejctShape() { size = new Vector2Int(0, 0), layer = 0b000, shape = null }, name = "none" });
        tmp.Add(new SObjectType() { shape = new SObejctShape() { size = new Vector2Int(1, 1), layer = 0b011, shape = null }, name = "stone" });
        tmp.Add(new SObjectType() { shape = new SObejctShape() { size = new Vector2Int(1, 1), layer = 0b011, shape = null }, name = "dirt" });
        tmp.Add(new SObjectType() { shape = new SObejctShape() { size = new Vector2Int(2, 2), layer = 0b011, shape = null }, name = "bigStuff1" });
        tmp.Add(new SObjectType() { shape = new SObejctShape() { size = new Vector2Int(3, 3), layer = 0b011, shape = null }, name = "bigStuff2" });
        SObjectTypes = tmp.AsReadOnly();
    }

}