using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace HexTiles.Utility;

public static class HexMath
{
    // returns whether Point point is inside hex with bounding Rectangle rect
    public static bool HexContains(Rectangle rect, Point point)
    {
        // shrink rectangle very slightly so hex regions don't overlap
        var xPad = (int)(rect.Width * .02f);
        var yPad = (int)(rect.Height * .02f);
        // rect = rect.GetRelativeRectangle(padding, padding, rect.Width - padding * 2, rect.Height - padding * 2 );
        rect = rect.GetRelativeRectangle(xPad, yPad, rect.Width - xPad * 2, rect.Height - yPad * 2);
        if (!rect.Contains(point)) return false;
        var isInsideHex = true;
        for (int j = 1; j <= 6; j++)
        {
            var newX = (point.X - rect.Center.X) * Math.Cos(60 * j) - (point.Y - rect.Center.Y) * Math.Sin(60 * j) + rect.Center.X;
            var newY = (point.X - rect.Center.X) * Math.Sin(60 * j) + (point.Y - rect.Center.Y) * Math.Cos(60 * j) + rect.Center.Y;
            if (rect.Contains(new Point((int)newX, (int)newY))) continue;
            isInsideHex = false;
            break;
        }
        return isInsideHex;
    }
}