using HexTiles.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;

namespace HexTiles;

public class UIBar (Game game) : ExtendedDrawableGameComponent (game)
{
    private string _barBackLeft = "Graphics/barBack_horizontalLeft";
    private string _barBackMiddle = "Graphics/barBack_horizontalMid";
    private string _barBackRight = "Graphics/barBack_horizontalRight";
    private string _barFillLeft = "Graphics/barGreen_horizontalLeft";
    private string _barFillMiddle = "Graphics/barGreen_horizontalMid";
    private string _barFillRight = "Graphics/barGreen_horizontalRight";
    public int MaxValue;
    public int CurrentValue;

    public UIBar(Game game, Rectangle bounds, int currentValue, int maxValue) : this(game)
    {
        Bounds = bounds;
        CurrentValue = currentValue;
        MaxValue = maxValue;
        Debug = true;
        BorderColor = Color.Red;
    }
    
    public override void Draw(GameTime gameTime)
    {
        var spriteBatch = Game.Services.GetService<SpriteBatch>();
        
        var bblTex = game.Content.Load<Texture2D>(_barBackLeft);
        var bbmTex = game.Content.Load<Texture2D>(_barBackMiddle);
        var bbrTex = game.Content.Load<Texture2D>(_barBackRight);
        var leftRect = new Rectangle(Bounds.X, Bounds.Y, bblTex.Width, Bounds.Height);
        var rightRect = new Rectangle(Bounds.X + Bounds.Width - bbrTex.Width, Bounds.Y, bbrTex.Width, Bounds.Height);
        var midRect = new Rectangle(Bounds.X + leftRect.Width, Bounds.Y, Bounds.Width - leftRect.Width - rightRect.Width, Bounds.Height);
        spriteBatch.Draw(bblTex, leftRect, Color.White);
        spriteBatch.Draw(bbmTex, midRect, Color.White);
        spriteBatch.Draw(bbrTex, rightRect, Color.White);
        
        
        var bflTex = game.Content.Load<Texture2D>(_barFillLeft);
        var bfmTex = game.Content.Load<Texture2D>(_barFillMiddle);
        var bfrTex = game.Content.Load<Texture2D>(_barFillRight);
        float pct = (float)CurrentValue / MaxValue;
        float barRightPct = (float)bfrTex.Width / Bounds.Width;
        float barLeftPct = (float)bflTex.Width / Bounds.Width;
        if (pct < barLeftPct)
        {
            var spriteWidth = bflTex.Width * pct / barLeftPct;
            leftRect.Width = (int)spriteWidth;
            var texRect = new Rectangle(0, 0, (int)spriteWidth, bflTex.Height);
            spriteBatch.Draw(bflTex, leftRect, texRect, Color.White);
            return;
        }
        spriteBatch.Draw(bflTex, leftRect, Color.White);
        if (pct < 1 - barRightPct)
        {
            var spriteWidth = Bounds.Width * (pct - barLeftPct);
            midRect.Width = (int)spriteWidth;
            spriteBatch.Draw(bfmTex, midRect, Color.White);
            return;
        }
        spriteBatch.Draw(bfmTex, midRect, Color.White);
        if (pct < 1)
        {
            var spritePct = pct - (1 - barRightPct);
            var spriteWidth = Bounds.Width * spritePct;
            rightRect.Width = (int)spriteWidth;
            var texRect = new Rectangle(0, 0, (int)spriteWidth, bfrTex.Height);
            spriteBatch.Draw(bfrTex, rightRect, texRect, Color.White);
            return;
        }
        spriteBatch.Draw(bfrTex, rightRect, Color.White);
        
    }
}