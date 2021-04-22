using Unity.Mathematics;

//used by chunk
public struct SObejctShape
{
    public int2 size;
    public int layer;// bitmask idk , 1(collision),2(building),4(background)
    //public Vector2Int[,] shape;// if null, then full rectangle shape
}
