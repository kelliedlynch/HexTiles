using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tweening;
using HexTiles.Utility;

namespace HexTiles;

public class GamePiece(Game game, Rectangle bounds) : ExtendedDrawableGameComponent(game, bounds)
{
    public PieceType PieceType;
    public int Value = 1;
    public bool Highlighted = false;
    public bool Selected = false;
    
    
    private float _heldSizeIncrease = 0.1f;
    private bool _isHeld = false;
    public bool IsHeld
    {
        get => _isHeld;
        set
        {
            _isHeld = value;
            OnIsHeldChanged(value);
        }
    }

    private void OnIsHeldChanged(bool value)
    {
        var pickUpDuration = 0.16f;
        var putDownDuration = 0.08f;
        var dur = pickUpDuration;
        var newBounds = Bounds;
        if (value)
        {
            var newSize = HexMath.ScaleBy(Bounds.Size, _heldSizeIncrease);
            // var hIncrease = (int)(Bounds.Width * _heldSizeIncrease);
            // var vIncrease = (int)(Bounds.Height * _heldSizeIncrease);
            // newBounds.Location -= new Point(hIncrease, vIncrease);
            // newBounds.Size += new Point(hIncrease * 2, vIncrease * 2);
            newBounds.Location = Bounds.Location + (Bounds.Size - newSize) / new Point(2);
            newBounds.Size = newSize;
        }
        else
        {
            var board = Game.Services.GetService<GameBoard>();
            newBounds = board.Spaces[GridCoords.X, GridCoords.Y].GamePieceBounds;
            dur = putDownDuration;
        }
        var tween = Game.Services.GetService<Tweener>();
        tween.TweenTo<GamePiece, Vector2>(this, (x) => x.Vector2Location, newBounds.Location.ToVector2(), dur, 0.05f);
        tween.TweenTo<GamePiece, Vector2>(this, (x) => x.Vector2Size, newBounds.Size.ToVector2(), dur, 0.05f);
    }

    public IntVector2 GridCoords;

    public Point TargetPosition;

    // This is needed for tweening, until I can figure out a more elegant solution
    public Vector2 Vector2Location
    {
        get => Bounds.Location.ToVector2();
        set => Bounds.Location = value.ToPoint();
    }
    public Vector2 Vector2Size
    {
        get => Bounds.Size.ToVector2();
        set => Bounds.Size = value.ToPoint();
    }
    
    public event Action<GamePiece> MoveCompleted;


    public Vector2 Velocity = Vector2.Zero;
    public float Speed = 0.4f;
    public MoveState MoveState = MoveState.NotMoving;
    


    public string FileName = "purple_rune";
    
    public Color Color
    {
        get
        {
            return PieceType switch
            {
                PieceType.Diamond => Color.Red,
                PieceType.Circle => Color.DeepSkyBlue,
                PieceType.Square => Color.Purple,
                PieceType.Pentagon => Color.Yellow,
                PieceType.Star => Color.Orange,
                PieceType.Jewel => Color.LawnGreen,
                _ => Color.White
            };
        }
    }

    public override void Update(GameTime gameTime)
    {
        if (MoveState == MoveState.NotMoving)
        {
            return;
        }

        // var tween = Game.Services.GetService<Tweener>();
        // tween.Update(gameTime.GetElapsedSeconds());
        if (Bounds.Location == TargetPosition)
        {
            // TODO: make TargetPosition nullable, and null it here
            MoveState = MoveState.NotMoving;
            MoveCompleted?.Invoke(this);

        }
        
    }

    public void MoveTo(Point position)
    {
        TargetPosition = position;
        var tween = Game.Services.GetService<Tweener>();
        tween.TweenTo<GamePiece, Vector2>(this, x => x.Vector2Location, position.ToVector2(), Speed)
            .Easing(EasingFunctions.CubicInOut);
        MoveState = MoveState.Moving;
    }

    public void FallTo(Point position)
    {
        TargetPosition = position;
        var delay = new Random().NextDouble() * 0.1;
        var tween = Game.Services.GetService<Tweener>();
        tween.TweenTo<GamePiece, Vector2>(this, x => x.Vector2Location, position.ToVector2(), Speed, (float)delay)
            .Easing(EasingFunctions.BounceOut);
        MoveState = MoveState.Moving;
    }

    public override void Draw(GameTime gameTime)
    {
        var spriteBatch = Game.Services.GetService<SpriteBatch>();
        if(Selected)
        {
            OutlineComponent();
        }

        var bounds = Bounds;
        if (IsHeld)
        {

        }
        var tex = Game.Content.Load<Texture2D>("Graphics/" + FileName);
        // var destinationRect = new Rectangle((int)Position.X, (int)Position.Y, (int)_size.X, (int)_size.Y);
        // spriteBatch.Draw(tex, Bounds, null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, DrawLayer.BoardContents);
        spriteBatch.DrawToLayer(tex, Bounds, LayerDepth);
        // base.Draw(gameTime);
    }

}

public enum PieceType
{
    Diamond,
    Square,
    Circle,
    Pentagon,
    Jewel,
    Star
}

public enum MoveState
{
    Moving,
    NotMoving
}

