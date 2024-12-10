using System;
using HexTiles.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace HexTiles;

public class GameBoardSpace(Game game, Rectangle bounds) : ExtendedDrawableGameComponent(game, bounds)
{
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
        bounds.Width - Padding, bounds.Height - (int)(Padding / float.Sqrt(3)) * 2);

    public override void Draw(GameTime gameTime)
    {
        var tex = Game.Content.Load<Texture2D>("Graphics/" + "rune_slot");
        var spriteBatch = Game.Services.GetService<SpriteBatch>();
        spriteBatch.DrawToLayer(tex, Bounds, Color, LayerDepth + DrawLayer.ObjectIncrement);
        base.Draw(gameTime);
    }
}