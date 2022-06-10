using UnityEditor;
using UnityEngine;

public struct ItemSlot
{
    public string NameT { get; private set; }
    public ItemInfo InfoRef { get; private set; }
    
    public int SlotIndex { get; private set; }

    public bool AddItem(int i)
    {
        return false;
    }

}