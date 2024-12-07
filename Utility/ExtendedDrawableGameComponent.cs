using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;

namespace HexTiles.Utility;

public abstract class ExtendedDrawableGameComponent(Game game) : DrawableGameComponent(game)
{

    public float LayerDepth = DrawLayer.GameBackground;
    
    private static readonly List<Color> DebugColors = new()
    {
        Color.White, Color.Chartreuse, Color.Crimson, Color.Cyan, Color.Magenta, Color.Orange, Color.Pink, Color.Red, Color.Violet, Color.Yellow
    }; 
    
    public Rectangle Bounds = Rectangle.Empty;
    public int BorderWidth = 3;
    private Color _borderColor = Color.Transparent;
    public Color BorderColor
    {
        get
        {
            if (_borderColor == Color.Transparent)
            {
                var rand = new Random();
                _borderColor = DebugColors[rand.Next(DebugColors.Count)];
            }

            return _borderColor;
        }
        set => _borderColor = value;
    }
    
    public bool Debug = false;

    public ExtendedDrawableGameComponent(Game game, Rectangle bounds) : this(game)
    {
        Bounds = bounds;
        
    }

    protected void OutlineComponent()
    {
        var spriteBatch = Game.Services.GetService<SpriteBatch>();
        var pixel = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
        pixel.SetData(new[] { Color.White });
        var pos = Bounds.Location.ToIntVector2();
        var size = Bounds.Size.ToIntVector2();
        var layer = (float)Math.Floor(LayerDepth / DrawLayer.LayerIncrement) * DrawLayer.LayerIncrement + DrawLayer.LayerIncrement - DrawLayer.ObjectIncrement;
        
        spriteBatch.DrawToLayer(pixel, new Rectangle(pos.X, pos.Y, size.X, BorderWidth), BorderColor, layer);

        spriteBatch.DrawToLayer(pixel, new Rectangle(pos.X, pos.Y, BorderWidth, size.Y), BorderColor, layer);

        spriteBatch.DrawToLayer(pixel, new Rectangle(pos.X + size.X - BorderWidth,
            pos.Y,
            BorderWidth,
            size.Y), BorderColor, layer);

        spriteBatch.DrawToLayer(pixel, new Rectangle(pos.X,
            pos.Y + size.Y - BorderWidth,
            size.X,
            BorderWidth), BorderColor, layer);
    }
    
    public override void Draw(GameTime gameTime)
    {
        if (Debug)
        {
            OutlineComponent();
        }
        
        base.Draw(gameTime);
    }

    public virtual void OnTouchEventBegan(TouchEventArgs args)
    {
    }

    public virtual void OnTouchEventEnded(TouchEventArgs args)
    {
    }

    public virtual void OnTouchEventMoved(TouchEventArgs args)
    {
    }
}