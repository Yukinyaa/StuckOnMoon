using System;
using Unity.Mathematics;



[Flags]
public enum Layer
{
    None = 0,
    CollisionOnly = 1,
    SpriteOnly = 2,
    Block = 3,
    BackgroundOnly = 4,
    All = 0b11111111
}

public struct SObejctShape
{
    public int2 size;
    public Layer layer;// bitmask idk , 1(collision),2(building),4(background)

    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        if (!(obj is SObejctShape))
            return false;
        SObejctShape other = (SObejctShape)obj;

        if (size.x != other.size.x) return false;
        if (size.y != other.size.y) return false;
        if (layer != other.layer) return false;

        return true;
    }

    public bool Equals(SObejctShape other)
    {
        if (size.x != other.size.x) return false;
        if (size.y != other.size.y) return false;
        if (layer != other.layer) return false;

        return true;
    }
    static public bool operator ==(SObejctShape a, SObejctShape b)
    {
        return a.Equals(b);
    }
    static public bool operator !=(SObejctShape a, SObejctShape b)
    {
        return !a.Equals(b);
    }

    static public SObejctShape Block => new SObejctShape() { size = new int2(1, 1), layer = Layer.Block };
    static public SObejctShape Background => new SObejctShape() { size = new int2(1, 1), layer = Layer.BackgroundOnly };
}
