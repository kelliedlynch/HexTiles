#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using HexTiles.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Collections;

namespace HexTiles;

public class GameBoard(Game game, Rectangle bounds) : ExtendedDrawableGameComponent(game, bounds)
{
    public readonly int Rows = 5;
    public readonly int Columns = 5;

    public readonly int Spacing = 10;
    private GamePiece? SelectedPiece {get; set;}
    private readonly Bag<GamePiece> _movingPieces = [];
    public GamePiece[,] GamePieces;

    public GameBoardSpace[,] Spaces;

    public IntVector2 TileSize;
    private readonly Random _random = new();

    // private event Action CascadeMatches;
    // public event Action<Bag<GamePiece>> MatchActivated;
    
    
    public override void Initialize()
    {
        GamePieces = new GamePiece[Columns, Rows];
        var man = Game.Services.GetService<InputManager>();
        man.RegisterComponent(this, new List<InputListeners>() { InputListeners.TouchEnded });
        Spaces = GameBoardFactory.GenerateGrid(this);
        foreach (var space in Spaces)
        {
            Game.Components.Add(space);
        }
    }

    private void SwapPieces(GamePiece piece1, GamePiece piece2)
    {
        SwapPositions(piece1, piece2);
        MovePieceTo(piece1, Spaces[piece1.GridCoords.X, piece1.GridCoords.Y].GamePieceBounds.Location);
        MovePieceTo(piece2, Spaces[piece2.GridCoords.X, piece2.GridCoords.Y].GamePieceBounds.Location);
    }

    private void SwapPositions(GamePiece piece1, GamePiece piece2)
    {
        (piece1.GridCoords, piece2.GridCoords) = (piece2.GridCoords, piece1.GridCoords);
        GamePieces[piece1.GridCoords.X, piece1.GridCoords.Y] = piece1;
        GamePieces[piece2.GridCoords.X, piece2.GridCoords.Y] = piece2;
    }

    private void OnMoveCompleted(GamePiece piece)
    {
        _movingPieces.Remove(piece);
        if (!_movingPieces.IsEmpty)
        {
            return;
        }
    }

    public void AddToBoard(GamePiece piece, IntVector2? coords = null)
    {
        var c = coords ?? ScreenPositionToGridCoords(piece.Bounds.Center);
        if (c is not { } gridCoords) throw new ArgumentOutOfRangeException(nameof(coords), "Couldn't find valid grid coords from screen position.");
        GamePieces[gridCoords.X, gridCoords.Y] = piece;
        piece.GridCoords = gridCoords;
    }
    
    private void MovePieceTo(GamePiece piece, Point screenPosition)
    {
        piece.MoveTo(screenPosition);
        _movingPieces.Add(piece);
        piece.MoveCompleted += OnMoveCompleted;
    }

    private GamePiece? PieceAtScreenPosition(Point position)
    {
        var c = ScreenPositionToGridCoords(position);
        if (c is { } coords)
        {
            return GamePieces[coords.X, coords.Y];
        }
        return null;
    }

    private IntVector2? ScreenPositionToGridCoords(Point position)
    {
        // TODO: throw error if outside bounds
        for (int i = 0; i < Columns; i++)
        {
            for (int j = 0; j < Rows; j++)
            {
                if (HexMath.HexContains(Spaces[i, j].GamePieceBounds, position))
                {
                    return new IntVector2(i, j);
                }
            }
        }
        return null;
    }

    public override void OnTouchEventEnded(TouchEventArgs args)
    {
        var startPiece = PieceAtScreenPosition(args.TouchDown);
        if (args.TouchUp is null) return;
        var endPiece = PieceAtScreenPosition((Point)args.TouchUp);
        if (startPiece is null || endPiece is null || startPiece != endPiece) return;
        if (endPiece.Selected)
        {
            endPiece.Selected = false;
            SelectedPiece = null;
            return;
        }
            
        if (SelectedPiece is not null && SelectedPiece != endPiece)
        {
            SwapPieces(SelectedPiece, endPiece);
            SelectedPiece.Selected = false;
            endPiece.Selected = false;
            SelectedPiece = null;
                
            return;
        }

        if (SelectedPiece is not null) return;
        SelectedPiece = endPiece;
        endPiece.Selected = true;
    }

    public override void Update(GameTime gameTime)
    {
        var mouse = Mouse.GetState();
        if (Bounds.Contains(mouse.Position))
        {
            foreach (var space in Spaces)
            {
                space.Highlighted = HexMath.HexContains(space.Bounds, mouse.Position);
            }
        }
        base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        // foreach (var rect in BoardLocationRects)
        // {
        //     var tex = Game.Content.Load<Texture2D>("Graphics/" + "rune_slot");
        //     var spriteBatch = Game.Services.GetService<SpriteBatch>();
        //     spriteBatch.DrawToLayer(tex, rect, LayerDepth + DrawLayer.ObjectIncrement);
        // }
        //
        base.Draw(gameTime);
    }
}

public static class Direction
{
    public static IntVector2 Up { get; } = new (0, -1);
    public static IntVector2 Down { get; } = new (0, 1);
    public static IntVector2 Left { get; } = new (-1, 0);

    public static IntVector2 Right { get; } = new (1, 0);

    // public static IntVector2 UpLeft = Up + Left;
    // public static IntVector2 UpRight = Up + Right;
    // public static IntVector2 DownLeft = Down + Left;
    // public static IntVector2 DownRight = Down + Right;
    public static readonly List<IntVector2> CardinalDirections = [Up, Down, Left, Right];
}