using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
//Okay genio maquiavélico, para que después no se te olvide, vamos a hacer lo siguiente
//Para la animación cuando se pulse el botón va a aparecer wally con un pincel asintiendo con la cabeza y se pufea
//Despues en el spawn aparece una brochita con el color del color actual y la animación consiste en esa brochita pintando
public partial class CanvasPanelscript : Panel
{
    private LineEdit _sizeInput;
    private Button _applyBtn;
    private TextureRect _gridTextureRect;
    private TextureRect _bigTextureRect;
    private ImageTexture _bigImageTexture;
    private Image _bigImage;
    private int _gridCount = 32;
    private readonly Color _gridColor = new Color("#cccccc");
    private int _maxCanvasSize = 512;
    private ImageTexture _gridTexture;
    private Control _xAxisContainer;
    private Control _yAxisContainer;
    private bool isHide = true;
    ScrollContainer _canvasScroll;
    ScrollContainer _xAxisScroll;
    ScrollContainer _yAxisScroll;
    //Elements for Animation
    private Timer _drawTimer;
    private Queue<DrawCommand> _drawQueue = new Queue<DrawCommand>();
    private bool _isAnimated = false;
    private Vector2 follow;
    private Vector2I wallePos;
    private Image _canvasImage;

    public override void _Ready()
    {
        //Getting controls
        _sizeInput = GetNode<LineEdit>("Size");
        _applyBtn = GetNode<Button>("ApplyBtn");
        _canvasScroll = GetNode<ScrollContainer>("CanvasScroll");
        _gridTextureRect = GetNode<ScrollContainer>("CanvasScroll").GetNode<TextureRect>("Canvas");
        _bigTextureRect = GetParent().GetNode<TextureRect>("BigCanvas");
        _xAxisScroll = GetNode<ScrollContainer>("xAxisScroll");
        _yAxisScroll = GetNode<ScrollContainer>("yAxisScroll");
        _xAxisContainer = GetNode<ScrollContainer>("xAxisScroll").GetNode<Control>("xAxisContainer");
        _yAxisContainer = GetNode<ScrollContainer>("yAxisScroll").GetNode<Control>("yAxisContainer");
        _canvasImage = Image.CreateEmpty(_maxCanvasSize, _maxCanvasSize, false, Image.Format.Rgba8);
        _bigImage = Image.CreateEmpty(_maxCanvasSize, _maxCanvasSize, false, Image.Format.Rgba8);

        _drawTimer = new Timer();
        AddChild(_drawTimer);
        _drawTimer.Timeout += ProcessNextCommand;
        _drawTimer.WaitTime = 0.1f;

        //Adding events
        _applyBtn.Pressed += OnApplyPressed;
        _sizeInput.TextChanged += OnSizeTextChange;
        _canvasScroll.GetHScrollBar().ValueChanged += ChangeHorizontal;
        _canvasScroll.GetVScrollBar().ValueChanged += ChangeVertical;
        _gridTextureRect.GuiInput += MouseTouch;
        _bigTextureRect.GuiInput += MouseTouch;

        //Set min canvas size
        _gridTextureRect.CustomMinimumSize = new Vector2(_maxCanvasSize, _maxCanvasSize);

        //Generating starting canvas
        GenerateGridTexture();

        //Generating axes
        UpdateAxisLabels();
    }
    private void ChangeHorizontal(double value) => _xAxisScroll.ScrollHorizontal = (int)value;
    private void ChangeVertical(double value) => _yAxisScroll.ScrollVertical = (int)value;
    private void OnApplyPressed()
    {
        //Verifying that LineEdit has a number greater than zero
        if (int.TryParse(_sizeInput.Text, out int newSize) &&
        newSize > 0)
        {
            _maxCanvasSize = (13 * newSize > 512) ? 13 * newSize : 512;
            _canvasImage = Image.CreateEmpty(_maxCanvasSize, _maxCanvasSize, false, Image.Format.Rgba8);
            _bigImage = Image.CreateEmpty(_maxCanvasSize, _maxCanvasSize, false, Image.Format.Rgba8);
            _gridTextureRect.CustomMinimumSize = new Vector2(_maxCanvasSize, _maxCanvasSize);
            _xAxisContainer.CustomMinimumSize = new Vector2(_maxCanvasSize, _xAxisContainer.Size.Y);
            _yAxisContainer.CustomMinimumSize = new Vector2(_yAxisContainer.Size.X, _maxCanvasSize);
            _gridCount = newSize;
            _drawTimer.WaitTime = (13 * newSize > 512)?(0.1f*1.5f)/Math.Sqrt(_gridCount):0.1f;
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
        Canvas.ChangeCanvas(_gridCount);
        //White Background
        _canvasImage.Fill(Colors.White);
        _bigImage.Fill(Colors.White);
        //Getting grid size
        float spacing = _maxCanvasSize / (float)_gridCount;
        Canvas.InitializePixel(spacing);
        Grid(spacing);
        //Release native resources. It's a recommended practice in Godot according with the documentation
        _gridTexture?.Dispose();
        _bigImageTexture?.Dispose();
        //Creating the texture
        _gridTexture = ImageTexture.CreateFromImage(_canvasImage);
        _bigImageTexture = ImageTexture.CreateFromImage(_bigImage);
        //Appying the texture to the Texture Rect
        _gridTextureRect.Texture = _gridTexture;
        _bigTextureRect.Texture = _bigImageTexture;
    }
    private void Grid(float spacing)
    {
        //Putting lines
        for (int i = 0; i < _gridCount; i++)
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
        float cellSize = _maxCanvasSize / (float)_gridCount;
        //Creating axes

        int dig = _gridCount.ToString().Length;
        for (int x = 0; x < _gridCount; x++)
        {
            Label label = new Label();
            label.Text = x.ToString();
            label.HorizontalAlignment = HorizontalAlignment.Left;
            label.Position = new Vector2(x * cellSize, 0);
            if ((int)cellSize <= 13)
            {
                label.AddThemeFontSizeOverride("font_size", (int)cellSize - (dig + 4));
            }
            else
                label.AddThemeFontSizeOverride("font_size", 11);
            label.AddThemeColorOverride("font_color", new Color(Colors.Black));
            _xAxisContainer.AddChild(label);
        }
        for (int y = 0; y < _gridCount; y++)
        {
            Label label = new Label();
            label.Text = y.ToString();
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.Position = new Vector2(0, y * cellSize);
            label.AddThemeColorOverride("font_color", new Color(Colors.Black));
            if ((int)cellSize < 13)
                label.AddThemeFontSizeOverride("font_size", (int)cellSize - 2);
            else
                label.AddThemeFontSizeOverride("font_size", 11);
            _yAxisContainer.AddChild(label);
        }

        //We check that the size of the grid is grater than 7 because if it is less or equal the letter will be to tinny and user will not able to see it.
        //Therefore we could't put the axes. With this setting the numbers from 1 to 73 grid size are shown.
        //If you don't like this setting you can change it deleting the if statement.
    }
    private void MouseTouch(InputEvent e)
    {
        if (e is InputEventMouseButton mouse && mouse.ButtonIndex == MouseButton.Left && mouse.Pressed)
        {
            if (isHide&&!_isAnimated)
            {
                _bigTextureRect.Visible = true;
                _bigTextureRect.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
                _bigTextureRect.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
                _bigTextureRect.SizeFlagsHorizontal = SizeFlags.ExpandFill;
                _bigTextureRect.SizeFlagsVertical = SizeFlags.ExpandFill;
                _bigTextureRect.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
                isHide = false;
            }
            else
            {
                _bigTextureRect.Visible = false;
                isHide = true;
            }
        }
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
            //Grid(_maxCanvasSize / (float)_gridCount);
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
                    _canvasImage.SetPixel(CenterX, CenterY, Colors.DarkOrange);
                    if (CenterY - 1 > 0 && CenterY - 1 < _maxCanvasSize)
                        _canvasImage.SetPixel(CenterX, CenterY - 1, Colors.DarkOrange);
                    if (CenterX + 1 > 0 && CenterX + 1 < _maxCanvasSize)
                        _canvasImage.SetPixel(CenterX + 1, CenterY, Colors.DarkOrange);
                    if (CenterX - 1 > 0 && CenterX - 1 < _maxCanvasSize)
                        _canvasImage.SetPixel(CenterX - 1, CenterY, Colors.DarkOrange);

                }
            }
        }
    }
    private void centerOnWallE(int x, int y)
    {
        float cellSize = _maxCanvasSize / (float)_gridCount;
        int CenterX = (int)(x * cellSize);
        int CenterY = (int)(y * cellSize);
        _canvasScroll.ScrollHorizontal = (int)Math.Clamp(Math.Abs(CenterX), 0, _gridTextureRect.Size.X);
        _canvasScroll.ScrollVertical = (int)Math.Clamp(Math.Abs(CenterY), 0, _gridTextureRect.Size.Y);
    }
    private void DrawPixel(int x, int y, string Color, int brushSize)
    {
        if (brushSize == 1)
        {
            //_canvasImage.SetPixel(CenterX, CenterY, new Godot.Color(Color));
            PaintPixel(x, y, new Godot.Color(Color));
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
                    PaintPixel(px, py, new Godot.Color(Color));
                }
            }
        }
    }
    private void DrawWallE(int x, int y, Color color)
    {
        float cellSize = _maxCanvasSize / (float)_gridCount;
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
        centerOnWallE(x,y);
    }
    private void PaintPixel(int x, int y, Color color)
    {
        for (int dy = Canvas.pixels[x, y].Y0 - 1; dy <= Canvas.pixels[x, y].Y1; dy++)
        {
            for (int dx = Canvas.pixels[x, y].X0 - 1; dx <= Canvas.pixels[x, y].X1; dx++)
            {
                if (dx >= 0 && dx < _canvasImage.GetWidth() && dy >= 0 && dy < _canvasImage.GetHeight())
                {
                    if (dy != Canvas.pixels[x, y].Y0 - 1 && dx != Canvas.pixels[x, y].X0 - 1) _canvasImage.SetPixel(dx, dy, new Godot.Color(color));
                    _bigImage.SetPixel(dx, dy, new Color(color));
                }
            }
        }
    }
    private void UpdateTexture()
    {
        _gridTexture.Update(_canvasImage);
        _bigImageTexture.Update(_bigImage);
    }
}
