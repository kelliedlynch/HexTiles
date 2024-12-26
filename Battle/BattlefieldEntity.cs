using System;
using System.Collections.ObjectModel;
using HexTiles.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace HexTiles;

public class BattlefieldEntity(Game game) : ExtendedDrawableGameComponent(game)
{
    // private Battlefield _field;
    public Monster Monster;
    public UIBar HealthBar;
    private int _barHeight = 10;
    public Rectangle SpritePosition;
    public Rectangle HealthBarPosition;
    public Texture2D SpriteTexture;
    // public bool Targeted = false;
    
    public BattlefieldEntity(Game game, Monster monster) : this(game)
    {
        Monster = monster;
        monster.HealthChanged += OnHealthChanged;
        // _field = field;
        LoadTexture();
    }
    
    public void LoadTexture()
    {
        SpriteTexture = Game.Content.Load<Texture2D>(Monster.FileName);
    }

    public void SetPosition(Rectangle rect)
    {
        SpritePosition = rect;
        HealthBarPosition = rect.GetRelativeRectangle(0, 0, rect.Width, _barHeight);
        HealthBar = new UIBar(Game, HealthBarPosition, Monster.CurrentHealth, Monster.MaxHealth);
        Game.Components.Add(HealthBar);
    }

    public override void Draw(GameTime gameTime)
    {
        var spriteBatch = Game.Services.GetService<SpriteBatch>();
        spriteBatch.Draw(SpriteTexture, SpritePosition, Color.White);
        
        if (Monster.Targeted)
        {
            OutlineComponent();
        }
        
    }

    public void OnHealthChanged()
    {
        HealthBar.CurrentValue = Monster.CurrentHealth;
        HealthBar.MaxValue = Monster.MaxHealth;
    }
}
