using System;
using HexTiles.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace HexTiles;

public class GameBoardSpace : ExtendedDrawableGameComponent
{
    public IntVector2 BoardCoords { get; set; }
    public bool Highlighted { get; set; } = false;
    public Color HighlightColor { get; set; } = Color.LightGoldenrodYellow;

    public Color Color
    {
        get
        {
            return Highlighted ? HighlightColor : Color.White;
        }
        
    }

    public int Padding = 10;
    public Rectangle GamePieceBounds => Bounds.GetRelativeRectangle(
        Padding / 2,
        (int)(Padding / float.Sqrt(3)),
        Bounds.Width - Padding, Bounds.Height - (int)(Padding / float.Sqrt(3)) * 2);

    private IntVector2 TargetingAreaSize => new IntVector2((int)(Bounds.Width / 1.5f), (int)(Bounds.Height / 1.5f));

    public GameBoardSpace(Game game, Rectangle bounds) : base(game, bounds)
    {
        
    }

    public Rectangle TargetingArea => Bounds.GetRelativeRectangle(
        Bounds.Width / 2 - TargetingAreaSize.X / 2,
        Bounds.Height / 2 - TargetingAreaSize.Y / 2,
        TargetingAreaSize.X, TargetingAreaSize.Y
    );

    public override void Draw(GameTime gameTime)
    {
        Debug = Highlighted;
        var tex = Game.Content.Load<Texture2D>("Graphics/" + "rune_slot");
        var spriteBatch = Game.Services.GetService<SpriteBatch>();
        spriteBatch.DrawToLayer(tex, Bounds, Color, LayerDepth + DrawLayer.ObjectIncrement);
        base.Draw(gameTime);
    }
}