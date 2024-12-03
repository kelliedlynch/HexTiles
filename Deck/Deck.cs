using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace HexTiles;

public class Deck(Game game) : GameComponent(game)
{
    public List<GamePiece> Pieces = new();

    public void GenerateDeck()
    {
        for (int i = 0; i < 10; i++)
        {
            Pieces.Add(GamePieceFactory.GenerateRandom(Game));
        }
    }
}