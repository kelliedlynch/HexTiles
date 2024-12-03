using System;
using System.Collections.Generic;
using HexTiles.Utility;
using Microsoft.Xna.Framework;

namespace HexTiles.Graphics;

public class Container : ExtendedDrawableGameComponent
{
    public Alignment ContentAlignment = Alignment.Center;
    public IntVector2 ContentsSize = IntVector2.Zero;
    public Container Parent;
    public List<Container> Children = new();
    public IntVector4 Padding = new IntVector4(0, 0, 0, 0);

    public IntVector2 Position
    {
        get
        {
            var p = Parent?.Position ?? IntVector2.Zero;
            return LocalPosition + p;
        }
        set
        {
            var p = Parent?.Position ?? IntVector2.Zero;
            LocalPosition = value - p;
        }
    }
    
    public IntVector2 LocalPosition = IntVector2.Zero;

    public AxisSizing Sizing = AxisSizing.ShrinkXShrinkY;

    public IntVector2 DisplayedSize
    {
        get
        {
            var x = Math.Max(Math.Max(AssignedSize.X, MinSize.X), FinalLayoutSize.X);
            var y = Math.Max(Math.Max(AssignedSize.Y, MinSize.Y), FinalLayoutSize.Y);
            return new IntVector2(x, y);
        }
    }
    
    public IntVector2 AssignedSize
    {
        get
        {
            // var x = Math.Max(Math.Max(_size.X, MinSize.X), LayoutSize.X);
            // var y = Math.Max(Math.Max(_size.Y, MinSize.Y), LayoutSize.Y);
            // return new IntVector2(x, y);
            return _assignedSize;
        }
        set
        {
            if (_assignedSize == value) return;
            _assignedSize = value;
            SizeChanged?.Invoke(this);

        }
    }

    private IntVector2 _assignedSize = IntVector2.Zero;

    public IntVector2 MinSize
    {
        get
        {
            var x = Math.Max(_minSize.X, ContentsSize.X);
            var y = Math.Max(_minSize.Y, ContentsSize.Y);
            return new IntVector2(x, y);
        }
        set
        {
            if (_minSize == value) return;
            _minSize = value;
            SizeChanged?.Invoke(this);
        }    
    }
    
    private IntVector2 _minSize = IntVector2.Zero;

    protected internal IntVector2 LayoutSize = IntVector2.Zero;
    
    // A container's FinalLayoutSize should ONLY be set after all its children are finalized (is this FinalLayoutSize or LayoutSize?)
    protected internal IntVector2 FinalLayoutSize
    {
        get => _finalLayoutSize;
        set
        {
            if (_finalLayoutSize == value) return;
            _finalLayoutSize = value;
            SizeChanged?.Invoke(this);
        }
    }

    private IntVector2 _finalLayoutSize = IntVector2.Zero;

    public Rectangle Bounds
    {
        get =>  new (Position, DisplayedSize);
        set
        {
            // TODO: THIS NEEDS TO BE EXAMINED LATER
            var oldSize = AssignedSize;
            Position = new IntVector2(value.X, value.Y);
            AssignedSize = new IntVector2(value.Width, value.Height);
            if (oldSize.X != AssignedSize.X)
            {
                SizeChanged?.Invoke(this);
            }

            if (oldSize.Y != AssignedSize.Y)
            {
                SizeChanged?.Invoke(this);
            }
        }
    }

    public delegate void SizeChangeEventHandler(Container sender);

    public event SizeChangeEventHandler SizeChanged;
    public delegate void PositionChangeEventHandler(Container sender);

    public event PositionChangeEventHandler PositionChanged;
    
    public event SizeChangeEventHandler LayoutSizeChanged;

    // public delegate void DrawOrderChangeEventHandler(Container sender);
    //
    // public event DrawOrderChangeEventHandler ChangedDrawOrder;
    
    public Container(Game game) : base(game)
    {
        PositionChanged += OnPositionChanged;
    }
    
    public Container(Game game, Rectangle bounds) : this(game)
    {
        Bounds = bounds;
    }

    protected override void OnVisibleChanged(object sender, EventArgs args)
    {
        if (Visible) LayoutElements();
        base.OnVisibleChanged(sender, args);
    }

    protected void OnSizeChanged(object sender)
    {
        // Outline.Visible = Visible;
        // if (Visible) LayoutElements();
        // base.OnSizeChanged(sender, args);
    }

    protected void OnPositionChanged(object sender)
    {
        if (Visible) LayoutElements();
    }
    
    public void AddChild(Container e)
    {
        e.Parent = this;

        SizeChanged += e.OnParentSizeChanged;
        // PositionChanged += e.OnPositionChanged;
        e.SizeChanged += OnChildSizeChanged;
        DrawOrderChanged += e.OnParentDrawOrderChanged;
        e.DrawOrder = DrawOrder + 1;
        Children.Add(e);
        Game.Components.Add(e);
        if (Visible) LayoutElements();
    }

    public void RemoveChildren()
    {
        foreach (var child in Children)
        {
            child.RemoveChildren();
            Game.Components.Remove(child);
        }
        Children.Clear();
    }

    protected virtual void OnParentDrawOrderChanged(object sender, EventArgs args)
    {
        DrawOrder = Parent.DrawOrder + 1;
    }

    protected virtual void OnParentSizeChanged(Container e)
    {
        // if ((Sizing & (AxisSizing.ExpandX | AxisSizing.ExpandY)) != 0)
        // {
        //     Parent.LayoutElements();
        // }
    }

    public virtual void OnChildSizeChanged(Container e)
    {
        // if ((Sizing & (AxisSizing.ShrinkX | AxisSizing.ShrinkY)) != 0)
        // {
        //     LayoutElements();
        // }
        // TODO: THIS IS WHERE MY PROBLEM IS. I NEED TO REDO LAYOUT WHEN CHILD SIZE CHANGES, BUT NOT IN A WAY THAT CREATES AN ENDLESS LOOP
        LayoutElements();
    }
    
    public virtual void LayoutElements()
    {
        SetChildrenLayoutSizes();
        SetThisLayoutSize();
        AdjustExpandableChildren();
        SetLocalPositions();
        AlignChildren();
    }

    public virtual void SetChildrenLayoutSizes()
    {
        var totalChildrenX = 0;
        var totalChildrenY = 0;
        foreach (var child in Children)
        {
            child.LayoutElements();
            totalChildrenX = child.DisplayedSize.X > totalChildrenX ? child.DisplayedSize.X : totalChildrenX;
            totalChildrenY = child.DisplayedSize.Y > totalChildrenY ? child.DisplayedSize.Y : totalChildrenY;
        }
        
        var thisWidth = Math.Max(DisplayedSize.X, 1);
        var thisHeight = Math.Max(DisplayedSize.Y, 1);;
        
        if ((Sizing & AxisSizing.ShrinkX) != 0)
        {
            thisWidth = totalChildrenX;
        }
        if ((Sizing & AxisSizing.ShrinkY) != 0)
        {
            thisHeight = totalChildrenY;
        }

        if (Parent is null || (Sizing & (AxisSizing.ExpandX | AxisSizing.ExpandY)) == 0)
        {
            FinalLayoutSize = new IntVector2(thisWidth, thisHeight);
        }
        else
        {
            LayoutSize  = new IntVector2(thisWidth, thisHeight);
        }
        
        ContentsSize = new IntVector2(totalChildrenX, totalChildrenY);
    }

    public virtual void SetLocalPositions()
    {
        
    }

    public virtual void SetThisLayoutSize()
    {
        var thisWidth = Math.Max(DisplayedSize.X, 1);
        var thisHeight = Math.Max(DisplayedSize.Y, 1);;
        
        if ((Sizing & AxisSizing.ShrinkX) != 0)
        {
            thisWidth = ContentsSize.X;
        }
        if ((Sizing & AxisSizing.ShrinkY) != 0)
        {
            thisHeight = ContentsSize.Y;
        }

        if (Parent is null || (Sizing & (AxisSizing.ExpandX | AxisSizing.ExpandY)) == 0)
        {
            FinalLayoutSize = new IntVector2(thisWidth, thisHeight);
        }
        else
        {
            LayoutSize  = new IntVector2(thisWidth, thisHeight);
        }
        
    }

    public virtual void AdjustExpandableChildren()
    {
        foreach (var child in Children)
        {
            var childWidth = child.DisplayedSize.X;
            var childHeight = child.DisplayedSize.Y;
            if ((child.Sizing & AxisSizing.ExpandX) != 0)
            {
                childWidth = DisplayedSize.X;
            }
            if ((child.Sizing & AxisSizing.ExpandY) != 0)
            {
                childHeight = DisplayedSize.Y;
            }

            child.FinalLayoutSize = new IntVector2(childWidth, childHeight);
        }
        
    }

    protected virtual void AlignChildren()
    {
        foreach (var child in Children)
        {
            var offsetX = 0;
            if ((ContentAlignment & Alignment.Right) != 0)
            {
                offsetX = DisplayedSize.X - child.DisplayedSize.X;
            }  
            else if ((ContentAlignment & Alignment.Left) != 0)
            {
                offsetX = 0;
            } 
            else if ((ContentAlignment & Alignment.Center) != 0)
            {
                offsetX = (DisplayedSize.X - child.DisplayedSize.X) / 2;
            }
            
            var offsetY = 0;
            if ((ContentAlignment & Alignment.Bottom) != 0)
            {
                offsetY = DisplayedSize.Y - child.DisplayedSize.Y;
            }  
            else if ((ContentAlignment & Alignment.Top) != 0)
            {
                offsetY = 0;
            } 
            else if ((ContentAlignment & Alignment.Center) != 0)
            {
                offsetY = (DisplayedSize.Y - child.DisplayedSize.Y) / 2;
            }
        
            child.LocalPosition += new IntVector2(offsetX, offsetY);
        }
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
    }
}

public enum SizingMode
{
    Fixed,
    ExpandToFit,
    FitContent
}

[Flags]
public enum AxisSizing
{
    FixedX = 1,
    FixedY = 2,
    ExpandX = 4,
    ExpandY = 8,
    ShrinkX = 16,
    ShrinkY = 32,
    FixedXFixedY = 3,
    ExpandXExpandY = 12,
    ShrinkXShrinkY = 48,
    FixedXExpandY = 9,
    FixedXShrinkY = 33,
    ExpandXShrinkY = 36,
    ExpandXFixedY = 6,
    ShrinkXFixedY = 19,
    ShrinkXExpandY = 24
}

[Flags]
public enum FlowDirection
{
    TopToBottom = 1,
    BottomToTop = 2,
    LeftToRight = 4,
    RightToLeft = 8,
    TopLeftToBottomRight = 5,
    BottomLeftToTopRight = 6,
    TopRightToBottomLeft = 9,
    BottomRightToTopLeft = 10
}

[Flags]
public enum Alignment
{
    Left = 1,
    Top = 2,
    TopLeft = 3,
    Right = 4,
    TopRight = 6,
    Bottom = 8,
    BottomLeft = 9,
    BottomRight = 12,
    Center = 16,
    CenterLeft = 17,
    TopCenter = 18,
    CenterRight = 20,
    BottomCenter = 24
}