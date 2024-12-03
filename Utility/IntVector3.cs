using System;
using System.Runtime.Intrinsics.X86;
using Microsoft.Xna.Framework;

namespace HexTiles.Utility;

public struct IntVector3 : IEquatable<IntVector3>
{
    public int X;
    public int Y;
    public int Z;
    
    public static IntVector3 Zero => new IntVector3(0, 0, 0);
    public static IntVector3 One => new IntVector3(1, 1, 1);
    public static IntVector3 Two => new IntVector3(2, 2, 2);

    public IntVector2 To2D => new(X, Y);

    public IntVector3(IntVector2 vec, int z) : this()
    {
        SetVars(vec.X, vec.Y, z);
    }
    
    public IntVector3(int x, int y, int z) : this()
    {
        SetVars(x, y, z);
    }
    
    private void SetVars(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public bool Equals(IntVector3 other)
    {
        return _equalsIntVector3(other);
    }

    public override bool Equals(object obj)
    {
        return obj switch
        {
            IntVector3 intVector3 => _equalsIntVector3(intVector3),
            Vector3 xVector3 => _equalsXNAVector3(xVector3),
            _ => false
        };
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X * 10000, Y);
    }

    private bool _equalsIntVector3(IntVector3 obj)
    {
        return obj.X == X && obj.Y == Y && obj.Z == Z;
    }

    private bool _equalsXNAVector3(Vector3 obj)
    {
        var diffX = Math.Abs(obj.X - X);
        var diffY = Math.Abs(obj.Y - Y);
        var diffZ = Math.Abs(obj.Z - Z);
        // NOTE: Adjust this value if you want to change the precision of equality checks.
        return diffX < .00001f && diffY < .00001f && diffZ < .00001f;
    }

    public static bool operator ==(IntVector3 left, object right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(IntVector3 left, object right)
    {
        return !(left == right);
    }
    
    public static IntVector3 operator +(IntVector3 l, IntVector3 r)
    {
        return new IntVector3(l.X + r.X, l.Y + r.Y, l.Z + r.Z);
    }
    
    public static IntVector3 operator *(IntVector3 l, IntVector3 r)
    {
        return new IntVector3(l.X * r.X, l.Y * r.Y, l.Z * r.Z);
    }
    
    public static IntVector3 operator -(IntVector3 l, IntVector3 r)
    {
        return new IntVector3(l.X - r.X, l.Y - r.Y, l.Z - r.Z);
    }
    
    public static IntVector3 operator /(IntVector3 l, IntVector3 r)
    {
        return new IntVector3(l.X / r.X, l.Y / r.Y, l.Z / r.Z);
    }

    // public static implicit operator Point(IntVector3 v)
    // {
    //     return new Point(v.X, v.Y);
    // }

    public static implicit operator Vector3(IntVector3 v)
    {
        return new Vector3(v.X, v.Y, v.Z);
    }
}

public static class IntVector3Extension
{
    public static IntVector3 ToIntVector3(this Vector3 v)
    {
        return new IntVector3((int)v.X, (int)v.Y, (int)v.Z);
    }
    
    // public static IntVector3 ToIntVector3(this Point p)
    // {
    //     return new IntVector3(p.X, p.Y);
    // }
}