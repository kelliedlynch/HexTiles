using System;
using Microsoft.Xna.Framework;
using HexTiles.Utility;

namespace HexTiles;

public static class GamePieceFactory
{
    private static readonly Random Random = new Random();

    public static GamePiece GenerateRandom(Game game)
    {
        var allTypes = Enum.GetValues(typeof(PieceType));
        var thisType = (PieceType)allTypes.GetValue(Random.Next(allTypes.Length))!;
        var newPiece = new GamePiece(game, Rectangle.Empty);
        newPiece.PieceType = thisType;
        return newPiece;
    }

    public static GamePiece GenerateRandom(Game game, IntVector2 gridPosition)
    {
        var newPiece = GenerateRandom(game);
        newPiece.GridCoords = gridPosition;
        return newPiece;
    }
}