using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HexTiles.Utility;
using MonoGame.Extended.VectorDraw;

namespace HexTiles;

public class Battlefield(Game game, Rectangle bounds) : ExtendedDrawableGameComponent(game, bounds)
{
    private readonly List<BattlefieldEntity> _monsters = new();
    
    private IntVector2 MaxMonsterSize => new IntVector2(
        (Bounds.Width - Spacing * (_monsters.Count - 1)) / _monsters.Count,
        (Bounds.Height - Spacing * (_monsters.Count - 1)) / _monsters.Count);
    private int Spacing => (int)((Bounds.Width * 0.1f)/ _monsters.Count);
    private int BarHeight => (int)(Bounds.Height * 0.04f);

    public event Action<BattlefieldEntity> MonsterTouched;
    public event Action BattlefieldTouched;
    
    public override void Initialize()
    {
        var man = Game.Services.GetService<InputManager>();
        man.TouchEventEnded += OnTouchEventEnded;
    }
    
    

    public void AddMonster(Monster monster)
    {
        var entity = new BattlefieldEntity(Game, monster);
        Game.Components.Add(entity);
        _monsters.Add(entity);

        PlaceMonsters();

    }

    public void PlaceMonsters()
    {
        int totalWidth = 0;

        foreach (var monster in _monsters)
        {
            var sprite = monster.SpriteTexture;
            var ratioX = (double)MaxMonsterSize.X / sprite.Width;
            var ratioY = (double)MaxMonsterSize.Y / sprite.Height;
            var ratio = Math.Min(Math.Min(ratioX, ratioY), 2);

            var scaledWidth = (int)(sprite.Width * ratio);
            var scaledHeight = (int)(sprite.Height * ratio);
            var spriteRect = new Rectangle(0, 0, scaledWidth, scaledHeight);
            monster.SpritePosition = spriteRect;
            totalWidth += scaledWidth;
            
        }
        
        int currentX = (Bounds.Width - totalWidth - Spacing * (_monsters.Count - 1)) / 2;
        foreach (var monster in _monsters)
        {
            int y = Bounds.Height - monster.SpritePosition.Height - Spacing;
            var location = new Point(currentX, y) + Bounds.Location;
            var newPosition = new Rectangle(location.X, location.Y, monster.SpritePosition.Width, monster.SpritePosition.Height);
            monster.SetPosition(newPosition);
            currentX += Spacing + monster.SpritePosition.Width;
        }
    }

    public override void OnTouchEventEnded(TouchEventArgs args)
    {
        if (args.TouchUp is null || !Bounds.Contains((Point)args.TouchUp)) return;
        
        foreach (var monster in _monsters)
        {
            if (!monster.SpritePosition.Contains((Point)args.TouchUp)) continue;
            MonsterTouched?.Invoke(monster);
            return;
        }
        BattlefieldTouched?.Invoke();
    }


}