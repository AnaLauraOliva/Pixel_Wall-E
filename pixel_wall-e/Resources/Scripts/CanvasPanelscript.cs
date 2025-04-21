using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
//Okay genio maquiavélico, para que después no se te olvide, vamos a hacer lo siguiente
//Para la animación cuando se pulse el botón va a aparecer wally con un pincel asintiendo con la cabeza y se pufea
//Despues en el spawn aparece una brochita con el color del color actual y la animación consiste en esa brochita pintando
public partial class CanvasPanelscript : Panel
{
    private LineEdit _sizeInput;
    private Button _applyBtn;
    private TextureRect _gridTextureRect;
    private int _gridSize = 32;
    private readonly Color _gridColor = new Color("#cccccc");
    private readonly int _maxCanvasSize = 512;
    private ImageTexture _gridTexture;
    private Control _xAxisContainer;
    private Control _yAxisContainer;
    //Elements for Animation
    private Timer _drawTimer;
    private Queue<DrawCommand> _drawQueue = new Queue<DrawCommand>();
    private bool _isAnimated = false;
    private Vector2I wallePos;
    private Image _canvasImage;

    public override void _Ready()
    {
        //Getting controls
        _sizeInput = GetNode<LineEdit>("Size");
        _applyBtn = GetNode<Button>("ApplyBtn");
        _gridTextureRect = GetNode<TextureRect>("Canvas");
        _xAxisContainer = GetNode<Control>("xAxisContainer");
        _yAxisContainer = GetNode<Control>("yAxisContainer");
        _canvasImage = Image.CreateEmpty(_maxCanvasSize, _maxCanvasSize, false, Image.Format.Rgba8);

        _drawTimer = new Timer();
        AddChild(_drawTimer);
        _drawTimer.Timeout += ProcessNextCommand;
        _drawTimer.WaitTime = 0.3f;

        //Adding events
        _applyBtn.Pressed += OnApplyPressed;
        _sizeInput.TextChanged += OnSizeTextChange;

        //Set min canvas size
        _gridTextureRect.CustomMinimumSize = new Vector2(_maxCanvasSize, _maxCanvasSize);

        //Generating starting canvas
        GenerateGridTexture();

        //Generating axes
        UpdateAxisLabels();
    }
    private void OnApplyPressed()
    {
        //Verifying that LineEdit has a number greater than zero
        if (int.TryParse(_sizeInput.Text, out int newSize) &&
        newSize > 0)
        {
            _gridSize = Math.Clamp(newSize, 1, 256);
            if (newSize > 256 || newSize > 256) return;
            GenerateGridTexture();
            UpdateAxisLabels();
        }
    }
    //Filtering the text from the LineText so it only supports numbers greater than zero and less or equal 256
    //That's because the Texture Rect size is 512 x 512, if you want a greater definition create a new scene that
    //only contains the canvas and change the number of maxCanvasSize variable and the top
    private void OnSizeTextChange(string newText)
    {
        string filtered = Filtered(newText);
        if (filtered != newText) _sizeInput.Text = filtered;
    }
    private string Filtered(string newText)
    {
        string filtered = string.Empty;
        foreach (char c in newText)
        {
            if (char.IsDigit(c)) filtered += c;
        }
        return filtered;
    }
    private void GenerateGridTexture()
    {
        Canvas.ChangeCanvas(_gridSize);
        //White Background
        _canvasImage.Fill(Colors.White);
        //Getting grid size
        float spacing = _maxCanvasSize / (float)_gridSize;
        Canvas.InitializePixel(spacing);
        Grid(spacing);
        //Release native resources. It's a recommended practice in Godot according with the documentation
        _gridTexture?.Dispose();
        //Creating the texture
        _gridTexture = ImageTexture.CreateFromImage(_canvasImage);
        //Appying the texture to the Texture Rect
        _gridTextureRect.Texture = _gridTexture;
    }
    private void Grid(float spacing)
    {
        //Putting lines
        for (int i = 0; i < _gridSize; i++)
        {
            int pos = (int)(i * spacing);
            for (int x = 0; x < _maxCanvasSize; x++)
            {
                _canvasImage.SetPixel(x, pos, _gridColor);
                _canvasImage.SetPixel(pos, x, _gridColor);
            }
        }
    }
    private void UpdateAxisLabels()
    {
        //Cleaning Axes
        foreach (Node child in _xAxisContainer.GetChildren())
        {
            child.QueueFree();
        }
        foreach (Node child in _yAxisContainer.GetChildren())
        {
            child.QueueFree();
        }
        //Getting grid sizes
        float cellSize = _maxCanvasSize / (float)_gridSize;
        //Creating axes
        if (cellSize > 7)
        {
            for (int x = 0; x < _gridSize; x++)
            {
                Label label = new Label();
                label.Text = x.ToString();
                label.HorizontalAlignment = HorizontalAlignment.Left;
                label.Position = new Vector2(x * cellSize, 0);
                if ((int)cellSize < 11)
                {
                    label.AddThemeFontSizeOverride("font_size", (int)cellSize - 2);
                }
                else
                    label.AddThemeFontSizeOverride("font_size", 11);
                label.AddThemeColorOverride("font_color", new Color(Colors.Black));
                _xAxisContainer.AddChild(label);
            }
            for (int y = 0; y < _gridSize; y++)
            {
                Label label = new Label();
                label.Text = y.ToString();
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.Position = new Vector2(0, y * cellSize);
                label.AddThemeColorOverride("font_color", new Color(Colors.Black));
                if ((int)cellSize < 11)
                    label.AddThemeFontSizeOverride("font_size", (int)cellSize - 2);
                else
                    label.AddThemeFontSizeOverride("font_size", 11);
                _yAxisContainer.AddChild(label);
            }
        }
        //We check that the size of the grid is grater than 7 because if it is less or equal the letter will be to tinny and user will not able to see it.
        //Therefore we could't put the axes. With this setting the numbers from 1 to 73 grid size are shown.
        //If you don't like this setting you can change it deleting the if statement.
    }
    public void ExecuteCommandQueue(Queue<DrawCommand> commands)
    {
        _drawQueue = new Queue<DrawCommand>(commands);
        if (!_isAnimated && _drawQueue.Count > 0)
        {
            _isAnimated = true;
            _drawTimer.Start();
        }
    }
    private void ProcessNextCommand()
    {
        if (_drawQueue.Count == 0)
        {
            _drawTimer.Stop();
            UpdateFullCanvas();
            Grid(_maxCanvasSize / (float)_gridSize);
            UpdateTexture();
            _isAnimated = false;
            return;
        }
        var cmd = _drawQueue.Dequeue();
        ExecuteCommandWithEffects(cmd);
    }
    private void ExecuteCommandWithEffects(DrawCommand cmd)
    {
        //This is Wall-E position acording with its position on the array, not the real canvas
        wallePos = new Vector2I(cmd.X, cmd.Y);

        if (cmd.Command == "PaintPixel")
        {
            DrawPixel(cmd.X, cmd.Y, cmd.Color, cmd.brushSize);
            return;
        }
        DrawWallE(wallePos.X, wallePos.Y, cmd.Color == "Transparent" ? Godot.Colors.DarkOrange : new Godot.Color(cmd.Color));
        UpdateTexture();
    }
    private void UpdateFullCanvas()
    {
        int size = Canvas.GetCanvasSize();
        for (int Y = 0; Y < size; Y++)
        {
            for (int X = 0; X < size; X++)
            {
                DrawPixel(X, Y, Canvas.pixels[X, Y].Color, 1);
                if (Canvas.pixels[X, Y].hasWallE)
                {
                    int CenterX = (Canvas.pixels[X, Y].X0 + Canvas.pixels[X, Y].X1) / 2;
                    int CenterY = (Canvas.pixels[X, Y].Y0 + Canvas.pixels[X, Y].Y1) / 2;
                    _canvasImage.SetPixel(CenterX,CenterY, Colors.DarkOrange);
                    _canvasImage.SetPixel(CenterX,CenterY-1, Colors.DarkOrange);
                    _canvasImage.SetPixel(CenterX+1,CenterY, Colors.DarkOrange);
                    _canvasImage.SetPixel(CenterX-1,CenterY, Colors.DarkOrange);
                    
                }
            }
        }
    }
    private void DrawPixel(int x, int y, string Color, int brushSize)
    {
        float cellSize = _maxCanvasSize / (float)_gridSize;
        if (brushSize == 1)
        {
            //_canvasImage.SetPixel(CenterX, CenterY, new Godot.Color(Color));
            PaintPixel(x, y, new Godot.Color(Color), cellSize);
            return;
        }
        int half = brushSize / 2;
        for (int dy = -half; dy <= half; dy++)
        {
            for (int dx = -half; dx <= half; dx++)
            {
                int px = x + dx;
                int py = y + dy;
                if (px >= 0 && px < _canvasImage.GetWidth() && py >= 0 && py < _canvasImage.GetHeight())
                {
                    PaintPixel(px, py, new Godot.Color(Color), cellSize);
                }
            }
        }
    }
    private void DrawWallE(int x, int y, Color color)
    {
        float cellSize = _maxCanvasSize / (float)_gridSize;
        int CenterX = (int)(x * cellSize + cellSize / 2);
        int CenterY = (int)(y * cellSize + cellSize / 2);
        for (int dy = -1; dy <= 1; dy++)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                int px = CenterX + dx;
                int py = CenterY + dy;
                if (px >= 0 && px < _canvasImage.GetWidth() && py >= 0 && py < _canvasImage.GetHeight())
                {
                    _canvasImage.SetPixel(px, py, color);
                }
            }
        }
    }
    private void PaintPixel(int x, int y, Color color, float cellSize)
    {
        for (int dy = Canvas.pixels[x, y].Y0; dy <= Canvas.pixels[x, y].Y1; dy++)
        {
            for (int dx = Canvas.pixels[x, y].X0; dx <= Canvas.pixels[x, y].X1; dx++)
            {
                if (dx >= 0 && dx < _canvasImage.GetWidth() && dy >= 0 && dy < _canvasImage.GetHeight())
                {
                    _canvasImage.SetPixel(dx, dy, new Godot.Color(color));
                }
            }
        }
    }
    private void UpdateTexture()
    {
        _gridTexture.Update(_canvasImage);
    }
}
