using System;
using System.Linq;
using HexTiles.Graphics.UserInterface;
using HexTiles.Utility;
using Microsoft.Xna.Framework;

namespace HexTiles.Graphics.Layout;

public class VStackContainer : Container
{
    public FlowDirection FlowType = FlowDirection.TopToBottom;
    public int ChildSpacing = 0;
    
    public VStackContainer(Game game) : base(game)
    {
    }

    public VStackContainer(Game game, Rectangle bounds) : base(game, bounds)
    {
    }
    
    public override void SetChildrenLayoutSizes()
    {
        var x = 0;
        var y = 0;
        if (FlowType is FlowDirection.BottomToTop)
        {
            y = DisplayedSize.Y;
        }
        var btt = (FlowType & FlowDirection.BottomToTop) != 0;
        if (btt)
        {
            x = DisplayedSize.X;
        }

        var totalChildrenX = 0;
        var totalChildrenY = 0;
        
        foreach (var child in Children)
        {
            child.LayoutElements();
            var childWidth = Math.Max(child.DisplayedSize.X, 1);
            var childHeight = Math.Max(child.DisplayedSize.Y, 1);
            
            if ((child.Sizing & AxisSizing.ExpandY) != 0)
            {
                childHeight = Math.Min(childHeight, child.MinSize.Y);
            }
            
            if ((FlowType & FlowDirection.BottomToTop) != 0)
            {
                y -= childHeight - ChildSpacing;
            }

            child.LayoutSize = new IntVector2(childWidth, childHeight);
            
            if ((FlowType & FlowDirection.TopToBottom) != 0)
            {
                y += childHeight + ChildSpacing;
            }
            
            totalChildrenX = childWidth > totalChildrenX ? childWidth : totalChildrenX;
            totalChildrenY += childHeight + ChildSpacing;
        }
        
        ContentsSize = new IntVector2(totalChildrenX, totalChildrenY);
    }

    public override void AdjustExpandableChildren()
    {
        var totalChildrenX = ContentsSize.X;
        var totalChildrenY = ContentsSize.Y;
        var toExpandY = Children.Where(el => (el.Sizing & AxisSizing.ExpandY) != 0).ToList();
        if (toExpandY.Count > 0)
        {
            var unusedY = DisplayedSize.Y - totalChildrenY;
            var expandEach = unusedY / toExpandY.Count;
            foreach (var element in toExpandY)
            {
                unusedY -= expandEach;
                if (unusedY < expandEach)
                {
                    expandEach += unusedY;
                }
                element.FinalLayoutSize = new IntVector2(element.LayoutSize.X, element.LayoutSize.Y + expandEach);
            }

            totalChildrenY = DisplayedSize.Y;
        }
        
        var toExpandX = Children.Where(el => (el.Sizing & AxisSizing.ExpandX) != 0).ToList();
        if (toExpandX.Count > 0)
        {
            foreach (var element in toExpandX)
            {
                element.FinalLayoutSize = new IntVector2(DisplayedSize.X, element.DisplayedSize.Y);
            }

            totalChildrenX = DisplayedSize.X;
        }

        ContentsSize = new IntVector2(totalChildrenX, totalChildrenY);
    }

    public override void SetLocalPositions()
    {
        var x = 0;
        var y = 0;
        if (FlowType is FlowDirection.BottomToTop)
        {
            y = DisplayedSize.Y;
        }
        
        foreach (var child in Children)
        {
            if (FlowType is FlowDirection.BottomToTop)
            {
                y -= child.DisplayedSize.Y - ChildSpacing;
            }
            
            child.LocalPosition = new IntVector2(x, y);
            
            if ((FlowType & FlowDirection.TopToBottom) != 0)
            {
                y += child.DisplayedSize.Y + ChildSpacing;
            }
            
        }
    }

    protected override void AlignChildren()
    {
        var offsetY = 0;
        if ((ContentAlignment & Alignment.Bottom) != 0)
        {
            offsetY = DisplayedSize.Y - ContentsSize.Y;
        }  
        else if ((ContentAlignment & Alignment.Top) != 0)
        {
            offsetY = 0;
        } 
        else if ((ContentAlignment & Alignment.Center) != 0)
        {
            offsetY = (DisplayedSize.Y - ContentsSize.Y) / 2;
        }

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

            child.LocalPosition += new IntVector2(offsetX, offsetY);
        }
    }
}