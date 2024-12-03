using System;
using HexTiles.Utility;
using Microsoft.Xna.Framework;

namespace HexTiles;

public static class GridFactory
{
    
    public static void GenerateGrid(GameBoard.GameBoard board)
    {
        var bounds = board.Bounds;
        var spacing = GameBoard.GameBoard.Spacing;
        var rows = GameBoard.GameBoard.Rows;
        var columns = GameBoard.GameBoard.Columns;
        var hexMaxWidth = (bounds.Width - spacing * (columns + 1))/ (columns + 0.5) ;
        var hexMaxHeight = (bounds.Height - spacing * (rows + 1))/ (rows * 0.75 + 0.25);
        var widthConstrainedSize = (float)hexMaxWidth / float.Sqrt(3);
        var heightConstrainedSize = (float)hexMaxHeight / 2;
        var radius = Math.Min(widthConstrainedSize, heightConstrainedSize);
        var tileSize = new IntVector2((int)(radius * float.Sqrt(3)), (int)radius * 2);
        board.TileSize = tileSize;
        
        board.CenterPoints = new IntVector2[columns, rows];
        board.GamePieceRects = new Rectangle[columns, rows];
        board.BoardLocationRects = new Rectangle[columns, rows];

        var gridSize = new IntVector2((int)(tileSize.X * (columns + 0.5f) + spacing * (columns + 1.0f) + spacing / 2.0f),
            (int)(tileSize.Y * rows * 0.75f + tileSize.Y * 0.25f + spacing / float.Sqrt(3) * (rows + 1.0f)));

        var gridBounds = new Rectangle(bounds.X + (bounds.Width - gridSize.X) / 2,
            bounds.Y + (bounds.Height - gridSize.Y) / 2, gridSize.X, gridSize.Y);

        var xPos = gridBounds.X + tileSize.X / 2 + spacing + spacing / 2;
        var yPos = gridBounds.Y + (int)(spacing / float.Sqrt(3));

        for (int j = 0; j < rows; j++)
        {
            for (int i = 0; i < columns; i++)
            {
                board.CenterPoints[i, j] = new IntVector2(xPos + tileSize.X / 2, yPos + tileSize.Y / 2);
                board.GamePieceRects[i, j] = new Rectangle(xPos, yPos, tileSize.X, tileSize.Y);
                board.BoardLocationRects[i, j] = new Rectangle(xPos - spacing / 2, yPos - spacing / 2,
                    tileSize.X + spacing, tileSize.Y + spacing);
                if (i < columns - 1)
                {
                    xPos += tileSize.X + spacing;
                }
                else
                {
                    xPos = gridBounds.X + spacing + (tileSize.X / 2 + spacing / 2) * ((j + 0) % 2);
                }
            }

            yPos += (int)((float)tileSize.Y * 0.75 + (float)spacing * 0.75);
        }
    }
}