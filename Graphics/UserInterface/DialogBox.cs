using System;
using HexTiles.Graphics.Layout;
using HexTiles.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace HexTiles.Graphics.UserInterface;

public class DialogBox : SpritePanel
{
    
    // private const string SpriteSheet = "Graphics/UI_tiles";
    // // SpriteLocation is the grid location of the upper left tile of a 3 x 3 block of dialog box tiles
    // private static readonly IntVector2 SpriteLocation = new(3, 12);
    // protected static readonly IntVector2 TileSize = new(16, 16);

    public TextLabel TextLabel;

    public int PaddingTop = TileSize.Y / 2;
    public int PaddingBottom = TileSize.Y / 2;
    public int PaddingLeft = TileSize.X / 2;
    public int PaddingRight = TileSize.X / 2;
    
    public DialogBox(Game game) : base(game)
    {
        Sizing = AxisSizing.FixedXFixedY;
        TextLabel = new TextLabel(game);
        TextLabel.DrawOrder = DrawOrder + 1;
        
        // game.Components.Add(TextLabel);
        AddChild(TextLabel);
    }

    public DialogBox(Game game, IntVector2 assignedSize) : this(game)
    {
        AssignedSize = assignedSize;
    }

    public override void LayoutElements()
    {
        // TextLabel.LayoutElements();
        var sizeX = Math.Max(DisplayedSize.X, MinSize.X);
        var sizeY = Math.Max(DisplayedSize.Y, MinSize.Y);
        if ((Sizing & AxisSizing.ShrinkX) != 0)
        {
            sizeX = Math.Max(MinSize.X, TextLabel.DisplayedSize.X);
            sizeX += PaddingLeft + PaddingRight;
        }
        if ((Sizing & AxisSizing.ShrinkY) != 0)
        {
            sizeY = Math.Max(MinSize.Y, TextLabel.DisplayedSize.Y);
            sizeY += PaddingTop + PaddingBottom;
        }

        var baseSize = new IntVector2(sizeX, sizeY);
        var tilesX = (int)Math.Ceiling((decimal)baseSize.X / TileSize.X);
        var tilesY = (int)Math.Ceiling((decimal)baseSize.Y / TileSize.Y);
        AssignedSize = new IntVector2(tilesX, tilesY) * TileSize;


        var safeAreaSize = new IntVector2(DisplayedSize.X - PaddingLeft - PaddingRight, DisplayedSize.Y - PaddingTop - PaddingBottom);
        var padding = DisplayedSize - TextLabel.DisplayedSize;
        var offsetX = padding.X / 2 + PaddingLeft;
        var offsetY = padding.Y / 2 + PaddingTop;
        if ((ContentAlignment & Alignment.Left) != 0)
        {
            offsetX = PaddingLeft;
        }
        else if ((ContentAlignment & Alignment.Right) != 0)
        {
            offsetX = padding.X - PaddingRight;
        }
        if ((ContentAlignment & Alignment.Top) != 0)
        {
            offsetY = PaddingTop;
        }
        else if ((ContentAlignment & Alignment.Bottom) != 0)
        {
            offsetY = padding.Y - PaddingBottom;
        }

        TextLabel.LocalPosition = new IntVector2(offsetX, offsetY);
        // DrawBox();
    }

    // protected override void LoadContent()
    // {
    //     var stringSize = TextLabel.Font.MeasureString(TextLabel.Text);
    //     var tilesSize = new IntVector2((int)Math.Ceiling(stringSize.X / TileSize.X),
    //         (int)Math.Ceiling(stringSize.Y / TileSize.Y));
    //     Size = (tilesSize + IntVector2.Two) * TileSize;
    //     base.LoadContent();
    // }

    // private IntVector2 FindPosition(Rectangle container, IntVector2 elementSize, Alignment align)
    // {
    //     // var screen = Game.GraphicsDevice.Viewport.Bounds;
    //     var vPadding = container.Height - elementSize.Y;
    //     var hPadding = container.Width - elementSize.X;
    //     var originX = container.X;
    //     var originY = container.Y;
    //     if ((align & Alignment.Center) != 0)
    //     {
    //         originX = container.X + hPadding / 2;
    //         originY = container.Y + vPadding / 2;
    //     }
    //     if ((align & Alignment.Left) != 0)
    //     {
    //         originX = container.X;
    //     } else if ((align & Alignment.Right) != 0)
    //     {
    //         originX = container.X + hPadding;
    //     } 
    //     if ((align & Alignment.Top) != 0)
    //     {
    //         originY = container.Y;
    //     } else if ((align & Alignment.Bottom) != 0)
    //     {
    //         originY = container.Y + vPadding;
    //     }
    //
    //     return new IntVector2(originX, originY);
    // }

    // public void DrawBox()
    // {
    //     var spriteBatch = Game.Services.GetService<SpriteBatch>();
    //     // var stringSize = TextLabel.Font.MeasureString(TextLabel.Text);
    //     var tilesX = (int)Math.Ceiling((decimal)Size.X / TileSize.X);
    //     var tilesY = (int)Math.Ceiling((decimal)Size.Y / TileSize.Y);
    //     // Size = new IntVector2(tilesX, tilesY) * TileSize;
    //
    //     // Position = FindPosition(Parent.Bounds, Size, BoxAlignment);
    //
    //     for (int x = 0; x < tilesX ; x++)
    //     {
    //         for (int y = 0; y < tilesY ; y++)
    //         {
    //             var tileRect = new Rectangle(SpriteLocation * TileSize, TileSize);
    //             var destinationRect = new Rectangle(Position + new IntVector2(x, y) * TileSize, TileSize);
    //             if (x == 0 && y == 0)
    //             {
    //                 
    //             }
    //             else if (x == tilesX  && y == 0)
    //             {
    //                 tileRect.Location += new Point(TileSize.X * 2, 0);
    //             }
    //             else if (y == 0)
    //             {
    //                 tileRect.Location += new Point(TileSize.X, 0);
    //             }
    //             else if (x == 0 && y == tilesY )
    //             {
    //                 tileRect.Location += new Point(0, TileSize.Y * 2);
    //             }
    //             else if (x == 0)
    //             {
    //                 tileRect.Location += new Point(0, TileSize.Y);
    //             }
    //             else if (x == tilesX  && y == tilesY )
    //             {
    //                 tileRect.Location += TileSize * new IntVector2(2, 2);
    //             }
    //             else if (y == tilesY )
    //             {
    //                 tileRect.Location += new Point(TileSize.X, TileSize.Y * 2);
    //             }
    //             else if (x == tilesX)
    //             {
    //                 tileRect.Location += new Point(TileSize.X * 2, TileSize.Y);
    //             }
    //             else
    //             {
    //                 tileRect.Location += TileSize;
    //             }
    //             var texture = Game.Content.Load<Texture2D>(SpriteSheet);
    //             spriteBatch.Draw(texture, destinationRect, tileRect, Color.DarkGray);
    //         }
    //     }
    // }

    public override void Draw(GameTime gameTime)
    {
        
        base.Draw(gameTime);
    }
}

