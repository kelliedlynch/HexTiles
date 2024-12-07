using System;
using System.Collections.Generic;
using HexTiles.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace HexTiles;

// public delegate void TouchEvent(TouchEventArgs args);

public class InputManager(Game game) : GameComponent(game)
{
    public bool LeftMousePressed = false;
    public Point? MouseDownAt = null;
    public Point LastPosition = Point.Zero;
    private readonly List<ExtendedDrawableGameComponent> RegisteredComponents = new();
    
    public event TouchEventHandler TouchEventEnded;
    public event TouchEventHandler TouchEventBegan;
    public event TouchEventHandler TouchEventMoved;


    public void RegisterComponent(ExtendedDrawableGameComponent component, List<InputListeners> listeners)
    {
        RegisteredComponents.Add(component);
        RegisteredComponents.Sort( (y, x) => x.LayerDepth.CompareTo(y.LayerDepth) );
        if (listeners.Contains(InputListeners.TouchBegan))
        {
            TouchEventBegan += component.OnTouchEventBegan;
        }

        if (listeners.Contains(InputListeners.TouchMoved))
        {
            TouchEventMoved += component.OnTouchEventMoved;
        }

        if (listeners.Contains(InputListeners.TouchEnded))
        {
            TouchEventEnded += component.OnTouchEventEnded;
        }
    }

    private void OnTouchEventCompleted(TouchEventArgs args)
    {
        TouchEventEnded?.Invoke(args);
    }

    private void OnTouchEventBegan(TouchEventArgs args)
    {
        TouchEventBegan?.Invoke(args);
    }

    private void OnTouchEventMoved(TouchEventArgs args)
    {
        TouchEventMoved?.Invoke(args);
    }
    
    public override void Update(GameTime gameTime)
    {
        var mouse = Mouse.GetState();
        if (LeftMousePressed && mouse.LeftButton == ButtonState.Released)
        {
            LeftMousePressed = false;
            if (MouseDownAt is not null)
            {
               OnTouchEventCompleted(new TouchEventArgs((Point)MouseDownAt, mouse.Position, mouse.Position));
               MouseDownAt = null;
            }

        }

        if (!LeftMousePressed && mouse.LeftButton == ButtonState.Pressed)
        {
            LeftMousePressed = true;
            MouseDownAt = mouse.Position;
            LastPosition = mouse.Position;
            OnTouchEventBegan(new TouchEventArgs((Point)mouse.Position, mouse.Position, null));
        }

        if (LeftMousePressed && mouse.Position != LastPosition)
        {
            if (MouseDownAt is null) return;
            LastPosition = mouse.Position;
            OnTouchEventMoved(new TouchEventArgs((Point)MouseDownAt, mouse.Position, null));
        }
        
        base.Update(gameTime);
    }
}

public delegate void TouchEventHandler(TouchEventArgs args);

public class TouchEventArgs(Point touchDown, Point currentLocation, Point? touchUp) : EventArgs
{
    public Point TouchDown = touchDown;
    public Point CurrentLocation = currentLocation;
    public Point? TouchUp = touchUp;
}

public class RegisteredComponent
{
    public ExtendedDrawableGameComponent Component { get; init; }
    public bool ConsumesInput = true;
    public List<int> Listeners = new();
}

public enum InputListeners
{
    TouchBegan,
    TouchMoved,
    TouchEnded
}