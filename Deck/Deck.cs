using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;

namespace HexTiles;

public class Deck(Game game) : GameComponent(game)
{
    private readonly Random _random = new();
    public List<GamePiece> Pieces = new();
    public List<GamePiece> DrawPile = new();
    public List<GamePiece> Hand = new();

    public void GenerateDeck()
    {
        for (int i = 0; i < 20; i++)
        {
            Pieces.Add(GamePieceFactory.GenerateRandom(Game));
        }
        
        DrawPile.AddRange(Pieces);
    }
    
    

    public void DrawHand()
    {
        DrawPile.Shuffle(_random);
        Hand.AddRange(DrawPile[..7]);
        DrawPile.RemoveRange(0, 7);
    }
}