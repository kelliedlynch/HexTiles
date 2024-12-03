using System;
using System.Linq;
using HexTiles.Graphics.UserInterface;
using HexTiles.Utility;
using Microsoft.Xna.Framework;


namespace HexTiles.Graphics.Layout;

public class HStackContainer : Container
{
    public FlowDirection FlowType = FlowDirection.LeftToRight;
    public int ChildSpacing = 0;
    
    public HStackContainer(Game game) : base(game)
    {
    }

    public HStackContainer(Game game, Rectangle bounds) : this(game)
    {
        
    }

    public override void SetChildrenLayoutSizes()
    {
        var x = 0;
        var y = 0;
        var rtl = (FlowType & FlowDirection.RightToLeft) != 0;
        if (rtl)
        {
            x = DisplayedSize.X;
        }

        var totalChildrenX = 0;
        var totalChildrenY = 0;
        
        foreach (var child in Children)
        {
            child.LayoutElements();
            var childWidth = Math.Max(child.DisplayedSize.X, 1);
            var childHeight = Math.Max(child.DisplayedSize.Y, 1);;

            if ((child.Sizing & AxisSizing.ExpandX) != 0)
            {
                childWidth = Math.Min(childWidth, child.MinSize.X);
            }
            
            if ((FlowType & FlowDirection.RightToLeft) != 0)
            {
                x -= childWidth - ChildSpacing;
            }
            
            child.LayoutSize = new IntVector2(childWidth, childHeight);
            
            if ((FlowType & FlowDirection.LeftToRight) != 0)
            {
                x += childWidth + ChildSpacing;
            }

            totalChildrenX += childWidth + ChildSpacing;
            totalChildrenY = childHeight > totalChildrenY ? childHeight : totalChildrenY;
        }
        
        ContentsSize = new IntVector2(totalChildrenX, totalChildrenY);
    }

    public override void AdjustExpandableChildren()
    {
        var totalChildrenX = ContentsSize.X;
        var totalChildrenY = ContentsSize.Y;
        var toExpandX = Children.Where(el => (el.Sizing & AxisSizing.ExpandX) != 0).ToList();
        if (toExpandX.Count > 0)
        {
            var unusedX = DisplayedSize.X - totalChildrenX;
            var expandEach = unusedX / toExpandX.Count;
            foreach (var element in toExpandX)
            {
                unusedX -= expandEach;
                if (unusedX < expandEach)
                {
                    expandEach += unusedX;
                }
                element.FinalLayoutSize = new IntVector2(element.LayoutSize.X + expandEach, element.LayoutSize.Y);
            }

            totalChildrenX = DisplayedSize.X;
        }
        
        var toExpandY = Children.Where(el => (el.Sizing & AxisSizing.ExpandY) != 0).ToList();
        if (toExpandY.Count > 0)
        {
            foreach (var element in toExpandY)
            {
                element.FinalLayoutSize = new IntVector2(DisplayedSize.X, element.DisplayedSize.Y);
            }

            totalChildrenY = DisplayedSize.Y;
        }

        ContentsSize = new IntVector2(totalChildrenX, totalChildrenY);
    }

    public override void SetLocalPositions()
    {
        var x = 0;
        var y = 0;
        if (FlowType is FlowDirection.RightToLeft)
        {
            x = DisplayedSize.X;
        }
        
        foreach (var child in Children)
        {
            if (FlowType is FlowDirection.RightToLeft)
            {
                x -= child.DisplayedSize.X - ChildSpacing;
            }
            
            child.LocalPosition = new IntVector2(x, y);
            
            if ((FlowType & FlowDirection.LeftToRight) != 0)
            {
                x += child.DisplayedSize.X + ChildSpacing;
            }
            
        }
    }


    protected override void AlignChildren()
    {
        var offsetX = 0;
        if ((ContentAlignment & Alignment.Right) != 0)
        {
            offsetX = DisplayedSize.X - ContentsSize.X;
        }  
        else if ((ContentAlignment & Alignment.Left) != 0)
        {
            offsetX = 0;
        } 
        else if ((ContentAlignment & Alignment.Center) != 0)
        {
            offsetX = (DisplayedSize.X - ContentsSize.X) / 2;
        }

        foreach (var child in Children)
        {
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
}