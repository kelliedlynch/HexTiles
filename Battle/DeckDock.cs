#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using HexTiles.Utility;
using Microsoft.Xna.Framework;

namespace HexTiles;

public class DeckDock(Game game, Rectangle bounds) : ExtendedDrawableGameComponent(game, bounds)
{
    private List<Rectangle> Slots { get; set; } = [];
    private Deck Deck => Game.Services.GetService<Deck>();
    private GameBoard Board => Game.Services.GetService<GameBoard>();
    private List<GamePiece> SelectedPieces => Deck.Hand.Where(x => x.Selected).ToList();
    public float MinSpacing = 0.05f;
    public GamePiece? HeldPiece = null;
    public Vector2 HeldPieceCursorOffset = Vector2.Zero;
    // private TouchUpAction TouchUpAction { get; set; } = TouchUpAction.Select;
    private bool SelectOnTouchEnded { get; set; } = true;
    
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
        var underCursor = new List<GamePiece>();
        for (int i = 0; i < Deck.Hand.Count; i++)
        {
            var testRect = Deck.Hand[i].Bounds;
            if (!testRect.Contains(location)) continue;
            var isInsideHex = true;
            for (int j = 1; j < 3; j++)
            {
                var newX = (location.X - testRect.Center.X) * Math.Cos(60 * j) - (location.Y - testRect.Center.Y) * Math.Sin(60 * j) + testRect.Center.X;
                var newY = (location.X - testRect.Center.X) * Math.Sin(60 * j) + (location.Y - testRect.Center.Y) * Math.Cos(60 * j) + testRect.Center.Y;
                if (testRect.Contains(new Point((int)newX, (int)newY))) continue;
                isInsideHex = false;
            }
            if (isInsideHex) underCursor.Add(Deck.Hand[i]);

        }
        return underCursor.Count > 0 ? underCursor.Last() : null;
    }


    public override void Update(GameTime gameTime)
    {
        if (HeldPiece != null)
        {
            
        }
        base.Update(gameTime);
    }

    public override void OnTouchEventEnded(TouchEventArgs args)
    {
        var select = SelectOnTouchEnded;
        SelectOnTouchEnded = true;
        if (args.TouchUp is null) return;
        if (HeldPiece is not null)
        {
            HeldPiece.Selected = select;
            HeldPiece = null;
            HeldPieceCursorOffset = Vector2.Zero;
            return;
        }
        
        var hex = PieceAtLocation((Point)args.TouchUp);
        if (hex is null && select)
        {
            foreach (var piece in SelectedPieces)
            {
                piece.Selected = false;
            }
            return;
        }
        
        if (hex!.Selected && !select)
        {
            hex.Selected = false;
            return;
        }
        
        hex.Selected = true;
    }

    public override void OnTouchEventMoved(TouchEventArgs args)
    {
        if (HeldPiece is null) return;
        SelectOnTouchEnded = false;
        HeldPiece.Bounds.Location = args.CurrentLocation - HeldPieceCursorOffset.ToPoint();
    }

    public override void OnTouchEventBegan(TouchEventArgs args)
    {
        var hex = PieceAtLocation(args.TouchDown);
        if (hex is null || hex.Selected)
        {
            SelectOnTouchEnded = false;
        }
        if (hex is null) return;
        hex.Selected = true;
        HeldPiece = hex;
        HeldPieceCursorOffset = (args.TouchDown - HeldPiece.Bounds.Location).ToVector2();
    }
}
