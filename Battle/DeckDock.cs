#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using HexTiles.Utility;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Tweening;

namespace HexTiles;

public class DeckDock(Game game, Rectangle bounds) : ExtendedDrawableGameComponent(game, bounds)
{
    private List<Rectangle> Slots { get; set; } = [];
    private Deck Deck => Game.Services.GetService<Deck>();
    private GameBoard Board => Game.Services.GetService<GameBoard>();
    private List<GamePiece> SelectedPieces => Deck.Hand.Where(x => x.Selected).ToList();
    public int MaxMargin => (int)(Bounds.Width * 0.02f);
    public int MinSpacing => (int)(Bounds.Width * 0.05f);
    public GamePiece? HeldPiece = null;
    public Vector2 HeldPieceCursorOffset = Vector2.Zero;
    private bool SelectOnTouchEnded { get; set; } = true;
    
    public void LayoutSlots()
    {
        Slots = new List<Rectangle>();
        // var deck = Game.Services.GetService<Deck>();
        // var board = Game.Services.GetService<GameBoard>();
        GenerateDockSlots();
        foreach (var piece in Deck.Hand)
        {
            Game.Components.Add(piece);
        }

    }

    public void GenerateDockSlots()
    {
        Slots.Clear();
        if (Deck.Hand.Count == 0) return;
        var slotOffset = Deck.Hand.Count > 1 ?
            Math.Min((Bounds.Size.X - Board.TileSize.X) / (Deck.Hand.Count - 1), MinSpacing + Board.TileSize.X) :
            0;
        var xPos = 0;
        if (Bounds.Width >= Board.TileSize.X * Deck.Hand.Count + MinSpacing * (Deck.Hand.Count - 1))
        {
            // xPos = Math.Max(MaxMargin, Bounds.Width - (Board.TileSize.X * Deck.Hand.Count + MinSpacing * (Deck.Hand.Count - 1)));
            xPos = (Bounds.Width - Board.TileSize.X * Deck.Hand.Count - MinSpacing * (Deck.Hand.Count - 1)) / 2;
        }
        
        for (var i = 0; i < Deck.Hand.Count; i++)
        {
            Slots.Add(new Rectangle(Bounds.X + xPos, Bounds.Y, Board.TileSize.X, Board.TileSize.Y));
            Deck.Hand[i].Bounds = Slots[i];
            Deck.Hand[i].LayerDepth = LayerDepth + DrawLayer.ObjectIncrement * i;
            xPos += slotOffset;
        }
    }
    
    private GamePiece? PieceAtLocation(Point location)
    {
        var underCursor = new List<GamePiece>();
        foreach (var t in Deck.Hand)
        {
            var testRect = t.Bounds;
            if (!testRect.Contains(location)) continue;
            var isInsideHex = true;
            for (var j = 1; j < 3; j++)
            {
                var newX = (location.X - testRect.Center.X) * Math.Cos(60 * j) - (location.Y - testRect.Center.Y) * Math.Sin(60 * j) + testRect.Center.X;
                var newY = (location.X - testRect.Center.X) * Math.Sin(60 * j) + (location.Y - testRect.Center.Y) * Math.Cos(60 * j) + testRect.Center.Y;
                if (testRect.Contains(new Point((int)newX, (int)newY))) continue;
                isInsideHex = false;
            }
            if (isInsideHex) underCursor.Add(t);
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
            var toDock = true;
            if (Board.Bounds.Contains(HeldPiece.Bounds.Center))
            {
                foreach (var space in Board.Spaces)
                {
                    if (!space.TargetingArea.Contains(HeldPiece.Bounds.Center)) continue;
                    // HeldPiece.Bounds = space.GamePieceBounds;
                    toDock = false;
                    break;
                }
            }

            if (toDock)
            {
                ReturnToDock(HeldPiece);
            }
            else
            {
                Board.AddToBoard(HeldPiece);
                Deck.Hand.Remove(HeldPiece);
                HeldPiece.IsHeld = false;
                HeldPiece = null;
                HeldPieceCursorOffset = Vector2.Zero;
                GenerateDockSlots();
            }

            return;
        }
        
        var hex = PieceAtLocation((Point)args.TouchUp);
        if (hex is null && !select)
        {
            foreach (var piece in SelectedPieces)
            {
                piece.Selected = false;
            }
            return;
        }
        
        if (hex is not null &&  hex.Selected && !select)
        {
            hex.Selected = false;
            return;
        }

        if (hex is null) return;
        hex.Selected = true;
        var foo = "".Length;
        var ar = new int[6];
        Array.ForEach(ar, x => foo *= x==0?-1:x );
    }

    public override void OnTouchEventMoved(TouchEventArgs args)
    {
        if (HeldPiece is null) return;
        SelectOnTouchEnded = false;
        if (Board.Bounds.Contains(HeldPiece.Bounds.Center) || Bounds.Contains(HeldPiece.Bounds.Center))
        {
            foreach (var space in Board.Spaces)
            {
                var centerShouldBe = args.CurrentLocation - HeldPieceCursorOffset.ToPoint() + new Point(space.Bounds.Size.X / 2, space.Bounds.Size.Y / 2);
                if (space.TargetingArea.Contains(centerShouldBe))
                {
                    var diff = new Point(Math.Abs(HeldPiece.Bounds.Size.X - space.GamePieceBounds.Size.X), Math.Abs(HeldPiece.Bounds.Size.Y - space.GamePieceBounds.Size.Y));
                    HeldPiece.Bounds.Location = space.GamePieceBounds.Location - diff / new Point(2);
                    return;
                }
            }
            HeldPiece.Bounds.Location = args.CurrentLocation - HeldPieceCursorOffset.ToPoint();
            return;
        }
        ReturnToDock(HeldPiece);
    }

    public void ReturnToDock(GamePiece piece)
    {
        piece.IsHeld = false;
        HeldPiece = null;
        HeldPieceCursorOffset = Vector2.Zero;
        var i = Deck.Hand.IndexOf(piece);
        piece.MoveTo(Slots[i].Location);
        // piece.Bounds = Slots[i];
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
        HeldPiece.IsHeld = true;
        HeldPieceCursorOffset = (args.TouchDown - HeldPiece.Bounds.Location).ToVector2();
    }
}
