using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = System.Numerics.Vector2;

namespace HexTiles.Utility;

public static class SpriteBatchExtensions
{
    public static void DrawToLayer(this SpriteBatch batch, Texture2D tex, Rectangle destinationRect, float layer)
    {
        batch.Draw(tex, destinationRect, tex.Bounds, Color.White, 0f, Vector2.Zero, SpriteEffects.None, layer);
    }
    
    public static void DrawToLayer(this SpriteBatch batch, Texture2D tex, Rectangle destinationRect, Color color, float layer)
    {
        batch.Draw(tex, destinationRect, tex.Bounds, color, 0f, Vector2.Zero, SpriteEffects.None, layer);
    }
    
    public static void DrawToLayer(this SpriteBatch batch, Texture2D tex, Rectangle destinationRect, Rectangle sourceRect, float layer)
    {
        batch.Draw(tex, destinationRect, sourceRect, Color.White, 0f, Vector2.Zero, SpriteEffects.None, layer);
    }
    
    public static void DrawToLayer(this SpriteBatch batch, Texture2D tex, Rectangle destinationRect, Rectangle sourceRect, Color color, float layer)
    {
        batch.Draw(tex, destinationRect, sourceRect, color, 0f, Vector2.Zero, SpriteEffects.None, layer);
    }
}
