using System;
using HexTiles.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HexTiles.Graphics.UserInterface;

public class SpritePanel : Container
{
    protected const string SpriteSheet = "Graphics/UI_tiles";
    // SpriteLocation is the grid location of the upper left tile of a 3 x 3 block of dialog box tiles
    private static readonly IntVector2 SpriteLocation = new(3, 12);
    protected static readonly IntVector2 TileSize = new(16, 16);
    
    public SpritePanel(Game game) : base(game)
    {
    }

    public void BuildPanel()
    {
        var spriteBatch = Game.Services.GetService<SpriteBatch>();
        var tilesX = (int)Math.Ceiling((decimal)DisplayedSize.X / TileSize.X);
        var tilesY = (int)Math.Ceiling((decimal)DisplayedSize.Y / TileSize.Y);

        for (int x = 0; x < tilesX ; x++)
        {
            for (int y = 0; y < tilesY ; y++)
            {
                var tileRect = new Rectangle(SpriteLocation * TileSize, TileSize);
                var destinationRect = new Rectangle(Position + new IntVector2(x, y) * TileSize, TileSize);
                if (x == 0 && y == 0)
                {
                    
                }
                else if (x == tilesX  && y == 0)
                {
                    tileRect.Location += new Point(TileSize.X * 2, 0);
                }
                else if (y == 0)
                {
                    tileRect.Location += new Point(TileSize.X, 0);
                }
                else if (x == 0 && y == tilesY )
                {
                    tileRect.Location += new Point(0, TileSize.Y * 2);
                }
                else if (x == 0)
                {
                    tileRect.Location += new Point(0, TileSize.Y);
                }
                else if (x == tilesX  && y == tilesY )
                {
                    tileRect.Location += TileSize * new IntVector2(2, 2);
                }
                else if (y == tilesY )
                {
                    tileRect.Location += new Point(TileSize.X, TileSize.Y * 2);
                }
                else if (x == tilesX)
                {
                    tileRect.Location += new Point(TileSize.X * 2, TileSize.Y);
                }
                else
                {
                    tileRect.Location += TileSize;
                }
                var texture = Game.Content.Load<Texture2D>(SpriteSheet);
                spriteBatch.Draw(texture, destinationRect, tileRect, Color.DarkGray);
            }
        }

        LayoutSize = TileSize * new IntVector2(tilesX, tilesY);
        // Console.WriteLine(Bounds);
    }

    public override void Draw(GameTime gameTime)
    {
        BuildPanel();
        base.Draw(gameTime);
    }
}