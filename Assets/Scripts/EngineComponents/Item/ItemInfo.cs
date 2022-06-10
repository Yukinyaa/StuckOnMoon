using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Items
{
    public List<ItemInfo> items = new List<ItemInfo>{
        new ItemInfo{ id = 0, name = "tlqkf",  nameT = "item-tlqkf", descriptionT = "item-desc-tlqkf", maxStack = 100},
        new ItemInfo{ id = 1, name = "qudtls",  nameT = "item-qudtls", descriptionT = "item-desc-qudtlsf", maxStack = 50}
    };
}
public class ItemInfo
{
    public int id;
    public string name;
    public string nameT;
    public string descriptionT;
    public int maxStack;
}
