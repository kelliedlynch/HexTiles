using System;
using System.Collections.Generic;
using HexTiles.Utility;
using Microsoft.Xna.Framework;

namespace HexTiles;

public static class GameBoardFactory
{
    
    public static GameBoardSpace[,] GenerateGrid(GameBoard board)
    {
        var bounds = board.Bounds;
        var spacing = board.Spacing;
        var rows = board.Rows;
        var columns = board.Columns;
        var hexMaxWidth = (bounds.Width)/ (columns + 0.5) ;
        var hexMaxHeight = (bounds.Height)/ (rows * 0.75 + 0.25);
        var widthConstrainedSize = (float)hexMaxWidth / float.Sqrt(3);
        var heightConstrainedSize = (float)hexMaxHeight / 2;
        var radius = Math.Min(widthConstrainedSize, heightConstrainedSize);
        var spaceSize = new IntVector2((int)(radius * float.Sqrt(3)), (int)radius * 2);
        var tileSize = new IntVector2(spaceSize.X - (int)(spacing / float.Sqrt(3) * 2), spaceSize.Y - (int)(spacing));
        board.TileSize = tileSize;
        // var spaceSize = ;
        
        var spaces = new GameBoardSpace[columns, rows];

        // var gridSize = new IntVector2((int)(tileSize.X * (columns + 0.5f) + spacing * (columns + 1.0f) + spacing / 2.0f),
            // (int)(tileSize.Y * rows * 0.75f + tileSize.Y * 0.25f + (spacing * float.Sqrt(3)) / 2 * (rows + 1.0f)));

        var gridSize = new IntVector2((int)(spaceSize.X * (columns + 0.5f)), (int)(spaceSize.Y * 0.75f * rows + spaceSize.Y * 0.25f));
            
            
        var gridBounds = new Rectangle(bounds.X + (bounds.Width - gridSize.X) / 2,
            bounds.Y + (bounds.Height - gridSize.Y) / 2, gridSize.X, gridSize.Y);

        var xPos = gridBounds.X + spaceSize.X / 2;
        var yPos = gridBounds.Y;

        for (int j = 0; j < rows; j++)
        {
            for (int i = 0; i < columns; i++)
            {
                // var space = new GameBoardSpace(board.Game, new Rectangle(xPos, yPos, tileSize.X, tileSize.Y));
                // var rect = new Rectangle(xPos - spacing / 2, yPos - (int)(spacing * float.Sqrt(3) / 2),
                //     tileSize.X + spacing, tileSize.Y + (int)(spacing * float.Sqrt(3) ));
                var space = new GameBoardSpace(board.Game, new Rectangle(xPos, yPos, spaceSize.X, spaceSize.Y));
                space.LayerDepth = board.LayerDepth;
                spaces[i, j] = space;
                // board.BoardLocationRects[i, j] = new Rectangle(xPos - spacing / 2, yPos - spacing / 2,
                //     tileSize.X + spacing, tileSize.Y + spacing);
                if (i < columns - 1)
                {
                    xPos += spaceSize.X;
                }
                else
                {
                    xPos = gridBounds.X + (spaceSize.X / 2) * ((j + 0) % 2);
                }
            }

            yPos += (int)((float)spaceSize.Y * 0.75 );
        }

        return spaces;
    }
    
}