﻿using System;
using System.Collections.Generic;
using HexTiles.Graphics;
using HexTiles.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.Tweening;
using MonoGame.Extended.VectorDraw;

namespace HexTiles;

public class MainGame : Game
{

    public IntVector2 ScreenSize = new(600, 1080);
    private const float ElementPadding = 0.02f;
    private const float BattleFieldHeight = 0.38f;
    private const float GameBoardHeight = 0.52f;
    private const float DockHeight = 0.12f;
    
    private Container BattleFieldContainer;
    private Container GameBoardContainer;
    private Container DockContainer;

    public MainGame()
    {
        var graphics = new GraphicsDeviceManager(this);
        graphics.PreferredBackBufferWidth = ScreenSize.X;
        graphics.PreferredBackBufferHeight = ScreenSize.Y;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        var inputManager = new InputManager(this);
        Components.Add(inputManager);
        Services.AddService(inputManager);

        var tweener = new Tweener();
        Services.AddService(tweener);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        var spriteBatch = new SpriteBatch(GraphicsDevice);
        Services.AddService(spriteBatch);


    }

    protected override void BeginRun()
    {
        var inputManager = Services.GetService<InputManager>();
        
        var padding = (int)Math.Min(ScreenSize.X * ElementPadding, ScreenSize.Y * ElementPadding);
        BattleFieldContainer = new Container(this);
        BattleFieldContainer.Bounds = new Rectangle(padding, padding, ScreenSize.X - padding * 2, (int)(ScreenSize.Y * BattleFieldHeight));
        // BattleFieldContainer.Debug = true;
        
        var battlefield = new Battlefield(this, BattleFieldContainer.Bounds);
        battlefield.LayerDepth = DrawLayer.FieldBackground;
        battlefield.Debug = true;
        Components.Add(battlefield);
        Services.AddService(battlefield);
        
        GameBoardContainer = new Container(this);
        GameBoardContainer.Bounds = new Rectangle(padding, padding * 2 + BattleFieldContainer.Bounds.Height, ScreenSize.X - padding * 2, (int)(ScreenSize.Y * GameBoardHeight));
        // GameBoardContainer.Debug = true;
        
        var gameBoard = new GameBoard(this, GameBoardContainer.Bounds);
        gameBoard.LayerDepth = DrawLayer.BoardBackground;
        gameBoard.Debug = true;
        Components.Add(gameBoard);
        Services.AddService(gameBoard);
        
        DockContainer = new Container(this);
        DockContainer.Bounds = new Rectangle(padding, ScreenSize.Y - padding - (int)(ScreenSize.Y * DockHeight), ScreenSize.X - padding * 2, (int)(ScreenSize.Y * DockHeight));
        // DockContainer.Debug = true;
        
        var dock = new DeckDock(this, DockContainer.Bounds);
        dock.LayerDepth = DrawLayer.DeckDock;
        dock.Debug = true;
        Components.Add(dock);
        Services.AddService(dock);
        inputManager.RegisterComponent(dock,
            [InputListeners.TouchEnded, InputListeners.TouchBegan, InputListeners.TouchMoved]);

        var deck = new Deck(this);
        deck.GenerateDeck();
        Components.Add(deck);
        Services.AddService(deck);
        deck.DrawHand();
        dock.LayoutSlots();
        
        var battleManager = new BattleManager(this, gameBoard, battlefield);
        Services.AddService(battleManager);

        var monsters = new List<Monster>();

        for (int i = 0; i < 1; i++)
        {
            var mon = new Monster();
            mon.FileName = "Graphics/Slime RPG Basic";
            mon.MaxHealth = 100;
            mon.CurrentHealth = 100;
            monsters.Add(mon);
        }
        
        battleManager.InitializeBattle(monsters);
        battleManager.BeginBattle();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
        var tween = Services.GetService<Tweener>();
        tween.Update(gameTime.GetElapsedSeconds());
        base.Update(gameTime);
    }

    protected override bool BeginDraw()
    {
        GraphicsDevice.Clear(Color.Gray);
        Services.GetService<SpriteBatch>().Begin(SpriteSortMode.FrontToBack);
        return base.BeginDraw();
    }

    protected override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
    }

    protected override void EndDraw()
    {
        Services.GetService<SpriteBatch>().End();
        base.EndDraw();
    }
}
