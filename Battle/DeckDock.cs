#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using HexTiles.Utility;
using Microsoft.Xna.Framework;

namespace HexTiles;

public class DeckDock(Game game, Rectangle bounds) : ExtendedDrawableGameComponent(game, bounds)
{
    public List<Rectangle> Slots;
    public float MinSpacing = 0.05f;

    public void LayoutSlots()
    {
        Slots = new List<Rectangle>();
        var deck = Game.Services.GetService<Deck>();
        var board = Game.Services.GetService<GameBoard>();
        var slotOffset = Math.Max((board.Bounds.Size.X - board.TileSize.X) / (deck.Hand.Count - 1), (int)(board.Bounds.Size.Y * MinSpacing));
        var xPos = 0;
        for (int i = 0; i < deck.Hand.Count; i++)
        {
            Slots.Add(new Rectangle(Bounds.X + xPos, Bounds.Y, board.TileSize.X, board.TileSize.Y));
            deck.Hand[i].Bounds = Slots[i];
            deck.Hand[i].LayerDepth = LayerDepth + DrawLayer.ObjectIncrement * i;
            Game.Components.Add(deck.Hand[i]);
            xPos += slotOffset;
        }
        
    }

    private GamePiece? PieceAtLocation(Point location)
    {
        var deck = Game.Services.GetService<Deck>();
        var underCursor = new List<GamePiece>();
        for (int i = 0; i < Slots.Count; i++)
        {
            if (!Slots[i].Contains(location)) continue;
            var isInsideHex = true;
            for (int j = 1; j < 3; j++)
            {
                var newX = (location.X - Slots[i].Center.X) * Math.Cos(60 * j) - (location.Y - Slots[i].Center.Y) * Math.Sin(60 * j) + Slots[i].Center.X;
                var newY = (location.X - Slots[i].Center.X) * Math.Sin(60 * j) + (location.Y - Slots[i].Center.Y) * Math.Cos(60 * j) + Slots[i].Center.Y;
                if (Slots[i].Contains(new Point((int)newX, (int)newY))) continue;
                isInsideHex = false;
            }
            if (isInsideHex) underCursor.Add(deck.Hand[i]);

        }
        return underCursor.Count > 0 ? underCursor.Last() : null;
    }

    public override void OnTouchEventEnded(TouchEventArgs args)
    {
        if (args.TouchUp is null) return;
        var hex = PieceAtLocation((Point)args.TouchUp);
        if (hex is null) return;
        hex.Selected = true;
    }
}