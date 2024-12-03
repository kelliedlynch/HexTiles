using System;
using System.Net;
using HexTiles.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HexTiles.Graphics.Layout;

public class TextLabel : Container
{
    public string Text
    {
        get => _text;
        set
        {
            if (_text == value) return;
            _text = value;
            MinSize = Font.MeasureString(value).ToIntVector2();
        }
    }

    private string _text = "";
    public SpriteFont Font;
    public IntVector2 TextPosition;

    
    
    public TextLabel(Game game) : base(game)
    {
        Font = game.Services.GetService<SpriteFont>();
        // LayoutSizeChanged += OnLayoutSizeChanged;
    }

    public TextLabel(Game game, Rectangle bounds) : this(game)
    {
        Bounds = bounds;
    }
    
    public TextLabel(Game game, Rectangle bounds, string text) : this(game, bounds)
    {
        Text = text;
        // var s = Font.MeasureString(Text);
        // Size = s.ToIntVector2();
    }
    
    public TextLabel(Game game, string text) : this(game, Rectangle.Empty)
    {
        Text = text;
        // var s = Font.MeasureString(Text);
        // Size = s.ToIntVector2();
    }

    // public void OnLayoutSizeChanged(Container sender)
    // {
    //     LayoutElements();
    // }

    public override void SetChildrenLayoutSizes()
    {
        if (Text == "0")
        {
            var a = 0;
        }
        var textSize = Font.MeasureString(Text).ToIntVector2();
        ContentsSize = textSize;
        if (Parent is null || (Sizing & (AxisSizing.ExpandX | AxisSizing.ExpandY)) == 0)
        {
            FinalLayoutSize = textSize;
        }
        else
        {
            LayoutSize  = textSize;
        }
    }

    public override void SetLocalPositions()
    {
        // var padding = DisplayedSize - ContentsSize;
        // var offsetX = padding.X / 2;
        // var offsetY = padding.Y / 2;
        // if ((ContentAlignment & Alignment.Top) != 0)
        // {
        //     offsetY = 0;
        // }
        // else if ((ContentAlignment & Alignment.Bottom) != 0)
        // {
        //     offsetY = padding.Y;
        // }
        // if ((ContentAlignment & Alignment.Left) != 0)
        // {
        //     offsetX = 0;
        // }
        // else if ((ContentAlignment & Alignment.Right) != 0)
        // {
        //     offsetX = padding.X;
        // }
        //
        // TextPosition = new IntVector2(offsetX, offsetY);
    }

    public override void Draw(GameTime gameTime)
    {
        var spriteBatch = Game.Services.GetService<SpriteBatch>();
        spriteBatch.DrawString(Font, Text, Position + TextPosition, Color.White);
        base.Draw(gameTime);
    }
}