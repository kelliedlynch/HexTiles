using System;
using System.Runtime.Intrinsics.X86;
using Microsoft.Xna.Framework;

namespace HexTiles.Utility;

public struct IntVector4 : IEquatable<IntVector4>
{
    public int X;
    public int Y;
    public int Z;
    public int W;

    public static IntVector4 Zero => new IntVector4(0, 0, 0, 0);
    public static IntVector4 One => new IntVector4(1, 1, 1, 1);
    public static IntVector4 Two => new IntVector4(2, 2, 2, 2);
    
    public IntVector2 To2D => new(X, Y);

    public IntVector3 To3D => new(X, Y, Z);

    public IntVector4(IntVector2 vec, int z, int w) : this()
    {
        SetVars(vec.X, vec.Y, z, w);
    }
    
    public IntVector4(IntVector3 vec, int w) : this()
    {
        SetVars(vec.X, vec.Y, vec.Z, w);
    }
    
    public IntVector4(int x, int y, int z, int w) : this()
    {
        SetVars(x, y, z, w);
    }
    
    private void SetVars(int x, int y, int z, int w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }

    public bool Equals(IntVector4 other)
    {
        return _equalsIntVector4(other);
    }

    public override bool Equals(object obj)
    {
        if (obj is IntVector4)
        {
            return _equalsIntVector4((IntVector4)obj);
        }

        if (obj is Vector4)
        {
            return _equalsXNAVector4((Vector4)obj);
        }

        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X * 10000, Y);
    }

    private bool _equalsIntVector4(IntVector4 obj)
    {
        return obj.X == X && obj.Y == Y && obj.Z == Z && obj.W == W;
    }

    private bool _equalsXNAVector4(Vector4 obj)
    {
        var diffX = Math.Abs(obj.X - X);
        var diffY = Math.Abs(obj.Y - Y);
        var diffZ = Math.Abs(obj.Z - Z);
        var diffW = Math.Abs(obj.W - W);
        // NOTE: Adjust this value if you want to change the precision of equality checks.
        var maxDiff = .00001f;
        return diffX < maxDiff && diffY < maxDiff && diffZ < maxDiff && diffW < maxDiff;
    }

    public static bool operator ==(IntVector4 left, object right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(IntVector4 left, object right)
    {
        return !(left == right);
    }
    
    public static IntVector4 operator +(IntVector4 l, IntVector4 r)
    {
        return new IntVector4(l.X + r.X, l.Y + r.Y, l.Z + r.Z, l.W + r.W);
    }
    
    public static IntVector4 operator *(IntVector4 l, IntVector4 r)
    {
        return new IntVector4(l.X * r.X, l.Y * r.Y, l.Z * r.Z, l.W * r.W);
    }
    
    public static IntVector4 operator -(IntVector4 l, IntVector4 r)
    {
        return new IntVector4(l.X - r.X, l.Y - r.Y, l.Z - r.Z, l.W - r.W);
    }
    
    public static IntVector4 operator /(IntVector4 l, IntVector4 r)
    {
        return new IntVector4(l.X / r.X, l.Y / r.Y, l.Z / r.Z, l.W / r.W);
    }

    // public static implicit operator Point(IntVector3 v)
    // {
    //     return new Point(v.X, v.Y);
    // }

    public static implicit operator Vector4(IntVector4 v)
    {
        return new Vector4(v.X, v.Y, v.Z, v.W);
    }
}

public static class IntVector4Extension
{
    public static IntVector4 ToIntVector4(this Vector4 v)
    {
        return new IntVector4((int)v.X, (int)v.Y, (int)v.Z, (int)v.W);
    }
    
    // public static IntVector3 ToIntVector3(this Point p)
    // {
    //     return new IntVector3(p.X, p.Y);
    // }
}