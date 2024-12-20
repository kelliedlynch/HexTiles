using System;
using Microsoft.Xna.Framework;

namespace HexTiles.Utility;

public static class HexMath
{
    // returns whether Point point is inside hex with bounding Rectangle rect
    public static bool HexContains(Rectangle rect, Point point)
    {
        if (!rect.Contains(point)) return false;
        var isInsideHex = true;
        for (int j = 1; j < 3; j++)
        {
            var newX = (point.X - rect.Center.X) * Math.Cos(60 * j) - (point.Y - rect.Center.Y) * Math.Sin(60 * j) + rect.Center.X;
            var newY = (point.X - rect.Center.X) * Math.Sin(60 * j) + (point.Y - rect.Center.Y) * Math.Cos(60 * j) + rect.Center.Y;
            if (rect.Contains(new Point((int)newX, (int)newY))) continue;
            isInsideHex = false;
        }
        return isInsideHex;
    }
}