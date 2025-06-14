using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;

public static class Canvas
{
    //pixels save the position of real canvas pixels
    public static Pixel[,] pixels { get; private set; }
    //Infinite cicles do not affect the real canvas
    public static WallE robot { get; private set; }
    //WallE x and y is the robot position on the pixels array
    public static string brushColor { get; set; } = "Transparent";
    public static int brushSize { get; set; } = 1;
    private static Queue<DrawCommand> commandQueue = new Queue<DrawCommand>();
    public static void ChangeCanvas(int size)
    {
        pixels = new Pixel[size, size];
    }
    public static void InitializePixel(float cellSize)
    {
        robot = new WallE();
        brushColor = "Transparent";
        brushSize = 1;
        for (int i = 0; i < pixels.GetLength(0); i++)
        {
            for (int j = 0; j < pixels.GetLength(1); j++)
            {
                pixels[i, j] = new Pixel((int)(j * cellSize) + 1, (int)(i * cellSize) + 1, (int)((j + 1) * cellSize) - 1, (int)((i + 1) * cellSize) - 1, "#FFFFFF");
            }
        }
    }
    #region Wall-E functions
    public static int GetCanvasSize() => pixels.GetLength(0);
    public static int GetActualX() => robot.X;
    public static int GetActualY() => robot.Y;
    public static int GetColorCount(string color, int x1, int y1, int x2, int y2)
    {
        if (x1 < 0 || x2 < 0 || y1 < 0 || y2 < 0 || x1 >= GetCanvasSize() || y1 >= GetCanvasSize() || x2 >= GetCanvasSize() || y2 >= GetCanvasSize()) return 0;
        int minX = Math.Min(x1, x2);
        int maxX = Math.Max(x1, x2);
        int minY = Math.Min(y1, y2);
        int maxY = Math.Max(y1, y2);
        int counter = 0;
        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                if (pixels[y, x].Color == color)
                {
                    counter++;
                }
            }
        }
        return counter;
    }
    public static int IsBrushColor(string color) => color == brushColor ? 1 : 0;
    public static int IsBrushSize(int size) => size == brushSize ? 1 : 0;
    public static int IsCanvasColor(string color, int vertical, int horizontal)
    {
        if (robot.X + horizontal < 0 || robot.X + horizontal >= GetCanvasSize() || robot.Y + vertical < 0 || robot.Y + vertical >= GetCanvasSize()) return 0;
        return pixels[robot.Y + vertical, robot.X + horizontal].Color == color ? 1 : 0;
    }
    #endregion
    #region Wall-E intructions

    public static void ChangeColor(string newColor) => brushColor = newColor;
    public static void ChangeSize(int k)
    {
        brushSize = k % 2 == 0 ? k - 1 : k;
    }
    public static void Spawn(int x, int y)
    {
        PutInstructionInTheQueue(
            new DrawCommand
            {
                Command = "Spawn",
                X = x,
                Y = y,
                Color = "#FF8C00"
            }
        );
        if (robot.X != -1 && robot.Y != -1)
            robot.MoveWallE(x, y, pixels);
        else
            robot.Spawn(x, y, pixels);
    }
    public static void DrawLine(int dirX, int dirY, int distance)
    {
        int x0 = robot.X;
        int y0 = robot.Y;
        int x1 = x0 + dirX * distance;
        int y1 = y0 + dirY * distance;
        int dx = Math.Abs(x1 - x0);
        int dy = Math.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;
        do
        {
            PaintPixel(x0, y0);
            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        } while (!(x0 == x1 && y0 == y1));

        PutInstructionInTheQueue(
            new DrawCommand
            {
                Command = "DrawLine",
                X = x1,
                Y = y1,
                dirX = dirX,
                dirY = dirY,
                distance = distance,
                Color = "#FF8C00",
                brushSize = brushSize,
            }
        );
        robot.MoveWallE(x1, y1, pixels);
    }
    public static void DrawCircle(int dirX, int dirY, int radius)
    {
        PutInstructionInTheQueue(
            new DrawCommand
            {
                Command = "QuitWallE",
                X = robot.X,
                Y = robot.Y,
                Color = pixels[robot.X, robot.Y].Color,
            }
        );
        int CenterX = robot.X + dirX * radius;
        int CenterY = robot.Y + dirY * radius;
        PutInstructionInTheQueue(
            new DrawCommand
            {
                Command = "DrawCircle",
                X = CenterX,
                Y = CenterY,
                dirX = dirX,
                dirY = dirY,
                radius = radius,
                Color = "#FF8C00",
                brushSize = brushSize,
            }
        );
        int x = 0;
        int y = radius;
        //int p = 3-(2*radius);
        //int p = 5 / 4 - radius;
        DrawCirclePoints(CenterX, CenterY, x, y);
        while (x <= y)
        {
            x++;
            /*if (p <= 0)
            {
                p = p + 2 * x + 1;
            }
            else
            {
                y--;
                p = p + 2 * x + 1 - 2 * y;
            }
            //PaintPixel(CenterX+x,CenterY+y);*/
            y=(int)Math.Round(Math.Sqrt(Math.Pow(radius,2)-Math.Pow(x,2)),0);
            DrawCirclePoints(CenterX, CenterY, x, y);
        }
        robot.MoveWallE(CenterX, CenterY, pixels);
    }

    public static void DrawRectangle(int dirX, int dirY, int distance, int width, int height)
    {
        PutInstructionInTheQueue(
            new DrawCommand
            {
                Command = "QuitWallE",
                X = robot.X,
                Y = robot.Y,
                Color = pixels[robot.X, robot.Y].Color,
            }
        );
        int CenterX = robot.X + dirX * distance;
        int CenterY = robot.Y + dirY * distance;

        PutInstructionInTheQueue(
            new DrawCommand
            {
                Command = "DrawRectangle",
                X = CenterX,
                Y = CenterY,
                dirX = dirX,
                dirY = dirY,
                distance = distance,
                width = width,
                height = height,
                Color = "#FF8C00",
                brushSize = brushSize,
            }
        );
        int halfWidth = width / 2;
        int halfHeight = height / 2;
        int left = CenterX - 1 - halfWidth;
        int right = CenterX + 1 + halfWidth;
        int top = CenterY - 1 - halfHeight;
        int button = CenterY + 1 + halfHeight;
        for (int y = top; y <= button; y++)
        {
            PaintPixel(left, y);
            PaintPixel(right, y);

        }
        for (int x = left; x <= right; x++)
        {
            PaintPixel(x, button);
            PaintPixel(x, top);
        }
        robot.MoveWallE(CenterX, CenterY, pixels);
    }
    public static void Fill()
    {
        if (brushColor == "Transparent") return;
        string targetColor = pixels[robot.Y, robot.X].Color;
        if (targetColor == brushColor) return;
        Queue<(int, int)> queue = new Queue<(int, int)>();
        queue.Enqueue((robot.Y, robot.X));
        while (queue.Count > 0)
        {
            (int, int) p = queue.Dequeue();
            if (p.Item1 < 0 || p.Item1 >= GetCanvasSize() || p.Item2 < 0 || p.Item2 >= GetCanvasSize()) continue;
            if (pixels[p.Item1, p.Item2].Color != targetColor) continue;
            pixels[p.Item1, p.Item2].Color = brushColor;
            queue.Enqueue((p.Item1 + 1, p.Item2));
            queue.Enqueue((p.Item1 - 1, p.Item2));
            queue.Enqueue((p.Item1, p.Item2 + 1));
            queue.Enqueue((p.Item1, p.Item2 - 1));
        }
    }
    #endregion
    #region Auxiliar methods
    private static void PaintPixel(int x, int y)
    {

        if (brushColor == "Transparent") return;
        if(x >= 0 && x < GetCanvasSize() && y >= 0 && y < GetCanvasSize() && pixels[y, x].Color != brushColor)
        PutInstructionInTheQueue(
            new DrawCommand
            {
                Command = "Draw",
                X = x,
                Y = y,
                Color = brushColor,
                brushSize = brushSize,
            }
        );
        if (brushSize == 1)
        {
            //if(!(pixels[x,y].Color==brushColor))
            if (x >= 0 && x < GetCanvasSize() && y >= 0 && y < GetCanvasSize() && pixels[y, x].Color != brushColor)
            {
                //Is y,x because on arrays is row, column
                pixels[y, x].Color = brushColor;
            }
            return;
        }
        int halfSize = brushSize / 2;
        for (int dy = -halfSize; dy <= halfSize; dy++)
        {
            for (int dx = -halfSize; dx <= halfSize; dx++)
            {
                int nx = x + dx;
                int ny = y + dy;
                if (nx >= 0 && nx < GetCanvasSize() && ny >= 0 && ny < GetCanvasSize() && pixels[ny, nx].Color != brushColor)
                    pixels[ny, nx].Color = brushColor;
            }
        }
    }
    private static void DrawCirclePoints(int xc, int yc, int x, int y)
    {
        PaintPixel(xc + x, yc + y);
        PaintPixel(xc + x, yc - y);
        PaintPixel(xc - x, yc + y);
        PaintPixel(xc - x, yc - y);
        PaintPixel(xc + y, yc + x);
        PaintPixel(xc + y, yc - x);
        PaintPixel(xc - y, yc + x);
        PaintPixel(xc - y, yc - x);
    }
    #endregion
    private static void PutInstructionInTheQueue(DrawCommand command)
    {
        commandQueue.Enqueue(command);
    }
    public static void QuitInstructionsInTheQueue() => commandQueue.Clear();
    public static Queue<DrawCommand> getQueue() => commandQueue;

}
public struct DrawCommand
{
    public int X;
    public int Y;
    public string Color;
    public int brushSize;
    public string Command;
    public int dirX, dirY, distance, width, height, radius;
}