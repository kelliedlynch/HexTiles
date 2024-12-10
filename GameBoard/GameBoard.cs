using System;
using System.Collections.Generic;
using System.Linq;
using HexTiles.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collections;

namespace HexTiles;

public class GameBoard(Game game, Rectangle bounds) : ExtendedDrawableGameComponent(game, bounds)
{
    public readonly int Rows = 5;
    public readonly int Columns = 5;

    public readonly int Spacing = 10;
    private GamePiece SelectedPiece {get; set;}
    private readonly Bag<GamePiece> _movingPieces = [];
    public GamePiece[,] GamePieces;

    public GameBoardSpace[,] Spaces;
    // public BoardSlot[,] Slots;
    // public Rectangle[,] GamePieceRects;
    // public Rectangle[,] BoardLocationRects;
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
        MovePieceTo(piece1, Spaces[piece1.GridPosition.X, piece1.GridPosition.Y].GamePieceBounds.Location);
        MovePieceTo(piece2, Spaces[piece2.GridPosition.X, piece2.GridPosition.Y].GamePieceBounds.Location);
    }

    private void SwapPositions(GamePiece piece1, GamePiece piece2)
    {
        (piece1.GridPosition, piece2.GridPosition) = (piece2.GridPosition, piece1.GridPosition);
        GamePieces[piece1.GridPosition.X, piece1.GridPosition.Y] = piece1;
        GamePieces[piece2.GridPosition.X, piece2.GridPosition.Y] = piece2;
    }

    private void OnMoveCompleted(GamePiece piece)
    {
        _movingPieces.Remove(piece);
        if (!_movingPieces.IsEmpty)
        {
            return;
        }
    }

    
    private void MovePieceTo(GamePiece piece, Point screenPosition)
    {
        piece.MoveTo(screenPosition);
        _movingPieces.Add(piece);
        piece.MoveCompleted += OnMoveCompleted;
    }

    private GamePiece PieceAtGridPosition(IntVector2 location)
    {
        try
        {
            return GamePieces[location.X, location.Y];
        }
        catch 
        {
            return null; 
        }
    }

    private GamePiece PieceAtScreenPosition(Point position)
    {
        for (int i = 0; i < Columns; i++)
        {
            for (int j = 0; j < Rows; j++)
            {
                if (HexMath.HexContains(Spaces[i, j].GamePieceBounds, position))
                {
                    return GamePieces[i, j];
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